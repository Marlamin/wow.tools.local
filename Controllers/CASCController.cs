using DBCD;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Text;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local.Controllers
{
    [Route("casc/")]
    [ApiController]
    public class CASCController : Controller
    {
        [Route("fdid")]
        [HttpGet]
        public ActionResult File(uint fileDataID, string filename = "", string build = "")
        {
            if (build == "")
                build = CASC.BuildName;
            
            if (!CASC.FileExists(fileDataID))
                return NotFound();

            if (string.IsNullOrEmpty(filename))
            {
                filename = fileDataID.ToString();
                if (CASC.Types.ContainsKey((int)fileDataID))
                    filename += "." + CASC.Types[(int)fileDataID];
            }

            var file = CASC.GetFileByID(fileDataID);
            if (file == null)
                return NotFound();

            return new FileStreamResult(file, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        [Route("buildname")]
        [HttpGet]
        public string BuildName()
        {
            return CASC.BuildName;
        }

        [Route("builds")]
        [HttpPost]
        public DataTablesResult Builds()
        {
            var result = new DataTablesResult();

            if (Request.Method == "POST" && Request.Form.TryGetValue("draw", out var draw))
            {
                result.draw = int.Parse(draw);
                result.data = new List<List<string>>();
            }

            if (SettingsManager.wowFolder != null && System.IO.File.Exists(Path.Combine(SettingsManager.wowFolder, ".build.info")))
            {
                foreach (var line in System.IO.File.ReadAllLines(Path.Combine(SettingsManager.wowFolder, ".build.info")))
                {
                    var splitLine = line.Split("|");
                    if (splitLine[0] == "Branch!STRING:0")
                        continue;

                    var buildConfig = splitLine[2];
                    var cdnConfig = splitLine[3];
                    var version = splitLine[12];
                    var product = splitLine[13];

                    var splitVersion = version.Split(".");
                    var patch = splitVersion[0] + "." + splitVersion[1] + "." + splitVersion[2];
                    var build = splitVersion[3];

                    var isActive = CASC.BuildName == version;
                    var hasManifest = System.IO.File.Exists("manifests/" + patch + "." + build + ".txt");
                    var hasDBCs = Directory.Exists(Path.Combine(SettingsManager.dbcFolder, patch + "." + build, "dbfilesclient"));
                    result.data.Add(new List<string>() { patch, build, product, buildConfig, cdnConfig, isActive.ToString(), hasManifest.ToString(), hasDBCs.ToString() });
                }

                result.data.OrderBy(x => x[0]).ToList();
                result.recordsTotal = result.data.Count;
                result.recordsFiltered = result.data.Count;
            }

            return result;
        }

        [Route("switchProduct")]
        [HttpGet]
        public bool SwitchProduct(string product)
        {
            CASC.InitCasc(SettingsManager.wowFolder, product);
            return true;
        }

        [Route("updateListfile")]
        [HttpGet]
        public bool UpdateListfile()
        {
            CASC.LoadListfile(true);
            return true;
        }
        
        [Route("exportListfile")]
        [HttpGet]
        public bool ExportListfile()
        {
            CASC.ExportListfile();
            return true;
        }

        [Route("updateTACTKeys")]
        [HttpGet]
        public bool UpdateTACTKeys()
        {
            CASC.LoadKeys(true);
            return true;
        }
        
        [Route("exportTACTKeys")]
        [HttpGet]
        public bool ExportTACTKeys()
        {
            CASC.ExportTACTKeys();
            return true;
        }


        [Route("listManifests")]
        [HttpGet]
        public List<string> ListManifests()
        {
            var cachedManifests = new List<string>();
            if (Directory.Exists("manifests"))
            {
                foreach (var file in Directory.GetFiles("manifests", "*.txt"))
                    cachedManifests.Add(Path.GetFileNameWithoutExtension(file));
            }
            return cachedManifests.OrderByDescending(x => int.Parse(x.Split(".")[3])).ToList();
        }

        [Route("analyzeUnknown")]
        [HttpGet]
        public bool AnalyzeUnknown()
        {
            var knownUnknowns = new Dictionary<int, string>();

            if (System.IO.File.Exists("cachedUnknowns.txt"))
            {
                knownUnknowns = System.IO.File.ReadAllLines("cachedUnknowns.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);
            }

            List<int> unknownFiles = CASC.AvailableFDIDs.Except(CASC.Types.Where(x => x.Value != "unk").Select(x => x.Key)).ToList();

            if (knownUnknowns.Count > 0)
            {
                foreach (var knownUnknown in knownUnknowns)
                {
                    CASC.SetFileType(knownUnknown.Key, knownUnknown.Value);
                }

                unknownFiles = CASC.AvailableFDIDs.Except(CASC.Types.Where(x => x.Value != "unk").Select(x => x.Key)).ToList();
            }
            var dbcd = new DBCD.DBCD(new DBCProvider(), new DBDProvider());

            try
            {
                var mfdStorage = dbcd.Load("ModelFileData", CASC.BuildName);
                foreach (dynamic mfdEntry in mfdStorage.Values)
                {
                    var fdid = (int)mfdEntry.FileDataID;
                    if (!CASC.Types.ContainsKey(fdid) || CASC.Types[fdid] == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "m2");
                        unknownFiles.Remove(fdid);
                        CASC.SetFileType(fdid, "m2");
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception during type guessing with ModelFileData:" + e.Message);
            }

            try
            {
                var tfdStorage = dbcd.Load("TextureFileData", CASC.BuildName);
                foreach (dynamic tfdEntry in tfdStorage.Values)
                {
                    var fdid = (int)tfdEntry.FileDataID;
                    if (!CASC.Types.ContainsKey(fdid) || CASC.Types[fdid] == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "blp");
                        unknownFiles.Remove(fdid);
                        CASC.SetFileType(fdid, "blp");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with TextureFileData:" + e.Message);
            }

            try
            {
                var mfdStorage = dbcd.Load("MovieFileData", CASC.BuildName);
                foreach (dynamic mfdEntry in mfdStorage.Values)
                {
                    var fdid = (int)mfdEntry.ID;
                    if (!CASC.Types.ContainsKey(fdid) || CASC.Types[fdid] == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "avi");
                        unknownFiles.Remove(fdid);
                        CASC.SetFileType(fdid, "avi");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with MovieFileData:" + e.Message);
            }

            try
            {
                var mp3Storage = dbcd.Load("ManifestMP3", CASC.BuildName);
                foreach (dynamic mp3Entry in mp3Storage.Values)
                {
                    var fdid = (int)mp3Entry.ID;
                    if (!CASC.Types.ContainsKey(fdid) || CASC.Types[fdid] == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "mp3");
                        unknownFiles.Remove(fdid);
                        CASC.SetFileType(fdid, "mp3");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with ManifestMP3:" + e.Message);
            }

            try
            {
                var skStorage = dbcd.Load("SoundKitEntry", CASC.BuildName);
                foreach (dynamic skEntry in skStorage.Values)
                {
                    var fdid = (int)skEntry.FileDataID;
                    if (!CASC.Types.ContainsKey(fdid) || CASC.Types[fdid] == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "ogg");
                        unknownFiles.Remove(fdid);
                        CASC.SetFileType(fdid, "ogg");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with SoundKitEntry:" + e.Message);
            }

            Console.WriteLine("Analyzing " + unknownFiles.Count() + " unknown files");
            var numFilesTotal = unknownFiles.Count();
            var numFilesDone = 0;
            var numFilesSkipped = 0;
            Parallel.ForEach(unknownFiles, unknownFile =>
            {
                try
                {
                    if (CASC.EncryptionStatuses.ContainsKey(unknownFile) && CASC.EncryptionStatuses[unknownFile] == CASC.EncryptionStatus.EncryptedUnknownKey)
                    {
                        numFilesSkipped++;
                        numFilesDone++;
                        return;
                    }

                    var file = CASC.GetFileByID((uint)unknownFile);
                    if (file == null)
                    {
                        numFilesSkipped++;
                        numFilesDone++;
                        return;
                    }

                    using (var bin = new BinaryReader(file))
                    {
                        if (bin.BaseStream.Length < 4)
                            return;

                        var magic = bin.ReadBytes(4);
                        var type = "unk";
                        var magicString = Encoding.ASCII.GetString(magic);
                        switch (magicString)
                        {
                            case "MD21":
                            case "MD20":
                                type = "m2";
                                break;
                            case "SKIN":
                                type = "skin";
                                break;
                            case "OggS":
                                type = "ogg";
                                break;
                            case "BLP2":
                                type = "blp";
                                break;
                            case "REVM":
                                var length = bin.ReadInt32();
                                bin.ReadBytes(length);

                                var secondChunk = bin.ReadBytes(4);
                                var subChunk = Encoding.ASCII.GetString(secondChunk);
                                switch (subChunk)
                                {
                                    case "RDHM": // ADT root
                                    case "FDDM": // ADT OBJ
                                    case "DDLM": // ADT OBJ
                                    case "DFLM": // ADT OBJ
                                    case "DHLM": // ADT LOD
                                    case "PMAM": // ADT TEX
                                        type = "adt";
                                        break;
                                    case "DHOM": // WMO root
                                        type = "wmo";
                                        break;
                                    case "PGOM": // WMO GROUP
                                        type = "gwmo";
                                        break;
                                    case "DHPM": // WDT root
                                    case "IOAM": // WDT OCC/LGT
                                        type = "wdt";
                                        break;
                                    default:
                                        Console.WriteLine("Unknown sub chunk " + subChunk + " for file " + (uint)unknownFile);
                                        type = "chUNK";
                                        break;
                                }
                                break;
                            case "RVXT":
                                type = "tex";
                                break;
                            case "AFM2":
                            case "AFSA":
                            case "AFSB":
                                type = "anim";
                                break;
                            case "WDC4":
                            case "WDC3":
                                type = "db2";
                                break;
                            case "RIFF":
                                type = "avi";
                                break;
                            case "HSXG":
                                type = "bls";
                                break;
                            default:
                                Console.WriteLine((uint)unknownFile + " - Unknown magic " + magicString + " (" + Convert.ToHexString(magic) + ")");
                                break;
                        }

                        CASC.SetFileType(unknownFile, type);
                        knownUnknowns.TryAdd(unknownFile, type);
                    }
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("nknown keyname"))
                    {
                        Console.WriteLine("Failed to guess type for file " + unknownFile + ": " + e.Message + "\n" + e.StackTrace);
                    }
                    numFilesSkipped++;
                    numFilesDone++;
                }

                if (numFilesDone % 1000 == 0)
                    Console.WriteLine("Analyzed " + numFilesDone + "/" + numFilesTotal + " files (skipped " + numFilesSkipped + " unreadable files)");

                numFilesDone++;
            });

            System.IO.File.WriteAllLines("cachedUnknowns.txt", knownUnknowns.Where(x => x.Value != "unk").Select(x => x.Key + ";" + x.Value));
            Console.WriteLine("Finished unknown file analysis");
            return true;
        }

        [Route("diff")]
        [HttpGet]
        public ActionResult DiffManifests(string from, string to)
        {
            if (BuildDiffCache.Get(from, to, out ApiDiff diff))
            {
                return Json(new
                {
                    added = diff.added.Count(),
                    modified = diff.modified.Count(),
                    removed = diff.removed.Count(),
                    data = diff.all.ToArray()
                });
            }

            Func<KeyValuePair<int, string>, DiffEntry> toDiffEntry(string action)
            {
                return delegate (KeyValuePair<int, string> entry)
                {
                    var file = CASC.Listfile.TryGetValue(entry.Key, out var filename) ? filename : "unknown/" + entry.Key + ".unk";

                    return new DiffEntry
                    {
                        action = action,
                        filename = file,
                        id = entry.Key.ToString(),
                        md5 = entry.Value.ToLower(),
                        type = CASC.Types.ContainsKey(entry.Key) ? CASC.Types[entry.Key] : "unk",
                        encryptedStatus = CASC.EncryptionStatuses.ContainsKey(entry.Key) ? CASC.EncryptionStatuses[entry.Key].ToString() : ""
                    };
                };
            }

            var rootFromEntries = System.IO.File.ReadAllLines(Path.Combine("manifests", from + ".txt")).Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);
            var rootToEntries = System.IO.File.ReadAllLines(Path.Combine("manifests", to + ".txt")).Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);

            var fromEntries = rootFromEntries.Keys.ToHashSet();
            var toEntries = rootToEntries.Keys.ToHashSet();

            var commonEntries = fromEntries.Intersect(toEntries);
            var removedEntries = fromEntries.Except(commonEntries);
            var addedEntries = toEntries.Except(commonEntries);

            var addedFiles = addedEntries.Select(entry => new KeyValuePair<int, string>(entry, rootToEntries[entry]));
            var removedFiles = removedEntries.Select(entry => new KeyValuePair<int, string>(entry, rootFromEntries[entry]));
            var modifiedFiles = new List<KeyValuePair<int, string>>();

            foreach (var entry in commonEntries)
            {
                var originalFile = rootFromEntries[entry];
                var patchedFile = rootToEntries[entry];

                if (originalFile != patchedFile)
                    modifiedFiles.Add(new KeyValuePair<int, string>(entry, patchedFile));
            }

            var toAddedDiffEntryDelegate = toDiffEntry("added");
            var toRemovedDiffEntryDelegate = toDiffEntry("removed");
            var toModifiedDiffEntryDelegate = toDiffEntry("modified");

            diff = new ApiDiff
            {
                added = addedFiles.Select(toAddedDiffEntryDelegate),
                removed = removedFiles.Select(toRemovedDiffEntryDelegate),
                modified = modifiedFiles.Select(toModifiedDiffEntryDelegate)
            };

            Console.WriteLine($"Added: {diff.added.Count()}, removed: {diff.removed.Count()}, modified: {diff.modified.Count()}, common: {commonEntries.Count()}");

            BuildDiffCache.Add(from, to, diff);

            return Json(new
            {
                added = diff.added.Count(),
                modified = diff.modified.Count(),
                removed = diff.removed.Count(),
                data = diff.all.ToArray()
            });
        }
    }
}
