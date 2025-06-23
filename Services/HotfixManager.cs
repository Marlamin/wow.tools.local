using DBCD.IO;

namespace wow.tools.local.Services
{
    public static class HotfixManager
    {
        private static Lock HotfixLock = new Lock();
        public static Dictionary<uint, HotfixReader> hotfixReaders = [];
        public static Dictionary<uint, List<DBCacheParser>> dbcacheParsers = [];
        public static HashSet<int> knownPushIDs = [];
        public static Dictionary<int, HashSet<int>> knownPushIDsByRegion = [];
        public static Dictionary<uint, string> tableNames = [];
        public static HashSet<(int pushID, uint recordID, uint tableHash)> knownData = [];
        public static HashSet<string> knownParsedMD5s = [];

        private static void LoadPushIDs()
        {
            if (SQLiteDB.hotfixDBConn.State != System.Data.ConnectionState.Open)
                SQLiteDB.hotfixDBConn.Open();

            knownPushIDs.Clear();
            knownPushIDsByRegion.Clear();
            knownData.Clear();
            knownParsedMD5s.Clear();

            Console.WriteLine("Loading known hotfix push IDs from database..");
            var sql = "SELECT DISTINCT(`PushID`) FROM wow_hotfixes";
            using var cmd = SQLiteDB.hotfixDBConn.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var pushID = reader.GetInt32(0);
                knownPushIDs.Add(pushID);
            }
            Console.WriteLine("Found " + knownPushIDs.Count + " known push IDs.");

            var sql2 = "SELECT DISTINCT(`PushID`), RegionID FROM wow_hotfixpushxbuild";
            using var cmd2 = SQLiteDB.hotfixDBConn.CreateCommand();
            cmd2.CommandText = sql2;
            using var reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                var pushID = reader2.GetInt32(0);
                var regionID = reader2.GetInt32(1);

                if (!knownPushIDsByRegion.ContainsKey(regionID))
                    knownPushIDsByRegion.Add(regionID, []);

                knownPushIDsByRegion[regionID].Add(pushID);
            }
            Console.WriteLine("Found " + knownPushIDsByRegion.Count + " regions with push IDs.");

            var sql3 = "SELECT PushID, RecordID, TableHash FROM wow_hotfixes_data";
            using var cmd3 = SQLiteDB.hotfixDBConn.CreateCommand();
            cmd3.CommandText = sql3;
            using var reader3 = cmd3.ExecuteReader();
            while (reader3.Read())
            {
                var pushID = reader3.GetInt32(0);
                var recordID = reader3.GetInt32(1);
                var tableHash = reader3.GetFieldValue<uint>(2);
                knownData.Add((pushID, (uint)recordID, tableHash));
            }
            Console.WriteLine("Loaded " + knownData.Count + " known hotfix data entries.");

            var sql4 = "SELECT md5 FROM wow_hotfixes_parsed";
            using var cmd4 = SQLiteDB.hotfixDBConn.CreateCommand();
            cmd4.CommandText = sql4;
            using var reader4 = cmd4.ExecuteReader();
            while (reader4.Read())
            {
                var md5 = reader4.GetString(0);
                knownParsedMD5s.Add(md5);
            }

