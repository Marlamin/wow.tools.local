using DBFileReaderLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace wow.tools.local.Services
{
    public static class HotfixManager
    {
        public static Dictionary<uint, HotfixReader> hotfixReaders = new Dictionary<uint, HotfixReader>();

        public static void LoadCaches()
        {
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

                Console.WriteLine("Loaded hotfixes from client for build " + reader.BuildId);
            }
        }
    }
}