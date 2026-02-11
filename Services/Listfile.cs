using System.Collections.Immutable;
using System.Globalization;
using TACTSharp;

namespace wow.tools.local.Services
{
    public static class Listfile
    {
        public static readonly Dictionary<int, string> NameMap = [];
        public static readonly Dictionary<string, int> DB2Map = [];
        public static readonly HashSet<int> PlaceholderFiles = [];
        public static readonly Dictionary<int, string> Types = [];
        public static readonly Dictionary<string, HashSet<int>> TypeMap = [];
        public static readonly Dictionary<int, ulong> LookupMap = [];

        public static int LoadID = 0;
        public static readonly Lock LoadLock = new Lock();
        private static readonly HttpClient WebClient = new();

        public static string[] GetLines(bool forceRedownload = false)
        {
            var listfileMode = "downloaded";

            if (!SettingsManager.ListfileURL.StartsWith("http") && Directory.Exists(SettingsManager.ListfileURL))
                listfileMode = "parts";

            var listfileLines = new List<string>();
            if (listfileMode == "downloaded")
            {
                Console.WriteLine("Loading listfile");

                var download = forceRedownload;
                bool shouldBackup = false;

                var listfileName = "listfile.csv";

                if (!File.Exists(listfileName))
                {
                    download = true;
                }
                else
                {
                    var info = new FileInfo(listfileName);
                    if (info.Length == 0 || DateTime.Now.Subtract(TimeSpan.FromDays(1)) > info.LastWriteTime)
                    {
                        Console.WriteLine("Listfile outdated, redownloading...");
                        download = true;
                    }
                    shouldBackup = true;
                }

                if (download)
                {
                    Console.WriteLine("Downloading listfile");

                    if (shouldBackup)
                    {
                        if (File.Exists(listfileName + ".bak"))
                            File.Delete(listfileName + ".bak");

                        File.Move(listfileName, listfileName + ".bak");
                        Console.WriteLine("Existing " + listfileName + " renamed to " + listfileName + ".bak");
                    }

                    using var s = WebClient.GetStreamAsync(SettingsManager.ListfileURL).Result;
                    using var fs = new FileStream(listfileName, FileMode.Create);
                    s.CopyTo(fs);
                }

                if (!File.Exists(listfileName))
                {
                    throw new FileNotFoundException("Could not find " + listfileName);
                }

                listfileLines.AddRange(File.ReadAllLines(listfileName));
            }
            else if (listfileMode == "parts")
            {
                Console.WriteLine("Loading listfile from parts");

                var files = Directory.GetFiles(SettingsManager.ListfileURL, "*.csv");
                var listfileLock = new Lock();
                Parallel.ForEach(files, file =>
                {
                    Console.WriteLine("Loading listfile parts from " + Path.GetFileNameWithoutExtension(file));
                    lock (listfileLock)
                        listfileLines.AddRange(File.ReadAllLines(file));
                });
            }

            return [.. listfileLines];
        }

        public static Dictionary<int, string> GetAllNames()
        {
            var allNames = new Dictionary<int, string>();

            foreach (var line in GetLines())
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var splitLine = line.Split(";");
                allNames[int.Parse(splitLine[0])] = splitLine[1];
            }

            Console.WriteLine("Finished loading full listfile: " + allNames.Count + " named files");

