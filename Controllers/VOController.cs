using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("vo/")]
    public class VOController : Controller
    {
        private readonly DBCManager dbcManager;
        private static readonly List<uint> voSoundKitIDs = [];
        private static readonly Dictionary<uint, List<uint>> soundKitIDToFDID = [];
        private static readonly Dictionary<uint, List<uint>> fdidToSoundKitID = [];

        private static Dictionary<uint, List<uint>> soundKitToBroadcastTextID = [];
        private static Dictionary<uint, string> creatureNames = [];

        public VOController(IDBCManager dbcManager)
        {
            this.dbcManager = (DBCManager)dbcManager;

            if (voSoundKitIDs.Count == 0)
            {
                var SoundKitDB = this.dbcManager.GetOrLoad("SoundKit", CASC.BuildName, true).Result;
                if (!SoundKitDB.AvailableColumns.Contains("DialogType") || !SoundKitDB.AvailableColumns.Contains("ID"))
                    throw new Exception("Unable to initialize VO list, missing one of the required columns.");

                foreach (var soundKit in SoundKitDB.Values)
                {
                    if (uint.Parse(soundKit["DialogType"].ToString()!) == 1)
                        voSoundKitIDs.Add(uint.Parse(soundKit["ID"].ToString()!));
                }
            }

            if (soundKitIDToFDID.Count == 0)
            {
                var SoundKitEntryDB = this.dbcManager.GetOrLoad("SoundKitEntry", CASC.BuildName, true).Result;
                if (!SoundKitEntryDB.AvailableColumns.Contains("SoundKitID") || !SoundKitEntryDB.AvailableColumns.Contains("FileDataID"))
                    throw new Exception("Unable to initialize VO list, missing one of the required columns.");

                foreach (var soundKitEntry in SoundKitEntryDB.Values)
                {
                    uint soundKitID = uint.Parse(soundKitEntry["SoundKitID"].ToString()!);
                    uint fileDataID = uint.Parse(soundKitEntry["FileDataID"].ToString()!);

                    if (!soundKitIDToFDID.ContainsKey(soundKitID))
                        soundKitIDToFDID[soundKitID] = [];

                    soundKitIDToFDID[soundKitID].Add(fileDataID);
                }

                foreach (var soundKitID in soundKitIDToFDID.Keys)
                {
                    foreach (var fileDataID in soundKitIDToFDID[soundKitID])
                    {
                        if (!fdidToSoundKitID.ContainsKey(fileDataID))
                            fdidToSoundKitID[fileDataID] = [];

                        fdidToSoundKitID[fileDataID].Add(soundKitID);
                    }
                }
            }

            if (soundKitToBroadcastTextID.Count == 0)
                soundKitToBroadcastTextID = SQLiteDB.GetSoundKitToBCTextIDs();

            if (creatureNames.Count == 0)
                creatureNames = SQLiteDB.GetCreatureNames();
        }

        [Route("clearCache")]
        [HttpGet]
        public void ClearCache()
        {
            voSoundKitIDs.Clear();
            soundKitIDToFDID.Clear();
            fdidToSoundKitID.Clear();
            soundKitToBroadcastTextID.Clear();
            creatureNames.Clear();
        }

        [Route("startBackfill")]
        [HttpGet]
        public void StartBackfill()
        {
            var uniqueCreatureNames = creatureNames.Values.Distinct().ToList();
            var uniqueCreatureNamesLower = uniqueCreatureNames.Select(x => x.ToLowerInvariant()).ToList();

            var addedToDBCount = 0;
            var conflictCount = 0;
            var skipCount = 0;
            var noNameCount = 0;

            var creatureVOFiles = Listfile.NameMap.Where(f => f.Value.StartsWith("sound/creature/", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(x => x.Key, x => x.Value);
            foreach (var creatureVOFile in creatureVOFiles)
            {
                var extractedName = creatureVOFile.Value.Split("/")[2].Replace("_", " ");

                extractedName = extractedName.Replace("vo 835", "").Replace("vo 83", "");

                var actualNameIndex = uniqueCreatureNamesLower.IndexOf(extractedName.ToLower());

                if (actualNameIndex != -1)
                {
                    var actualNameProperCase = uniqueCreatureNames[actualNameIndex];

                    var currentName = SQLiteDB.getCreatureNameByFileDataID(creatureVOFile.Key);
                    if (currentName != "")
                    {
                        if (currentName == actualNameProperCase)
                        {
                            skipCount++;
                            //Console.WriteLine($"Skipping {actualNameProperCase} for file " + creatureVOFile.Key + " as it already exists in the database");
                        }
                        else
                        {
                            Console.WriteLine($"Conflict for file " + creatureVOFile.Key + " (" + creatureVOFile.Value + "): " + currentName + " != " + actualNameProperCase);
                            conflictCount++;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Adding {actualNameProperCase} to the database for file " + creatureVOFile.Key + " (" + creatureVOFile.Value + ")");
                        SQLiteDB.SetCreatureNameForFDID(creatureVOFile.Key, actualNameProperCase);
                        addedToDBCount++;
                    }
                }
                else
                {
                    noNameCount++;
                }
            }

            Console.WriteLine("New entries: " + addedToDBCount + ", conflicts: " + conflictCount + ", skips (same): " + skipCount + ", no valid name: " + noNameCount + ", total checked: " + creatureVOFiles.Count);
        }

        [Route("list")]
        [HttpGet]
        public DataTablesResult VOList(int draw, int start, int length)
        {
            var result = new DataTablesResult()
            {
                draw = draw,
                data = [],
                recordsFiltered = voSoundKitIDs.Count,
                recordsTotal = voSoundKitIDs.Count
            };

            var voSoundKitView = new List<uint>(voSoundKitIDs);

            var parameters = new Dictionary<string, string>();

            foreach (var get in Request.Query)
                parameters.Add(get.Key, get.Value!);

            if (parameters.TryGetValue("columns[0][search][value]", out var skitFilter) && !string.IsNullOrWhiteSpace(skitFilter))
            {
                voSoundKitView = voSoundKitView.Where(x => x.ToString().Contains(skitFilter)).ToList();
                result.recordsFiltered = voSoundKitView.Count;
            }

            if (parameters.TryGetValue("columns[1][search][value]", out var fileFilter) && !string.IsNullOrWhiteSpace(fileFilter))
            {
                if (uint.TryParse(fileFilter, out var fileDataID))
                {
                    voSoundKitView = voSoundKitView.Where(x => soundKitIDToFDID[x].Contains(fileDataID)).ToList();
                    result.recordsFiltered = voSoundKitView.Count;
                }
                else
                {
                    fileFilter = fileFilter.ToLowerInvariant();
                    voSoundKitView = voSoundKitView.Where(x => soundKitIDToFDID[x].Any(y => Listfile.NameMap.TryGetValue((int)y, out var filename) && filename.Contains(fileFilter, StringComparison.InvariantCultureIgnoreCase))).ToList();
                    result.recordsFiltered = voSoundKitView.Count;
                }
            }

            if (parameters.TryGetValue("columns[2][search][value]", out var bcTextFilter) && !string.IsNullOrWhiteSpace(bcTextFilter))
            {
                if (uint.TryParse(bcTextFilter, out var bcTextID))
                {
                    voSoundKitView = voSoundKitView.Where(x => soundKitToBroadcastTextID.TryGetValue(x, out var bcTexts) && bcTexts.Contains(bcTextID)).ToList();
                    result.recordsFiltered = voSoundKitView.Count;
                }
                else
                {
                    bcTextFilter = bcTextFilter.ToLowerInvariant();
                    var broadcastTextIDs = SQLiteDB.SearchBroadcastText(bcTextFilter);
                    voSoundKitView = voSoundKitView.Where(x => soundKitToBroadcastTextID.TryGetValue(x, out var bcTexts) && bcTexts.Any(y => broadcastTextIDs.Contains(y))).ToList();

                    result.recordsFiltered = voSoundKitView.Count;
                }
            }

            foreach (var soundKitID in voSoundKitView.Skip(start).Take(length))
            {
                var bcTexts = new List<BCText>();
                if (soundKitToBroadcastTextID.TryGetValue(soundKitID, out var bcTextIDs))
                {
                    foreach (var id in bcTextIDs)
                    {
                        bcTexts.Add(new BCText
                        {
                            id = bcTextIDs[0],
                            text = SQLiteDB.GetBroadcastTextByID((int)bcTextIDs[0])
                        });
                    }
                }

                var voFiles = new List<VOFile>();
                foreach (var voFileDataID in soundKitIDToFDID[soundKitID])
                {
                    var isAvailable = true;

                    if (!CASC.AvailableFDIDs.Contains((int)voFileDataID) || CASC.EncryptionStatuses.TryGetValue((int)voFileDataID, out var status) && status == CASC.EncryptionStatus.EncryptedUnknownKey)
                        isAvailable = false;

                    voFiles.Add(new VOFile()
                    {
                        fileDataID = voFileDataID,
                        name = Listfile.NameMap.TryGetValue((int)voFileDataID, out var voFilename) ? voFilename : "",
                        available = isAvailable,
                        creatureName = SQLiteDB.getCreatureNameByFileDataID((int)voFileDataID),
                        addedIn = SQLiteDB.GetFirstVersionNumberByFileDataID((int)voFileDataID)
                    });
                }

                result.data.Add(
                   [
                       soundKitID.ToString(), // ID
                       JsonConvert.SerializeObject(voFiles), // Files
                       JsonConvert.SerializeObject(bcTexts), // BroadcastTexts
                   ]);
            }

            return result;
        }

        [Route("update")]
        [HttpGet]
        public bool updateCreatureName(uint fileDataID, string name)
        {
            return SQLiteDB.SetCreatureNameForFDID((int)fileDataID, name);
        }

        public struct DataTablesResult
        {
            public int draw { get; set; }
            public int recordsFiltered { get; set; }
            public int recordsTotal { get; set; }
            public List<List<string>> data { get; set; }
        }

        public struct BCText
        {
            public uint id { get; set; }
            public string text { get; set; }
        }

        public struct VOFile
        {
            public uint fileDataID { get; set; }
            public string name { get; set; }
            public string creatureName { get; set; }
            public bool available { get; set; }
            public uint addedIn { get; set; }
        }
    }
}
