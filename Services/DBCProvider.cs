using CASCLib;
using DBCD.Providers;

namespace wow.tools.local.Services
{
    public class DBCProvider : IDBCProvider
    {
        public LocaleFlags localeFlags = LocaleFlags.All_WoW;

        public Stream StreamForTableName(string tableName, string build)
        {
            if (tableName.Contains("."))
                throw new Exception("Invalid DBC name!");

            if (string.IsNullOrEmpty(build))
                throw new Exception("No build given!");

            tableName = tableName.ToLower();

            if (build == CASC.BuildName)
            {
                // Load from CASC
                var fullFileName = "dbfilesclient/" + tableName + ".db2";
                return CASC.GetDB2ByName(fullFileName);
            }
            else
            {
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
                if (!File.Exists(fileName))
                    throw new FileNotFoundException($"Unable to find {tableName}");

                return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

        }
    }
}