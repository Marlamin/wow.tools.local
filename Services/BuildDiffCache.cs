using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace wow.tools.local.Services
{
    public struct DiffEntry
    {
        public string action;
        public string filename;
        public string id;
        public string type;
        public string md5;
        public string encryptedStatus;
    }

    public struct ApiDiff
    {
        public IEnumerable<DiffEntry> Added;
        public IEnumerable<DiffEntry> Removed;
        public IEnumerable<DiffEntry> Modified;

        public readonly IEnumerable<DiffEntry> All
        {
            get
            {
                return Added.Concat(Removed.Concat(Modified));
            }
        }
    }

    public static class BuildDiffCache
    {
        private static readonly MemoryCache Cache = new(new MemoryCacheOptions() { SizeLimit = 15 });

        private static HashSet<string> Keys = [];

        public static bool Get(string from, string to, out ApiDiff diff)
        {
            var cacheKey = $"{from}_{to}";

            return Cache.TryGetValue(cacheKey, out diff);
        }

        public static void Add(string from, string to, ApiDiff diff)
        {
            var cacheKey = $"{from}_{to}";

            Keys.Add(cacheKey);

            Cache.Set(cacheKey, diff, new MemoryCacheEntryOptions().SetSize(1));
        }

        public static void Invalidate()
        {
            foreach (var key in Keys)
            {
                Cache.Remove(key);
            }

            Keys = [];
        }
    }
}
