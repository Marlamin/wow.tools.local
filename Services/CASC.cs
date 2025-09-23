using CASCLib;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TACTSharp;
using wow.tools.Services;
using WoWFormatLib;

namespace wow.tools.local.Services
{
    public static partial class CASC
    {
        public static CASCHandler? cascHandler;
        public static bool IsCASCLibInit = false;
        public static bool IsTACTSharpInit = false;

        public static string BuildName = "";
        public static string FullBuildName = "";
        public static string CurrentProduct = "";
        public static bool IsOnline = false;

        public static List<int> AvailableFDIDs = [];

        public static readonly Dictionary<int, EncryptionStatus> EncryptionStatuses = [];
        public static readonly Dictionary<int, List<ulong>> EncryptedFDIDs = [];

        public static Dictionary<int, ulong> LookupMap = [];

        public static readonly Dictionary<string, List<int>> CHashToFDID = [];
        public static readonly Dictionary<int, byte[]> FDIDToCHash = [];
        public static readonly Dictionary<int, HashSet<string>> FDIDToCHashSet = [];
        public static readonly Dictionary<int, List<byte[]>> FDIDToExtraCHashes = [];
        public static readonly Dictionary<string, uint> CHashToSize = [];
        public static Dictionary<int, List<Version>> VersionHistory = [];
        public static List<AvailableBuild> AvailableBuilds = [];
        public static List<int> OtherLocaleOnlyFiles = [];
        public static List<InstallEntry> InstallEntries = [];

        private static readonly Lock CHashLock = new();

        [GeneratedRegex(@"(?<=e:\{)([0-9a-fA-F]{16})(?=,)", RegexOptions.Compiled)]
        private static partial Regex eKeyRegex();

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

        public static BuildInstance? buildInstance;