            return allNames;
        }
        public static bool Load(bool forceRedownload = false)
        {
            lock (LoadLock)
            {
                NameMap.Clear();
                DB2Map.Clear();
                Types.Clear();
                PlaceholderFiles.Clear();
                CASC.AvailableFDIDs.ForEach(x => Listfile.NameMap.TryAdd(x, ""));

                var listfileLines = GetLines(forceRedownload);

                if (File.Exists("custom-listfile.csv"))
                    listfileLines = listfileLines.Concat(File.ReadAllLines("custom-listfile.csv")).ToArray();

                foreach (var line in listfileLines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var splitLine = line.Split(";");
                    var fdid = int.Parse(splitLine[0]);

                    if (SettingsManager.ShowAllFiles == false && !NameMap.ContainsKey(fdid))
                        continue;

                    var filename = splitLine[1];

                    var ext = Path.GetExtension(filename).Replace(".", "").ToLower();

                    if (!TypeMap.ContainsKey(ext))
                        TypeMap.Add(ext, []);

                    NameMap[fdid] = filename;

                    // Don't add WMOs to the type map, rely on scans for setting WMO/group WMOs correctly
                    if (ext != "wmo")
                    {
                        Types.TryAdd(fdid, ext);
                        TypeMap[ext].Add(fdid);
                    }

                    var filenameLower = filename.ToLower();

                    if (ext == "db2")
                        DB2Map.Add(filenameLower, fdid);

                    if (
                        filenameLower.StartsWith("models", StringComparison.Ordinal) ||
                        filenameLower.StartsWith("unkmaps", StringComparison.Ordinal) ||
                        filenameLower.Contains("autogen-names", StringComparison.Ordinal) ||
                        filenameLower.Contains(fdid.ToString(), StringComparison.Ordinal) ||
                        filenameLower.Contains("unk_exp", StringComparison.Ordinal) ||
                        filenameLower.Contains("tileset/unused", StringComparison.Ordinal) ||
                        string.IsNullOrEmpty(filename)
                        )
                    {
                        PlaceholderFiles.Add(fdid);
                    }
                }

                Console.WriteLine("Finished loading listfile: " + NameMap.Count + " named files for this build");

                // Load DBD manifest for additional DB2s
                DBDManifest.Load();

                LoadID++;
            }

            return true;
        }
        public static bool Export()
        {
            File.WriteAllLines("exported-listfile.csv", NameMap.OrderBy(x => x.Key).Select(x => x.Key + ";" + x.Value).ToArray());
            return true;
        }
        public static void SetFileType(int filedataid, string type)
        {
            type = type.ToLower();

            if (!NameMap.ContainsKey(filedataid))
                return;

            if (!Types.TryGetValue(filedataid, out string? value))
                Types.Add(filedataid, type);
            else if (value == "unk")
                Types[filedataid] = type;

            if (!TypeMap.ContainsKey(type))
                TypeMap.Add(type, []);

            TypeMap[type].Add(filedataid);
        }

