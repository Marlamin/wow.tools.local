using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace wow.tools.local.Services
{
    public enum DiffAction
    {
        Added,
        Removed,
        Modified
    }

    public struct DiffEntry
    {
        public int id;
        [JsonConverter(typeof(StringEnumConverter))]
        public DiffAction action;
        public CASC.EncryptionStatus encryptedStatus;
        public string type;
        public string md5;
        public string filename;
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
