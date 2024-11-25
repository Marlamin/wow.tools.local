using System.Text.Json;
using System.Text.Json.Serialization;

namespace wow.tools.local.Services
{
    public static class DBDManifest
    {
        public static void Load()
        {
            var manifestLocation = Path.GetFullPath(Path.Combine(SettingsManager.definitionDir, "..", "manifest.json"));
            if(!File.Exists(manifestLocation))
            {
                Console.WriteLine("No manifest found at " + manifestLocation + ", skipping DBD manifest initialization.");
                return;
            }

            var manifest = JsonSerializer.Deserialize<List<ManifestEntry>>(File.ReadAllText(manifestLocation));

            foreach(var entry in manifest)
            {
                if(!CASC.DB2Map.ContainsKey("dbfilesclient/" + entry.tableName.ToLower() + ".db2"))
                    CASC.DB2Map.Add("dbfilesclient/" + entry.tableName.ToLower() + ".db2", entry.db2FileDataID);

                if (!CASC.DB2Map.ContainsKey("dbfilesclient/" + entry.tableName.ToLower() + ".dbc"))
                    CASC.DB2Map.Add("dbfilesclient/" + entry.tableName.ToLower() + ".dbc", entry.dbcFileDataID);
            }
        }
    }

    public struct ManifestEntry
    {
        public string tableName { get; set; }
        public string tableHash { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int dbcFileDataID { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int db2FileDataID { get; set; }
    }
}
