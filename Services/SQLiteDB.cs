using Microsoft.Data.Sqlite;
using static wow.tools.local.Services.Linker;

namespace wow.tools.local.Services
{
    public static class SQLiteDB
    {
        public static SqliteConnection dbConn = new("Data Source=WTL.db");
        public static Dictionary<string, HashSet<int>> newFilesBetweenVersion = new();
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
        }

        public static HashSet<int> getNewFilesBetweenVersions(string oldBuild, string newBuild)
        {
            // Check if valid builds
            if(oldBuild.Split('.').Length != 4 || newBuild.Split('.').Length != 4)
                return new HashSet<int>();

            if(newFilesBetweenVersion.ContainsKey(oldBuild + "|" + newBuild))
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

            if(!File.Exists(Path.Combine("manifests", newBuild + ".txt")))
            {
                Console.WriteLine("Manifest file for build {0} not found, can't compare", newBuild);
                return newFiles;
            }

            var oldBuildFiles = new List<int>();
            foreach(var line in File.ReadAllLines(Path.Combine("manifests", oldBuild + ".txt")))
            {
                var splitLine = line.Split(";");
                if(splitLine.Length != 2)
                    continue;

                oldBuildFiles.Add(int.Parse(splitLine[0]));
            }

            var newBuildFiles = new List<int>();
            foreach(var line in File.ReadAllLines(Path.Combine("manifests", newBuild + ".txt")))
            {
                var splitLine = line.Split(";");
                if(splitLine.Length != 2)
                    continue;

                newBuildFiles.Add(int.Parse(splitLine[0]));
            }

            newFiles = newBuildFiles.Except(oldBuildFiles).ToHashSet();

            if (newFiles.Count > 0)
                newFilesBetweenVersion[oldBuild + "|" + newBuild] = newFiles;
            
            return newFiles;
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
                        fileDataID = uint.Parse(reader["parent"].ToString()),
                        linkType = reader["type"].ToString()
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
                        fileDataID = uint.Parse(reader["child"].ToString()),
                        linkType = reader["type"].ToString()
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
                        buildName = reader["build"].ToString(),
                        contentHash = reader["chash"].ToString()
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

            var manifestPath = Path.Combine(SettingsManager.manifestFolder, buildName + ".txt");
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
    }
}
