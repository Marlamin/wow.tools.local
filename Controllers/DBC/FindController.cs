using DBCD;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers.DBC
{
    [Route("dbc/find")]
    [ApiController]
    public class FindController(IDBCManager dbcManager) : ControllerBase
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

        // GET: find/
        [HttpGet]
        public async Task<List<Dictionary<string, string>>> Get(string name, string value, string build, bool useHotfixes = false)
        {
            Console.WriteLine("Finding results in " + name + " (" + build + ", hotfixes: " + useHotfixes + ") for value " + value);

            IDBCDStorage storage;

            try
            {
                storage = await dbcManager.GetOrLoad(name, build, useHotfixes);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Error loading DBC: " + e.Message);
                return new List<Dictionary<string, string>>();
            }

            var result = new List<Dictionary<string, string>>();

            if (storage.Values.Count == 0)
                return result;

            foreach (DBCDRow row in storage.Values)
            {
                for (var i = 0; i < storage.AvailableColumns.Length; ++i)
                {
                    string fieldName = storage.AvailableColumns[i];

                    var field = row[fieldName];

                    if (field is Array arrayFields)
                    {
                        foreach (var arrayField in arrayFields)
                        {
                            if (arrayField.ToString() == value)
                            {
                                var newDict = new Dictionary<string, string>();
                                for (var j = 0; j < storage.AvailableColumns.Length; ++j)
                                {
                                    string subfieldName = storage.AvailableColumns[j];
                                    var subfield = row[subfieldName];

                                    if (subfield is Array a)
                                    {
                                        for (var k = 0; k < a.Length; k++)
                                        {
                                            newDict.Add(subfieldName + "[" + k + "]", a.GetValue(k).ToString());
                                        }
                                    }
                                    else
                                    {
                                        newDict.Add(subfieldName, subfield.ToString());
                                    }
                                }

                                result.Add(newDict);
                            }
                        }
                    }
                    else
                    {
                        if (field.ToString() == value)
                        {
                            var newDict = new Dictionary<string, string>();
                            for (var j = 0; j < storage.AvailableColumns.Length; ++j)
                            {
                                string subfieldName = storage.AvailableColumns[j];
                                var subfield = row[subfieldName];

                                if (subfield is Array a)
                                {
                                    for (var k = 0; k < a.Length; k++)
                                    {
                                        newDict.Add(subfieldName + "[" + k + "]", a.GetValue(k).ToString());
                                    }
                                }
                                else
                                {
                                    newDict.Add(subfieldName, subfield.ToString());
                                }
                            }

                            result.Add(newDict);
                        }
                    }
                }
            }

            return result;
        }

        // GET: find/name
        [HttpGet("{name}")]
        public async Task<List<Dictionary<string, string>>> Get(string name, string build, string col, int val, bool useHotfixes = false, bool calcOffset = true)
        {
            Console.WriteLine("Finding results for " + name + "::" + col + " (" + build + ", hotfixes: " + useHotfixes + ") value " + val);

            var storage = await dbcManager.GetOrLoad(name, build, useHotfixes);

            var result = new List<Dictionary<string, string>>();

            if (storage.Values.Count == 0)
                return result;

            if (!calcOffset && col == "ID")
            {
                if (storage.TryGetValue(val, out DBCDRow row))
                {
                    for (var i = 0; i < storage.AvailableColumns.Length; ++i)
                    {
                        string fieldName = storage.AvailableColumns[i];

                        if (fieldName != col)
                            continue;

                        var field = row[fieldName];

                        // Don't think FKs to arrays are possible, so only check regular value
                        if (field.ToString() == val.ToString())
                        {
                            var newDict = new Dictionary<string, string>();
                            for (var j = 0; j < storage.AvailableColumns.Length; ++j)
                            {
                                string subfieldName = storage.AvailableColumns[j];
                                var subfield = row[subfieldName];

                                if (subfield is Array a)
                                {
                                    for (var k = 0; k < a.Length; k++)
                                    {
                                        newDict.Add(subfieldName + "[" + k + "]", a.GetValue(k).ToString());
                                    }
                                }
                                else
                                {
                                    newDict.Add(subfieldName, subfield.ToString());
                                }
                            }

                            result.Add(newDict);
                        }
                    }
                }
            }
            else
            {
                var arrIndex = 0;

                if (col.Contains('['))
                {
                    arrIndex = int.Parse(col.Split("[")[1].Replace("]", string.Empty));
                    col = col.Split("[")[0];
                }

                foreach (DBCDRow row in storage.Values)
                {
                    for (var i = 0; i < storage.AvailableColumns.Length; ++i)
                    {
                        string fieldName = storage.AvailableColumns[i];

                        if (fieldName != col)
                            continue;

                        var field = row[fieldName];

                        if (field is Array arrayField)
                        {
                            field = arrayField.GetValue(arrIndex).ToString();
                        }

                        // Don't think FKs to arrays are possible, so only check regular value
                        if (field.ToString() == val.ToString())
                        {
                            var newDict = new Dictionary<string, string>();
                            for (var j = 0; j < storage.AvailableColumns.Length; ++j)
                            {
                                string subfieldName = storage.AvailableColumns[j];
                                var subfield = row[subfieldName];

                                if (subfield is Array a)
                                {
                                    for (var k = 0; k < a.Length; k++)
                                    {
                                        newDict.Add(subfieldName + "[" + k + "]", a.GetValue(k).ToString());
                                    }
                                }
                                else
                                {
                                    newDict.Add(subfieldName, subfield.ToString());
                                }
                            }

                            result.Add(newDict);
                        }
                    }
                }
            }

            return result;
        }
    }
}