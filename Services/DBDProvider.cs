using DBCD.Providers;
using DBDefsLib;
using wow.tools.local;

namespace wow.tools.Services
{
    public class DBDProvider : IDBDProvider
    {
        private readonly DBDReader dbdReader;
        public Dictionary<string, (string FilePath, Structs.DBDefinition Definition)> definitionLookup;
        private Dictionary<string, List<string>> relationshipMap;
        
        public DBDProvider()
        {
            dbdReader = new DBDReader();
            LoadDefinitions();
        }

        public int LoadDefinitions()
        {
            var definitionsDir = SettingsManager.definitionDir;
            Console.WriteLine("Reloading definitions from directory " + definitionsDir);

            // lookup needs both filepath and def for DBCD to work
            // also no longer case sensitive now
            var definitionFiles = Directory.EnumerateFiles(definitionsDir);
            definitionLookup = definitionFiles.ToDictionary(x => Path.GetFileNameWithoutExtension(x), x => (x, dbdReader.Read(x)), StringComparer.OrdinalIgnoreCase);

            Console.WriteLine("Loaded " + definitionLookup.Count + " definitions!");

            Console.WriteLine("Reloading relationship map");

            relationshipMap = new Dictionary<string, List<string>>();

            foreach (var definition in definitionLookup)
            {
                foreach (var column in definition.Value.Definition.columnDefinitions)
                {
                    if (column.Value.foreignTable == null)
                        continue;

                    var currentName = definition.Key + "::" + column.Key;
                    var foreignName = column.Value.foreignTable + "::" + column.Value.foreignColumn;
                    if (relationshipMap.ContainsKey(foreignName))
                    {
                        relationshipMap[foreignName].Add(currentName);
                    }
                    else
                    {
                        relationshipMap.Add(foreignName, new List<string>() { currentName });
                    }
                }
            }

            Console.WriteLine("Reloaded relationship map: " + relationshipMap.Count + " relations");

            return definitionLookup.Count;
        }

        public Stream StreamForTableName(string tableName, string build = null)
        {
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
                    var results = definitionLookup.Where(x => x.Key.ToLower() == splitCol[0].ToLower()).Select(x => x.Key);
                    if (results.Any())
                    {
                        splitCol[0] = results.First();
                    }
                }
                foreignColumn = string.Join("::", splitCol);
            }

            if (!relationshipMap.TryGetValue(foreignColumn, out List<string> relations))
            {
                return new List<string>();
            }

            return relations;
        }
    }
}