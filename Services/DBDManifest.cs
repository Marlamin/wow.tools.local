using System.Text.Json;
using System.Text.Json.Serialization;

namespace wow.tools.local.Services
{
    public static class DBDManifest
    {
        public static Dictionary<string, ManifestEntry> DB2Map = new(StringComparer.InvariantCultureIgnoreCase);
        public static void Load(bool forceNew = false)
        {
            var manifestLocation = Path.GetFullPath(Path.Combine(SettingsManager.DefinitionDir, "..", "manifest.json"));

            if (string.IsNullOrEmpty(SettingsManager.DefinitionDir) || !File.Exists(manifestLocation))
            {
                Console.WriteLine("DBD directory not set, using remote manifest (cached up to a day)");

                if(!Directory.Exists("cache"))
                    Directory.CreateDirectory("cache");

                var downloadManifest = false;
                var cacheLocation = Path.Combine("cache", "manifest.json");
                var fileInfo = new FileInfo(cacheLocation);

                if (fileInfo.Exists)
                {
                    if(DateTime.Now > fileInfo.LastWriteTime.AddDays(1))
                        downloadManifest = true;
                    else
                        manifestLocation = cacheLocation;
                }
                else
                {
                    downloadManifest = true;
                }

                if(forceNew)
                    downloadManifest = true;

                if (downloadManifest)
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "wow.tools.local");
                        var response = client.GetAsync("https://github.com/wowdev/WoWDBDefs/releases/latest/download/manifest.json").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            File.WriteAllBytes(cacheLocation, response.Content.ReadAsByteArrayAsync().Result);
                            manifestLocation = cacheLocation;
                        }
                        else
                            Console.WriteLine("Failed to download manifest.json from GitHub: " + response.StatusCode.ToString());
                    }
                }
            }

            if(!File.Exists(manifestLocation))
            {
                Console.WriteLine("Manifest.json not found, hotfixes and some other features might misbehave.");
                return;
            }

            var manifest = JsonSerializer.Deserialize<List<ManifestEntry>>(File.ReadAllText(manifestLocation));

            DB2Map.Clear();

            if(manifest != null)
            {
                foreach (var entry in manifest)
                {
                    if (!Listfile.DB2Map.ContainsKey("dbfilesclient/" + entry.tableName.ToLower() + ".db2"))
                        Listfile.DB2Map.Add("dbfilesclient/" + entry.tableName.ToLower() + ".db2", entry.db2FileDataID);

                    if (!Listfile.DB2Map.ContainsKey("dbfilesclient/" + entry.tableName.ToLower() + ".dbc"))
                        Listfile.DB2Map.Add("dbfilesclient/" + entry.tableName.ToLower() + ".dbc", entry.dbcFileDataID);

                    DB2Map.TryAdd(entry.tableName, entry);
                }
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
