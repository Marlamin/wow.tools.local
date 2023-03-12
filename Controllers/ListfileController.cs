using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("listfile/")]
    [ApiController]
    public class ListfileController : Controller
    {
        private readonly DBCManager dbcManager;

        public ListfileController(IDBCManager dbcManager)
        {
            this.dbcManager = dbcManager as DBCManager;
        }

        public Dictionary<int, string> DoSearch(Dictionary<int, string> resultsIn, string search)
        {
            var listfileResults = new Dictionary<int, string>();
            if (search.StartsWith("type:"))
            {
                var cleaned = "." + search.Replace("type:", "");
                listfileResults = resultsIn.Where(x => x.Value.EndsWith(cleaned)).ToDictionary(x => x.Key, x => x.Value);
            }
            else if(search == "unnamed")
            {
                listfileResults = resultsIn.Where(x => x.Value.ToLower().StartsWith("unknown")).ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                // Simple search
                listfileResults = resultsIn.Where(x => x.Value.ToLower().Contains(search) || x.Key.ToString() == search).ToDictionary(x => x.Key, x => x.Value);
            }
            return listfileResults;
        }

        [Route("files")]
        [HttpGet]
        public DataTablesResult FileDataTables(int draw, int start, int length)
        {
            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = CASC.M2Listfile.Count,
                data = new List<List<string>>()
            };

            var listfileResults = new Dictionary<int, string>(CASC.Listfile);
            if (Request.Query.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
            {
                var searchStr = search.ToString().ToLower();
                if (searchStr.Contains(','))
                {
                    var filters = searchStr.Split(',');
                    foreach(var filter in filters)
                    {
                        listfileResults = DoSearch(listfileResults, filter);
                    }
                }
                else
                {
                    listfileResults = DoSearch(listfileResults, search);
                }
               
                result.recordsFiltered = listfileResults.Count();
            }
            else
            {
                listfileResults = CASC.Listfile;
                result.recordsFiltered = CASC.Listfile.Count;
            }

            var sortedResults = listfileResults.OrderBy(x => x.Key);

            var rows = new List<string>();

            foreach (var listfileResult in sortedResults.Skip(start).Take(length))
            {
                result.data.Add(
                    new List<string>() {
                        listfileResult.Key.ToString(), // ID
                        listfileResult.Value, // Filename 
                        "", // Lookup
                        "", // Versions
                        Path.GetExtension(listfileResult.Value).Replace(".", ""), // Type
                        CASC.EncryptionStatuses.ContainsKey(listfileResult.Key) ? CASC.EncryptionStatuses[listfileResult.Key].ToString() : "", // Extra data
                        "", // Comment
                        ""
                    });
            }

            return result;
        }

        [Route("datatables")]
        [HttpGet]
        public DataTablesResult DataTables(int draw, int start, int length)
        {
            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = CASC.M2Listfile.Count,
                data = new List<List<string>>()
            };

            var listfileResults = new Dictionary<int, string>();

            if (Request.Query.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
            {
                var searchStr = search.ToString().ToLower();
                listfileResults = CASC.M2Listfile.Where(x => x.Value.ToLower().Contains(searchStr)).ToDictionary(x => x.Key, x => x.Value);
                result.recordsFiltered = listfileResults.Count();
            }
            else
            {
                listfileResults = CASC.M2Listfile.ToDictionary(x => x.Key, x => x.Value);
                result.recordsFiltered = CASC.M2Listfile.Count;
            }

            var rows = new List<string>();

            foreach (var listfileResult in listfileResults.Skip(start).Take(length))
            {
                result.data.Add(
                    new List<string>() {
                        listfileResult.Key.ToString(), // ID
                        listfileResult.Value, // Filename 
                        "", // Lookup
                        "", // Versions
                        Path.GetExtension(listfileResult.Value).Replace(".", ""), // Type
                        CASC.EncryptionStatuses.ContainsKey(listfileResult.Key) ? CASC.EncryptionStatuses[listfileResult.Key].ToString() : "0"
                    });
            }

            return result;
        }

        [Route("info")]
        [HttpGet]
        public string Info(int filename, string filedataid)
        {
            var split = filedataid.Split(",");
            if (split.Length > 1)
            {
                foreach (var id in split)
                {
                    if (CASC.Listfile.TryGetValue(int.Parse(id), out string name))
                    {
                        return name;
                    }
                }

                return "";
            }
            else
            {
                if (CASC.Listfile.TryGetValue(int.Parse(filedataid), out string name))
                {
                    return name;
                }
                else
                {
                    return "";
                }
            }
        }

        [Route("db2s")]
        [HttpGet]
        public List<string> DB2s()
        {
            return dbcManager.GetDBCNames();
        }

        [HttpGet("db2/{databaseName}/versions")]
        public List<string> BuildsForDatabase(string databaseName, bool uniqueOnly = false)
        {
            var versionList = new List<Version>
            {
                new Version(CASC.BuildName)
            };

            if (!string.IsNullOrEmpty(SettingsManager.dbcFolder) && Directory.Exists(SettingsManager.dbcFolder))
            {
                var dbcFolder = new DirectoryInfo(SettingsManager.dbcFolder);
                var dbcFiles = dbcFolder.GetFiles("*" + databaseName + ".db*", SearchOption.AllDirectories);
                foreach (var dbcFile in dbcFiles)
                {
                    var splitFolders = dbcFile.DirectoryName.Split(Path.DirectorySeparatorChar);
                    foreach (var splitFolder in splitFolders)
                    {
                        var buildTest = splitFolder.Split(".");
                        if (buildTest.Length == 4 && buildTest.All(s => s.All(char.IsDigit)))
                        {
                            if (!versionList.Contains(new Version(splitFolder)))
                            {
                                versionList.Add(new Version(splitFolder));
                                continue;
                            }
                        }
                    }
                }
            }
            return versionList.OrderDescending().Select(v => v.ToString()).ToList();
        }
    }

    public struct DataTablesResult
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<List<string>> data { get; set; }
    }
}
