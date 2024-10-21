using CASCLib;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Globalization;
using WoWFormatLib;

namespace wow.tools.local.Services
{
    public static class CASC
    {
        public static CASCHandler cascHandler;
        public static bool IsCASCInit = false;
        public static string BuildName;
        public static string CurrentProduct;

        public static readonly Dictionary<int, string> Listfile = [];
        public static readonly Dictionary<string, int> DB2Map = [];

        public static List<int> AvailableFDIDs = [];
        public static readonly List<ulong> KnownKeys = [];

        public static readonly Dictionary<int, EncryptionStatus> EncryptionStatuses = [];
        public static readonly Dictionary<int, List<ulong>> EncryptedFDIDs = [];
        public static readonly Dictionary<int, string> Types = [];
        public static readonly Dictionary<string, HashSet<int>> TypeMap = [];
        public static Dictionary<int, ulong> LookupMap = [];

        public static readonly Dictionary<string, List<int>> CHashToFDID = [];
        public static readonly Dictionary<int, string> FDIDToCHash = [];
        public static readonly Dictionary<int, HashSet<string>> FDIDToCHashSet = [];
        public static readonly Dictionary<string, long> CHashToSize = [];
        public static readonly List<int> PlaceholderFiles = [];
        public static Dictionary<int, List<Version>> VersionHistory = [];
        public static List<AvailableBuild> AvailableBuilds = [];
        public static List<int> OtherLocaleOnlyFiles = [];

        public struct Version
        {
            public string buildName;
            public string contentHash;
        }

        public enum EncryptionStatus : byte
        {
            EncryptedKnownKey,
            EncryptedUnknownKey,
            EncryptedMixed,
            EncryptedButNot
        }

        public struct AvailableBuild
        {
            public string Branch;
            public string BuildConfig;
            public string CDNConfig;
            public string CDNPath;
            public string KeyRing;
            public string Version;
            public string Product;
            public string Folder;
            public string Armadillo;
        }

