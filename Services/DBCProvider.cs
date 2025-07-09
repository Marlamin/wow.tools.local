using CASCLib;
using DBCD.Providers;

namespace wow.tools.local.Services
{
    public class DBCProvider : IDBCProvider
    {
        public LocaleFlags localeFlags = LocaleFlags.All_WoW;
        private static HttpClient webClient = new();
        public bool LoadFromBuildManager { get; set; } = false;

        public bool DB2IsCached(string tableName, string build)
        {
            if (tableName.Contains('.'))
                throw new Exception("Invalid DBC name!");

            if (string.IsNullOrEmpty(build))
                throw new Exception("No build given!");

            tableName = tableName.ToLower();

            var fullFileName = "dbfilesclient/" + tableName + ".db2";

            if (build == CASC.BuildName && CASC.DB2Exists(fullFileName))
                return true;

            // Try from disk
            if (string.IsNullOrEmpty(SettingsManager.DBCFolder))
            {
                Console.WriteLine("DBC folder not set up, can't load DB2 " + tableName + " for build " + build + " from disk");
                throw new FileNotFoundException($"Unable to find {tableName}");
            }

            string fileName = Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient", $"{tableName}.db2");

            // if the db2 variant doesn't exist try dbc
            if (File.Exists(fileName))
                return true;

            fileName = Path.ChangeExtension(fileName, ".dbc");

            // if the dbc variant doesn't exist throw
            if (File.Exists(fileName))
                return true;

            return false;
        }

        public Stream StreamForTableName(string tableName, string build)
        {
            if (tableName.Contains('.'))
                throw new Exception("Invalid DBC name!");

            if (string.IsNullOrEmpty(build))
                throw new Exception("No build given!");

            tableName = tableName.ToLower();

            var fullFileName = "dbfilesclient/" + tableName + ".db2";

            if (build == CASC.BuildName && CASC.DB2Exists(fullFileName))
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
            if (string.IsNullOrEmpty(SettingsManager.DBCFolder))
            {
                Console.WriteLine("DBC folder not set up, can't load DB2 " + tableName + " for build " + build + " from disk");
                throw new FileNotFoundException($"Unable to find {tableName}");
            }

            string fileName = Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient", $"{tableName}.db2");

            // if the db2 variant doesn't exist try dbc
            if (File.Exists(fileName))
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // try dbc on disk
            fileName = Path.ChangeExtension(fileName, ".dbc");

            if (File.Exists(fileName))
                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // if the dbc variant doesn't exist on disk, try BuildManager if enabled
            if (LoadFromBuildManager)
            {
                try
                {
                    var buildInstance = BuildManager.GetBuildByVersion(build);
                    if (buildInstance == null)
                    {
                        Console.WriteLine("Build not found in BuildManager: " + build);
                        throw new FileNotFoundException($"Unable to find {tableName} for {build}");
                    }

                    // Try DB2
                    if (CASC.DB2Map.TryGetValue(fullFileName.ToLower(), out int fileDataID) && buildInstance.Root!.FileExists((uint)fileDataID))
                        return new MemoryStream(buildInstance.OpenFileByFDID((uint)fileDataID));

                    // Try DBC
                    if (CASC.DB2Map.TryGetValue(Path.ChangeExtension(fullFileName, ".dbc").ToLower(), out fileDataID) && buildInstance.Root!.FileExists((uint)fileDataID))
                        return new MemoryStream(buildInstance.OpenFileByFDID((uint)fileDataID));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to load DB2 from BuildManager: " + ex.Message);
                }
            }

            if (CASC.DB2Map.TryGetValue(fullFileName, out int db2fileDataID))
            {
                var db2Req = webClient.GetAsync($"https://wago.tools/api/casc/{db2fileDataID}?download&version=" + build).Result;

                if (db2Req.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var db2Stream = new MemoryStream();
                    db2Req.Content.CopyToAsync(db2Stream).Wait();

                    if (db2Stream.Length > 40)
                    {
                        db2Stream.Position = 0;

                        if (!string.IsNullOrEmpty(SettingsManager.DBCFolder) && !File.Exists(Path.Combine(SettingsManager.DBCFolder, "dbfilesclient", $"{tableName}.db2")))
                        {
                            try
                            {
                                Console.WriteLine("Caching " + tableName + " for build " + build + " to disk..");
                                if (!Directory.Exists(Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient")))
                                    Directory.CreateDirectory(Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient"));

                                var db2File = Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient", $"{tableName}.db2");

                                using (var fileStream = new FileStream(db2File, FileMode.Create, FileAccess.Write))
                                {
                                    db2Stream.CopyTo(fileStream);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error caching DB2 to disk: " + ex.Message);
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

                        if (!string.IsNullOrEmpty(SettingsManager.DBCFolder) && !File.Exists(Path.Combine(SettingsManager.DBCFolder, "dbfilesclient", $"{tableName}.dbc")))
                        {
                            try
                            {
                                Console.WriteLine("Caching " + tableName + " for build " + build + " to disk..");
                                if (!Directory.Exists(Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient")))
                                    Directory.CreateDirectory(Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient"));

                                var db2File = Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient", $"{tableName}.dbc");

                                using (var fileStream = new FileStream(db2File, FileMode.Create, FileAccess.Write))
                                {
                                    dbcStream.CopyTo(fileStream);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error caching DBC to disk: " + ex.Message);
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