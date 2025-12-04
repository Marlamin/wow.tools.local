using AsyncKeyedLock;
using CASCLib;
using DBCD;
using DBCD.IO;
using DBCD.Providers;
using Microsoft.Extensions.Caching.Memory;
using wow.tools.Services;

namespace wow.tools.local.Services
{
    public class DBCManager(IDBDProvider dbdProvider, IDBCProvider dbcProvider) : IDBCManager
    {
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;
        private readonly DBCProvider dbcProvider = (DBCProvider)dbcProvider;

        private MemoryCache Cache = new(new MemoryCacheOptions() { SizeLimit = 250 });
        private readonly AsyncKeyedLocker<(string, string, bool, LocaleFlags)> Locks = new();

        public async Task<IDBCDStorage> GetOrLoad(string name, string build)
        {
            return await GetOrLoad(name, build, false);
        }

        public async Task<IDBCDStorage> GetOrLoad(string name, string build, bool useHotfixes = false, LocaleFlags locale = LocaleFlags.All_WoW, List<int>? pushIDFilter = null)
        {
            if (locale != LocaleFlags.All_WoW)
            {
                return LoadDBC(name, build, useHotfixes, locale);
            }

            if (pushIDFilter != null)
            {
                return LoadDBC(name, build, useHotfixes, locale, pushIDFilter);
            }

            if (Cache.TryGetValue((name, build, useHotfixes, locale), out var cachedDBC))
                return (DBCD.IDBCDStorage)cachedDBC!;

            using (await Locks.LockAsync((name, build, useHotfixes, locale)))
            {
                if (!Cache.TryGetValue((name, build, useHotfixes, locale), out cachedDBC))
                {
                    // Key not in cache, load DBC
                    Console.WriteLine("DBC " + name + " for build " + build + " (hotfixes: " + useHotfixes + ") is not cached, loading!");
                    cachedDBC = LoadDBC(name, build, useHotfixes, locale);
                    Cache.Set((name, build, useHotfixes, locale), cachedDBC, new MemoryCacheEntryOptions().SetSize(1));
                }
            }

            return (DBCD.IDBCDStorage)cachedDBC!;
        }

        private IDBCDStorage LoadDBC(string name, string build, bool useHotfixes = false, LocaleFlags locale = LocaleFlags.All_WoW, List<int>? pushIDFilter = null)
        {
            if (locale != LocaleFlags.All_WoW)
            {
                dbcProvider.localeFlags = locale;
            }

            DBCD.DBCD dbcd;

            if (dbdProvider.isUsingBDBD)
                dbcd = new DBCD.DBCD(dbcProvider, DBDProvider.GetBDBDStream());
            else
                dbcd = new DBCD.DBCD(dbcProvider, dbdProvider);

            var storage = dbcd.Load(name, build);

            dbcProvider.localeFlags = locale;

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
                    storage.ApplyingHotfixes(hotfixReaders, (row, shouldDelete) =>
                    {
                        if (!pushIDFilter.Contains(row.PushId))
                            return RowOp.Ignore;

                        return HotfixReader.DefaultProcessor(row, shouldDelete);
                    });
                }
                else
                {
                    storage.ApplyingHotfixes(hotfixReaders);
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

        public string[] GetDBCNames(string? build = null)
        {
            var dbcNames = dbdProvider.GetNames();

            if (build != null)
            {
                var filteredNames = new List<string>();
                foreach (var name in dbcNames)
                {
                    if (dbcProvider.DB2IsCached(name, build))
                        filteredNames.Add(name);
                }
                return filteredNames.ToArray();
            }
            else
            {
                return dbcNames;
            }
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