using CASCLib;
using DBCD;
using DBCD.Providers;
using DBFileReaderLib;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using wow.tools.Services;

namespace wow.tools.local.Services
{
    public class DBCManager(IDBDProvider dbdProvider) : IDBCManager
    {
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;

        private MemoryCache Cache = new(new MemoryCacheOptions() { SizeLimit = 250 });
        private readonly ConcurrentDictionary<(string, string, bool), SemaphoreSlim> Locks = [];

        public async Task<IDBCDStorage> GetOrLoad(string name, string build)
        {
            return await GetOrLoad(name, build, false);
        }

        public async Task<IDBCDStorage> GetOrLoad(string name, string build, bool useHotfixes = false, LocaleFlags locale = LocaleFlags.All_WoW, List<int> pushIDFilter = null)
        {
            if (locale != LocaleFlags.All_WoW)
            {
                return LoadDBC(name, build, useHotfixes, locale);
            }

            if (pushIDFilter != null)
            {
                return LoadDBC(name, build, useHotfixes, locale, pushIDFilter);
            }

            if (Cache.TryGetValue((name, build, useHotfixes), out IDBCDStorage cachedDBC))
                return cachedDBC;

            SemaphoreSlim mylock = Locks.GetOrAdd((name, build, useHotfixes), k => new SemaphoreSlim(1, 1));

            await mylock.WaitAsync();

            try
            {
                if (!Cache.TryGetValue((name, build, useHotfixes), out cachedDBC))
                {
                    // Key not in cache, load DBC
                    Logger.WriteLine("DBC " + name + " for build " + build + " (hotfixes: " + useHotfixes + ") is not cached, loading!");
                    cachedDBC = LoadDBC(name, build, useHotfixes);
                    Cache.Set((name, build, useHotfixes), cachedDBC, new MemoryCacheEntryOptions().SetSize(1));
                }
            }
            finally
            {
                mylock.Release();
            }

            return cachedDBC;
        }

        private IDBCDStorage LoadDBC(string name, string build, bool useHotfixes = false, LocaleFlags locale = LocaleFlags.All_WoW, List<int> pushIDFilter = null)
        {
            var dbcProvider = new DBCProvider();
            if (locale != LocaleFlags.All_WoW)
            {
                dbcProvider.localeFlags = locale;
            }

            var dbcd = new DBCD.DBCD(dbcProvider, dbdProvider);
            var storage = dbcd.Load(name, build);

            dbcProvider.localeFlags = LocaleFlags.All_WoW;

            var splitBuild = build.Split('.');

            if (splitBuild.Length != 4)
            {
                throw new Exception("Invalid build!");
            }

            var buildNumber = uint.Parse(splitBuild[3]);

            if (!useHotfixes)
                return storage;

            if (HotfixManager.hotfixReaders.Count == 0)
                HotfixManager.LoadCaches();

            if (HotfixManager.hotfixReaders.TryGetValue(buildNumber, out HotfixReader? hotfixReaders))
            {
                // DBCD PR #17 support
                if (pushIDFilter != null)
                {
                    storage = storage.ApplyingHotfixes(hotfixReaders, (row, shouldDelete) =>
                    {
                        if (!pushIDFilter.Contains(row.PushId))
                            return RowOp.Ignore;

                        return HotfixReader.DefaultProcessor(row, shouldDelete);
                    });
                }
                else
                {
                    storage = storage.ApplyingHotfixes(hotfixReaders);
                }
            }

            return storage;
        }

        public void ClearCache()
        {
            Cache.Dispose();
            Cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = 250 });
        }

        public void ClearHotfixCache()
        {
            // TODO: Only clear hotfix caches? :(
            Cache.Dispose();
            Cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = 250 });
        }

        public List<string> GetDBCNames(string? build = null)
        {
            var existingDB2s = new List<string>();

            foreach (var db2 in dbdProvider.GetNames())
            {
                if (string.IsNullOrEmpty(build) || build == CASC.BuildName)
                {
                    if (CASC.DB2Exists("DBFilesClient/" + db2 + ".db2"))
                        existingDB2s.Add(db2);
                }
                else if (!string.IsNullOrEmpty(SettingsManager.dbcFolder))
                {
                    string fileName = Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient", db2 + ".db2");

                    if (File.Exists(fileName))
                        existingDB2s.Add(db2);

                    fileName = Path.ChangeExtension(fileName, ".dbc");

                    if (File.Exists(fileName))
                        existingDB2s.Add(db2);
                }
            }

            return [.. existingDB2s.Order()];
        }

        public async Task<List<DBCDRow>> FindRecords(string name, string build, string col, int val, bool single = false)
        {
            var rowList = new List<DBCDRow>();

            var storage = await GetOrLoad(name, build);
            if (col == "ID")
            {
                if (storage.TryGetValue(val, out var row))
                {
                    foreach (var fieldName in storage.AvailableColumns)
                    {
                        if (fieldName != col)
                            continue;

                        // Don't think FKs to arrays are possible, so only check regular value
                        if (row[fieldName].ToString() == val.ToString())
                        {
                            rowList.Add(row);
                            if (single)
                                return rowList;
                        }
                    }
                }
            }
            else
            {
                foreach (var row in storage.Values)
                {
                    foreach (var fieldName in storage.AvailableColumns)
                    {
                        if (fieldName != col)
                            continue;

                        // Don't think FKs to arrays are possible, so only check regular value
                        if (row[fieldName].ToString() == val.ToString())
                        {
                            rowList.Add(row);
                            if (single)
                                return rowList;
                        }
                    }
                }
            }

            return rowList;
        }
    }
}