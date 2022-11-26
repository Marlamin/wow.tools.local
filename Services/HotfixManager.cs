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

        public static Dictionary<uint, List<string>> GetHotfixDBsPerBuild(uint targetBuild = 0)
        {
            Console.WriteLine("Listing hotfixes for build " + targetBuild);
            if (targetBuild == 0)
                throw new Exception("Tried reloading hotfixes for invalid build " + targetBuild);

            var filesPerBuild = new Dictionary<uint, List<string>>();

            if (!filesPerBuild.ContainsKey(targetBuild))
            {
                filesPerBuild.Add(targetBuild, Directory.GetFiles("caches", "DBCache-" + targetBuild + "-*.bin").ToList());
            }

            return filesPerBuild;
        }

        public static void LoadCaches(uint targetBuild = 0)
        {
            var filesPerBuild = GetHotfixDBsPerBuild(targetBuild);

            if (targetBuild != 0)
            {
                Console.WriteLine("Reloading hotfixes for build " + targetBuild + "..");
                hotfixReaders.Remove(targetBuild);
            }
            else
            {
                Console.WriteLine("Reloading all hotfixes..");
                hotfixReaders.Clear();
            }

            foreach (var fileList in filesPerBuild)
            {
                if (fileList.Value.Count == 0)
                    continue;
                hotfixReaders.Add(fileList.Key, new HotfixReader(fileList.Value[0]));
                hotfixReaders[fileList.Key].CombineCaches(fileList.Value.ToArray());
                Console.WriteLine("Loaded " + fileList.Value.Count + " hotfix DBs for build " + fileList.Key + "!");
            }
        }

        public static void LoadCache(uint targetBuild, string file)
        {
            if (!hotfixReaders.ContainsKey(targetBuild))
            {
                LoadCaches(targetBuild);
            }
            else
            {
                Console.WriteLine("Adding " + file + " for " + targetBuild + " to loaded caches");
                hotfixReaders[targetBuild].CombineCache(file);
            }
        }

        public static void AddCache(MemoryStream cache, uint build, int userID)
        {
            var filename = Path.Combine("caches", "DBCache-" + build + "-" + userID + "-" + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + "-" + DateTime.Now.Millisecond + ".bin");
            using (var stream = File.Create(filename))
            {
                cache.CopyTo(stream);
            }

            LoadCache(build, filename);
        }
    }
}