        public static void LoadCachedUnknowns()
        {
            // Loaded cached types from disk
            if (File.Exists("cachedUnknowns.txt"))
            {
                Console.WriteLine("Loading cached types from disk");
                var knownUnknowns = File.ReadAllLines("cachedUnknowns.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);
                if (knownUnknowns.Count > 0)
                {
                    foreach (var knownUnknown in knownUnknowns)
                    {
                        // Remove old M3 related placeholder types: m3strtbl, m3shlib, m3matlib
                        if (knownUnknown.Value == "m3strtbl" || knownUnknown.Value == "m3shlib" || knownUnknown.Value == "m3matlib")
                            continue;

                        if (Types.TryGetValue(knownUnknown.Key, out var currentType) && currentType != "unk")
                            continue;

                        if (knownUnknown.Key == 5569152 || knownUnknown.Key == 5916032 || knownUnknown.Key == 6022679)
                            SetFileType(knownUnknown.Key, "m3");
                        else
                            SetFileType(knownUnknown.Key, knownUnknown.Value);
                    }
                }
            }
        }

        public static void EnsureFDIDsPresent(List<int> fdids)
        {
            lock (LoadLock)
            {
                fdids.ForEach(x => NameMap.TryAdd(x, ""));
                LoadID++;
            }
        }
        public static void ManualNameOverride(int id, string name, bool isPlaceholderName)
        {
            lock (LoadLock)
            {
                NameMap[id] = name;
                if (isPlaceholderName)
                {
                    PlaceholderFiles.Add(id);
                }
                else
                {
                    PlaceholderFiles.Remove(id);
                }
                LoadID++;
            }
        }

        public static void LoadLookups(bool forceRedownload = false)
        {
            var listfileMode = "downloaded";

            if (!SettingsManager.ListfileURL.StartsWith("http") && Directory.Exists(SettingsManager.ListfileURL))
                listfileMode = "parts";

            LookupMap.Clear();

            if (File.Exists("cachedLookups.txt"))
            {
                var cachedLookups = File.ReadAllLines("cachedLookups.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => ulong.Parse(x[1]));
                foreach (var lookup in cachedLookups)
                    LookupMap[lookup.Key] = lookup.Value;
            }

            if (listfileMode == "downloaded")
            {
                var download = forceRedownload;
                bool shouldBackup = false;

                var lookupName = "lookup.csv";

                if (!File.Exists(lookupName))
                {
                    download = true;
                }
                else
                {
                    var info = new FileInfo(lookupName);
                    if (info.Length == 0 || DateTime.Now.Subtract(TimeSpan.FromDays(1)) > info.LastWriteTime)
                    {
                        Console.WriteLine("Lookups outdated, redownloading...");
                        download = true;
                    }
                    shouldBackup = true;
                }

                if (download)
                {
                    Console.WriteLine("Downloading lookups");

                    if (shouldBackup)
                    {
                        if (File.Exists(lookupName + ".bak"))
                            File.Delete(lookupName + ".bak");

                        File.Move(lookupName, lookupName + ".bak");
                        Console.WriteLine("Existing " + lookupName + " renamed to " + lookupName + ".bak");
                    }

                    using var s = WebClient.GetStreamAsync("https://github.com/wowdev/wow-listfile/releases/latest/download/lookup.csv").Result;
                    using var fs = new FileStream(lookupName, FileMode.Create);
                    s.CopyTo(fs);
                }

                if (!File.Exists(lookupName))
                {
                    throw new FileNotFoundException("Could not find " + lookupName);
                }

                //formatted like 5569038;86bdc3495d82c335
                var lookupLines = File.ReadAllLines(lookupName);
                foreach (var line in lookupLines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var splitLine = line.Split(";");
                    LookupMap[int.Parse(splitLine[0])] = ulong.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber);
                }
            }
            else if (listfileMode == "parts")
            {
                var lookupFile = Path.Combine(SettingsManager.ListfileURL, "..", "meta", "lookup.csv");
                if (!File.Exists(lookupFile))
                    throw new FileNotFoundException("Could not find " + lookupFile);

                Console.WriteLine("Loading lookups from " + Path.GetFileNameWithoutExtension(lookupFile));
                var lookupLines = File.ReadAllLines(lookupFile);
                foreach (var line in lookupLines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var splitLine = line.Split(";");
                    LookupMap[int.Parse(splitLine[0])] = ulong.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber);
                }
            }
        }

        public static void ExportLookups()
        {
            var hasher = new Jenkins96();
            using (var sw = new StreamWriter("exported-lookups.csv"))
            {
                var sortedMap = Listfile.LookupMap.ToDictionary();

                // If our listfile setting points to a parts dir, merge in the existing meta lookup file
                if (!SettingsManager.ListfileURL.StartsWith("http") && Directory.Exists(SettingsManager.ListfileURL))
                {
                    var metaFile = Path.Combine(SettingsManager.ListfileURL, "..", "meta", "lookup.csv");

                    if (System.IO.File.Exists(metaFile))
                    {
                        foreach (var line in System.IO.File.ReadAllLines(metaFile))
                        {
                            if (string.IsNullOrEmpty(line))
                                continue;

                            var split = line.Split(";");
                            var fdid = int.Parse(split[0]);
                            var lookup = ulong.Parse(split[1], NumberStyles.HexNumber);
                            if (sortedMap.TryGetValue(fdid, out var currentLookup))
                            {
                                if (currentLookup != lookup)
                                    Console.WriteLine("LOOKUP MISMATCH FOR FDID " + fdid + ": Exists as " + currentLookup.ToString("X16").ToLower() + " but tried to set to " + split[1]);
                            }
                            else
                            {
                                sortedMap.Add(fdid, lookup);
                            }
                        }
                    }
                }

                foreach (var kvp in sortedMap.ToImmutableSortedDictionary())
                {
                    sw.WriteLine(kvp.Key + ";" + kvp.Value.ToString("X16").ToLower());
                }
            }
        }
    }
}
