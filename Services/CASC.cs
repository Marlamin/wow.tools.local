using CASCLib;
using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace wow.tools.local.Services
{
    public class CASC
    {
        private static CASCHandler cascHandler;
        public static bool IsCASCInit = false;
        public static string BuildName;
        public static Dictionary<int, string> Listfile = new();
        public static Dictionary<string, int> ListfileReverse = new();
        public static SortedDictionary<int, string> M2Listfile = new();
        
        public static List<int> AvailableFDIDs = new();
        public static List<ulong> KnownKeys = new();
        
        public static Dictionary<int, EncryptionStatus> EncryptionStatuses = new();
        public static Dictionary<int, List<ulong>> EncryptedFDIDs = new();
        public static Dictionary<int, string> Types = new();
        public static Dictionary<string, List<int>> TypeMap = new();

        public enum EncryptionStatus
        {
            EncryptedKnownKey,
            EncryptedUnknownKey,
            EncryptedMixed
        }

        private static HttpClient WebClient = new HttpClient();
        public static async void InitCasc(string? basedir = null, string program = "wowt", LocaleFlags locale = LocaleFlags.enUS)
        {
            WebClient.DefaultRequestHeaders.Add("User-Agent", "wow.tools.local");

            CASCConfig.LoadFlags &= ~(LoadFlags.Download | LoadFlags.Install);
            CASCConfig.ValidateData = false;
            CASCConfig.ThrowOnFileNotFound = false;

            locale = SettingsManager.locale;

            if (basedir == null)
            {
                Console.WriteLine("Initializing CASC from web for program " + program + " and locale " + locale);
                cascHandler = CASCHandler.OpenOnlineStorage(program, SettingsManager.region);
            }
            else
            {
                basedir = basedir.Replace("_retail_", "").Replace("_ptr_", "");
                Console.WriteLine("Initializing CASC from local disk with basedir " + basedir + " and program " + program + " and locale " + locale);
                cascHandler = CASCHandler.OpenLocalStorage(basedir, program);
            }

            var splitName = cascHandler.Config.BuildName.Replace("WOW-", "").Split("patch");
            BuildName = splitName[1].Split("_")[0] + "." + splitName[0];

            cascHandler.Root.SetFlags(locale);

            if (!Directory.Exists("manifests"))
                Directory.CreateDirectory("manifests");
            
            if (cascHandler.Root is WowTVFSRootHandler wtrh)
            {
                AvailableFDIDs = wtrh.RootEntries.Keys.ToList();
                if (!File.Exists(Path.Combine("manifests", BuildName + ".txt")))
                {
                    var manifestLines = new List<string>();
                    foreach (var entry in wtrh.RootEntries)
                    {
                        var preferredEntry = entry.Value.FirstOrDefault(subentry =>
                       subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                        manifestLines.Add(entry.Key + ";" + preferredEntry.cKey.ToHexString());
                    }

                    manifestLines.Sort();

                    File.WriteAllLines(Path.Combine("manifests", BuildName + ".txt"), manifestLines);
                }
            }
            else if (cascHandler.Root is WowRootHandler wrh)
            {
                AvailableFDIDs = wrh.RootEntries.Keys.ToList();
                if (!File.Exists(Path.Combine("manifests", BuildName + ".txt")))
                {
                    var manifestLines = new List<string>();
                    foreach (var entry in wrh.RootEntries)
                    {
                        var preferredEntry = entry.Value.FirstOrDefault(subentry =>
                       subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                        manifestLines.Add(entry.Key + ";" + preferredEntry.cKey.ToHexString());
                    }

                    manifestLines.Sort();

                    File.WriteAllLines(Path.Combine("manifests", BuildName + ".txt"), manifestLines);
                }
            }

            Listfile = new Dictionary<int, string>();
            ListfileReverse = new Dictionary<string, int>();
            M2Listfile = new SortedDictionary<int, string>();
            EncryptedFDIDs = new Dictionary<int, List<ulong>>();
            EncryptionStatuses = new Dictionary<int, EncryptionStatus>();
            TypeMap = new Dictionary<string, List<int>>();
            Types = new Dictionary<int, string>();

            AvailableFDIDs.ForEach(x => Listfile.Add(x, ""));

            var listfileRes = LoadListfile();
            if (!listfileRes)
                throw new Exception("Failed to load listfile");

            Console.WriteLine("Analyzing encrypted files");
            if (cascHandler.Root is WowTVFSRootHandler ewtrh)
            {
                foreach (var entry in ewtrh.RootEntries)
                {
                    foreach (var subentry in entry.Value)
                    {
                        if (EncryptedFDIDs.ContainsKey(entry.Key))
                            continue;

                        if (cascHandler.Encoding.GetEntry(subentry.cKey, out var eKey))
                        {
                            var usedKeys = cascHandler.Encoding.GetEncryptionKeys(eKey.Keys[0]);
                            if (usedKeys != null)
                                EncryptedFDIDs.Add(entry.Key, new List<ulong>(usedKeys));
                        }
                    }
                }
            }
            else if (cascHandler.Root is WowRootHandler ewrh)
            {
                foreach (var entry in ewrh.RootEntries)
                {
                    foreach (var subentry in entry.Value)
                    {
                        if (EncryptedFDIDs.ContainsKey(entry.Key))
                            continue;

                        if (cascHandler.Encoding.GetEntry(subentry.cKey, out var eKey))
                        {
                            var usedKeys = cascHandler.Encoding.GetEncryptionKeys(eKey.Keys[0]);
                            if (usedKeys != null)
                                EncryptedFDIDs.Add(entry.Key, new List<ulong>(usedKeys));
                        }
                    }
                }
            }

            Console.WriteLine("Found " + EncryptedFDIDs.Count + " encrypted files");
            foreach (var encryptedFile in EncryptedFDIDs)
            {
                EncryptionStatus encryptionStatus;
                if (encryptedFile.Value.All(value => KnownKeys.Contains(value)))
                {
                    encryptionStatus = EncryptionStatus.EncryptedKnownKey;
                }
                else if (encryptedFile.Value.Any(value => KnownKeys.Contains(value)))
                {
                    encryptionStatus = EncryptionStatus.EncryptedMixed;
                }
                else
                {
                    encryptionStatus = EncryptionStatus.EncryptedUnknownKey;
                }

                EncryptionStatuses.Add(encryptedFile.Key, encryptionStatus);
            }
            Console.WriteLine("Done analyzing encrypted files");

            // Loaded cached types from disk
            if (File.Exists("cachedUnknowns.txt"))
            {
                Console.WriteLine("Loading cached types from disk");
                var knownUnknowns = File.ReadAllLines("cachedUnknowns.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);
                if (knownUnknowns.Count > 0)
                {
                    foreach (var knownUnknown in knownUnknowns)
                    {
                        SetFileType(knownUnknown.Key, knownUnknown.Value);
                    }
                }
            }
            
            IsCASCInit = true;
            Console.WriteLine("Finished loading " + BuildName);
        }

        public static bool ExportListfile()
        {
            File.WriteAllLines("exported-listfile.csv", Listfile.OrderBy(x => x.Key).Select(x => x.Key + ";" + x.Value).ToArray());
            return true;
        }

        public static bool ExportTACTKeys()
        {
            KnownKeys.Sort();
            File.WriteAllLines("WoW.txt", KnownKeys.Select(x => x.ToString("X16") + " " + Convert.ToHexString(KeyService.GetKey(x))).ToArray());
            return true;
        }
        
        public static bool LoadListfile(bool forceRedownload = false)
        {
            var download = forceRedownload;
            
            if (File.Exists("listfile.csv"))
            {
                var info = new FileInfo("listfile.csv");
                if (info.Length == 0 || DateTime.Now.Subtract(TimeSpan.FromDays(1)) > info.LastWriteTime)
                {
                    Console.WriteLine("Listfile outdated, redownloading..");
                    download = true;
                }
            }
            else
            {
                download = true;
            }

            if (download)
            {
                Console.WriteLine("Downloading listfile");

                using var s = WebClient.GetStreamAsync(SettingsManager.listfileURL).Result;
                using var fs = new FileStream("listfile.csv", FileMode.OpenOrCreate);
                s.CopyTo(fs);
            }

            if (!File.Exists("listfile.csv"))
            {
                throw new FileNotFoundException("Could not find listfile.csv");
            }

            if (forceRedownload)
            {
                Listfile.Clear();
                AvailableFDIDs.ForEach(x => Listfile.Add(x, ""));

                ListfileReverse.Clear();
                Types.Clear();
                M2Listfile.Clear();
            }

            Console.WriteLine("Loading listfile");

            foreach (var line in File.ReadAllLines("listfile.csv"))
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var splitLine = line.Split(";");
                var fdid = int.Parse(splitLine[0]);

                if (Listfile.ContainsKey(fdid))
                {
                    var ext = Path.GetExtension(splitLine[1]).Replace(".", "");

                    if (!TypeMap.ContainsKey(ext))
                        TypeMap.Add(ext, new List<int>());

                    Listfile[fdid] = splitLine[1];
                    ListfileReverse.Add(splitLine[1], fdid);

                    Types.Add(fdid, ext);
                    TypeMap[ext].Add(fdid);
                    if (ext == "m2")
                        M2Listfile.Add(fdid, splitLine[1]);
                }
            }

            Console.WriteLine("Finished loading listfile: " + Listfile.Count + " named files for this build");

            return true;
        }

        public static bool LoadKeys(bool forceRedownload = false)
        {
            var download = forceRedownload;
            if (File.Exists("TactKey.csv"))
            {
                var info = new FileInfo("TactKey.csv");
                if (info.Length == 0 || DateTime.Now.Subtract(TimeSpan.FromDays(1)) > info.LastWriteTime)
                {
                    Console.WriteLine("TACT Keys outdated, redownloading..");
                    download = true;
                }
            }
            else
            {
                download = true;
            }

            if (download)
            {
                Console.WriteLine("Downloading TACT keys");

                List<string> tactKeyLines = new();
                using (var s = WebClient.GetStreamAsync(SettingsManager.tactKeyURL).Result)
                using (var sr = new StreamReader(s))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var splitLine = line.Split(" ");
                        tactKeyLines.Add(splitLine[0] + ";" + splitLine[1]);
                    }
                }

                File.WriteAllLines("TactKey.csv", tactKeyLines);
            }

            if (forceRedownload)
                KnownKeys.Clear();
            
            foreach (var line in File.ReadAllLines("TactKey.csv"))
            {
                var splitLine = line.Split(";");
                if (splitLine.Length != 2)
                    continue;
                KnownKeys.Add(ulong.Parse(splitLine[0], NumberStyles.HexNumber));
            }

            if(IsCASCInit)
                KeyService.LoadKeys();

            Console.WriteLine("Finished loading TACT keys: " + KnownKeys.Count + " known keys");

            return true;
        }

        public static Stream? GetFileByID(uint filedataid)
        {
            try
            {
                return cascHandler.OpenFile((int)filedataid);
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("keyname"))
                {
                    Console.WriteLine("Exception retrieving FileDataID " + filedataid + ": " + e.Message);
                }
                else
                {
                    Console.WriteLine("Missing key for " + filedataid + ": " + e.Message);
                }
                return null;
            }
        }

        public static Stream GetFileByName(string filename)
        {
            if (ListfileReverse.TryGetValue(filename.ToLower(), out int fileDataID))
            {
                return cascHandler.OpenFile(fileDataID);
            }

            throw new FileNotFoundException("Could not find " + filename + " in listfile");
        }

        public static bool FileExists(string filename)
        {
            if (ListfileReverse.TryGetValue(filename.ToLower(), out int fileDataID))
            {
                return cascHandler.FileExists(fileDataID);
            }

            return false;
        }

        public static bool FileExists(uint filedataid)
        {
            return cascHandler.FileExists((int)filedataid);
        }
        
        public static string GetFullBuild()
        {
            return cascHandler.Config.BuildName;
        }
        
        public static string GetKey(ulong lookup)
        {
            if (cascHandler == null)
                return "";

            var key = KeyService.GetKey(lookup);
            if (key == null)
                return "";
            
            return Convert.ToHexString(key);
        }

        public static void SetFileType(int filedataid, string type)
        {
            if (!Listfile.ContainsKey(filedataid))
                return;

            if (!Types.ContainsKey(filedataid))
                Types.Add(filedataid, type);
            else if (Types[filedataid] == "unk")
                Types[filedataid] = type;

            if (!TypeMap.ContainsKey(type))
                TypeMap.Add(type, new List<int>());

            TypeMap[type].Add(filedataid);
        }
    }
}