        private static readonly HttpClient WebClient = new();
        public static void InitCasc(string? basedir = null, string program = "wowt", LocaleFlags locale = LocaleFlags.enUS)
        {
            WebClient.DefaultRequestHeaders.Add("User-Agent", "wow.tools.local");

            CASCConfig.ValidateData = false;
            CASCConfig.ThrowOnFileNotFound = false;
            CASCConfig.UseWowTVFS = false;
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

                LoadBuildInfo();

                foreach (var build in AvailableBuilds)
                {
                    if (!string.IsNullOrEmpty(build.KeyRing))
                    {
                        try
                        {
                            using (var httpClient = new HttpClient())
                            {
                                var keyring = httpClient.GetStreamAsync("http://cdn.blizzard.com/" + build.CDNPath + "/config/" + build.KeyRing[0] + build.KeyRing[1] + "/" + build.KeyRing[2] + build.KeyRing[3] + "/" + build.KeyRing).Result;

                                string keyringContents;
                                if (!string.IsNullOrEmpty(build.Armadillo))
                                    keyringContents = new StreamReader(new ArmadilloCrypt(build.Armadillo).DecryptFileToStream(build.KeyRing, keyring)).ReadToEnd();
                                else
                                    keyringContents = new StreamReader(keyring).ReadToEnd();

                                foreach (var line in keyringContents.Split("\n"))
                                {
                                    var splitLine = line.Split(" = ");
                                    if (splitLine.Length != 2)
                                        continue;

                                    var lookup = splitLine[0].Replace("key-", "");
                                    if(lookup.Length != 16)
                                    {
                                        Console.WriteLine("Warning: KeyRing lookup " + lookup + " is not 16 characters long, skipping..");
                                        continue;
                                    }

                                    var parsedLookup = BitConverter.ToUInt64(lookup.ToByteArray(), 0);

                                    if (!KnownKeys.Contains(parsedLookup))
                                        KnownKeys.Add(parsedLookup);

                                    if (KeyService.HasKey(parsedLookup))
                                        continue;

                                    Console.WriteLine("Setting key " + parsedLookup.ToString("X") + " from KeyRing " + build.KeyRing);
                                    KeyService.SetKey(parsedLookup, splitLine[1].ToByteArray());
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error loading keyring: " + e.Message);
                        }
                    }
                }
            }
            CurrentProduct = program;

            var splitName = cascHandler.Config.BuildName.Replace("WOW-", "").Split("patch");
            BuildName = splitName[1].Split("_")[0] + "." + splitName[0];

            cascHandler.Root.SetFlags(locale, false, SettingsManager.preferHighResTextures);
            var manifestFolder = SettingsManager.manifestFolder;

            if (!Directory.Exists(manifestFolder))
                Directory.CreateDirectory(manifestFolder);

            AvailableFDIDs.Clear();

            if (cascHandler.Root is WowTVFSRootHandler wtrh)
            {
                AvailableFDIDs.AddRange(wtrh.RootEntries.Keys);
                if (!File.Exists(Path.Combine(manifestFolder, BuildName + ".txt")))
                {
                    var manifestLines = new List<string>();
                    foreach (var entry in wtrh.RootEntries)
                    {
                        var preferredEntry = entry.Value.FirstOrDefault(subentry =>
                       subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                        if (preferredEntry.cKey.lowPart == 0 && preferredEntry.cKey.highPart == 0)
                            preferredEntry = entry.Value.First();

                        manifestLines.Add(entry.Key + ";" + preferredEntry.cKey.ToHexString());
                    }

                    manifestLines.Sort();

                    File.WriteAllLines(Path.Combine(manifestFolder, BuildName + ".txt"), manifestLines);

                    SQLiteDB.ImportBuildIntoFileHistory(BuildName);
                }
            }
            else if (cascHandler.Root is WowRootHandler wrh)
            {
                AvailableFDIDs.AddRange(wrh.RootEntries.Keys);
                if (!File.Exists(Path.Combine(manifestFolder, BuildName + ".txt")))
                {
                    var manifestLines = new List<string>();
                    foreach (var entry in wrh.RootEntries)
                    {
                        var preferredEntry = entry.Value.FirstOrDefault(subentry =>
                       subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                        if (preferredEntry.cKey.lowPart == 0 && preferredEntry.cKey.highPart == 0)
                            preferredEntry = entry.Value.First();

                        manifestLines.Add(entry.Key + ";" + preferredEntry.cKey.ToHexString());
                    }

                    manifestLines.Sort();

                    File.WriteAllLines(Path.Combine(manifestFolder, BuildName + ".txt"), manifestLines);

                    SQLiteDB.ImportBuildIntoFileHistory(BuildName);
                }
            }

            Listfile.Clear();
            DB2Map.Clear();
            EncryptedFDIDs.Clear();
            EncryptionStatuses.Clear();
            TypeMap.Clear();
            Types.Clear();
            LookupMap = [];

            if (File.Exists("cachedLookups.txt"))
                LookupMap = File.ReadAllLines("cachedLookups.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => ulong.Parse(x[1]));

            AvailableFDIDs.ForEach(x => Listfile.Add(x, ""));

            bool listfileRes;

            try
            {
                listfileRes = LoadListfile();
            }
            catch (Exception e)
            {   // attempt automatic redownload of the listfile if it wasn't able to be parsed - this will also backup the old listfile to listfile.csv.bak
                Console.WriteLine("Good heavens! Encountered an error reading listfile (" + e.Message + "). Attempting redownload...");
                listfileRes = LoadListfile(true);
            }

            if (!listfileRes)
            {   // still no listfile, exit
                Console.WriteLine("Failed to read listfile after automatic redownload.");
                Environment.Exit(1);
            }

            Console.WriteLine("Analyzing files");
            if (cascHandler.Root is WowTVFSRootHandler ewtrh)
            {
                foreach (var entry in ewtrh.RootEntries)
                {
                    var preferredEntry = entry.Value.FirstOrDefault(subentry =>
subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                    if (preferredEntry.cKey.lowPart == 0 && preferredEntry.cKey.highPart == 0)
                    {
                        preferredEntry = entry.Value.First();
                        OtherLocaleOnlyFiles.Add(entry.Key);
                    }

                    foreach (var subentry in entry.Value)
                    {
                        if (EncryptedFDIDs.ContainsKey(entry.Key))
                            continue;

                        if (subentry.ContentFlags.HasFlag(ContentFlags.Encrypted))
                            EncryptedFDIDs.Add(entry.Key, []);

                        if (cascHandler.Encoding.GetEntry(subentry.cKey, out var eKey))
                        {
                            var usedKeys = cascHandler.Encoding.GetEncryptionKeys(eKey.Keys[0]);
                            if (usedKeys != null)
                            {
                                if (EncryptedFDIDs.TryGetValue(entry.Key, out List<ulong>? encryptedIDs))
                                {
                                    encryptedIDs.AddRange(usedKeys);
                                }
                                else
                                {
                                    EncryptedFDIDs.Add(entry.Key, new List<ulong>(usedKeys));
                                }
                            }
                        }
                    }
                }
            }
            else if (cascHandler.Root is WowRootHandler ewrh)
            {
                // Encryption
                foreach (var entry in ewrh.RootEntries)
                {
                    var preferredEntry = entry.Value.FirstOrDefault(subentry =>
subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                    if (preferredEntry.cKey.lowPart == 0 && preferredEntry.cKey.highPart == 0)
                    {
                        preferredEntry = entry.Value.First();
                        OtherLocaleOnlyFiles.Add(entry.Key);
                    }

                    foreach (var subentry in entry.Value)
                    {
                        if (EncryptedFDIDs.ContainsKey(entry.Key))
                            continue;

                        if (subentry.ContentFlags.HasFlag(ContentFlags.Encrypted))
                            EncryptedFDIDs.Add(entry.Key, []);

                        if (cascHandler.Encoding.GetEntry(subentry.cKey, out var eKey))
                        {
                            var usedKeys = cascHandler.Encoding.GetEncryptionKeys(eKey.Keys[0]);
                            if (usedKeys != null)
                            {
                                if (EncryptedFDIDs.TryGetValue(entry.Key, out List<ulong>? encryptedIDs))
                                {
                                    encryptedIDs.AddRange(usedKeys);
                                }
                                else
                                {
                                    EncryptedFDIDs.Add(entry.Key, new List<ulong>(usedKeys));
                                }
                            }
                        }
                    }
                }

                // Lookups
                foreach (var entry in ewrh.FileDataToLookup)
                {
                    if (!LookupMap.ContainsKey(entry.Key) && entry.Value != FileDataHash.ComputeHash(entry.Key))
                    {
                        LookupMap.Add(entry.Key, entry.Value);
                    }
                }

                File.WriteAllLines("cachedLookups.txt", LookupMap.Select(x => x.Key + ";" + x.Value));
            }

            Console.WriteLine("Found " + EncryptedFDIDs.Count + " encrypted files");
            RefreshEncryptionStatus();
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

            try
            {
                HotfixManager.LoadCaches();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading hotfixes: " + e.Message);
            }
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

        public static void LoadBuildInfo()
        {
            AvailableBuilds.Clear();

            var folderMap = new Dictionary<string, string>();
            foreach (var flavorFile in Directory.GetFiles(SettingsManager.wowFolder, ".flavor.info", SearchOption.AllDirectories))
            {
                var flavorLines = File.ReadAllLines(flavorFile);
                if (flavorLines.Length < 2)
                    continue;

                folderMap.Add(flavorLines[1], Path.GetFileName(Path.GetDirectoryName(flavorFile)));
            }

            var headerMap = new Dictionary<string, byte>();
            foreach (var line in File.ReadAllLines(Path.Combine(SettingsManager.wowFolder, ".build.info")))
            {
                var splitLine = line.Split("|");
                if (splitLine[0] == "Branch!STRING:0")
                {
                    foreach (var header in splitLine)
                        headerMap.Add(header.Split("!")[0], (byte)Array.IndexOf(splitLine, header));

                    continue;
                }

                var availableBuild = new AvailableBuild
                {
                    BuildConfig = splitLine[headerMap["Build Key"]],
                    CDNConfig = splitLine[headerMap["CDN Key"]],
                    CDNPath = splitLine[headerMap["CDN Path"]],
                    Version = splitLine[headerMap["Version"]],
                    KeyRing = splitLine[headerMap["KeyRing"]],
                    Armadillo = splitLine[headerMap["Armadillo"]],
                    Product = splitLine[headerMap["Product"]]
                };

                if (folderMap.TryGetValue(availableBuild.Product, out string folder))
                    availableBuild.Folder = folder;
                else
                    Console.WriteLine("No flavor found matching " + availableBuild.Product);

                AvailableBuilds.Add(availableBuild);
            }
        }

        public static string[] GetListfileLines(bool forceRedownload = false)
        {
            var listfileMode = "downloaded";

            if (!SettingsManager.listfileURL.StartsWith("http") && Directory.Exists(SettingsManager.listfileURL))
                listfileMode = "parts";

            var listfileLines = new List<string>();

            if (forceRedownload)
            {
                Listfile.Clear();
                AvailableFDIDs.ForEach(x => Listfile.Add(x, ""));

                DB2Map.Clear();
                Types.Clear();
                PlaceholderFiles.Clear();
            }

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

                    using var s = WebClient.GetStreamAsync(SettingsManager.listfileURL).Result;
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

                foreach (var file in Directory.GetFiles(SettingsManager.listfileURL, "*.csv"))
                {
                    Console.WriteLine("Loading listfile parts from " + Path.GetFileNameWithoutExtension(file));
                    listfileLines.AddRange(File.ReadAllLines(file));
                }
            }

            return [.. listfileLines];
        }

        public static bool LoadListfile(bool forceRedownload = false)
        {
            var listfileLines = GetListfileLines(forceRedownload);

            if (File.Exists("custom-listfile.csv"))
                listfileLines = listfileLines.Concat(File.ReadAllLines("custom-listfile.csv")).ToArray();

            foreach (var line in listfileLines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var splitLine = line.Split(";");
                var fdid = int.Parse(splitLine[0]);
                var filename = splitLine[1];

                if (SettingsManager.showAllFiles == false && !Listfile.ContainsKey(fdid))
                    continue;

                var ext = Path.GetExtension(filename).Replace(".", "").ToLower();

                if (!TypeMap.ContainsKey(ext))
                    TypeMap.Add(ext, []);

                Listfile[fdid] = filename;

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
                    filenameLower.StartsWith("models") ||
                    filenameLower.StartsWith("unkmaps") ||
                    filenameLower.Contains("autogen-names") ||
                    filenameLower.Contains(fdid.ToString()) ||
                    filenameLower.Contains("unk_exp") ||
                    filenameLower.Contains("tileset/unused") ||
                    string.IsNullOrEmpty(filename)
                    )
                {
                    PlaceholderFiles.Add(fdid);
                }
            }

            Console.WriteLine("Finished loading listfile: " + Listfile.Count + " named files for this build");

            return true;
        }

        public static Dictionary<int, string> GetAllListfileNames()
        {
            var allNames = new Dictionary<int, string>();

            foreach (var line in GetListfileLines())
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var splitLine = line.Split(";");
                allNames[int.Parse(splitLine[0])] = splitLine[1];
            }

            Console.WriteLine("Finished loading full listfile: " + allNames.Count + " named files");

            return allNames;
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

                List<string> tactKeyLines = [];
                using (var s = WebClient.GetStreamAsync(SettingsManager.tactKeyURL + "?=v" + (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds).Result)
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

            if (IsCASCInit)
                KeyService.LoadKeys();

            // If there are known statuses, make sure to reload.
            if (EncryptionStatuses.Count > 0)
                RefreshEncryptionStatus();

            Console.WriteLine("Finished loading TACT keys: " + KnownKeys.Count + " known keys");

            return true;
        }

        public static void RefreshEncryptionStatus()
        {
            EncryptionStatuses.Clear();

            foreach (var encryptedFile in EncryptedFDIDs)
            {
                EncryptionStatus encryptionStatus;
                if (encryptedFile.Value.Count == 0)
                {
                    encryptionStatus = EncryptionStatus.EncryptedButNot;
                }
                else if (encryptedFile.Value.All(value => KnownKeys.Contains(value)))
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
        }

        public static Stream? GetFileByID(uint filedataid, string? build = null)
        {
            if (string.IsNullOrEmpty(build))
                build = BuildName;

            if (build == BuildName)
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
            else
            {
                try
                {
                    if (!Directory.Exists("temp"))
                        Directory.CreateDirectory("temp");

                    if (!Directory.Exists("temp/" + build))
                        Directory.CreateDirectory("temp/" + build);

                    if (!File.Exists("temp/" + build + "/" + filedataid))
                    {
                        var stream = WebClient.GetStreamAsync("https://wago.tools/api/casc/" + filedataid + "/?version=" + build + "&download").Result;
                        using (var fs = new FileStream("temp/" + build + "/" + filedataid, FileMode.Create))
                        {
                            stream.CopyTo(fs);
                            fs.Close();
                        }
                    }

                    return new FileStream("temp/" + build + "/" + filedataid, FileMode.Open, FileAccess.Read);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error retrieving file " + filedataid + " from build " + build + " from wago.tools: " + e.Message);
                    return null;
                }
            }
        }

        public static Stream GetDB2ByName(string filename)
        {
            if (DB2Map.TryGetValue(filename.ToLower(), out int fileDataID) && cascHandler.FileExists(fileDataID))
            {
                return cascHandler.OpenFile(fileDataID);
            }

            throw new FileNotFoundException("Could not find " + filename + " in listfile");
        }

        public static bool DB2Exists(string filename)
        {
            if (DB2Map.TryGetValue(filename.ToLower(), out int fileDataID))
            {
                return cascHandler.FileExists(fileDataID);
            }

            return false;
        }

        public static bool FileExists(uint filedataid)
        {
            return cascHandler.FileExists((int)filedataid);
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
            type = type.ToLower();

            if (!Listfile.ContainsKey(filedataid))
                return;

            if (!Types.TryGetValue(filedataid, out string? value))
                Types.Add(filedataid, type);
            else if (value == "unk")
                Types[filedataid] = type;

            if (!TypeMap.ContainsKey(type))
                TypeMap.Add(type, []);

            TypeMap[type].Add(filedataid);
        }

        public static bool EnsureCHashesLoaded()
        {
            if (FDIDToCHash.Count == 0)
            {
                // Load when requesting for first time to keep resource use low
                if (cascHandler.Root is WowTVFSRootHandler wtrh)
                {
                    foreach (var entry in wtrh.RootEntries)
                    {
                        var preferredEntry = entry.Value.FirstOrDefault(subentry =>
                        subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                        var ckey = preferredEntry.cKey.ToHexString();

                        if (ckey == "00000000000000000000000000000000")
                        {
                            preferredEntry = entry.Value.First();
                            ckey = preferredEntry.cKey.ToHexString();
                        }

                        FDIDToCHash.Add(entry.Key, ckey);

                        if (CHashToFDID.TryGetValue(ckey, out List<int>? value))
                        {
                            value.Add(entry.Key);
                        }
                        else
                        {
                            CHashToFDID.Add(ckey, [entry.Key]);
                        }
                    }
                }
                else if (cascHandler.Root is WowRootHandler wrh)
                {
                    foreach (var entry in wrh.RootEntries)
                    {
                        var preferredEntry = entry.Value.FirstOrDefault(subentry =>
                       subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                        var ckey = preferredEntry.cKey.ToHexString();

                        if (ckey == "00000000000000000000000000000000")
                        {
                            preferredEntry = entry.Value.First();
                            ckey = preferredEntry.cKey.ToHexString();
                        }

                        FDIDToCHash.Add(entry.Key, ckey);

                        if (CHashToFDID.TryGetValue(ckey, out List<int>? value))
                        {
                            value.Add(entry.Key);
                        }
                        else
                        {
                            CHashToFDID.Add(ckey, [entry.Key]);
                        }
                    }
                }

                foreach (var chash in CHashToFDID.Keys)
                {
                    if (cascHandler.Encoding.GetEntry(chash.FromHexString().ToMD5(), out var eKey))
                    {
                        CHashToSize.Add(chash, eKey.Size);
                    }
                }
            }

            return true;
        }

        public static List<int> GetSameFiles(string contenthash)
        {
            EnsureCHashesLoaded();
            if (CHashToFDID.TryGetValue(contenthash.ToUpper(), out var fdids))
            {
                return fdids;
            }
            else
            {
                return [];
            }
        }

        public static bool GenerateFileHistory()
        {
            Console.WriteLine("Generating file history, this may take a while");

            if (File.Exists("versionHistory.json"))
                File.Delete("versionHistory.json");

            var sortedManifestList = new List<string>();
            foreach (var manifest in Directory.GetFiles(SettingsManager.manifestFolder, "*.txt"))
            {
                sortedManifestList.Add(manifest);
            }

            // sort by build
            sortedManifestList.Sort((x, y) => int.Parse(Path.GetFileNameWithoutExtension(x).Split(".")[3]).CompareTo(int.Parse(Path.GetFileNameWithoutExtension(y).Split(".")[3])));

            foreach (var manifest in sortedManifestList)
            {
                var buildName = Path.GetFileNameWithoutExtension(manifest);

                // Skip wowdev build
                if (buildName == "10.0.0.43342")
                    continue;

                foreach (var line in File.ReadAllLines(manifest))
                {
                    var splitLine = line.Split(";");
                    if (splitLine.Length != 2)
                        continue;

                    var fileDataID = int.Parse(splitLine[0]);

                    if (!FDIDToCHashSet.ContainsKey(fileDataID))
                        FDIDToCHashSet.Add(fileDataID, []);

                    if (FDIDToCHashSet[fileDataID].Contains(splitLine[1]))
                        continue;

                    FDIDToCHashSet[fileDataID].Add(splitLine[1]);

                    if (!VersionHistory.ContainsKey(fileDataID))
                        VersionHistory.Add(fileDataID, []);

                    VersionHistory[fileDataID].Add(new Version() { buildName = buildName, contentHash = splitLine[1] });
                }

                Console.WriteLine(Path.GetFileNameWithoutExtension(manifest));
            }

            File.WriteAllText("versionHistory.json", JsonConvert.SerializeObject(VersionHistory, Formatting.Indented));

            return true;
        }

        public static bool LoadFileHistory()
        {
            if (!File.Exists("versionHistory.json"))
            {
                Console.WriteLine("versionHistory.json not found, please generate it first");
                return false;
            }

            VersionHistory = JsonConvert.DeserializeObject<Dictionary<int, List<Version>>>(File.ReadAllText("versionHistory.json"));

            return true;
        }

        public static bool ImportAllFileHistory()
        {
            if (VersionHistory.Count == 0)
                LoadFileHistory();

            var truncateCmd = new SqliteCommand("DELETE FROM wow_rootfiles_chashes", SQLiteDB.dbConn);
            truncateCmd.ExecuteNonQuery();

            var dropIndexCmd = new SqliteCommand("DROP INDEX IF EXISTS wow_rootfiles_chashes_idx", SQLiteDB.dbConn);
            dropIndexCmd.ExecuteNonQuery();

            var transaction = SQLiteDB.dbConn.BeginTransaction();

            var insertCmd = new SqliteCommand("INSERT INTO wow_rootfiles_chashes VALUES (@filedataid, @build, @chash)", SQLiteDB.dbConn);
            insertCmd.Parameters.AddWithValue("@filedataid", 0);
            insertCmd.Parameters.AddWithValue("@build", "");
            insertCmd.Parameters.AddWithValue("@chash", "");
            insertCmd.Prepare();

            var count = VersionHistory.Count;
            var done = 0;
            foreach (var entry in VersionHistory)
            {
                insertCmd.Parameters["@filedataid"].Value = entry.Key;
                insertCmd.Transaction = transaction;

                foreach (var version in entry.Value)
                {
                    insertCmd.Parameters["@build"].Value = version.buildName;
                    insertCmd.Parameters["@chash"].Value = version.contentHash;

                    insertCmd.ExecuteNonQuery();
                }

                done++;

                Console.Write("\r" + done + "/" + count + " (" + (done * 100 / count) + "%)");

                if (done % 1000 == 0)
                {
                    transaction.Commit();
                    transaction = SQLiteDB.dbConn.BeginTransaction();
                }
            }

            Console.WriteLine();
            transaction.Commit();

            var indexCmd = new SqliteCommand("CREATE UNIQUE INDEX IF NOT EXISTS wow_rootfiles_chashes_idx ON wow_rootfiles_chashes (fileDataID, chash)", SQLiteDB.dbConn);
            indexCmd.ExecuteNonQuery();

            return true;
        }
    }
}
