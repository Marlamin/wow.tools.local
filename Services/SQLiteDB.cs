﻿using Microsoft.Data.Sqlite;
using WoWNamingLib.Utils;
using static wow.tools.local.Services.Linker;

namespace wow.tools.local.Services
{
    public static class SQLiteDB
    {
        public static SqliteConnection dbConn = new("Data Source=WTL.db");
        public static readonly Dictionary<string, HashSet<int>> newFilesBetweenVersion = [];
        private static readonly Dictionary<int, int> broadcastTextCache = [];
        public static readonly Dictionary<int, int> creatureCache = [];
        public static readonly Dictionary<int, string> VOFDIDToCreatureNameCache = [];
        public static readonly Dictionary<int, List<int>> displayIDToCreatureIDCache = [];
        public static object SQLiteLock = new();

        static SQLiteDB()
        {
            dbConn.Open();

            // wow_rootfiles_links
            var createCmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS wow_rootfiles_links (parent INTEGER, child INTEGER, type TEXT)", dbConn);
            createCmd.ExecuteNonQuery();

            var indexCmd = new SqliteCommand("CREATE UNIQUE INDEX IF NOT EXISTS wow_rootfiles_links_idx ON wow_rootfiles_links (parent, child)", dbConn);
            indexCmd.ExecuteNonQuery();

            // wow_rootfiles_chashes 
            createCmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS wow_rootfiles_chashes (fileDataID INTEGER, build TEXT, chash TEXT)", dbConn);
            createCmd.ExecuteNonQuery();

            indexCmd = new SqliteCommand("CREATE UNIQUE INDEX IF NOT EXISTS wow_rootfiles_chashes_idx ON wow_rootfiles_chashes (fileDataID, chash)", dbConn);
            indexCmd.ExecuteNonQuery();

            // wow_creatures 
            createCmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS wow_creatures (creatureID INTEGER, name TEXT, LastUpdatedBuild INTEGER)", dbConn);
            createCmd.ExecuteNonQuery();

            indexCmd = new SqliteCommand("CREATE UNIQUE INDEX IF NOT EXISTS wow_creatures_idx ON wow_creatures (creatureID)", dbConn);
            indexCmd.ExecuteNonQuery();

            // wow_broadcasttext 
            createCmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS wow_broadcasttext (broadcastTextID INTEGER, Text_lang TEXT, Text1_lang TEXT, SoundKitID0 INTEGER, SoundKitID1 INTEGER, LastUpdatedBuild INTEGER)", dbConn);
            createCmd.ExecuteNonQuery();

            indexCmd = new SqliteCommand("CREATE UNIQUE INDEX IF NOT EXISTS wow_broadcasttext_idx ON wow_broadcasttext (broadcastTextID)", dbConn);
            indexCmd.ExecuteNonQuery();

            // wow_broadcasttext_creature
            createCmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS wow_files_creature (fileDataID INTEGER, creature TEXT)", dbConn);
            createCmd.ExecuteNonQuery();

            indexCmd = new SqliteCommand("CREATE UNIQUE INDEX IF NOT EXISTS wow_files_creature_idx ON wow_files_creature (fileDataID)", dbConn);
            indexCmd.ExecuteNonQuery();

            // wow_displayids_creature
            createCmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS wow_displayids_creature (displayID INTEGER, creatureID INTEGER)", dbConn);
            createCmd.ExecuteNonQuery();

            indexCmd = new SqliteCommand("CREATE UNIQUE INDEX IF NOT EXISTS wow_displayids_creature_idx ON wow_displayids_creature (displayID, creatureID)", dbConn);
            indexCmd.ExecuteNonQuery();

            // prepare broadcastTextCache
            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT broadcastTextID, LastUpdatedBuild FROM wow_broadcasttext";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    broadcastTextCache[int.Parse(reader["broadcastTextID"].ToString()!)] = int.Parse(reader["LastUpdatedBuild"].ToString()!);
                }

                reader.Close();
            }

