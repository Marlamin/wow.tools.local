using DBFileReaderLib;
using Newtonsoft.Json;

namespace wow.tools.local.Services
{
    public static class HotfixManager
    {
        public static Dictionary<uint, HotfixReader> hotfixReaders = new Dictionary<uint, HotfixReader>();
        public static Dictionary<uint, List<DBCacheParser>> dbcacheParsers = new Dictionary<uint, List<DBCacheParser>>();
        public static Dictionary<int, DateTime> pushIDDetected = new Dictionary<int, DateTime>();
        public static Dictionary<uint, string> tableNames = Directory.EnumerateFiles(SettingsManager.definitionDir).ToDictionary(x => Hash(Path.GetFileNameWithoutExtension(x).ToUpper()), x => Path.GetFileNameWithoutExtension(x));

        private static void LoadPushIDs() => pushIDDetected = JsonConvert.DeserializeObject<Dictionary<int, DateTime>>(File.ReadAllText("knownPushIDs.json"));

        private static void SavePushIDs() => File.WriteAllText("knownPushIDs.json", JsonConvert.SerializeObject(pushIDDetected));

        public static void LoadCaches()
        {
            if (!File.Exists("knownPushIDs.json"))
                SavePushIDs();

            LoadPushIDs();

            Console.WriteLine("Reloading all hotfixes..");
            hotfixReaders.Clear();

            if (Directory.Exists("caches"))
            {
                foreach (var file in Directory.GetFiles("caches", "*.bin", SearchOption.AllDirectories))
                {
                    var reader = new HotfixReader(file);
                    if (!hotfixReaders.ContainsKey((uint)reader.BuildId))
                        hotfixReaders.Add((uint)reader.BuildId, reader);

                    hotfixReaders[(uint)reader.BuildId].CombineCache(file);

                    if (!dbcacheParsers.ContainsKey((uint)reader.BuildId))
                        dbcacheParsers.Add((uint)reader.BuildId, new List<DBCacheParser>());

                    var newCache = new DBCacheParser(file);
                    dbcacheParsers[(uint)reader.BuildId].Add(newCache);

                    var newPushIDs = newCache.hotfixes.Where(x => x.pushID > 0 && !pushIDDetected.ContainsKey(x.pushID)).Select(x => x.pushID).ToList();
                    foreach (var newPushID in newPushIDs)
                    {
                        pushIDDetected.TryAdd(newPushID, DateTime.Now);
                        Console.WriteLine("Detected new pushID " + newPushID + " at " + DateTime.Now.ToShortTimeString());
                    }

                    Console.WriteLine("Loaded hotfixes from caches directory for build " + reader.BuildId);
                }
            }

            if (SettingsManager.wowFolder == null)
            {
                Console.WriteLine("No WoW folder set, skipping further hotfix load");
                return;
            }

            foreach (var file in Directory.GetFiles(SettingsManager.wowFolder, "DBCache.bin", SearchOption.AllDirectories))
            {
                var reader = new HotfixReader(file);
                if (!hotfixReaders.ContainsKey((uint)reader.BuildId))
                    hotfixReaders.Add((uint)reader.BuildId, reader);

                hotfixReaders[(uint)reader.BuildId].CombineCache(file);

                if (!dbcacheParsers.ContainsKey((uint)reader.BuildId))
                    dbcacheParsers.Add((uint)reader.BuildId, new List<DBCacheParser>());

                var newCache = new DBCacheParser(file);
                dbcacheParsers[(uint)reader.BuildId].Add(newCache);

                var newPushIDs = newCache.hotfixes.Where(x => x.pushID > 0 && !pushIDDetected.ContainsKey(x.pushID)).Select(x => x.pushID).ToList();
                foreach (var newPushID in newPushIDs)
                {
                    pushIDDetected.TryAdd(newPushID, DateTime.Now);
                    Console.WriteLine("Detected new pushID " + newPushID + " at " + DateTime.Now.ToShortTimeString());
                }

                Console.WriteLine("Loaded hotfixes from client for build " + reader.BuildId);
            }

            SavePushIDs();
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