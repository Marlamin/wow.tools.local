using CASCLib;
using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Globalization;
using wow.tools.local.Services;
using wow.tools.Services;
using WoWFormatLib.FileProviders;
using WoWFormatLib.FileReaders;

namespace wow.tools.local.Controllers
{
    [Route("listfile/")]
    [ApiController]
    public class ListfileController(IDBCManager dbcManager, IDBDProvider dbdProvider) : Controller
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;

        private readonly Jenkins96 hasher = new();
        private static Dictionary<int, List<uint>>? SoundKitMap;
        private static Dictionary<uint, List<int>>? SoundKitMapReverse;
        private static Dictionary<int, List<uint>>? MFDMap;
        private static Dictionary<int, List<uint>>? TFDMap;
        private static Dictionary<int, List<uint>>? CMDMap;
        private static readonly Lock dbcLock = new Lock();

        private readonly int ListfileSearchCacheMax = 10;
        private static List<KeyValuePair<string, Dictionary<int, string>>> ListfileSearchCache = new();
        public static int ListfileSearchCacheIsForListefileLoadID = -1;
        private static readonly Lock ListfileSearchCacheLock = new();

        private void ensureSoundKitMapInitialized()
        {
            lock (dbcLock)
            {
                if (SoundKitMap == null)
                {
                    try
                    {
                        var soundKitEntryDB = dbcManager.GetOrLoad("SoundKitEntry", CASC.BuildName).Result;
                        if (!soundKitEntryDB.AvailableColumns.Contains("SoundKitID") || !soundKitEntryDB.AvailableColumns.Contains("FileDataID"))
                            throw new Exception("Missing required columns in SoundKitEntry");

                        if (soundKitEntryDB != null)
                        {
                            SoundKitMap = new Dictionary<int, List<uint>>();
                            SoundKitMapReverse = new Dictionary<uint, List<int>>();
                            foreach (var row in soundKitEntryDB.Values)
                            {
                                var soundKitID = row.Field<uint>("SoundKitID");
                                var fileDataID = row.Field<int>("FileDataID");
                                if (SoundKitMap.TryGetValue(fileDataID, out List<uint>? soundKitIDs))
                                    soundKitIDs.Add(soundKitID);
                                else
                                    SoundKitMap[fileDataID] = new List<uint> { soundKitID };

                                if(SoundKitMapReverse.TryGetValue(soundKitID, out List<int>? fileDataIDs))
                                    fileDataIDs.Add((int)fileDataID);
                                else
                                    SoundKitMapReverse[soundKitID] = new List<int> { (int)fileDataID };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load SoundKitEntry: " + e.Message);
                    }
                }
            }
        }

        private Func<KeyValuePair<int, string>, bool> MakeFilter(string search)
        {
            if (search.StartsWith("!") && search.Length > 1)
            {
                var sub = MakeFilter(search.Substring("!".Length));
                return p => !sub(p);
            }
            else if (search.StartsWith("type:"))
            {
                var cleaned = search.Substring("type:".Length);
                if (cleaned == "model")
                {
                    var m2AndWMO = new HashSet<int>(Listfile.TypeMap["m2"].Concat(Listfile.TypeMap["wmo"]));
                    return p => m2AndWMO.Contains(p.Key);
                }
                else if (Listfile.TypeMap.TryGetValue(cleaned, out var fdids))
                {
                    return p => fdids.Contains(p.Key);
                }
            }
            else if (search.StartsWith("added:"))
            {
                var builds = search.Substring("added:".Length).Split("|");
                if (builds.Length == 2)
                {
                    if (builds[1] == "current" || builds[1] == "now")
                        builds[1] = CASC.BuildName;

                    var newFiles = new HashSet<int>();
                    if (SQLiteDB.newFilesBetweenVersion.ContainsKey(builds[0] + "|" + builds[1]))
                        newFiles = SQLiteDB.newFilesBetweenVersion[builds[0] + "|" + builds[1]];
                    else
                        newFiles = SQLiteDB.getNewFilesBetweenVersions(builds[0], builds[1]);

                    return p => newFiles.Contains(p.Key);
                }
            }
            else if (search.StartsWith("in:"))
            {
                var build = search.Substring("in:".Length);
                var presentFiles = SQLiteDB.getFilesInVersion(build);
                return p => presentFiles.Contains(p.Key);
            }
            else if (search == "unnamed")
            {
                return p => p.Value.Length == 0;
            }
            else if (search == "isplaceholder")
            {
                return p => Listfile.PlaceholderFiles.Contains(p.Key);
            }
            else if (search == "encrypted")
            {
                var fdids = new HashSet<int>(CASC.EncryptedFDIDs.Keys);
                return p => fdids.Contains(p.Key);
            }
            else if (search.StartsWith("encrypted:"))
            {
                var args = search.Substring("encrypted:".Length);
                if (ulong.TryParse(args, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var converted))
                {
                    var fdids = new HashSet<int>(CASC.EncryptedFDIDs.Where(kvp => kvp.Value.Contains(converted)).Select(kvp => kvp.Key));
                    return p => fdids.Contains(p.Key);
                }
            }
            else if (search == "knownkey")
            {
                return p => CASC.EncryptionStatuses.ContainsKey(p.Key) &&
                              (CASC.EncryptionStatuses[p.Key] == CASC.EncryptionStatus.EncryptedKnownKey ||
                               CASC.EncryptionStatuses[p.Key] == CASC.EncryptionStatus.EncryptedMixed);
            }
            else if (search == "unknownkey")
            {
                return p => CASC.EncryptionStatuses.ContainsKey(p.Key) &&
                              (CASC.EncryptionStatuses[p.Key] == CASC.EncryptionStatus.EncryptedUnknownKey ||
                               CASC.EncryptionStatuses[p.Key] == CASC.EncryptionStatus.EncryptedMixed);
            }
            else if (search.StartsWith("range:"))
            {
                string[] fdidRange = search.Substring("range:".Length).Split("-");

                if (fdidRange.Length == 2 && int.TryParse(fdidRange[0], out var fdidLower) && int.TryParse(fdidRange[1], out var fdidUpper))
                {
                    var fdids = new HashSet<int>(Listfile.NameMap.Where(kvp => fdidLower <= kvp.Key && kvp.Key <= fdidUpper).Select(kvp => kvp.Key));
                    return p => fdids.Contains(p.Key);
                }
            }
            else if (search.StartsWith("skitid:") || search.StartsWith("skit:"))
            {
                ensureSoundKitMapInitialized();
                if (uint.TryParse(search.Replace("skitid:", "").Replace("skit:", ""), out var skitID))
                    return x => SoundKitMapReverse!.ContainsKey(skitID) && SoundKitMapReverse[skitID].Contains(x.Key);
            }
            else if (search.StartsWith("chash:"))
            {
                if (CASC.CHashToFDID.TryGetValue(search.Substring("chash:".Length).ToUpperInvariant(), out var resultFDIDs))
                {
                    return x => resultFDIDs.Contains(x.Key);
                }
            }
            else if (search == "haslookup")
            {
                var fdids = new HashSet<int>(Listfile.LookupMap.Keys);
                return p => fdids.Contains(p.Key);
            }
            else if (search == "otherlocaleonly")
            {
                var fdids = new HashSet<int>(CASC.OtherLocaleOnlyFiles);
                return p => fdids.Contains(p.Key);
            }
            else if (search.StartsWith("tag:"))
            {
                var tag = search.Substring("tag:".Length);
                if (tag.Contains('='))
                {
                    var split = tag.Split('=', 2);
                    var fdids = new HashSet<int>(TagService.GetFileDataIDsByTagAndValue(split[0], split[1]));
                    return p => fdids.Contains(p.Key);
                }
                else
                {
                    var fdids = new HashSet<int>(TagService.GetFileDataIDsByTag(tag));
                    return p => fdids.Contains(p.Key);
                }
            }

            return x => x.Value.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                        x.Key.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        public Dictionary<int, string> DoSearch(Dictionary<int, string> resultsIn, string search)
        {
            IEnumerable<KeyValuePair<int, string>> results = resultsIn;
            foreach (var filter in search.ToLowerInvariant().Split(',', StringSplitOptions.TrimEntries))
            {
                results = results.Where(MakeFilter(filter));
            }
            return results.ToDictionary();
        }

        public Dictionary<int, string> GetFilteredListfileNameMap(string search)
        {
            lock (Listfile.LoadLock)
            {
                lock (ListfileSearchCacheLock)
                {
                    if (ListfileSearchCacheIsForListefileLoadID != Listfile.LoadID)
                    {
                        ListfileSearchCache.Clear();
                        ListfileSearchCacheIsForListefileLoadID = Listfile.LoadID;
                    }

                    foreach (var cacheEntry in ListfileSearchCache)
                    {
                        if (cacheEntry.Key == search)
                        {
                            return cacheEntry.Value;
                        }
                    }
                }

                var result = DoSearch(Listfile.NameMap, search);

                lock (ListfileSearchCacheLock)
                {
                    ListfileSearchCache.Add(new(search, result));
                    while (ListfileSearchCache.Count > ListfileSearchCacheMax)
                    {
                        ListfileSearchCache.RemoveAt(0);
                    }
                }

                return result;
            }
        }

        // Files page uses this, not modelviewer
        [Route("files")]
        [HttpGet]
        public DataTablesResult FileDataTables(int draw, int start, int length)
        {
            ensureSoundKitMapInitialized();

            if (MFDMap == null)
            {
                lock (dbcLock)
                {
                    try
                    {
                        var modelFileDataDB = dbcManager.GetOrLoad("ModelFileData", CASC.BuildName).Result;
                        if (!modelFileDataDB.AvailableColumns.Contains("ModelResourcesID") || !modelFileDataDB.AvailableColumns.Contains("FileDataID"))
                            throw new Exception("Missing required columns in ModelFileData");

                        if (modelFileDataDB != null)
                        {
                            MFDMap = new Dictionary<int, List<uint>>();
                            foreach (var row in modelFileDataDB.Values)
                            {
                                var modelResoucesID = row.Field<int>("ModelResourcesID");
                                var fileDataID = row.Field<int>("FileDataID");
                                if (MFDMap.TryGetValue(fileDataID, out List<uint>? modelResourceIDs))
                                    modelResourceIDs.Add((uint)modelResoucesID);
                                else
                                    MFDMap[fileDataID] = new List<uint> { (uint)modelResoucesID };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load ModelFileData: " + e.Message);
                    }
                }
            }

            if (TFDMap == null)
            {
                lock (dbcLock)
                {
                    try
                    {
                        var textureFileDataDB = dbcManager.GetOrLoad("TextureFileData", CASC.BuildName).Result;
                        if (!textureFileDataDB.AvailableColumns.Contains("MaterialResourcesID") || !textureFileDataDB.AvailableColumns.Contains("FileDataID"))
                            throw new Exception("Missing required columns in TextureFileData");

                        if (textureFileDataDB != null)
                        {
                            TFDMap = new Dictionary<int, List<uint>>();
                            foreach (var row in textureFileDataDB.Values)
                            {
                                var materialResourcesID = row.Field<int>("MaterialResourcesID");
                                var fileDataID = row.Field<int>("FileDataID");
                                if (TFDMap.TryGetValue(fileDataID, out List<uint>? materialResouceIDs))
                                    materialResouceIDs.Add((uint)materialResourcesID);
                                else
                                    TFDMap[fileDataID] = new List<uint> { (uint)materialResourcesID };
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load TextureFileData: " + e.Message);
                    }
                }
            }

            if (CMDMap == null)
            {
                lock (dbcLock)
                {
                    try
                    {
                        var creatureModelDataDB = dbcManager.GetOrLoad("CreatureModelData", CASC.BuildName).Result;
                        if (!creatureModelDataDB.AvailableColumns.Contains("FileDataID") || !creatureModelDataDB.AvailableColumns.Contains("ID"))
                        {
                            Console.WriteLine("Missing required columns in CreatureModelData");
                        }
                        else
                        {
                            if (creatureModelDataDB != null)
                            {
                                CMDMap = new Dictionary<int, List<uint>>();
                                foreach (var row in creatureModelDataDB.Values)
                                {
                                    var cmdID = row.Field<int>("ID");
                                    var fileDataID = row.Field<int>("FileDataID");
                                    if (CMDMap.TryGetValue(fileDataID, out List<uint>? creatureModelDataIDs))
                                        creatureModelDataIDs.Add((uint)cmdID);
                                    else
                                        CMDMap[fileDataID] = new List<uint> { (uint)cmdID };
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load CreatureModelData: " + e.Message);
                    }
                }
            }

            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = Listfile.NameMap.Count,
                data = []
            };

            Dictionary<int, string> listfileResults;
            if (Request.Query.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
            {
                listfileResults = GetFilteredListfileNameMap(search.ToString());
            }
            else
            {
                listfileResults = new Dictionary<int, string>(Listfile.NameMap);
            }

            result.recordsFiltered = listfileResults.Count;

            if (Request.Query.TryGetValue("order[0][column]", out var orderCol) && !string.IsNullOrEmpty(orderCol) && Request.Query.TryGetValue("order[0][dir]", out var orderDir) && !string.IsNullOrEmpty(orderDir))
            {
                switch (orderCol)
                {
                    case "0":
                        if (orderDir == "desc")
                            listfileResults = listfileResults.OrderByDescending(x => x.Key).ToDictionary();
                        else
                            listfileResults = listfileResults.OrderBy(x => x.Key).ToDictionary();
                        break;
                    case "1":
                        if (orderDir == "desc")
                            listfileResults = listfileResults.OrderByDescending(x => x.Value).ToDictionary();
                        else
                            listfileResults = listfileResults.OrderBy(x => x.Value).ToDictionary();
                        break;
                }
            }

            if (length == -1)
            {
                start = 0;
                length = listfileResults.Count;
            }

            foreach (var listfileResult in listfileResults.Skip(start).Take(length))
            {
                var lookupMatch = false;

                if (Listfile.LookupMap.TryGetValue(listfileResult.Key, out ulong lookup))
                {
                    if (hasher.ComputeHash(listfileResult.Value) == lookup)
                        lookupMatch = true;
                }

                lock (dbcLock)
                {
                    result.data.Add(
                  [
                      listfileResult.Key.ToString(), // ID
                        listfileResult.Value, // Filename
                        lookup != 0 ? lookup.ToString("X16") : "", // Lookup
                        CASC.AvailableFDIDs.Contains(listfileResult.Key) ? "true" : "false", // Versions
                        Listfile.Types.TryGetValue(listfileResult.Key, out string? value) ? value : "unk", // Type
                        CASC.EncryptionStatuses.TryGetValue(listfileResult.Key, out CASC.EncryptionStatus encryptionStatus) ? encryptionStatus.ToString() : "",
                        CASC.OtherLocaleOnlyFiles.Contains(listfileResult.Key) ? "true" : "false", // Non-native locale
                        "", // Placeholder filename
                        lookupMatch ? "true" : "false", // Lookup match
                        SoundKitMap != null ? SoundKitMap.TryGetValue(listfileResult.Key, out var soundKits) ? string.Join(", ", soundKits) : "" : "", // SoundKits
                        MFDMap != null ? MFDMap.TryGetValue(listfileResult.Key, out var modelResourceIDs) ? string.Join(", ", modelResourceIDs) : "" : "", // ModelFileData
                        TFDMap != null ? TFDMap.TryGetValue(listfileResult.Key, out var materialResourceIDs) ? string.Join(", ", materialResourceIDs) : "" : "", // TextureFileData
                        CMDMap != null ? CMDMap.TryGetValue(listfileResult.Key, out var creatureModelDataIDs) ? string.Join(", ", creatureModelDataIDs) : "" : "" // CreatureModelData
                  ]);
                }
            }

            return result;
        }

        // Modelviewer uses this, not files page
        [Route("datatables")]
        [HttpGet]
        public DataTablesResult DataTables(int draw, int start, int length)
        {
            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = Listfile.NameMap.Count,
                data = []
            };

            var showM2 = true;
            if (Request.Query.TryGetValue("showM2", out var showM2String))
                showM2 = bool.Parse(showM2String!);

            var showWMO = true;
            if (Request.Query.TryGetValue("showWMO", out var showWMOString))
                showWMO = bool.Parse(showWMOString!);

            var showM3 = false;
            if (Request.Query.TryGetValue("showM3", out var showM3String))
                showM3 = bool.Parse(showM3String!);

            //var showADT = true;
            //if (Request.Query.TryGetValue("showADT", out var showADTString))
            //    showADT = bool.Parse(showADTString);

            var listfileResults = new Dictionary<int, string>(Listfile.NameMap.Where(x => (showM2 && Listfile.TypeMap["m2"].Contains(x.Key)) || (showWMO && Listfile.TypeMap["wmo"].Contains(x.Key)) || (showM3 && Listfile.TypeMap["m3"].Contains(x.Key))));

            if (Request.Query.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
            {
                var searchStr = search.ToString().ToLower();
                listfileResults = listfileResults.Where(x => x.Value.Contains(searchStr, StringComparison.CurrentCultureIgnoreCase) || x.Key.ToString() == search).ToDictionary(x => x.Key, x => x.Value);
                result.recordsFiltered = listfileResults.Count;
            }
            else
            {
                listfileResults = listfileResults.ToDictionary(x => x.Key, x => x.Value);
                result.recordsFiltered = listfileResults.Count;
            }

            var rows = new List<string>();

            listfileResults = listfileResults.OrderBy(x => x.Key).ToDictionary();

            foreach (var listfileResult in listfileResults.Skip(start).Take(length))
            {
                result.data.Add(
                    [
                        listfileResult.Key.ToString(), // ID
                        listfileResult.Value, // Filename
                        Listfile.LookupMap.TryGetValue(listfileResult.Key, out ulong lookup) ? lookup.ToString("X16") : "", // Lookup
                        "", // Versions
                        Listfile.Types.TryGetValue(listfileResult.Key, out string? type) ? type : "unk", // Type
                        CASC.EncryptionStatuses.TryGetValue(listfileResult.Key, out CASC.EncryptionStatus encStatus) ? encStatus.ToString() : "0"
                    ]);
            }

            return result;
        }

        [Route("installFiles")]
        [HttpGet]
        public DataTablesResult InstallTable(int draw, int start, int length)
        {
            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = CASC.InstallEntries.Count,
                recordsFiltered = CASC.InstallEntries.Count,
                data = []
            };

            var installResults = CASC.InstallEntries;

            if (Request.Query.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
            {
                installResults = installResults.Where(x => x.Name.Contains(search!)).ToList();
            }

            foreach (var installResult in installResults.Skip(start).Take(length))
            {
                result.data.Add(
                    [
                        installResult.Name,
                        installResult.Size.ToString(),
                        string.Join(", ", installResult.Tags),
                        installResult.MD5.ToHexString()
                    ]);
            }

            return result;
        }

        [Route("info")]
        [HttpGet]
        public string Info(int filename, string filedataid)
        {
            var split = filedataid.Split(",");
            if (split.Length > 1)
            {
                foreach (var id in split)
                {
                    if (Listfile.NameMap.TryGetValue(int.Parse(id), out var name))
                        return name;
                }

                return "";
            }
            else
            {
                if (Listfile.NameMap.TryGetValue(int.Parse(filedataid), out var name))
                    return name;
                else
                    return "";
            }
        }

        [Route("db2s")]
        [HttpGet]
        public string[] DB2s()
        {
            return dbcManager.GetDBCNames();
        }

        [HttpGet("db2/{databaseName}/versions")]
        public List<(Version, string)> BuildsForDatabase(string databaseName, bool uniqueOnly = false)
        {
            var versionList = new SortedDictionary<Version, string>();

            if (CASC.DB2Exists("DBFilesClient/" + databaseName + ".db2"))
                versionList.Add(new Version(CASC.BuildName), "CASC");

            if (!string.IsNullOrEmpty(SettingsManager.DBCFolder))
            {
                try
                {
                    if (!Directory.Exists(SettingsManager.DBCFolder))
                        Directory.CreateDirectory(SettingsManager.DBCFolder);

                    var dbcFolder = new DirectoryInfo(SettingsManager.DBCFolder);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error creating or listing DBC folder: " + e.Message);
                }


                foreach (var build in dbdProvider.GetVersionsInDBD(databaseName))
                {
                    var buildAsVersion = new Version(build);

                    if (!versionList.ContainsKey(buildAsVersion))
                    {
                        string directoryPath = Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient");
                        if (Directory.Exists(directoryPath))
                        {
                            // Try to either find a db2 or dbc file, ignoring casing to maintain identical behavior on all platforms
                            bool hasBuildOnDisk = Directory.EnumerateFiles(directoryPath).FirstOrDefault(fn =>
                                Path.GetFileName(fn).Equals($"{databaseName}.db2", StringComparison.OrdinalIgnoreCase) ||
                                Path.GetFileName(fn).Equals($"{databaseName}.dbc", StringComparison.OrdinalIgnoreCase)) != null;

                            if (hasBuildOnDisk)
                            {
                                versionList.Add(buildAsVersion, "disk");
                                continue;
                            }
                        }

                        if (
                            (buildAsVersion.Major > 6) ||
                            (buildAsVersion.Major == 1 && buildAsVersion.Minor >= 13) ||
                            (buildAsVersion.Major == 2 && buildAsVersion.Minor >= 5) ||
                            (buildAsVersion.Major == 3 && buildAsVersion.Minor >= 4) ||
                            (buildAsVersion.Major == 4 && buildAsVersion.Minor >= 4) ||
                            (buildAsVersion.Major == 5 && buildAsVersion.Minor >= 5)
                            )
                        {
                            versionList.Add(buildAsVersion, "online");
                        }
                    }
                }
            }

            return versionList.Select(kvp => (kvp.Key, kvp.Value)).OrderByDescending(x => x.Key).ToList();
        }

        [HttpGet("db2/builds")]
        public List<string> Builds()
        {
            var versionList = new List<Version>
            {
                new(CASC.BuildName)
            };

            if (!string.IsNullOrEmpty(SettingsManager.DBCFolder) && Directory.Exists(SettingsManager.DBCFolder))
            {
                var dbcFolder = new DirectoryInfo(SettingsManager.DBCFolder);
                foreach (var subfolder in dbcFolder.EnumerateDirectories())
                {
                    var buildTest = Path.GetFileName(subfolder.Name).Split(".");
                    if (buildTest.Length == 4 && buildTest.All(s => s.All(char.IsDigit)))
                    {
                        if (!versionList.Contains(new Version(subfolder.Name)))
                        {
                            versionList.Add(new Version(subfolder.Name));
                            continue;
                        }
                    }
                }
            }
            return versionList.OrderDescending().Select(v => v.ToString()).ToList();
        }

        [Route("extractFiles")]
        [HttpGet]
        public async Task<bool> ExtractFiles(string search, bool byID = false)
        {
            if (string.IsNullOrEmpty(search) || SettingsManager.ReadOnly)
                return false;

            var listfileResults = GetFilteredListfileNameMap(search.ToString());

            foreach (var result in listfileResults)
            {
                try
                {
                    using (var file = CASC.GetFileByID((uint)result.Key))
                    {
                        if (file == null)
                            continue;

                        string filePath;
                        if (!byID)
                        {
                            filePath = result.Value;
                            if (string.IsNullOrEmpty(result.Value))
                            {
                                if (Listfile.Types.TryGetValue(result.Key, out var type))
                                    filePath = "unknown/" + result.Key.ToString() + "." + Listfile.Types[result.Key];
                                else
                                    filePath = "unknown/" + result.Key.ToString() + ".unk";
                            }
                        }
                        else
                        {
                            var fdidStr = result.Key.ToString();
                            filePath = "file/" + fdidStr.Substring(fdidStr.Length - 2, 2) + "/" + fdidStr.Substring(fdidStr.Length - 4, 2) + "/" + fdidStr;
                        }

                        var path = Path.Combine(SettingsManager.ExtractionDir, filePath);
                        if (!Directory.Exists(Path.GetDirectoryName(path)))
                            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                        using (var fs = new FileStream(path, FileMode.Create))
                            await file.CopyToAsync(fs);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to extract " + result.Key + ": " + e.Message);
                }
            }

            return true;
        }

        [Route("extractFileList")]
        [HttpGet]
        public async Task<bool> ExtractFileList(string listfile, bool related = false, string exceptInBuild = "")
        {
            if (string.IsNullOrEmpty(listfile) || SettingsManager.ReadOnly)
                return false;

            var listfileResults = new Dictionary<int, string>();
            foreach (var line in System.IO.File.ReadAllLines(listfile))
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                var split = line.Split(";");
                if (split.Length > 1)
                {
                    var fdid = int.Parse(split[0]);
                    var filename = split[1];
                    listfileResults.Add(fdid, filename);
                }
            }

            var skipFDIDs = new HashSet<int>();

            if (!string.IsNullOrEmpty(exceptInBuild))
            {
                if (!ManifestManager.ExistsForVersion(exceptInBuild))
                {
                    Console.WriteLine("Manifest file for build {0} not found", exceptInBuild);
                }
                else
                {
                    foreach (var entry in ManifestManager.GetEntriesForVersion(exceptInBuild))
                    {
                        skipFDIDs.Add((int)entry.FileDataID);
                    }
                }
            }

            if (related)
            {
                if (!FileProvider.HasProvider(CASC.BuildName))
                {
                    if (CASC.IsCASCLibInit)
                    {
                        var casc = new CASCFileProvider();
                        casc.InitCasc(CASC.cascHandler);
                        FileProvider.SetProvider(casc, CASC.BuildName);
                    }
                    else if (CASC.IsTACTSharpInit)
                    {
                        var tact = new TACTSharpFileProvider();
                        tact.InitTACT(CASC.buildInstance);
                        FileProvider.SetProvider(tact, CASC.BuildName);
                    }
                }

                var listfileResultsCopy = new Dictionary<int, string>(listfileResults);

                // first do WMOs, they can also add M2s to the list
                foreach (var result in listfileResultsCopy)
                {
                    if (skipFDIDs.Contains(result.Key))
                        continue;

                    if (!CASC.AvailableFDIDs.Contains(result.Key))
                    {
                        Console.WriteLine("File " + result.Key + " (" + result.Value + ") not found in CASC, skipping");
                        continue;
                    }

                    var fileDataID = result.Key;

                    var ext = Path.GetExtension(result.Value).ToLowerInvariant();
                    if (ext != ".wmo")
                        continue;

                    try
                    {
                        var reader = new WMOReader();
                        var wmo = new WoWFormatLib.Structs.WMO.WMO();
                        try
                        {
                            wmo = reader.LoadWMO((uint)result.Key);
                        }
                        catch (NotSupportedException)
                        {
                            Console.WriteLine("[WMO] " + fileDataID + " is a group WMO, skipping..");
                            continue;
                        }

                        if (wmo.groupFileDataIDs != null)
                            foreach (var groupFileDataID in wmo.groupFileDataIDs)
                                listfileResults.TryAdd((int)groupFileDataID, Listfile.NameMap.TryGetValue((int)groupFileDataID, out var fn) ? fn : "unknown/" + groupFileDataID.ToString() + ".wmo");

                        if (wmo.doodadIds != null)
                            foreach (var doodadID in wmo.doodadIds)
                                listfileResults.TryAdd((int)doodadID, Listfile.NameMap.TryGetValue((int)doodadID, out var fn) ? fn : "unknown/" + doodadID.ToString() + ".m2");

                        if (wmo.newLightDefinitions != null)
                            foreach (var light in wmo.newLightDefinitions)
                                if (light.lightCookieFileID != 0)
                                    listfileResults.TryAdd((int)light.lightCookieFileID, Listfile.NameMap.TryGetValue((int)light.lightCookieFileID, out var fn) && !string.IsNullOrEmpty(fn) ? fn : "unknown/" + light.lightCookieFileID.ToString() + ".blp");

                        if (wmo.textures == null && wmo.materials != null)
                        {
                            foreach (var material in wmo.materials)
                            {
                                if (material.texture1 != 0)
                                    listfileResults.TryAdd((int)material.texture1, Listfile.NameMap.TryGetValue((int)material.texture1, out var fn) ? fn : "unknown/" + material.texture1.ToString() + ".blp");

                                if (material.texture2 != 0 && !listfileResults.ContainsKey((int)material.texture2))
                                    listfileResults.TryAdd((int)material.texture2, Listfile.NameMap.TryGetValue((int)material.texture2, out var fn) ? fn : "unknown/" + material.texture2.ToString() + ".blp");

                                if (material.texture3 != 0 && !listfileResults.ContainsKey((int)material.texture3))
                                    listfileResults.TryAdd((int)material.texture3, Listfile.NameMap.TryGetValue((int)material.texture3, out var fn) ? fn : "unknown/" + material.texture3.ToString() + ".blp");

                                if ((uint)material.shader == 23)
                                {
                                    if (material.color3 != 0)
                                        listfileResults.TryAdd((int)material.color3, Listfile.NameMap.TryGetValue((int)material.color3, out var fn) ? fn : "unknown/" + material.color3.ToString() + ".blp");

                                    if (material.flags3 != 0)
                                        listfileResults.TryAdd((int)material.flags3, Listfile.NameMap.TryGetValue((int)material.flags3, out var fn) ? fn : "unknown/" + material.flags3.ToString() + ".blp");

                                    if (material.runtimeData0 != 0)
                                        listfileResults.TryAdd((int)material.runtimeData0, Listfile.NameMap.TryGetValue((int)material.runtimeData0, out var fn) ? fn : "unknown/" + material.runtimeData0.ToString() + ".blp");

                                    if (material.runtimeData1 != 0)
                                        listfileResults.TryAdd((int)material.runtimeData1, Listfile.NameMap.TryGetValue((int)material.runtimeData1, out var fn) ? fn : "unknown/" + material.runtimeData1.ToString() + ".blp");

                                    if (material.runtimeData2 != 0)
                                        listfileResults.TryAdd((int)material.runtimeData2, Listfile.NameMap.TryGetValue((int)material.runtimeData2, out var fn) ? fn : "unknown/" + material.runtimeData2.ToString() + ".blp");

                                    if (material.runtimeData3 != 0)
                                        listfileResults.TryAdd((int)material.runtimeData3, Listfile.NameMap.TryGetValue((int)material.runtimeData3, out var fn) ? fn : "unknown/" + material.runtimeData3.ToString() + ".blp");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    }
                }

                listfileResultsCopy = new Dictionary<int, string>(listfileResults);

                foreach (var result in listfileResultsCopy)
                {
                    if (skipFDIDs.Contains(result.Key))
                        continue;

                    if (!CASC.AvailableFDIDs.Contains(result.Key))
                    {
                        Console.WriteLine("File " + result.Key + " (" + result.Value + ") not found in CASC, skipping");
                        continue;
                    }

                    uint fileDataID = (uint)result.Key;

                    var ext = Path.GetExtension(result.Value).ToLowerInvariant();
                    if (ext == ".m2")
                    {
                        if (!CASC.FileExists(fileDataID))
                            continue;

                        try
                        {
                            var reader = new M2Reader();
                            reader.LoadM2(fileDataID, false);

                            if (reader.model.textureFileDataIDs != null)
                                foreach (var textureID in reader.model.textureFileDataIDs)
                                    listfileResults.TryAdd((int)textureID, Listfile.NameMap.TryGetValue((int)textureID, out var fn) ? fn : "unknown/" + textureID.ToString() + ".blp");


                            if (reader.model.animFileDataIDs != null)
                            {
                                foreach (var animFileID in reader.model.animFileDataIDs)
                                    listfileResults.TryAdd((int)animFileID.fileDataID, Listfile.NameMap.TryGetValue((int)animFileID.fileDataID, out var fn) ? fn : "unknown/" + animFileID.fileDataID.ToString() + ".anim");
                            }

                            if (reader.model.skinFileDataIDs != null)
                                foreach (var skinFileID in reader.model.skinFileDataIDs)
                                    listfileResults.TryAdd((int)skinFileID, Listfile.NameMap.TryGetValue((int)skinFileID, out var fn) ? fn : "unknown/" + skinFileID.ToString() + ".skin");

                            if (reader.model.boneFileDataIDs != null)
                                foreach (var boneFileID in reader.model.boneFileDataIDs)
                                    listfileResults.TryAdd((int)boneFileID, Listfile.NameMap.TryGetValue((int)boneFileID, out var fn) ? fn : "unknown/" + boneFileID.ToString() + ".bone");

                            if (reader.model.recursiveParticleModelFileIDs != null)
                                foreach (var rpID in reader.model.recursiveParticleModelFileIDs)
                                    listfileResults.TryAdd((int)rpID, Listfile.NameMap.TryGetValue((int)rpID, out var fn) ? fn : "unknown/" + rpID.ToString() + ".m2");

                            if (reader.model.geometryParticleModelFileIDs != null)
                                foreach (var gpID in reader.model.geometryParticleModelFileIDs)
                                    listfileResults.TryAdd((int)gpID, Listfile.NameMap.TryGetValue((int)gpID, out var fn) ? fn : "unknown/" + gpID.ToString() + ".m2");

                            listfileResults.TryAdd((int)reader.model.skelFileID, Listfile.NameMap.TryGetValue((int)reader.model.skelFileID, out var sfn) ? sfn : "unknown/" + reader.model.skelFileID.ToString() + ".skel");

                            listfileResults.TryAdd((int)reader.model.physFileID, Listfile.NameMap.TryGetValue((int)reader.model.physFileID, out var pfn) ? pfn : "unknown/" + reader.model.physFileID.ToString() + ".m2");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message + "\n" + e.StackTrace);
                        }
                    }
                }
            }

            foreach (var result in listfileResults)
            {
                if (skipFDIDs.Contains(result.Key))
                    continue;

                var filePath = result.Value;

                try
                {
                    using (var file = CASC.GetFileByID((uint)result.Key))
                    {
                        if (file == null)
                            continue;

                        if (string.IsNullOrEmpty(result.Value))
                        {
                            if (Listfile.Types.TryGetValue(result.Key, out var type))
                                filePath = "unknown/" + result.Key.ToString() + "." + Listfile.Types[result.Key];
                            else
                                filePath = "unknown/" + result.Key.ToString() + ".unk";
                        }

                        var path = Path.Combine(SettingsManager.ExtractionDir, filePath);

                        if (!Directory.Exists(Path.GetDirectoryName(path)))
                            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                        using (var fs = new FileStream(path, FileMode.Create))
                            await file.CopyToAsync(fs);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to extract " + result.Key + ": " + e.Message);
                }
            }

            return true;
        }

        [Route("dumpLookups")]
        [HttpGet]
        public bool DumpLookups()
        {
            if (SettingsManager.ReadOnly)
                return false;

            var lookupPath = Path.Combine(SettingsManager.ExtractionDir, "lookups.txt");
            using (var sw = new StreamWriter(lookupPath))
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
            return true;
        }

        [Route("dumpUnkLookups")]
        [HttpGet]
        public bool DumpUnkLookups(string search = "")
        {
            if (SettingsManager.ReadOnly)
                return false;

            if (string.IsNullOrEmpty(SettingsManager.ExtractionDir))
                throw new Exception("Extraction dir not set in config, please set it and try again");

            if (!Directory.Exists(SettingsManager.ExtractionDir))
                Directory.CreateDirectory(SettingsManager.ExtractionDir);
            var lookupPath = Path.Combine(SettingsManager.ExtractionDir, "unk_listfile.txt");
            var hasher = new Jenkins96();
            using (var sw = new StreamWriter(lookupPath))
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

                var searchResults = DoSearch(Listfile.GetAllNames(), search);
                foreach (var kvp in sortedMap.ToImmutableSortedDictionary())
                {
                    if (!searchResults.ContainsKey(kvp.Key))
                        continue;

                    if (Listfile.NameMap.TryGetValue(kvp.Key, out var filename))
                    {
                        if (hasher.ComputeHash(filename) != kvp.Value)
                        {
                            Console.WriteLine("Including currently incorrect filename for filedata ID " + kvp.Key + " (" + filename + ")");
                            sw.WriteLine(kvp.Key + ";" + kvp.Value.ToString("X16").ToLower());
                        }
                    }
                    else if (CASC.AvailableFDIDs.Contains(kvp.Key))
                    {
                        sw.WriteLine(kvp.Key + ";" + kvp.Value.ToString("X16").ToLower());
                    }
                }
            }
            return true;
        }

        [Route("clearCache")]
        [HttpGet]
        public bool ClearListfileCache()
        {
            lock (ListfileSearchCacheLock)
                ListfileSearchCache.Clear();

            return true;
        }
    }

    public struct DataTablesResult
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<List<string>> data { get; set; }
    }
}