        public static async void InitTACT(string wowFolder, string product, string overrideBC = "", string overrideCDNC = "")
        {
            IsTACTSharpInit = false;

            buildInstance = new BuildInstance();

            if (!string.IsNullOrEmpty(product))
                buildInstance.Settings.Product = product;

            string? overrideBuildConfig = null;
            if ((File.Exists(SettingsManager.BuildConfigFile) && (overrideBuildConfig = SettingsManager.BuildConfigFile) != null) ||
                (File.Exists("fakebuildconfig") && (overrideBuildConfig = "fakebuildconfig") != null))
            {
                Console.WriteLine("Using override build config: " + overrideBuildConfig);
                buildInstance.Settings.BuildConfig = overrideBuildConfig;
            }

			string? overrideCDNConfig = null;
			if ((File.Exists(SettingsManager.CDNConfigFile) && (overrideCDNConfig = SettingsManager.CDNConfigFile) != null) ||
				(File.Exists("fakecdnconfig") && (overrideCDNConfig = "fakecdnconfig") != null))
			{
				Console.WriteLine("Using override CDN config: " + overrideCDNConfig);
				buildInstance.Settings.CDNConfig = overrideCDNConfig;
			}

			buildInstance.Settings.Locale = SettingsManager.TACTLocale;
            buildInstance.Settings.Region = SettingsManager.Region;

            if (SettingsManager.PreferHighResTextures)
                Console.WriteLine("!!!! Warning: High res textures setting is not supported when using TACTSharp.");

            if (SettingsManager.WoWProduct != product || (!string.IsNullOrEmpty(overrideBC) && !string.IsNullOrEmpty(overrideCDNC)))
            {
                Console.WriteLine("Switching builds, resetting configs..");
                buildInstance.Settings.BuildConfig = null;
                buildInstance.Settings.CDNConfig = null;
            }

            buildInstance.Settings.RootMode = RootInstance.LoadMode.Full;

            if (SettingsManager.AdditionalCDNs.Length > 0 && !string.IsNullOrEmpty(SettingsManager.AdditionalCDNs[0]))
                buildInstance.Settings.AdditionalCDNs.AddRange(SettingsManager.AdditionalCDNs);

            bool loadOnline = false;
            if (!string.IsNullOrEmpty(wowFolder) && string.IsNullOrEmpty(overrideBC) && string.IsNullOrEmpty(overrideCDNC))
            {
                // Load from build.info
                var buildInfoPath = Path.Combine(wowFolder, ".build.info");
                if (!File.Exists(buildInfoPath))
                    throw new Exception("No build.info found in base directory");

                buildInstance.Settings.BaseDir = wowFolder;

                var buildInfo = new BuildInfo(buildInfoPath, buildInstance.Settings, buildInstance.cdn);

                if (!buildInfo.Entries.Any(x => x.Product == product))
                {
                    Console.WriteLine("No .build.info found for product " + product + ", falling back to online mode.");
                    loadOnline = true;
                }
                else
                {
                    var build = buildInfo.Entries.First(x => x.Product == product);

                    if (buildInstance.Settings.BuildConfig == null)
                        buildInstance.Settings.BuildConfig = build.BuildConfig;

                    if (buildInstance.Settings.CDNConfig == null)
                        buildInstance.Settings.CDNConfig = build.CDNConfig;
                }
            }
            else
            {
                loadOnline = true;
            }

            if (loadOnline)
            {
                IsOnline = true;
                if (!string.IsNullOrEmpty(overrideBC) && !string.IsNullOrEmpty(overrideCDNC))
                {
                    buildInstance.Settings.BuildConfig = overrideBC;
                    buildInstance.Settings.CDNConfig = overrideCDNC;
                }
                else
                {
                    var versions = await buildInstance.cdn.GetPatchServiceFile(product);
                    foreach (var line in versions.Split('\n'))
                    {
                        if (!line.StartsWith(buildInstance.Settings.Region + "|"))
                            continue;

                        var splitLine = line.Split('|');

                        if (buildInstance.Settings.BuildConfig == null)
                            buildInstance.Settings.BuildConfig = splitLine[1];

                        if (buildInstance.Settings.CDNConfig == null)
                            buildInstance.Settings.CDNConfig = splitLine[2];
                    }
                }
            }

            #region Configs
            if (SettingsManager.WoWProduct == "wowdev")
            {
                buildInstance.cdn.ProductDirectory = "tpr/wowdev";
                buildInstance.Settings.TryCDN = false;
            }
            else
            {
                buildInstance.cdn.ProductDirectory = "tpr/wow";
            }

            if (!string.IsNullOrEmpty(SettingsManager.CDNFolder))
                buildInstance.Settings.CDNDir = SettingsManager.CDNFolder;

            if (!IsOnline)
                buildInstance.cdn.OpenLocal();

            try
            {
                if (buildInstance.Settings.BuildConfig == null || buildInstance.Settings.CDNConfig == null)
                    throw new Exception("BuildConfig or CDNConfig is null");

                buildInstance.LoadConfigs(buildInstance.Settings.BuildConfig, buildInstance.Settings.CDNConfig);
                if (buildInstance.BuildConfig == null || buildInstance.CDNConfig == null)
                    throw new Exception("Failed to load configs");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load configs: " + e.Message);
                return;
            }

            try
            {
                buildInstance.Load();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load build: " + e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }

            if (!buildInstance.BuildConfig.Values.TryGetValue("encoding", out var encodingKey))
                throw new Exception("No encoding key found in build config");

            if (buildInstance.Encoding == null || buildInstance.Root == null || buildInstance.Install == null)
                throw new Exception("Encoding, root or install are null");

            #endregion

            var totalTimer = new Stopwatch();
            totalTimer.Start();

            CurrentProduct = product;
            FullBuildName = buildInstance.BuildConfig.Values["build-name"][0];
            var splitName = FullBuildName.Replace("WOW-", "").Split("patch");
            BuildName = splitName[1].Split("_")[0] + "." + splitName[0];

            try
            {
                SQLiteDB.InsertBuildIfNotExists(CurrentProduct, BuildName, buildInstance.Settings.BuildConfig, buildInstance.Settings.CDNConfig);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error inserting build into database: " + e.Message);
            }

            var manifestFolder = SettingsManager.ManifestFolder;

            IsTACTSharpInit = true;

            WTLKeyService.LoadKeys();

            #region Install entry conversion between TACTSharp and CASCLib
            var hasher = new CASCLib.Jenkins96();

            var installTags = new Dictionary<string, InstallTag>();
            foreach (var installTag in buildInstance.Install.Tags)
            {
                var cascInstallTag = new InstallTag()
                {
                    Name = installTag.name,
                    Type = (short)installTag.type,
                    Bits = installTag.files
                };

                installTags.Add(installTag.name, cascInstallTag);
            }

            InstallEntries.Clear();

            foreach (var installEntry in buildInstance.Install.Entries)
            {
                var cascInstallEntry = new InstallEntry()
                {
                    MD5 = installEntry.md5.ToMD5(),
                    Size = (int)installEntry.size,
                    Name = installEntry.name,
                    Hash = hasher.ComputeHash(installEntry.name),
                    Tags = new List<InstallTag>()
                };

                foreach (var usedTag in installEntry.tags)
                {
                    cascInstallEntry.Tags.Add(installTags[usedTag.Split('=')[1]]);
                }

                InstallEntries.Add(cascInstallEntry);
            }
            #endregion

            if (buildInstance.Settings.BaseDir != null)
            {
                LoadBuildInfo();

                foreach (var build in AvailableBuilds)
                {
                    if (!string.IsNullOrEmpty(build.KeyRing))
                    {
                        try
                        {
                            var keyring = WebClient.GetStreamAsync("https://blzddist1-a.akamaihd.net/" + build.CDNPath + "/config/" + build.KeyRing[0] + build.KeyRing[1] + "/" + build.KeyRing[2] + build.KeyRing[3] + "/" + build.KeyRing).Result;

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
                                if (lookup.Length != 16)
                                {
                                    Console.WriteLine("Warning: KeyRing lookup " + lookup + " is not 16 characters long, skipping..");
                                    continue;
                                }

                                var parsedLookup = BitConverter.ToUInt64(lookup.ToByteArray(), 0);

                                if (WTLKeyService.HasKey(parsedLookup))
                                    continue;

                                Console.WriteLine("Setting key " + parsedLookup.ToString("X") + " from KeyRing " + build.KeyRing);
                                WTLKeyService.SetKey(parsedLookup, splitLine[1].ToByteArray());
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error loading keyring: " + e.Message);
                        }
                    }
                }
            }

            AvailableFDIDs.Clear();
            AvailableFDIDs.AddRange(buildInstance.Root.GetAvailableFDIDs().Select(x => (int)x));

            Directory.CreateDirectory(manifestFolder);
            if (!File.Exists(Path.Combine(manifestFolder, BuildName + ".txt")))
            {
                var manifestLines = new List<string>();
                foreach (var fdid in buildInstance.Root.GetAvailableFDIDs())
                {
                    var rootEntries = buildInstance.Root.GetEntriesByFDID(fdid);
                    if (rootEntries.Count == 0)
                        continue;

                    var preferredEntry = rootEntries.FirstOrDefault(subentry =>
subentry.contentFlags.HasFlag(RootInstance.ContentFlags.LowViolence) == false && (subentry.localeFlags.HasFlag(RootInstance.LocaleFlags.All_WoW) || subentry.localeFlags.HasFlag(RootInstance.LocaleFlags.enUS)));

                    if (preferredEntry.fileDataID == 0)
                        preferredEntry = rootEntries.First();

                    manifestLines.Add(fdid + ";" + Convert.ToHexString(preferredEntry.md5.AsSpan()));
                }

                manifestLines.Sort();

                File.WriteAllLines(Path.Combine(manifestFolder, BuildName + ".txt"), manifestLines);

                SQLiteDB.ImportBuildIntoFileHistory(BuildName);

                Console.WriteLine("Force updating DBDs after new build..");
                DBDProvider.GetBDBDStream(true);
            }

            EncryptedFDIDs.Clear();
            EncryptionStatuses.Clear();
            LookupMap = [];

            if (File.Exists("cachedLookups.txt"))
                LookupMap = File.ReadAllLines("cachedLookups.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => ulong.Parse(x[1]));

            #region Listfile
            bool listfileRes;

            try
            {
                listfileRes = Listfile.Load();
            }
            catch (Exception e)
            {   // attempt automatic redownload of the listfile if it wasn't able to be parsed - this will also backup the old listfile to listfile.csv.bak
                Console.WriteLine("Good heavens! Encountered an error reading listfile (" + e.Message + "). Attempting redownload...");
                listfileRes = Listfile.Load(true);
            }

            if (!listfileRes)
            {   // still no listfile, exit
                Console.WriteLine("Failed to read listfile after automatic redownload.");
                Environment.Exit(1);
            }
            #endregion

            Console.WriteLine("Analyzing files");
            var chashLock = new Lock();

            Parallel.ForEach(buildInstance.Root.GetAvailableFDIDs(), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, fdid =>
            {
                var entries = buildInstance.Root.GetEntriesByFDID(fdid);
                if (entries.Count == 0)
                    return;

                int fdidInt = (int)fdid;

                lock (EncryptedFDIDs)
                {
                    if (EncryptedFDIDs.ContainsKey(fdidInt))
                        return;

                    if ((entries[0].contentFlags & RootInstance.ContentFlags.Encrypted) != 0)
                        EncryptedFDIDs.TryAdd(fdidInt, new List<ulong>());
                }
                var cKey = entries[0].md5.AsSpan();
                var eKeys = buildInstance.Encoding.FindContentKey(cKey);
                if (eKeys != false)
                {
                    lock (chashLock)
                        CHashToSize.TryAdd(Convert.ToHexStringLower(cKey.ToArray()), (uint)eKeys.DecodedFileSize);

                    var eSpec = buildInstance.Encoding.GetESpec(eKeys[0]);
                    var matches = eKeyRegex().Matches(eSpec.eSpec);

                    if (matches.Count > 0)
                    {
                        var keys = matches.Cast<Match>().Select(m => BitConverter.ToUInt64(m.Value.FromHexString(), 0)).ToList();
                        if (keys.Count > 0)
                        {
                            lock (EncryptedFDIDs)
                            {
                                if (EncryptedFDIDs.TryGetValue(fdidInt, out List<ulong>? encryptedIDs))
                                    encryptedIDs.AddRange(keys);
                                else
                                    EncryptedFDIDs[fdidInt] = new List<ulong>(keys);
                            }
                        }
                    }
                }
            });

            // Lookups
            foreach (var entry in buildInstance.Root.GetAvailableLookups())
            {
                var fileEntries = buildInstance.Root.GetEntriesByLookup(entry);
                if (fileEntries.Count == 0)
                    continue;

                LookupMap.TryAdd((int)fileEntries[0].fileDataID, entry);
            }

            File.WriteAllLines("cachedLookups.txt", LookupMap.Select(x => x.Key + ";" + x.Value));

            Console.WriteLine("Found " + EncryptedFDIDs.Count + " encrypted files");
            RefreshEncryptionStatus();
            Console.WriteLine("Done analyzing encrypted files");

            Listfile.LoadCachedUnknowns();

            try
            {
                HotfixManager.LoadCaches();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading hotfixes: " + e.Message);
            }

            IsTACTSharpInit = true;

            Console.WriteLine("Finished loading " + BuildName);
        }

        public static void InitCasc(string? basedir = null, string program = "wowt", LocaleFlags locale = LocaleFlags.enUS)
        {
            IsCASCLibInit = false;

            WebClient.DefaultRequestHeaders.Add("User-Agent", "wow.tools.local");

            CASCConfig.ValidateData = false;
            CASCConfig.ThrowOnFileNotFound = false;
            CASCConfig.UseWowTVFS = false;
            CASCConfig.LoadFlags = LoadFlags.Install;
            CASCConfig.BuildConfigOverride = "fakebuildconfig";
            CASCConfig.CDNConfigOverride = "fakecdnconfig";

            locale = SettingsManager.CASCLocale;

            if (basedir == null)
            {
                Console.WriteLine("Initializing CASC from web for program " + program + " and locale " + locale);
                cascHandler = CASCHandler.OpenOnlineStorage(program, SettingsManager.Region);
                IsCASCLibInit = true;
                IsOnline = true;
            }
            else
            {
                basedir = basedir.Replace("_retail_", "").Replace("_ptr_", "");
                Console.WriteLine("Initializing CASC from local disk with basedir " + basedir + " and program " + program + " and locale " + locale);
                cascHandler = CASCHandler.OpenLocalStorage(basedir, program);
                IsCASCLibInit = true;

                LoadBuildInfo();

                foreach (var build in AvailableBuilds)
                {
                    if (!string.IsNullOrEmpty(build.KeyRing))
                    {
                        try
                        {
                            var keyring = WebClient.GetStreamAsync("https://blzddist1-a.akamaihd.net/" + build.CDNPath + "/config/" + build.KeyRing[0] + build.KeyRing[1] + "/" + build.KeyRing[2] + build.KeyRing[3] + "/" + build.KeyRing).Result;

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
                                if (lookup.Length != 16)
                                {
                                    Console.WriteLine("Warning: KeyRing lookup " + lookup + " is not 16 characters long, skipping..");
                                    continue;
                                }

                                var parsedLookup = BitConverter.ToUInt64(lookup.ToByteArray(), 0);

                                if (WTLKeyService.HasKey(parsedLookup))
                                    continue;

                                Console.WriteLine("Setting key " + parsedLookup.ToString("X") + " from KeyRing " + build.KeyRing);
                                WTLKeyService.SetKey(parsedLookup, splitLine[1].ToByteArray());
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error loading keyring: " + e.Message);
                        }
                    }
                }
            }

            WTLKeyService.LoadKeys();

            CurrentProduct = program;

            FullBuildName = cascHandler.Config.BuildName;
            var splitName = FullBuildName.Replace("WOW-", "").Split("patch");
            BuildName = splitName[1].Split("_")[0] + "." + splitName[0];

            cascHandler.Root.SetFlags(locale, false, SettingsManager.PreferHighResTextures);
            var manifestFolder = SettingsManager.ManifestFolder;

            if (!Directory.Exists(manifestFolder))
                Directory.CreateDirectory(manifestFolder);

            InstallEntries = cascHandler.Install.GetEntries().ToList();

            try
            {
                SQLiteDB.InsertBuildIfNotExists(CurrentProduct, BuildName, cascHandler.Config.GetVersionsVariable("BuildConfig"), cascHandler.Config.GetVersionsVariable("CDNConfig"));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error inserting build into database: " + e.Message);
            }

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

                    Console.WriteLine("Force updating DBDs after new build..");
                    DBDProvider.GetBDBDStream(true);
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

                    Console.WriteLine("Force updating DBDs after new build..");
                    DBDProvider.GetBDBDStream(true);
                }
            }

            EncryptedFDIDs.Clear();
            EncryptionStatuses.Clear();
            LookupMap = [];

            if (File.Exists("cachedLookups.txt"))
                LookupMap = File.ReadAllLines("cachedLookups.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => ulong.Parse(x[1]));

            AvailableFDIDs.ForEach(x => Listfile.NameMap.TryAdd(x, ""));

            bool listfileRes;

            try
            {
                listfileRes = Listfile.Load();
            }
            catch (Exception e)
            {   // attempt automatic redownload of the listfile if it wasn't able to be parsed - this will also backup the old listfile to listfile.csv.bak
                Console.WriteLine("Good heavens! Encountered an error reading listfile (" + e.Message + "). Attempting redownload...");
                listfileRes = Listfile.Load(true);
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

            Listfile.LoadCachedUnknowns();

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



        public static void LoadBuildInfo()
        {
            AvailableBuilds.Clear();

            var folderMap = new Dictionary<string, string>();
            foreach (var flavorFile in Directory.GetFiles(SettingsManager.WoWFolder, ".flavor.info", SearchOption.AllDirectories))
            {
                var flavorLines = File.ReadAllLines(flavorFile);
                if (flavorLines.Length < 2)
                    continue;

                folderMap.Add(flavorLines[1], Path.GetFileName(Path.GetDirectoryName(flavorFile)!));
            }

            var headerMap = new Dictionary<string, byte>();
            foreach (var line in File.ReadAllLines(Path.Combine(SettingsManager.WoWFolder, ".build.info")))
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
                    Armadillo = splitLine[headerMap["Armadillo"]],
                    Product = splitLine[headerMap["Product"]]
                };

                if (headerMap.TryGetValue("KeyRing", out byte keyRing))
                    availableBuild.KeyRing = splitLine[keyRing];

                if (folderMap.TryGetValue(availableBuild.Product, out var folder))
                    availableBuild.Folder = folder;
                else
                    Console.WriteLine("No flavor found matching " + availableBuild.Product);

                try
                {
                    SQLiteDB.InsertBuildIfNotExists(availableBuild.Product, availableBuild.Version, availableBuild.BuildConfig, availableBuild.CDNConfig);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error inserting build into database: " + e.Message);
                }

                AvailableBuilds.Add(availableBuild);
            }
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
                else if (encryptedFile.Value.All(value => WTLKeyService.KnownKeys.Contains(value)))
                {
                    encryptionStatus = EncryptionStatus.EncryptedKnownKey;
                }
                else if (encryptedFile.Value.Any(value => WTLKeyService.KnownKeys.Contains(value)))
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

        public static Stream? GetFileByEKey(MD5Hash EKey, long decodedSize)
        {
            if (IsCASCLibInit)
                return cascHandler!.OpenFile(EKey);
            else if (IsTACTSharpInit)
            {
                var eKey = Convert.FromHexString(EKey.ToHexString());
                var (offset, size, archiveIndex) = buildInstance!.GroupIndex!.GetIndexInfo(eKey);
                byte[] fileBytes;
                if (offset == -1)
                    fileBytes = buildInstance.cdn.GetFile("data", Convert.ToHexStringLower(eKey), 0, (ulong)decodedSize, true);
                else
                    fileBytes = buildInstance.cdn.GetFileFromArchive(Convert.ToHexStringLower(eKey), buildInstance.CDNConfig!.Values["archives"][archiveIndex], offset, size, (ulong)decodedSize, true);

                return new MemoryStream(fileBytes);
            }
            else
                return null;
        }

        public static bool TryGetEKeysByCKey(MD5Hash CKey, out EncodingEntry EKeys)
        {
            if (IsCASCLibInit)
                return cascHandler!.Encoding.GetEntry(CKey, out EKeys);
            else if (IsTACTSharpInit)
            {
                var ckey = Convert.FromHexString(CKey.ToHexString());
                var TEKeys = buildInstance!.Encoding!.FindContentKey(ckey);

                if (TEKeys)
                {
                    var md5HashEkeys = new List<MD5Hash>();
                    for (var i = 0; i < TEKeys.Length; i++)
                    {
                        var ekey = TEKeys[i];
                        md5HashEkeys.Add(ekey.ToArray().ToMD5());
                    }

                    EKeys = new EncodingEntry
                    {
                        Keys = md5HashEkeys,
                        Size = (long)TEKeys.DecodedFileSize
                    };

                    return true;
                }
                else
                {
                    EKeys = new EncodingEntry();
                    return false;
                }
            }
            else
            {
                throw new Exception("No CASC or TACTSharp handler initialized");
            }
        }

        public static Stream? GetFileByID(uint filedataid, string? build = null, LocaleFlags locale = LocaleFlags.All_WoW)
        {
            if (string.IsNullOrEmpty(build))
                build = BuildName;

            if (build == BuildName)
            {
                if (IsCASCLibInit)
                {
                    try
                    {
                        return cascHandler!.OpenFile((int)filedataid);
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
                else if (IsTACTSharpInit)
                {
                    var tactLocale = (RootInstance.LocaleFlags)locale;

                    if (locale == LocaleFlags.All_WoW)
                        tactLocale = buildInstance!.Settings.Locale;

                    var rootEntries = buildInstance!.Root!.GetEntriesByFDID(filedataid);
                    if (rootEntries.Count == 0)
                        return null;

                    var preferredEntry = rootEntries.FirstOrDefault(subentry =>
subentry.contentFlags.HasFlag(RootInstance.ContentFlags.LowViolence) == false && (subentry.localeFlags.HasFlag((RootInstance.LocaleFlags)tactLocale)) || subentry.localeFlags.HasFlag(RootInstance.LocaleFlags.All_WoW));

                    if (preferredEntry.fileDataID == 0)
                        preferredEntry = rootEntries.First();

                    var fileEKeys = buildInstance.Encoding!.FindContentKey(preferredEntry.md5);
                    if (fileEKeys == false)
                        throw new Exception("EKey not found in encoding");

                    var eKey = fileEKeys[0];

                    var (offset, size, archiveIndex) = buildInstance!.GroupIndex!.GetIndexInfo(eKey);
                    byte[] fileBytes;
                    if (offset == -1)
                        fileBytes = buildInstance.cdn.GetFile("data", Convert.ToHexStringLower(eKey), 0, fileEKeys.DecodedFileSize, true);
                    else
                        fileBytes = buildInstance.cdn.GetFileFromArchive(Convert.ToHexStringLower(eKey), buildInstance.CDNConfig!.Values["archives"][archiveIndex], offset, size, fileEKeys.DecodedFileSize, true);

                    return new MemoryStream(fileBytes);
                }
                else
                {
                    throw new Exception("No CASC or TACTSharp handler initialized");
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

        public static Stream GetDB2ByName(string filename, LocaleFlags locale)
        {
            if (Listfile.DB2Map.TryGetValue(filename.ToLower(), out int fileDataID) && FileExists((uint)fileDataID))
            {
                return GetFileByID((uint)fileDataID, CASC.BuildName, locale)!;
            }

            throw new FileNotFoundException("Could not find " + filename + " in listfile");
        }

        public static bool DB2Exists(string filename)
        {
            if (Listfile.DB2Map.TryGetValue(filename.ToLower(), out int fileDataID))
            {
                return FileExists((uint)fileDataID);
            }

            return false;
        }

        public static bool FileExists(uint filedataid)
        {
            if (IsCASCLibInit)
                return cascHandler!.FileExists((int)filedataid);
            else if (IsTACTSharpInit)
                return buildInstance!.Root!.FileExists(filedataid);
            else
                return false;
        }

        public static bool EnsureCHashesLoaded()
        {
            if (FDIDToCHash.Count == 0)
            {
                // Load when requesting for first time to keep resource use low

                if (IsCASCLibInit)
                {
                    if (cascHandler!.Root is WowTVFSRootHandler wtrh)
                    {
                        foreach (var entry in wtrh.RootEntries)
                        {
                            var preferredEntry = entry.Value.FirstOrDefault(subentry =>
                            subentry.ContentFlags.HasFlag(ContentFlags.Alternate) == false && (subentry.LocaleFlags.HasFlag(LocaleFlags.All_WoW) || subentry.LocaleFlags.HasFlag(LocaleFlags.enUS)));

                            var ckey = preferredEntry.cKey.ToHexString();

                            if (preferredEntry.cKey.lowPart == 0 && preferredEntry.cKey.highPart == 0)
                            {
                                preferredEntry = entry.Value.First();
                                ckey = preferredEntry.cKey.ToHexString();
                            }

                            FDIDToCHash.Add(entry.Key, ckey.ToByteArray());

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

                            if (preferredEntry.cKey.lowPart == 0 && preferredEntry.cKey.highPart == 0)
                            {
                                preferredEntry = entry.Value.First();
                                ckey = preferredEntry.cKey.ToHexString();
                            }

                            FDIDToCHash.Add(entry.Key, ckey.ToByteArray());

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
                            CHashToSize.Add(chash, (uint)eKey.Size);
                        }
                    }
                }
                else if (IsTACTSharpInit)
                {
                    Parallel.ForEach(buildInstance!.Root!.GetAvailableFDIDs(), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, fdid =>
                    {
                        var rootEntries = buildInstance.Root.GetEntriesByFDID(fdid);
                        if (rootEntries.Count == 0)
                            return;

                        var preferredEntry = rootEntries.FirstOrDefault(subentry => subentry.contentFlags.HasFlag(RootInstance.ContentFlags.LowViolence) == false && (subentry.localeFlags.HasFlag(RootInstance.LocaleFlags.All_WoW) || subentry.localeFlags.HasFlag(buildInstance.Settings.Locale)));

                        if (preferredEntry.fileDataID == 0)
                            preferredEntry = rootEntries.First();

                        var ckey = Convert.ToHexString(preferredEntry.md5.AsSpan());

                        lock (CHashLock)
                        {
                            FDIDToCHash.Add((int)preferredEntry.fileDataID, preferredEntry.md5.AsSpan().ToArray());

                            if (CHashToFDID.TryGetValue(ckey, out List<int>? currentFDIDs))
                                currentFDIDs.Add((int)preferredEntry.fileDataID);
                            else
                                CHashToFDID.Add(ckey, [(int)preferredEntry.fileDataID]);
                        }

                        if (rootEntries.Count > 1)
                        {
                            for (int i = 1; i < rootEntries.Count; i++)
                            {
                                var cKey = rootEntries[i].md5.AsSpan().ToArray();

                                lock (CHashLock)
                                {
                                    if (FDIDToCHash[(int)fdid].SequenceEqual(cKey))
                                        continue;

                                    if (FDIDToExtraCHashes.TryGetValue((int)fdid, out List<byte[]>? extraCHashes))
                                    {
                                        if (!extraCHashes.Contains(cKey))
                                            extraCHashes.Add(cKey);
                                    }
                                    else
                                    {
                                        FDIDToExtraCHashes[(int)fdid] = new List<byte[]> { cKey };
                                    }
                                }
                            }
                        }
                    });

                    Parallel.ForEach(CHashToFDID.Keys, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, chash =>
                    {
                        var eKeys = buildInstance.Encoding!.FindContentKey(Convert.FromHexString(chash));
                        if (eKeys)
                            lock (CHashLock)
                                CHashToSize.Add(chash, (uint)eKeys.DecodedFileSize);
                    });
                }
                else
                {
                    throw new Exception("No CASC or TACTSharp handler initialized");
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
            foreach (var manifest in Directory.GetFiles(SettingsManager.ManifestFolder, "*.txt"))
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

            VersionHistory = JsonConvert.DeserializeObject<Dictionary<int, List<Version>>>(File.ReadAllText("versionHistory.json")) ?? [];

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
