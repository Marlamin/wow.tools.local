using wow.tools.local.Managers;

namespace wow.tools.local.Services
{
    public static class FileResidency
    {
        private static Dictionary<(string, string), List<uint>> residencyCache = [];

        public static void Load()
        {
            residencyCache.Clear();
            var distinctProducts = SQLiteDB.GetDistinctProducts();
            var builds = SQLiteDB.GetBuilds();
            var ignoredProducts = new HashSet<string> { "wowz", "wowdev", "wowlivetest" }; // we dont care about these for residency
            foreach (var product in distinctProducts)
            {
                if (ignoredProducts.Contains(product))
                    continue;

                var productBuilds = builds.Where(b => b.product == product).OrderByDescending(b => b.build).ToList();
                foreach (var build in productBuilds)
                {
                    if (ManifestManager.ExistsForVersion(build.version))
                    {
                        var manifestForBuild = ManifestManager.GetEntriesForVersion(build.version);
                        residencyCache[(product, build.version)] = manifestForBuild.Select(x => x.FileDataID).ToList();
                        break;
                    }
                }
            }
            Console.WriteLine("Loaded " + residencyCache.Count + " products for file availability");
        }

        public static async Task<bool> Reload()
        {
            residencyCache.Clear();
            Load();
            return true;
        }

        public static Dictionary<(string product, string build), bool> GetResidencyByFDID(uint fdid)
        {
            var result = new Dictionary<(string product, string build), bool>();
            foreach (var key in residencyCache.Keys)
                result[key] = residencyCache[key].Contains(fdid);
            return result;
        }

    }
}
