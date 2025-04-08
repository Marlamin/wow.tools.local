using DBCD.Providers;
using DBDefsLib;
using wow.tools.local;

namespace wow.tools.Services
{
    public class DBDProvider : IDBDProvider
    {
        private readonly DBDReader dbdReader;
        private Dictionary<string, (string FilePath, Structs.DBDefinition Definition)> definitionLookup = new(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<string>> relationshipMap = [];
        public bool isUsingBDBD = false;

        public DBDProvider()
        {
            dbdReader = new DBDReader();
            LoadDefinitions();
        }

        public Stream GetBDBDStream()
        {
            var downloadBDBD = false;
            var cacheLocation = Path.Combine("cache", "all.bdbd");
            var fileInfo = new FileInfo(cacheLocation);

            if (fileInfo.Exists)
            {
                if (fileInfo.LastWriteTime.AddDays(1) > DateTime.Now)
                    downloadBDBD = true;
            }
            else
            {
                downloadBDBD = true;
            }

            if (downloadBDBD)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "wow.tools.local");
                    var response = client.GetAsync("https://github.com/wowdev/WoWDBDefs/releases/latest/download/all.bdbd").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        File.WriteAllBytes(cacheLocation, response.Content.ReadAsByteArrayAsync().Result);
                    }
                    else
                        Console.WriteLine("Failed to download all.bdbd from GitHub: " + response.StatusCode.ToString());
                }
            }

            if (!File.Exists(cacheLocation))
                throw new Exception("all.bdbd not found");

            return new FileStream(cacheLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public int LoadDefinitions()
        {
            if(string.IsNullOrEmpty(SettingsManager.definitionDir) || !Directory.Exists(SettingsManager.definitionDir))
            {
                Console.WriteLine("Loading definitions from BDBD file");
                using(var fs = GetBDBDStream())
                {
                    var bdbd = BDBDReader.Read(fs);
                    foreach (var entry in bdbd)
                    {
                        var name = Path.GetFileNameWithoutExtension(entry.Key);
                        var definition = entry.Value.dbd;
                        
                        definitionLookup.Add(name, (entry.Key, definition));
                    }
                    Console.WriteLine("Loaded " + definitionLookup.Count + " definitions from BDBD file!");
                }

                isUsingBDBD = true;
            }
            else
            {
                var definitionsDir = SettingsManager.definitionDir;
                Console.WriteLine("Reloading definitions from directory " + definitionsDir);

                // lookup needs both filepath and def for DBCD to work
                // also no longer case sensitive now
                var definitionFiles = Directory.EnumerateFiles(definitionsDir);
                definitionLookup = definitionFiles.ToDictionary(x => Path.GetFileNameWithoutExtension(x), x => (x, dbdReader.Read(x)), StringComparer.OrdinalIgnoreCase);

                Console.WriteLine("Loaded " + definitionLookup.Count + " definitions from definitions folder!");
            }

            Console.WriteLine("Reloading relationship map");

            relationshipMap = [];

            foreach (var definition in definitionLookup)
            {
                foreach (var column in definition.Value.Definition.columnDefinitions)
                {
                    if (column.Value.foreignTable == null)
                        continue;

                    var currentName = definition.Key + "::" + column.Key;
                    var foreignName = column.Value.foreignTable + "::" + column.Value.foreignColumn;
                    if (relationshipMap.TryGetValue(foreignName, out List<string>? relations))
                    {
                        relations.Add(currentName);
                    }
                    else
                    {
                        relationshipMap.Add(foreignName, [currentName]);
                    }
                }
            }

            Console.WriteLine("Reloaded relationship map: " + relationshipMap.Count + " relations");

            return definitionLookup.Count;
        }

        public Stream StreamForTableName(string tableName, string build = null)
        {
            if(isUsingBDBD)
                throw new Exception("DBD definitions were loaded from BDBD, we should never be using this function");

            tableName = Path.GetFileNameWithoutExtension(tableName);

            if (definitionLookup.TryGetValue(tableName, out var lookup))
                return new FileStream(lookup.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            throw new FileNotFoundException("Definition for " + tableName + " not found");
        }

        public bool TryGetDefinition(string tableName, out Structs.DBDefinition definition)
        {
            if (definitionLookup.TryGetValue(tableName, out var lookup))
            {
                definition = lookup.Definition;
                return true;
            }

            definition = default;
            return false;
        }

        public Dictionary<string, List<string>> GetAllRelations()
        {
            return relationshipMap;
        }

        public List<string> GetRelationsToColumn(string foreignColumn, bool fixCase = false)
        {
            if (fixCase)
            {
                var splitCol = foreignColumn.Split("::");
                if (splitCol.Length == 2)
                {
                    var results = definitionLookup.Where(x => x.Key.Equals(splitCol[0], StringComparison.CurrentCultureIgnoreCase)).Select(x => x.Key);
                    if (results.Any())
                    {
                        splitCol[0] = results.First();
                    }
                }
                foreignColumn = string.Join("::", splitCol);
            }

            if (!relationshipMap.TryGetValue(foreignColumn, out var relations))
                return [];

            return relations;
        }

        public string[] GetVersionsInDBD(string tableName)
        {
            if (!definitionLookup.TryGetValue(tableName, out var tableDefinition))
                throw new Exception("No DBD found for table name " + tableName);

            var buildList = new List<string>();

            foreach(var definition in tableDefinition.Definition.versionDefinitions)
            {
                foreach(var build in definition.builds)
                    buildList.Add(build.ToString());

                foreach(var buildRange in definition.buildRanges)
                {
                    buildList.Add(buildRange.minBuild.ToString());
                    buildList.Add(buildRange.maxBuild.ToString());
                }
            }

            return buildList.ToArray();
        }

        public string[] GetNames()
        {
            return [.. definitionLookup.Keys.Order()];
        }
    }
}