            Console.WriteLine("Loaded " + knownParsedMD5s.Count + " known parsed DBCache hashes.");
        }

        public static void LoadCaches()
        {
            // Cleanup old push IDs file if it exists
            if (File.Exists("knownPushIDs.json"))
                File.Delete("knownPushIDs.json");

            lock (HotfixLock)
            {
                if (tableNames.Count == 0)
                {
                    if (DBDManifest.DB2Map.Count == 0)
                        DBDManifest.Load();

                    foreach (var entry in DBDManifest.DB2Map)
                        tableNames.Add(Hash(entry.Key.ToUpper()), entry.Key);
                }

                LoadPushIDs();

                Console.WriteLine("Reloading all hotfixes..");
                hotfixReaders.Clear();

                if (Directory.Exists("caches"))
                    foreach (var file in Directory.GetFiles("caches", "*.bin", SearchOption.AllDirectories))
                        ParseCache(file);

                if (string.IsNullOrEmpty(SettingsManager.WoWFolder))
                {
                    Console.WriteLine("No WoW folder set, skipping further hotfix load");
                    return;
                }

                foreach (var file in Directory.GetFiles(SettingsManager.WoWFolder, "DBCache.bin", SearchOption.AllDirectories))
                    ParseCache(file);
            }
        }

        public static void ParseCache(string file)
        {
            var reader = new HotfixReader(file);
            if (!hotfixReaders.ContainsKey((uint)reader.BuildId))
                hotfixReaders.Add((uint)reader.BuildId, reader);

            hotfixReaders[(uint)reader.BuildId].CombineCache(file);

            var md5 = Convert.ToHexString(System.Security.Cryptography.MD5.HashData(File.ReadAllBytes(file)));
            if (knownParsedMD5s.Contains(md5))
                return;

            var newCache = new DBCacheParser(file);
            if (!dbcacheParsers.ContainsKey((uint)newCache.build))
                dbcacheParsers.Add((uint)newCache.build, []);

            dbcacheParsers[(uint)newCache.build].Add(newCache);

            if (newCache.hotfixes.Count == 0)
                return;

            var hotfixRegion = (int)newCache.hotfixes.First().regionID;
            if (!knownPushIDsByRegion.ContainsKey(hotfixRegion))
                knownPushIDsByRegion.Add(hotfixRegion, []);

            var newHotfixes = newCache.hotfixes.Where(x => x.pushID > 0 && !knownPushIDsByRegion[hotfixRegion].Contains(x.pushID)).ToList();

            using var transaction = SQLiteDB.hotfixDBConn.BeginTransaction();

            var insertHotfixPushXBuildCMD = SQLiteDB.hotfixDBConn.CreateCommand();
            insertHotfixPushXBuildCMD.Transaction = transaction;
            insertHotfixPushXBuildCMD.CommandText = "INSERT INTO wow_hotfixpushxbuild (PushID, RegionID, Build, DetectedAt) VALUES (@pushID, @regionID, @build, @detectedAt)";
            insertHotfixPushXBuildCMD.Parameters.AddWithValue("@regionID", hotfixRegion);
            insertHotfixPushXBuildCMD.Parameters.AddWithValue("@build", newCache.build);
            insertHotfixPushXBuildCMD.Parameters.AddWithValue("@detectedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            insertHotfixPushXBuildCMD.Parameters.AddWithValue("@pushID", 0);

            var insertHotfixCMD = SQLiteDB.hotfixDBConn.CreateCommand();
            insertHotfixCMD.Transaction = transaction;
            insertHotfixCMD.CommandText = "INSERT INTO wow_hotfixes (pushID, recordID, tableName, isValid, build, region, firstdetected) VALUES (@pushID, @recordID, @tableName, @isValid, @build, @region, @firstdetected)";

            // note: we can do this but its theoretically possible that individual hotfixes have different regions, but whatever for now
            insertHotfixCMD.Parameters.AddWithValue("@region", hotfixRegion);
            insertHotfixCMD.Parameters.AddWithValue("@build", newCache.build);
            insertHotfixCMD.Parameters.AddWithValue("@firstdetected", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // defaults
            insertHotfixCMD.Parameters.AddWithValue("@pushID", 0);
            insertHotfixCMD.Parameters.AddWithValue("@tableName", "");
            insertHotfixCMD.Parameters.AddWithValue("@isValid", 0);
            insertHotfixCMD.Parameters.AddWithValue("@recordID", 0);

            var insertHotfixDataCMD = SQLiteDB.hotfixDBConn.CreateCommand();
            insertHotfixDataCMD.Transaction = transaction;
            insertHotfixDataCMD.CommandText = "INSERT INTO wow_hotfixes_data (pushID, recordID, tableHash, dataAfter) VALUES (@pushID, @recordID, @tableHash, @data)";
            insertHotfixDataCMD.Parameters.AddWithValue("@pushID", 0);
            insertHotfixDataCMD.Parameters.AddWithValue("@tableHash", 0);
            insertHotfixDataCMD.Parameters.AddWithValue("@recordID", 0);
            insertHotfixDataCMD.Parameters.AddWithValue("@data", Array.Empty<byte>());

            var insertParsedCMD = SQLiteDB.hotfixDBConn.CreateCommand();
            insertParsedCMD.Transaction = transaction;
            insertParsedCMD.CommandText = "INSERT INTO wow_hotfixes_parsed (md5) VALUES (@md5)";
            insertParsedCMD.Parameters.AddWithValue("@md5", string.Empty);

            var knownPushIDRecordsInCache = new HashSet<(int pushID, uint recordID, uint tableHash)>();

            foreach (var newHotfix in newHotfixes)
            {
                var tableName = tableNames.TryGetValue(newHotfix.tableHash, out string? value) ? value : "Unknown_" + newHotfix.tableHash.ToString("X8");

                Console.WriteLine("Detected new hotfix for " + tableName + " (ID " + newHotfix.recordID + ") with pushID " + newHotfix.pushID + " for region " + newHotfix.regionID + " at " + DateTime.Now.ToShortTimeString());

                if (!knownPushIDsByRegion[hotfixRegion].Contains(newHotfix.pushID))
                {
                    insertHotfixPushXBuildCMD.Parameters["@pushID"].Value = newHotfix.pushID;

                    try
                    {
                        insertHotfixPushXBuildCMD.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error inserting hotfix push x build: " + ex.Message);
                    }

                    knownPushIDsByRegion[hotfixRegion].Add(newHotfix.pushID);
                }


                if (!knownPushIDs.Contains(newHotfix.pushID))
                {
                    if(knownPushIDRecordsInCache.Contains((newHotfix.pushID, newHotfix.recordID, newHotfix.tableHash)))
                    {
                        Console.WriteLine("Warning: Skipping hotfix with pushID " + newHotfix.pushID + " and recordID " + newHotfix.recordID + " as it was already seen in this push.");
                        continue;
                    }
                    insertHotfixCMD.Parameters["@pushID"].Value = newHotfix.pushID;
                    insertHotfixCMD.Parameters["@tableName"].Value = tableName;
                    insertHotfixCMD.Parameters["@isValid"].Value = newHotfix.status;
                    insertHotfixCMD.Parameters["@recordID"].Value = newHotfix.recordID;

                    try
                    {
                        insertHotfixCMD.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error inserting hotfix: " + ex.Message);
                    }

                    if (newHotfix.data.Length > 0 && newHotfix.pushID != -1)
                    {
                        var knownDataEntry = (newHotfix.pushID, newHotfix.recordID, newHotfix.tableHash);
                        if (!knownData.Contains(knownDataEntry))
                        {
                            insertHotfixDataCMD.Parameters["@pushID"].Value = newHotfix.pushID;
                            insertHotfixDataCMD.Parameters["@recordID"].Value = newHotfix.recordID;
                            insertHotfixDataCMD.Parameters["@tableHash"].Value = newHotfix.tableHash;
                            insertHotfixDataCMD.Parameters["@data"].Value = newHotfix.data;

                            try
                            {
                                insertHotfixDataCMD.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error inserting hotfix data: " + ex.Message);
                            }

                            knownData.Add(knownDataEntry);
                        }
                    }

                    knownPushIDRecordsInCache.Add((newHotfix.pushID, newHotfix.recordID, newHotfix.tableHash));
                }
            }

            // Add to list after done inserting
            foreach (var newHotfix in newHotfixes)
                knownPushIDs.Add(newHotfix.pushID);

            if (!knownParsedMD5s.Contains(md5))
            {
                insertParsedCMD.Parameters["@md5"].Value = md5;
                try
                {
                    insertParsedCMD.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting parsed MD5: " + ex.Message);
                }
                knownParsedMD5s.Add(md5);
            }

            try
            {
                transaction.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with hotfix transation: " + e.Message);
            }

            Console.WriteLine("Loaded hotfixes for build " + reader.BuildId + " from directory " + Path.GetDirectoryName(file));

            // TODO: Cached entries in the future?
        }

        public static void Clear()
        {
            hotfixReaders.Clear();
            dbcacheParsers.Clear();
        }

        public static uint Hash(string s)
        {
            var s_hashtable = new uint[] {
        0x486E26EE, 0xDCAA16B3, 0xE1918EEF, 0x202DAFDB,
        0x341C7DC7, 0x1C365303, 0x40EF2D37, 0x65FD5E49,
        0xD6057177, 0x904ECE93, 0x1C38024F, 0x98FD323B,
        0xE3061AE7, 0xA39B0FA1, 0x9797F25F, 0xE4444563,
        };

            uint v = 0x7fed7fed;
            var x = 0xeeeeeeee;
            for (var i = 0; i < s.Length; i++)
            {
                var c = (byte)s[i];
                v += x;
                v ^= s_hashtable[(c >> 4) & 0xf] - s_hashtable[c & 0xf];
                x = x * 33 + v + c + 3;
            }
            return v;
        }
    }
}