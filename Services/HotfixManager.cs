using DBFileReaderLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace wow.tools.local.Services
{
    public static class HotfixManager
    {
        public static Dictionary<uint, List<HotfixReader>> hotfixReaders = new Dictionary<uint, List<HotfixReader>>();

        public static void LoadCaches()
        {
            if (SettingsManager.wowFolder == null)
            {
                Console.WriteLine("No WoW folder set, skipping hotfix load");
                return;
            }

            Console.WriteLine("Reloading all hotfixes..");
            hotfixReaders.Clear();

            foreach (var file in Directory.GetFiles(SettingsManager.wowFolder, "DBCache.bin", SearchOption.AllDirectories))
            {
                var reader = new HotfixReader(file);
                if (!hotfixReaders.ContainsKey((uint)reader.BuildId))
                    hotfixReaders.Add((uint)reader.BuildId, new List<HotfixReader>());
                
                hotfixReaders[(uint)reader.BuildId].Add(reader);

                Console.WriteLine("Loaded hotfixes for build " + reader.BuildId);
            }
        }
    }
}