using DBCD;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace wow.tools.local.Services
{
    public class DBCViewFilter
    {
        public bool Searching { get; private set; }
        public bool Filtering { get; private set; }
        public bool Sorting { get; private set; }
        public string SearchValue { get; private set; }
        public string SortDirection { get; private set; }

        private readonly IDBCDStorage Storage;
        private readonly IReadOnlyDictionary<string, string> Parameters;
        private readonly Func<string, string> StringFormatter;
        private int SortBySiteCol;

        private static readonly MethodInfo ObjectToString = typeof(object).GetMethod("ToString");

        private Func<DBCDRow, bool> FilterFunc;
        private Func<DBCDRow, object> SortFunc;
        private Func<DBCDRow, string[]> ConverterFunc;

        public DBCViewFilter(IDBCDStorage storage, IReadOnlyDictionary<string, string> parameters, Func<string, string> stringFormatter = null)
        {
            Storage = storage;
            Parameters = parameters;
            StringFormatter = stringFormatter;

            InitialiseSort();
            InitialiseSearch();
            Initialise();
        }

        /// <summary>
        /// Returns the result set of formatted records filtered and sorted accordingly
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<string[]> GetRecords(CancellationToken? token = null)
        {
            var records = Storage.Values.AsEnumerable();

            // apply sort
            if (SortFunc != null)
            {
                if (SortDirection == "asc")
                    records = records.OrderBy(SortFunc);
                else
                    records = records.OrderByDescending(SortFunc);
            }

            // apply filter
            if (FilterFunc != null)
            {
                records = records.Where(FilterFunc);
            }

            var rowIDFilter = new List<int>();
            if (Searching && SearchValue == "encrypted")
            {
                rowIDFilter.AddRange(Storage.GetEncryptedIDs().SelectMany(x => x.Value));
                Searching = false;
            }
            else if (Searching && SearchValue.StartsWith("encrypted:"))
            {
                if (Storage.GetEncryptedSections().TryGetValue(ulong.Parse(SearchValue.Substring(10), NumberStyles.HexNumber), out var encryptedSection))
                    rowIDFilter.AddRange(Storage.GetEncryptedIDs().Where(x => x.Key == encryptedSection).SelectMany(x => x.Value));

                Searching = false;
            }

            if(rowIDFilter.Count > 0)
                records = records.Where(x => rowIDFilter.Contains(x.ID));

            // apply converter
            var result = records.Select(ConverterFunc);
          
            foreach (var rowList in result)
            {
                token?.ThrowIfCancellationRequested();
                
                // if searching we need to futher filter the returned records for SearchValue
                var matches = !Searching;
                for (var i = 0; !matches && i < rowList.Length; i++)
                    matches = rowList[i].Contains(SearchValue, StringComparison.InvariantCultureIgnoreCase);

                if (matches)
                    yield return rowList;
            }
        }


        /// <summary>
        /// Creates the Sort, Where and Converter functions from the provided parameters
        /// </summary>
        private void Initialise()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            
            var param = Expression.Parameter(typeof(DBCDRow), "row");
            var properties = new List<Expression>();
            var filters = new Dictionary<Expression, Predicate<object>>(); // [DBCDRow.Property, Predicate]

            var firstItem = Storage.Values.First();
            var siteColIndex = 0;

            for (var i = 0; i < Storage.AvailableColumns.Length; ++i)
            {
                var field = firstItem[Storage.AvailableColumns[i]];
                var size = 1;
                var isArray = false;

                if (field is Array array)
                {
                    size = array.Length;
                    isArray = true;
                }

                for (var j = 0; j < size; j++)
                {
                    var property = GetProperty(param, Storage.AvailableColumns[i], isArray, j);

                    if (SortBySiteCol == siteColIndex)
                    {
                        SortFunc = Expression.Lambda<Func<DBCDRow, object>>(property, param).Compile();
                    }

                    if (Parameters.TryGetValue("columns[" + siteColIndex + "][search][value]", out var filterVal) && !string.IsNullOrWhiteSpace(filterVal))
                    {
                        filters.Add(property, CreateFilterPredicate(filterVal));
                    }

                    // cast to string and apply any custom string formatting i.e. HtmlEncode and StringToCSVCell
                    var formattedProperty = Expression.Call(property, ObjectToString);
                    if (StringFormatter != null && (field.GetType() == typeof(string) || field.GetType() == typeof(string[])))
                    {
                        formattedProperty = Expression.Call(StringFormatter.Method, formattedProperty);
                    }

                    properties.Add(formattedProperty);
                    siteColIndex++;
                }
            }

            // compile the filter predicate
            if (filters.Count > 0)
            {
                Searching = false; // NOTE: remove this line to allow filtered searches
                Filtering = true;
                CreateFilterFunc(param, filters);
            }

            // compile the DBCDRow to string[] converter
            CreateConverterFunc(param, properties);
        }

        /// <summary>
        /// Attempts to parse the sorted column and it's direction - if any
        /// </summary>
        /// <returns></returns>
        private void InitialiseSort()
        {
            SortBySiteCol = -1;

            if (Parameters.TryGetValue("order[0][column]", out var sortbycol))
            {
                if (int.TryParse(sortbycol, out var sortBySiteCol))
                {
                    SortDirection = Parameters["order[0][dir]"];
                    SortBySiteCol = sortBySiteCol;
                }
            }
        }

        /// <summary>
        /// Attempts to parse and normalise the search value - if any
        /// </summary>
        /// <returns></returns>
        private void InitialiseSearch()
        {
            if (Parameters.TryGetValue("search[value]", out var searchValue) && !string.IsNullOrWhiteSpace(searchValue))
            {
                Searching = true;

                // format the searchvalue so it matches in GetRecords
                if (StringFormatter != null)
                    SearchValue = StringFormatter(searchValue.Trim());
                else
                    SearchValue = searchValue.Trim();
            }
            else
            {
                Searching = false;
            }
        }

        #region Predicates

        private static Predicate<object> CreateFilterPredicate(string filterVal)
        {
            if (filterVal.StartsWith("exact:"))
            {
                return CreateRegexPredicate("^" + filterVal.Remove(0, 6) + "$");
            }
            else if (filterVal.StartsWith("regex:"))
            {
                return CreateRegexPredicate(filterVal.Remove(0, 6));
            }
            else if (filterVal.StartsWith("0x"))
            {
                if (ulong.TryParse(filterVal.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong flags))
                {
                    return CreateFlagsPredicate(flags);
                }
            }

            // Fallback logic is kept outside of an `else` branch to permit invalid filter recovery.
            return CreateRegexPredicate(filterVal);
        }

        private static Predicate<object> CreateRegexPredicate(string pattern)
        {
            var re = new Regex(pattern, RegexOptions.IgnoreCase);
            return (field) => re.IsMatch(field.ToString());
        }

        private static Predicate<object> CreateFlagsPredicate(ulong flags)
        {
            return (field) =>
            {
                if (field is int @int)
                {
                    field = unchecked((uint)@int);
                }

                var num = Convert.ToUInt64(field, CultureInfo.InvariantCulture);
                return (num & flags) == flags;
            };
        }

        #endregion

        #region Helpers

        private void CreateFilterFunc(ParameterExpression param, Dictionary<Expression, Predicate<object>> filters)
        {
            Expression expression = Expression.Constant(true); // null check avoidance
            foreach (var filter in filters)
            {
                var call = Expression.Invoke(Expression.Constant(filter.Value), filter.Key);
                expression = Expression.AndAlso(expression, call);
            }

            var lambda = Expression.Lambda<Func<DBCDRow, bool>>(expression, param);

            FilterFunc = lambda.Compile();
        }

        private void CreateConverterFunc(ParameterExpression param, ICollection<Expression> properties)
        {
            var expression = Expression.NewArrayInit(typeof(string), properties);
            var lambda = Expression.Lambda<Func<DBCDRow, string[]>>(expression, param);

            ConverterFunc = lambda.Compile();
        }

        private static Expression GetProperty(ParameterExpression param, string fieldname, bool isArray, int index = 0)
        {
            if (isArray)
                return Expression.Property(param, "Item", Expression.Constant(fieldname), Expression.Constant(index));
            else
                return Expression.Property(param, "Item", Expression.Constant(fieldname));
        }

        #endregion
    }
}