            // prepare creatureCache
            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT creatureID, LastUpdatedBuild FROM wow_creatures";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    creatureCache[int.Parse(reader["creatureID"].ToString()!)] = int.Parse(reader["LastUpdatedBuild"].ToString()!);
                }

                reader.Close();
            }

            // prepare VOFDIDToCreatureNameCache
            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT fileDataID, creature FROM wow_files_creature";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    VOFDIDToCreatureNameCache[int.Parse(reader["fileDataID"].ToString()!)] = reader["creature"].ToString()!;
                }
            }

            // prepare displayIDToCreatureIDCache
            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT displayID, creatureID FROM wow_displayids_creature";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (!displayIDToCreatureIDCache.ContainsKey(int.Parse(reader["displayID"].ToString()!)))
                    {
                        displayIDToCreatureIDCache[int.Parse(reader["displayID"].ToString()!)] = [];
                    }

                    displayIDToCreatureIDCache[int.Parse(reader["displayID"].ToString()!)].Add(int.Parse(reader["creatureID"].ToString()!));
                }
            }
        }

        public static void InsertOrUpdateCreature(int creatureID, string name, int build)
        {
            var insertNew = false;
            var updateExisting = false;

            if (!creatureCache.TryGetValue(creatureID, out var cachedBuild))
            {
                insertNew = true;
                updateExisting = false;
            }
            else if (cachedBuild < build)
            {
                updateExisting = true;
            }
            else if (cachedBuild > build)
            {
                updateExisting = false;
            }

            if (insertNew)
            {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO wow_creatures (creatureID, name, LastUpdatedBuild) VALUES (@creatureID, @name, @build)";
                    cmd.Parameters.AddWithValue("@creatureID", creatureID);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@build", build);
                    cmd.ExecuteNonQuery();

                    creatureCache[creatureID] = build;
                }
            }
            else if (updateExisting)
            {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE wow_creatures SET name = @name, LastUpdatedBuild = @build WHERE creatureID = @creatureID";
                    cmd.Parameters.AddWithValue("@creatureID", creatureID);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@build", build);
                    cmd.ExecuteNonQuery();
                }

                creatureCache[creatureID] = build;
            }
        }

        public static void InsertOrUpdateDisplayIDToCreatureID(int displayID, int creatureID)
        {
            if (displayIDToCreatureIDCache.TryGetValue(displayID, out var cachedCreatureIDs) && cachedCreatureIDs.Contains(creatureID))
                return;

            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO wow_displayids_creature (displayID, creatureID) VALUES (@displayID, @creatureID)";
                cmd.Parameters.AddWithValue("@displayID", displayID);
                cmd.Parameters.AddWithValue("@creatureID", creatureID);
                cmd.ExecuteNonQuery();

                if (!displayIDToCreatureIDCache.ContainsKey(displayID))
                    displayIDToCreatureIDCache[displayID] = [];

                displayIDToCreatureIDCache[displayID].Add(creatureID);
            }
        }

        public static void InsertOrUpdateBroadcastText(int broadcastTextID, string text_lang, string text1_lang, int soundKitID0, int soundKitID1, int build)
        {
            var insertNew = false;
            var updateExisting = false;

            if (!broadcastTextCache.TryGetValue(broadcastTextID, out var cachedBuild))
            {
                insertNew = true;
                updateExisting = false;
            }
            else if (cachedBuild < build)
            {
                updateExisting = true;
            }
            else if (cachedBuild > build)
            {
                updateExisting = false;
            }

            if (insertNew)
            {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO wow_broadcasttext (broadcastTextID, Text_lang, Text1_lang, SoundKitID0, SoundKitID1, LastUpdatedBuild) VALUES (@broadcastTextID, @Text_lang, @Text1_lang, @SoundKitID0, @SoundKitID1, @Build)";
                    cmd.Parameters.AddWithValue("@broadcastTextID", broadcastTextID);
                    cmd.Parameters.AddWithValue("@Text_lang", text_lang);
                    cmd.Parameters.AddWithValue("@Text1_lang", text1_lang);
                    cmd.Parameters.AddWithValue("@SoundKitID0", soundKitID0);
                    cmd.Parameters.AddWithValue("@SoundKitID1", soundKitID1);
                    cmd.Parameters.AddWithValue("@Build", build);
                    cmd.ExecuteNonQuery();

                    broadcastTextCache[broadcastTextID] = build;
                }
            }
            else if (updateExisting)
            {
                var needsFullUpdate = false;

                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Text_lang, Text1_lang, SoundKitID0, SoundKitID1 FROM wow_broadcasttext WHERE broadcastTextID = @broadcastTextID";
                    cmd.Parameters.AddWithValue("@broadcastTextID", broadcastTextID);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var currentText_lang = reader["Text_lang"].ToString();
                        var currentText1_lang = reader["Text1_lang"].ToString();
                        var currentSoundKitID0 = int.Parse(reader["SoundKitID0"].ToString()!);
                        var currentSoundKitID1 = int.Parse(reader["SoundKitID1"].ToString()!);

                        if (currentText_lang != text_lang)
                        {
                            Console.WriteLine(build + " " + broadcastTextID + " Text_lang " + reader["Text_lang"].ToString() + " != " + text_lang);
                            needsFullUpdate = true;

                        }
                        else if (currentText1_lang != text1_lang)
                        {
                            Console.WriteLine(build + " " + broadcastTextID + " Text1_lang " + reader["Text1_lang"].ToString() + " != " + text1_lang);
                            needsFullUpdate = true;
                        }
                        else if (currentSoundKitID0 != soundKitID0)
                        {
                            Console.WriteLine(build + " " + broadcastTextID + " SoundKitID0 " + reader["SoundKitID0"].ToString() + " != " + soundKitID0);
                            needsFullUpdate = true;
                        }
                        else if (currentSoundKitID1 != soundKitID1)
                        {
                            Console.WriteLine(build + " " + broadcastTextID + " SoundKitID1 " + reader["SoundKitID1"].ToString() + " != " + soundKitID1);
                            needsFullUpdate = true;
                        }
                    }

                    reader.Close();
                }

                if (needsFullUpdate)
                {
                    using (var cmd = dbConn.CreateCommand())
                    {
                        cmd.CommandText = "REPLACE INTO wow_broadcasttext (broadcastTextID, Text_lang, Text1_lang, SoundKitID0, SoundKitID1, LastUpdatedBuild) VALUES (@broadcastTextID, @Text_lang, @Text1_lang, @SoundKitID0, @SoundKitID1, @Build)";
                        cmd.Parameters.AddWithValue("@broadcastTextID", broadcastTextID);
                        cmd.Parameters.AddWithValue("@Text_lang", text_lang);
                        cmd.Parameters.AddWithValue("@Text1_lang", text1_lang);
                        cmd.Parameters.AddWithValue("@SoundKitID0", soundKitID0);
                        cmd.Parameters.AddWithValue("@SoundKitID1", soundKitID1);
                        cmd.Parameters.AddWithValue("@Build", build);
                        cmd.ExecuteNonQuery();
                    }
                }

                broadcastTextCache[broadcastTextID] = build;
            }
        }

        public static Dictionary<string, List<uint>> GetTextToSoundKitIDs()
        {
            var textToSoundKitID = new Dictionary<string, List<uint>>();

            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT Text_lang, Text1_lang, SoundKitID0, SoundKitID1 FROM wow_broadcasttext";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var Text_lang = reader["Text_lang"].ToString();
                    var Text1_lang = reader["Text1_lang"].ToString();
                    var soundKitID0 = uint.Parse(reader["SoundKitID0"].ToString()!);
                    var soundKitID1 = uint.Parse(reader["SoundKitID1"].ToString()!);

                    if (soundKitID0 != 0 && !string.IsNullOrEmpty(Text_lang))
                    {
                        if (!textToSoundKitID.ContainsKey(Text_lang))
                            textToSoundKitID[Text_lang] = [];

                        textToSoundKitID[Text_lang].Add(soundKitID0);
                    }

                    if (soundKitID1 != 0 && !string.IsNullOrEmpty(Text1_lang))
                    {
                        if (!textToSoundKitID.ContainsKey(Text1_lang))
                            textToSoundKitID[Text1_lang] = [];

                        textToSoundKitID[Text1_lang].Add(soundKitID1);
                    }
                }

                reader.Close();
            }

            return textToSoundKitID;
        }

        public static bool SetCreatureNameForFDID(int fileDataID, string creatureName)
        {
            if (VOFDIDToCreatureNameCache.TryGetValue(fileDataID, out var cachedCreatureName) && cachedCreatureName == creatureName)
            {
                return false;
            }

            if (creatureName == "" || creatureName == null) // yes this happens
            {
                // If creature name is empty -- delete it
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM wow_files_creature WHERE fileDataID = @fileDataID";
                    cmd.Parameters.AddWithValue("@fileDataID", fileDataID);
                    VOFDIDToCreatureNameCache.Remove(fileDataID);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            else
            {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "INSERT OR REPLACE INTO wow_files_creature (fileDataID, creature) VALUES (@fileDataID, @creature)";
                    cmd.Parameters.AddWithValue("@fileDataID", fileDataID);
                    cmd.Parameters.AddWithValue("@creature", creatureName);
                    VOFDIDToCreatureNameCache[fileDataID] = creatureName;
                    return cmd.ExecuteNonQuery() > 0;
                }
            }

        }

        public static Dictionary<uint, (uint, uint)> GetBroadcastTextIDToSoundKitIDs()
        {
            var BroadcastTextIDToSoundKitIDs = new Dictionary<uint, (uint, uint)>();

            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT SoundKitID0,SoundKitID1, broadcastTextID FROM wow_broadcasttext WHERE SoundKitID0 != 0 OR SoundKitID1 != 0";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var broadcastTextID = uint.Parse(reader["broadcastTextID"].ToString()!);
                    var soundKitID0 = uint.Parse(reader["SoundKitID0"].ToString()!);
                    var soundKitID1 = uint.Parse(reader["SoundKitID1"].ToString()!);
                    BroadcastTextIDToSoundKitIDs.Add(broadcastTextID, (soundKitID0, soundKitID1));
                }

                reader.Close();
            }


            return BroadcastTextIDToSoundKitIDs;
        }

        public static Dictionary<uint, List<uint>> GetSoundKitToBCTextIDs()
        {
            var SoundKitIDToBCTextID = new Dictionary<uint, List<uint>>();

            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT SoundKitID0,SoundKitID1, broadcastTextID FROM wow_broadcasttext WHERE SoundKitID0 != 0 OR SoundKitID1 != 0";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var broadcastTextID = uint.Parse(reader["broadcastTextID"].ToString()!);
                    var soundKitID0 = uint.Parse(reader["SoundKitID0"].ToString()!);
                    if (soundKitID0 != 0)
                    {
                        if (!SoundKitIDToBCTextID.ContainsKey(soundKitID0))
                            SoundKitIDToBCTextID[soundKitID0] = [];

                        if (!SoundKitIDToBCTextID[soundKitID0].Contains(broadcastTextID))
                            SoundKitIDToBCTextID[soundKitID0].Add(broadcastTextID);
                    }

                    var soundKitID1 = uint.Parse(reader["SoundKitID1"].ToString()!);
                    if (soundKitID1 != 0)
                    {
                        if (!SoundKitIDToBCTextID.ContainsKey(soundKitID1))
                            SoundKitIDToBCTextID[soundKitID1] = [];

                        if (!SoundKitIDToBCTextID[soundKitID1].Contains(broadcastTextID))
                            SoundKitIDToBCTextID[soundKitID1].Add(broadcastTextID);
                    }
                }

                reader.Close();
            }


            return SoundKitIDToBCTextID;
        }

        public static int GetCreatureCount()
        {
            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM wow_creatures";
                return int.Parse(cmd.ExecuteScalar()!.ToString()!);
            }
        }

        public static Dictionary<uint, string> GetCreatureToFDIDMap()
        {
            return VOFDIDToCreatureNameCache.ToDictionary(x => (uint)x.Key, x => x.Value);
        }

        public static Dictionary<uint, string> GetCreatureNames(int start = -1, int count = -1)
        {
            var creatures = new Dictionary<uint, string>();

            using (var cmd = dbConn.CreateCommand())
            {
                if (start != -1 && count != -1)
                {
                    cmd.CommandText = "SELECT creatureID, name FROM wow_creatures ORDER BY creatureID ASC LIMIT @start, @count ";
                    cmd.Parameters.AddWithValue("@start", start);
                    cmd.Parameters.AddWithValue("@count", count);
                }
                else
                {
                    cmd.CommandText = "SELECT creatureID, name FROM wow_creatures";
                }

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    creatures[uint.Parse(reader["creatureID"].ToString()!)] = reader["name"].ToString()!;
                }

                reader.Close();
            }

            return creatures;
        }

        public static string GetCreatureNameByID(int creatureID)
        {
            lock (SQLiteLock)
            {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM wow_creatures WHERE creatureID = @creatureID";
                    cmd.Parameters.AddWithValue("@creatureID", creatureID);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        return reader["name"].ToString()!;
                    }
                }

                return "";
            }
        }

        public static HashSet<int> getNewFilesBetweenVersions(string oldBuild, string newBuild)
        {
            // Check if valid builds
            if (oldBuild.Split('.').Length != 4 || newBuild.Split('.').Length != 4)
                return [];

            if (newFilesBetweenVersion.ContainsKey(oldBuild + "|" + newBuild))
            {
                return newFilesBetweenVersion[oldBuild + "|" + newBuild];
            }

            // TODO: Store this in SQLite propery, right now we only store changed files between versions
            //using (var cmd = dbConn.CreateCommand())
            //{
            //    cmd.CommandText = "SELECT fileDataID FROM wow_rootfiles_chashes WHERE build = @newBuild AND fileDataID NOT IN (SELECT fileDataID FROM wow_rootfiles_chashes WHERE build = @oldBuild)";
            //    cmd.Parameters.AddWithValue("@oldBuild", oldBuild);
            //    cmd.Parameters.AddWithValue("@newBuild", newBuild);

            //    var reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        newFiles.Add(int.Parse(reader["fileDataID"].ToString()));
            //    }

            //    reader.Close();
            var newFiles = new HashSet<int>();

            if (!File.Exists(Path.Combine("manifests", oldBuild + ".txt")))
            {
                Console.WriteLine("Manifest file for build {0} not found, can't compare", oldBuild);
                return newFiles;
            }

            if (!File.Exists(Path.Combine("manifests", newBuild + ".txt")))
            {
                Console.WriteLine("Manifest file for build {0} not found, can't compare", newBuild);
                return newFiles;
            }

            var oldBuildFiles = new List<int>();
            foreach (var line in File.ReadAllLines(Path.Combine("manifests", oldBuild + ".txt")))
            {
                var splitLine = line.Split(";");
                if (splitLine.Length != 2)
                    continue;

                oldBuildFiles.Add(int.Parse(splitLine[0]));
            }

            var newBuildFiles = new List<int>();
            foreach (var line in File.ReadAllLines(Path.Combine("manifests", newBuild + ".txt")))
            {
                var splitLine = line.Split(";");
                if (splitLine.Length != 2)
                    continue;

                newBuildFiles.Add(int.Parse(splitLine[0]));
            }

            newFiles = newBuildFiles.Except(oldBuildFiles).ToHashSet();

            if (newFiles.Count > 0)
                newFilesBetweenVersion[oldBuild + "|" + newBuild] = newFiles;

            return newFiles;
        }

        public static string GetBroadcastTextByID(int broadcastTextID)
        {
            lock (SQLiteLock)
            {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Text_lang, Text1_Lang FROM wow_broadcasttext WHERE broadcastTextID = @broadcastTextID";
                    cmd.Parameters.AddWithValue("@broadcastTextID", broadcastTextID);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!string.IsNullOrWhiteSpace(reader["Text_lang"].ToString()))
                            return reader["Text_lang"].ToString()!;
                        else if (!string.IsNullOrWhiteSpace(reader["Text1_lang"].ToString()))
                            return reader["Text1_lang"].ToString()!;
                    }
                }

                return "";
            }
        }

        public static List<uint> SearchBroadcastText(string search)
        {
            lock (SQLiteLock)
            {
                var results = new List<uint>();

                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT broadcastTextID FROM wow_broadcasttext WHERE Text_lang LIKE @search OR Text1_lang LIKE @search";
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var broadcastTextID = uint.Parse(reader["broadcastTextID"].ToString()!);
                        if (broadcastTextID != 0)
                            results.Add(broadcastTextID);
                    }
                }

                return results;
            }
        }

        public static string getCreatureNameByFileDataID(int fileDataID)
        {
            if (VOFDIDToCreatureNameCache.TryGetValue(fileDataID, out var cachedCreatureName))
            {
                return cachedCreatureName;
            }
            else
            {
                return "";
            }
        }

        public static List<LinkedFile> GetParentFiles(int fileDataID)
        {
            var files = new List<LinkedFile>();

            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT parent, type FROM wow_rootfiles_links WHERE child = @child";
                cmd.Parameters.AddWithValue("@child", fileDataID);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    files.Add(new LinkedFile()
                    {
                        fileDataID = uint.Parse(reader["parent"].ToString()!),
                        linkType = reader["type"].ToString()!
                    });
                }

                reader.Close();
            }

            return files;
        }

        public static List<LinkedFile> GetFilesByParent(int fileDataID)
        {
            var files = new List<LinkedFile>();

            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT child, type FROM wow_rootfiles_links WHERE parent = @parent";
                cmd.Parameters.AddWithValue("@parent", fileDataID);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    files.Add(new LinkedFile()
                    {
                        fileDataID = uint.Parse(reader["child"].ToString()!),
                        linkType = reader["type"].ToString()!
                    });
                }

                reader.Close();
            }

            return files;
        }

        public static List<CASC.Version> GetFileVersions(int fileDataID)
        {
            var versions = new List<CASC.Version>();

            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT build, chash FROM wow_rootfiles_chashes WHERE fileDataID = @fileDataID";
                cmd.Parameters.AddWithValue("@fileDataID", fileDataID);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    versions.Add(new CASC.Version()
                    {
                        buildName = reader["build"].ToString()!,
                        contentHash = reader["chash"].ToString()!
                    });
                }

                reader.Close();
            }

            // Sort by build
            versions.Sort((x, y) => int.Parse(x.buildName.Split(".")[3]).CompareTo(int.Parse(y.buildName.Split(".")[3])));

            return versions;
        }

        public static uint GetFirstVersionNumberByFileDataID(int fileDataID)
        {
            var firstVersion = GetFileVersions(fileDataID);
            if (firstVersion.Count == 0)
                return 0;

            var firstBuild = firstVersion[0].buildName.Split('.');

            return (uint)((int.Parse(firstBuild[0]) * 10000) + (int.Parse(firstBuild[1]) * 100) + int.Parse(firstBuild[2]));
        }

        public static void ImportBuildIntoFileHistory(string buildName)
        {
            Console.WriteLine("Importing build {0} into file history", buildName);

            var transaction = dbConn.BeginTransaction();

            var insertCmd = new SqliteCommand("INSERT INTO wow_rootfiles_chashes VALUES (@filedataid, @build, @chash)", SQLiteDB.dbConn);
            insertCmd.Parameters.AddWithValue("@filedataid", 0);
            insertCmd.Parameters.AddWithValue("@build", buildName);
            insertCmd.Parameters.AddWithValue("@chash", "");
            insertCmd.Transaction = transaction;
            insertCmd.Prepare();

            var manifestPath = Path.Combine(SettingsManager.ManifestFolder, buildName + ".txt");
            if (!File.Exists(manifestPath))
            {
                Console.WriteLine("Manifest file for build {0} not found, can't import", buildName);
                return;
            }

            foreach (var line in File.ReadAllLines(manifestPath))
            {
                var splitLine = line.Split(";");
                if (splitLine.Length != 2)
                    continue;

                var fileDataID = int.Parse(splitLine[0]);
                var fileHash = splitLine[1];

                var fileVersions = GetFileVersions(fileDataID);

                // Don't insert if hash for this file is already known
                if (fileVersions.Any(x => x.contentHash == fileHash))
                    continue;

                insertCmd.Parameters["@filedataid"].Value = fileDataID;
                insertCmd.Parameters["@chash"].Value = fileHash;
                insertCmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public static void ClearLinks()
        {
            new SqliteCommand("DELETE FROM wow_rootfiles_links", dbConn).ExecuteNonQuery();
        }

        public static void ClearHistory()
        {
            new SqliteCommand("DELETE FROM wow_rootfiles_chashes", dbConn).ExecuteNonQuery();
        }

        public static string GetCreatureNameByDisplayID(int displayID)
        {
            if (displayIDToCreatureIDCache.TryGetValue(displayID, out var creatureIDs))
            {
                return GetCreatureNameByID(creatureIDs[0]);
            }

            return "";
        }
    }
}
