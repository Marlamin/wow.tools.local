using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("size")]
    [ApiController]
    public class SizeController() : Controller
    {
        private static string CachedBuild = "";
        private static Dictionary<byte, TACTSharp.CASCIndexInstance> CASCIndexInstances = [];
        private static Dictionary<string, ulong> ESpecCache = [];
        private static Dictionary<int, string> AddedInCache = [];

        [Route("data")]
        [HttpGet]
        public IActionResult Data(string groupType = "filetype", bool uniqueOnly = false, bool localOnly = false, bool encodedSizes = false, string listfileSearch = "available")
        {
            if (CachedBuild != CASC.BuildName)
            {
                CASCIndexInstances.Clear();
                ESpecCache.Clear();
            }

            CachedBuild = CASC.BuildName;
            var results = new Dictionary<string, ulong>();
            var seenCHashes = new HashSet<string>();

            var resultLock = new Lock();
            var chashLock = new Lock();
            var ekeyLock = new Lock();
            var addedInLock = new Lock();

            var listfileResults = Listfile.DoSearch(Listfile.NameMap, listfileSearch);

            if (localOnly)
            {
                if (CASCIndexInstances.Count == 0)
                {
                    var dataDir = Path.Combine(SettingsManager.WoWFolder, "Data", "data");
                    if (Directory.Exists(dataDir))
                    {
                        var indexFiles = Directory.GetFiles(dataDir, "*.idx");
                        var highestIndexPerBucket = new Dictionary<byte, int>(16);

                        foreach (var indexFile in indexFiles)
                        {
                            if (indexFile.Contains("tempfile"))
                                continue;

                            var indexBucket = Convert.ToByte(Path.GetFileNameWithoutExtension(indexFile)[0..2], 16);
                            var indexVersion = Convert.ToInt32(Path.GetFileNameWithoutExtension(indexFile)[2..], 16);

                            if (highestIndexPerBucket.TryGetValue(indexBucket, out var highestIndex))
                            {
                                if (indexVersion > highestIndex)
                                    highestIndexPerBucket[indexBucket] = indexVersion;
                            }
                            else
                            {
                                highestIndexPerBucket.Add(indexBucket, indexVersion);
                            }
                        }

                        foreach (var index in highestIndexPerBucket)
                        {
                            var indexFile = Path.Combine(dataDir, index.Key.ToString("x2") + index.Value.ToString("x2").PadLeft(8, '0') + ".idx");
                            CASCIndexInstances.Add(index.Key, new TACTSharp.CASCIndexInstance(indexFile));
                        }
                    }
                }
            }

            // pre-warm cache with a query for all teh things
            if (groupType == "patch" || groupType == "expansion" || groupType == "majorpatch")
            {
                var history = SQLiteDB.RunQuery("SELECT filedataid, build FROM wow_rootfiles_chashes");
                foreach (var fileHistory in history)
                {
                    if (int.TryParse(fileHistory[0], out var fileDataID))
                    {
                        if (!AddedInCache.ContainsKey(fileDataID))
                        {
                            var splitPatch = fileHistory[1].Split('.');
                            var patchOnly = splitPatch[0] + "." + splitPatch[1] + "." + splitPatch[2];
                            AddedInCache[fileDataID] = patchOnly;
                        }
                    }
                }
            }

            Parallel.ForEach(listfileResults, (result) =>
            {
                var fdid = result.Key;
                var allCKeys = CASC.GetCKeysAndFlagsByFDID(result.Key);
                if(allCKeys.Count == 0)
                    return;

                var chash = CASC.GetPreferredCKey(allCKeys);

                var eKeys = CASC.GetEKeysByCKey(chash);
                var eKey = eKeys[0];

                if (localOnly)
                {
                    var i = eKey[0] ^ eKey[1] ^ eKey[2] ^ eKey[3] ^ eKey[4] ^ eKey[5] ^ eKey[6] ^ eKey[7] ^ eKey[8];
                    var indexBucket = (i & 0xf) ^ (i >> 4);
                    var targetIndex = CASCIndexInstances[(byte)indexBucket];

                    var (archiveOffset, archiveSize, archiveIndex) = targetIndex.GetIndexInfo(eKey.ToArray());

                    if (archiveIndex == -1)
                        return;
                }

                if (uniqueOnly)
                {
                    var chashString = Convert.ToHexStringLower(chash);

                    lock (chashLock)
                    {
                        if (seenCHashes.Contains(chashString))
                            return;

                        seenCHashes.Add(chashString);
                    }
                }
                ulong fileSize = 0;

                if (encodedSizes)
                {
                    lock (ekeyLock)
                    {
                        if (!ESpecCache.TryGetValue(Convert.ToHexStringLower(eKey), out var eSpecSize))
                        {
                            var eSpec = CASC.GetESpecByEKey(eKey);
                            ESpecCache[Convert.ToHexStringLower(eKey)] = eSpec.encodedFileSize;
                            eSpecSize = eSpec.encodedFileSize;
                        }

                        fileSize = eSpecSize;
                    }
                }
                else
                {
                    fileSize = eKeys.DecodedFileSize;
                }

                if (groupType == "filetype")
                {
                    var type = Services.Listfile.Types.TryGetValue(result.Key, out string? value) ? value : "unk";

                    lock (resultLock)
                    {
                        if (!results.ContainsKey(type))
                            results[type] = 0;

                        results[type] += fileSize;
                    }
                }
                else if (groupType == "folder")
                {
                    var path = "unnamed";
                    var filename = result.Value.ToLowerInvariant();
                    if (!string.IsNullOrEmpty(filename))
                    {
                        var splitPath = filename.Split('/');
                        if (splitPath.Length == 1)
                        {
                            path = "root";

                        }
                        else
                        {
                            path = splitPath[0];
                        }
                    }

                    lock (resultLock)
                    {
                        if (!results.ContainsKey(path))
                            results[path] = 0;

                        results[path] += fileSize;
                    }
                }
                else if (groupType == "expansion" || groupType == "majorpatch" || groupType == "patch")
                {
                    string addedIn = "unk";

                    lock (addedInLock)
                    {
                        if (!AddedInCache.TryGetValue(result.Key, out addedIn))
                        {
                            addedIn = SQLiteDB.GetFileVersions(result.Key)[0].buildName;
                            AddedInCache[result.Key] = addedIn;
                        }
                    }

                    lock (resultLock)
                    {
                        if (groupType == "expansion")
                        {
                            var splitAddedIn = addedIn.Split('.');
                            addedIn = splitAddedIn[0];
                        }
                        else if (groupType == "majorpatch")
                        {
                            var splitAddedIn = addedIn.Split('.');
                            addedIn = splitAddedIn[0] + "." + splitAddedIn[1];
                        }

                        if (!results.ContainsKey(addedIn))
                            results[addedIn] = 0;

                        results[addedIn] += fileSize;
                    }
                }

            });

            return Json(results);
        }
    }
}
