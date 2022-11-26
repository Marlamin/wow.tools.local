using DBCD;
using DBCD.Providers;
using wow.tools.local.Controllers;
using DBFileReaderLib;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CASCLib;
using wow.tools.Services;

namespace wow.tools.local.Services
{
    public class DBCManager : IDBCManager
    {
        private readonly DBDProvider dbdProvider;

        private MemoryCache Cache;
        private ConcurrentDictionary<(string, string, bool), SemaphoreSlim> Locks;

        public DBCManager(IDBDProvider dbdProvider)
        {
            this.dbdProvider = dbdProvider as DBDProvider;

            Cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = 250 });
            Locks = new ConcurrentDictionary<(string, string, bool), SemaphoreSlim>();
        }

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

            if (!HotfixManager.hotfixReaders.ContainsKey(buildNumber))
                HotfixManager.LoadCaches(buildNumber);

            if (HotfixManager.hotfixReaders.ContainsKey(buildNumber))
            {
                //storage = storage.ApplyingHotfixes(HotfixManager.hotfixReaders[buildNumber], pushIDFilter);

                // DBCD PR #17 support
                if (pushIDFilter != null)
                {
                    storage = storage.ApplyingHotfixes(HotfixManager.hotfixReaders[buildNumber], (row, shouldDelete) =>
                    {
                        if (!pushIDFilter.Contains(row.PushId))
                            return RowOp.Ignore;

                        return HotfixReader.DefaultProcessor(row, shouldDelete);
                    });
                }
                else
                {
                    storage = storage.ApplyingHotfixes(HotfixManager.hotfixReaders[buildNumber]);
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

        public List<string> GetDBCNames()
        {
            var db2s = dbdProvider.definitionLookup.Keys;
            var existingDB2s = new List<string>();
            
            foreach(var db2 in db2s)
            {
                if (CASC.FileExists("DBFilesClient/" + db2 + ".db2"))
                    existingDB2s.Add(db2);
            }
            
            return existingDB2s;
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