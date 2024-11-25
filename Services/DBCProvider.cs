using CASCLib;
using DBCD.Providers;

namespace wow.tools.local.Services
{
    public class DBCProvider : IDBCProvider
    {
        public LocaleFlags localeFlags = LocaleFlags.All_WoW;
        private static HttpClient webClient = new();

        public Stream StreamForTableName(string tableName, string build)
        {
            if (tableName.Contains("."))
                throw new Exception("Invalid DBC name!");

            if (string.IsNullOrEmpty(build))
                throw new Exception("No build given!");

            tableName = tableName.ToLower();

            var fullFileName = "dbfilesclient/" + tableName + ".db2";

            if (build == CASC.BuildName)
            {
                // Load from CASC
                try
                {
                    return CASC.GetDB2ByName(fullFileName);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("Unable to extract DB2 from CASC (" + e.Message + "), trying disk..");
                }
            }

            // Try from disk
            if (string.IsNullOrEmpty(SettingsManager.dbcFolder))
            {
                Console.WriteLine("DBC folder not set up, can't load DB2 " + tableName + " for build " + build + " from disk");
                throw new FileNotFoundException($"Unable to find {tableName}");
            }

            string fileName = Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient", $"{tableName}.db2");

            // if the db2 variant doesn't exist try dbc
            if (File.Exists(fileName))
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            fileName = Path.ChangeExtension(fileName, ".dbc");

            // if the dbc variant doesn't exist throw
            if (File.Exists(fileName)) 
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (CASC.DB2Map.TryGetValue(fullFileName, out int db2fileDataID))
            {
                var db2Req = webClient.GetAsync($"https://wago.tools/api/casc/{db2fileDataID}?version=" + build).Result;

                if (db2Req.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var db2Stream = new MemoryStream();
                    db2Req.Content.CopyToAsync(db2Stream).Wait();

                    if (db2Stream.Length > 40)
                    {
                        db2Stream.Position = 0;

                        if (!string.IsNullOrEmpty(SettingsManager.dbcFolder) && !File.Exists(Path.Combine(SettingsManager.dbcFolder, "dbfilesclient", $"{tableName}.db2")))
                        {
                            Console.WriteLine("Caching " + tableName + " for build " + build + " to disk..");
                            if (!Directory.Exists(Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient")))
                                Directory.CreateDirectory(Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient"));

                            var db2File = Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient", $"{tableName}.db2");

                            using (var fileStream = new FileStream(db2File, FileMode.Create, FileAccess.Write))
                            {
                                db2Stream.CopyTo(fileStream);
                            }

                            db2Stream.Position = 0;
                        }

                        return db2Stream;
                    }
                }
            }

            if (CASC.DB2Map.TryGetValue(Path.ChangeExtension(fullFileName, ".dbc"), out int dbcfileDataID))
            {
                var dbcReq = webClient.GetAsync($"https://wago.tools/api/casc/{dbcfileDataID}?version=" + build).Result;

                if (dbcReq.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var dbcStream = new MemoryStream();
                    dbcReq.Content.CopyToAsync(dbcStream).Wait();

                    if (dbcStream.Length > 40)
                    {
                        dbcStream.Position = 0;

                        if (!string.IsNullOrEmpty(SettingsManager.dbcFolder) && !File.Exists(Path.Combine(SettingsManager.dbcFolder, "dbfilesclient", $"{tableName}.dbc")))
                        {
                            Console.WriteLine("Caching " + tableName + " for build " + build + " to disk..");
                            if (!Directory.Exists(Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient")))
                                Directory.CreateDirectory(Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient"));

                            var db2File = Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient", $"{tableName}.dbc");

                            using (var fileStream = new FileStream(db2File, FileMode.Create, FileAccess.Write))
                            {
                                dbcStream.CopyTo(fileStream);
                            }

                            dbcStream.Position = 0;
                        }

                        return dbcStream;
                    }
                }
            }

            throw new FileNotFoundException($"Unable to find {tableName} for {build}");
        }
    }
}