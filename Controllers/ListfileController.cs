using CASCLib;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("listfile/")]
    [ApiController]
    public class ListfileController(IDBCManager dbcManager) : Controller
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;
        private readonly Jenkins96 hasher = new();
        private static Dictionary<int, List<uint>> SoundKitMap;
        private static Dictionary<int, List<uint>> MFDMap;
        private static Dictionary<int, List<uint>> TFDMap;
        private static Dictionary<int, List<uint>> CMDMap;

        public Dictionary<int, string> DoSearch(Dictionary<int, string> resultsIn, string search)
        {
            search = search.Trim();

            if (search.StartsWith("type:"))
            {
                var cleaned = search.Replace("type:", "").ToLowerInvariant();

                if (cleaned == "model")
                {
                    var m2AndWMO = new HashSet<int>(CASC.TypeMap["m2"].Concat(CASC.TypeMap["wmo"]));
                    return resultsIn.Where(p => m2AndWMO.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
                }
                else
                {
                    if (CASC.TypeMap.TryGetValue(cleaned, out var fdids))
                        return resultsIn.Where(p => fdids.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
                }


                return [];
            }
            else if (search.StartsWith("added:"))
            {
                var builds = search.Replace("added:", "").Split("|");
                if (builds.Length != 2)
                    return [];

                if (builds[1] == "current")
                    builds[1] = CASC.BuildName;

                var newFiles = new HashSet<int>();
                if (SQLiteDB.newFilesBetweenVersion.ContainsKey(builds[0] + "|" + builds[1]))
                    newFiles = SQLiteDB.newFilesBetweenVersion[builds[0] + "|" + builds[1]];
                else
                    newFiles = SQLiteDB.getNewFilesBetweenVersions(builds[0], builds[1]);

                return resultsIn.Where(p => newFiles.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
            else if (search == "unnamed")
            {
                return resultsIn.Where(p => p.Value.Length == 0).ToDictionary();
            }
            else if (search == "isplaceholder")
            {
                return resultsIn.Where(p => CASC.PlaceholderFiles.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
            else if (search == "encrypted")
            {
                var fdids = new HashSet<int>(CASC.EncryptedFDIDs.Keys);
                return resultsIn.Where(p => fdids.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
            else if (search.StartsWith("encrypted:"))
            {
                var cleaned = search.Trim().Replace("encrypted:", "");
                if (!ulong.TryParse(cleaned, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var converted))
                    return [];

                var fdids = new HashSet<int>(CASC.EncryptedFDIDs.Where(kvp => kvp.Value.Contains(converted)).Select(kvp => kvp.Key));
                return resultsIn.Where(p => fdids.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
            else if (search == "knownkey")
            {
                return resultsIn.Where(p => CASC.EncryptionStatuses.ContainsKey(p.Key) && CASC.EncryptionStatuses[p.Key] == CASC.EncryptionStatus.EncryptedKnownKey || CASC.EncryptionStatuses.ContainsKey(p.Key) && CASC.EncryptionStatuses[p.Key] == CASC.EncryptionStatus.EncryptedMixed).ToDictionary(p => p.Key, p => p.Value);
            }
            else if (search == "unknownkey")
            {
                return resultsIn.Where(p => CASC.EncryptionStatuses.ContainsKey(p.Key) && CASC.EncryptionStatuses[p.Key] == CASC.EncryptionStatus.EncryptedUnknownKey || CASC.EncryptionStatuses.ContainsKey(p.Key) && CASC.EncryptionStatuses[p.Key] == CASC.EncryptionStatus.EncryptedMixed).ToDictionary(p => p.Key, p => p.Value);
            }
            else if (search.StartsWith("range:"))
            {
                string[] fdidRange = search.Trim().Replace("range:", "").Split("-");

                if (fdidRange.Length != 2 || !int.TryParse(fdidRange[0], out var fdidLower) || !int.TryParse(fdidRange[1], out var fdidUpper))
                    return [];

                var fdids = new HashSet<int>(CASC.Listfile.Where(kvp => fdidLower <= kvp.Key && kvp.Key <= fdidUpper).Select(kvp => kvp.Key));
                return resultsIn.Where(p => fdids.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
            else if (search == "haslookup")
            {
                var fdids = new HashSet<int>(CASC.LookupMap.Keys);
                return resultsIn.Where(p => fdids.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
            else if (search == "otherlocaleonly")
            {
                var fdids = new HashSet<int>(CASC.OtherLocaleOnlyFiles);
                return resultsIn.Where(p => fdids.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                // Simple search
                var searchLower = search.ToLowerInvariant();
                return resultsIn.Where(x => x.Value.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase) || x.Key.ToString().Equals(searchLower, StringComparison.CurrentCultureIgnoreCase) || x.Key.ToString().Contains(searchLower, StringComparison.CurrentCultureIgnoreCase)).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        // Files page uses this, not modelviewer
        [Route("files")]
        [HttpGet]
        public DataTablesResult FileDataTables(int draw, int start, int length)
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
                        foreach (var row in soundKitEntryDB.Values)
                        {
                            var soundKitID = row.Field<uint>("SoundKitID");
                            var fileDataID = row.Field<int>("FileDataID");
                            if (SoundKitMap.TryGetValue(fileDataID, out List<uint>? soundKitIDs))
                                soundKitIDs.Add(soundKitID);
                            else
                                SoundKitMap[fileDataID] = new List<uint> { soundKitID };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to load SoundKitEntry: " + e.Message);
                }
            }

            if (MFDMap == null)
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

            if (TFDMap == null)
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

            if (CMDMap == null)
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

            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = CASC.Listfile.Count,
                data = []
            };

            var listfileResults = new Dictionary<int, string>(CASC.Listfile);
            if (Request.Query.TryGetValue("search[value]", out var search) && !string.IsNullOrEmpty(search))
            {
                var searchStr = search.ToString().ToLower();
                if (searchStr.Contains(','))
                {
                    var filters = searchStr.Split(',');
                    foreach (var filter in filters)
                    {
                        listfileResults = DoSearch(listfileResults, filter);
                    }
                }
                else
                {
                    listfileResults = DoSearch(listfileResults, search!);
                }

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

                if (CASC.LookupMap.TryGetValue(listfileResult.Key, out ulong lookup))
                {
                    if (hasher.ComputeHash(listfileResult.Value) == lookup)
                        lookupMatch = true;
                }

                result.data.Add(
                    [
                        listfileResult.Key.ToString(), // ID
                        listfileResult.Value, // Filename 
                        lookup != 0 ? lookup.ToString("X16") : "", // Lookup
                        CASC.AvailableFDIDs.Contains(listfileResult.Key) ? "true" : "false", // Versions
                        CASC.Types.TryGetValue(listfileResult.Key, out string? value) ? value : "unk", // Type
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
                recordsTotal = CASC.Listfile.Count,
                data = []
            };

            var showM2 = true;
            if(Request.Query.TryGetValue("showM2", out var showM2String))
                showM2 = bool.Parse(showM2String);

            var showWMO = true;
            if (Request.Query.TryGetValue("showWMO", out var showWMOString))
                showWMO = bool.Parse(showWMOString);

            //var showADT = true;
            //if (Request.Query.TryGetValue("showADT", out var showADTString))
            //    showADT = bool.Parse(showADTString);

            var listfileResults = new Dictionary<int, string>(CASC.Listfile.Where(x => (showM2 && CASC.TypeMap["m2"].Contains(x.Key)) || (showWMO && CASC.TypeMap["wmo"].Contains(x.Key))));

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
                        CASC.LookupMap.TryGetValue(listfileResult.Key, out ulong lookup) ? lookup.ToString("X16") : "", // Lookup
                        "", // Versions
                        CASC.Types.TryGetValue(listfileResult.Key, out string? type) ? type : "unk", // Type
                        CASC.EncryptionStatuses.TryGetValue(listfileResult.Key, out CASC.EncryptionStatus encStatus) ? encStatus.ToString() : "0"
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
                    if (CASC.Listfile.TryGetValue(int.Parse(id), out var name))
                        return name;
                }

                return "";
            }
            else
            {
                if (CASC.Listfile.TryGetValue(int.Parse(filedataid), out var name))
                    return name;
                else
                    return "";
            }
        }

        [Route("db2s")]
        [HttpGet]
        public List<string> DB2s()
        {
            return dbcManager.GetDBCNames();
        }

        [HttpGet("db2/{databaseName}/versions")]
        public List<string> BuildsForDatabase(string databaseName, bool uniqueOnly = false)
        {
            var versionList = new List<Version>
            {
                new(CASC.BuildName)
            };

            if (!string.IsNullOrEmpty(SettingsManager.dbcFolder) && Directory.Exists(SettingsManager.dbcFolder))
            {
                var dbcFolder = new DirectoryInfo(SettingsManager.dbcFolder);
                var dbcFiles = dbcFolder.GetFiles("*" + databaseName + ".db*", SearchOption.AllDirectories);
                foreach (var dbcFile in dbcFiles)
                {
                    var splitFolders = dbcFile.DirectoryName.Split(Path.DirectorySeparatorChar);
                    foreach (var splitFolder in splitFolders)
                    {
                        var buildTest = splitFolder.Split(".");
                        if (buildTest.Length == 4 && buildTest.All(s => s.All(char.IsDigit)))
                        {
                            if (!versionList.Contains(new Version(splitFolder)))
                            {
                                versionList.Add(new Version(splitFolder));
                                continue;
                            }
                        }
                    }
                }
            }
            return versionList.OrderDescending().Select(v => v.ToString()).ToList();
        }

        [Route("extractFiles")]
        [HttpGet]
        public async Task<bool> ExtractFiles(string search)
        {
            if (string.IsNullOrEmpty(search))
                return false;
            var listfileResults = new Dictionary<int, string>(CASC.Listfile);
            if (!string.IsNullOrEmpty(search))
            {
                var searchStr = search.ToString().ToLower();
                if (searchStr.Contains(','))
                {
                    var filters = searchStr.Split(',');
                    foreach (var filter in filters)
                    {
                        listfileResults = DoSearch(listfileResults, filter);
                    }
                }
                else
                {
                    listfileResults = DoSearch(listfileResults, search);
                }
            }

            foreach (var result in listfileResults)
            {
                try
                {
                    using (var file = CASC.cascHandler.OpenFile(result.Key))
                    {
                        if (file == null)
                            continue;

                        var filePath = result.Value;
                        if (string.IsNullOrEmpty(result.Value))
                        {
                            if (CASC.Types.TryGetValue(result.Key, out var type))
                                filePath = "unknown/" + result.Key.ToString() + "." + CASC.Types[result.Key];
                            else
                                filePath = "unknown/" + result.Key.ToString() + ".unk";
                        }

                        var path = Path.Combine(SettingsManager.extractionDir, filePath);
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
    }

    public struct DataTablesResult
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<List<string>> data { get; set; }
    }
}
