using CASCLib;
using Microsoft.AspNetCore.Mvc;
using SereniaBLPLib;
using SixLabors.ImageSharp;
using System.Diagnostics;
using System.Text;
using System.Web;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local.Controllers
{
    [Route("casc/")]
    [ApiController]
    public class CASCController : Controller
    {
        private readonly DBCManager dbcManager;

        public CASCController(IDBCManager dbcManager)
        {
            this.dbcManager = dbcManager as DBCManager;
        }

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

            var file = CASC.GetFileByID(fileDataID, build);
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
                var headerMap = new Dictionary<string, byte>();
                foreach (var line in System.IO.File.ReadAllLines(Path.Combine(SettingsManager.wowFolder, ".build.info")))
                {
                    var splitLine = line.Split("|");
                    if (splitLine[0] == "Branch!STRING:0")
                    {
                        foreach (var header in splitLine)
                            headerMap.Add(header.Split("!")[0], (byte)Array.IndexOf(splitLine, header));

                        continue;
                    }

                    var buildConfig = splitLine[headerMap["Build Key"]];
                    var cdnConfig = splitLine[headerMap["CDN Key"]];
                    var version = splitLine[headerMap["Version"]];
                    var product = splitLine[headerMap["Product"]];

                    var splitVersion = version.Split(".");
                    var patch = splitVersion[0] + "." + splitVersion[1] + "." + splitVersion[2];
                    var build = splitVersion[3];

                    var isActive = CASC.CurrentProduct == product;
                    var hasManifest = System.IO.File.Exists(Path.Combine(SettingsManager.manifestFolder, patch + "." + build + ".txt"));
                    var hasDBCs = Directory.Exists(Path.Combine(SettingsManager.dbcFolder, patch + "." + build, "dbfilesclient"));
                    result.data.Add(new List<string>() { patch, build, product, buildConfig, cdnConfig, isActive.ToString(), hasManifest.ToString(), hasDBCs.ToString() });
                }

                result.data = result.data.OrderBy(x => x[0]).ToList();
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
            BuildDiffCache.Invalidate();
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
            if (Directory.Exists(SettingsManager.manifestFolder))
            {
                foreach (var file in Directory.GetFiles(SettingsManager.manifestFolder, "*.txt"))
                    cachedManifests.Add(Path.GetFileNameWithoutExtension(file));
            }
            return cachedManifests.OrderByDescending(x => int.Parse(x.Split(".")[3])).ToList();
        }

        [Route("analyzeUnknown")]
        [HttpGet]
        public bool AnalyzeUnknown()
        {
            Console.WriteLine("Analyzing unknown files");
            var knownUnknowns = new Dictionary<int, string>();

            if (System.IO.File.Exists("cachedUnknowns.txt"))
            {
                knownUnknowns = System.IO.File.ReadAllLines("cachedUnknowns.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);
            }

            List<int> unknownFiles = CASC.AvailableFDIDs.Except(CASC.Types.Where(x => x.Value != "unk").Select(x => x.Key)).ToList();

            if (knownUnknowns.Count > 0)
            {
                foreach (var knownUnknown in knownUnknowns)
                    CASC.SetFileType(knownUnknown.Key, knownUnknown.Value);

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
            catch (Exception e)
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

            var typeLock = new object();

            Console.WriteLine("Analyzing " + unknownFiles.Count + " unknown files");
            var numFilesTotal = unknownFiles.Count;
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
                        if (magic[0] == 0 || magic[0] == 4)
                        {
                            if (bin.BaseStream.Length >= 8)
                            {
                                var wwfMagic = bin.ReadUInt32();
                                switch (wwfMagic)
                                {
                                    case 0x932C64B4: // WWFParticulateGroup
                                        type = "wwf";
                                        break;
                                }
                            }

                            bin.BaseStream.Position = 4;
                        }

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
                                    case "XDMM": // ADT OBJ (old)
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
                            case "WDC5":
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
                            case "SKL1":
                                type = "skel";
                                break;
                            case "SYHP":
                                type = "phys";
                                break;
                            case "TAFG":
                                type = "gfat";
                                break;
                            default:
                                break;
                        }

                        if (magicString.StartsWith("ID3"))
                            type = "mp3";

                        if (type == "unk")
                            Console.WriteLine((uint)unknownFile + " - Unknown magic " + magicString + " (" + Convert.ToHexString(magic) + ")");

                        lock (typeLock)
                        {
                            CASC.SetFileType(unknownFile, type);
                            knownUnknowns.TryAdd(unknownFile, type);
                        }
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

            var allListfileNames = CASC.GetAllListfileNames();

            Func<KeyValuePair<int, string>, DiffEntry> toDiffEntry(string action)
            {
                return delegate (KeyValuePair<int, string> entry)
                {
                    var file = allListfileNames.TryGetValue(entry.Key, out var filename) ? filename : "";

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

            var rootFromEntries = System.IO.File.ReadAllLines(Path.Combine(SettingsManager.manifestFolder, from + ".txt")).Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);
            var rootToEntries = System.IO.File.ReadAllLines(Path.Combine(SettingsManager.manifestFolder, to + ".txt")).Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);

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

        [Route("samehashes")]
        [HttpGet]
        public string SameHashes(string chash)
        {
            CASC.EnsureCHashesLoaded();

            var html = "The table below lists files that are identical in content to the requested file.<br><table class='table table-striped'><thead><tr><th>ID</th><th>Name (if available)</th></tr></thead>";

            var filedataids = CASC.GetSameFiles(chash);
            foreach (var filedataid in filedataids)
            {
                html += "<tr><td>" + filedataid + "</td><td>" + (CASC.Listfile.TryGetValue(filedataid, out string filename) ? filename : "N/A") + "</td></tr>";
            }
            html += "</table>";

            return html;
        }

        [Route("moreinfo")]
        [HttpGet]
        public async Task<string> MoreInfo(int filedataid)
        {
            CASC.EnsureCHashesLoaded();

            // Yes, generating HTML here is ugly but that's how the old system worked and I can't be arsed to redo it.
            var html = "<div style='float: right'><a class='btn btn-sm btn-primary' id='fileRelinkButton' onClick='relinkFile(" + filedataid + ")'>Recrawl file links</a></div><table style='clear: both' class='table table-striped'><thead><tr><th style='width:400px'></th><th></th></tr></thead>";
            html += "<tr><td>FileDataID</td><td>" + filedataid + "</td></tr>";
            html += "<tr><td>Filename</td><td>" + (CASC.Listfile.TryGetValue(filedataid, out var filename) ? filename : "unknown/" + filedataid + ".unk") + "</td></tr>";
            html += "<tr><td>Lookup</td>";

            if (CASC.LookupMap.TryGetValue(filedataid, out var lookup))
            {
                var hasher = new Jenkins96();
                html += "<td>" + lookup.ToString("X16");

                if (CASC.Listfile.TryGetValue(filedataid, out var lookupFilename) && lookupFilename != "")
                {
                    if (hasher.ComputeHash(lookupFilename) == lookup)
                    {
                        html += " <span class='text-success'>(Filename matches)</span>";
                    }
                    else
                    {
                        html += " <span class='text-danger'>(Filename doesn't match)</span>";
                    }
                }
                else
                {
                    html += " <span class='text-warning'>(Can't check filename, none set)</span>";
                }

                html += "</td></tr>";
            }
            else
            {
                html += "<td>N/A</td></tr>";
            }

            html += "<tr><td>Type</td><td>" + (CASC.Types.ContainsKey(filedataid) ? CASC.Types[filedataid] : "unk") + "</td></tr>";

            if (CASC.FDIDToCHash.TryGetValue(filedataid, out var cKey))
            {
                html += "<tr><td>Content hash (MD5)</td><td style='font-family: monospace;'><a href='#' data-toggle='modal' data-target='#chashModal' onClick='fillChashModal(\"" + cKey.ToLower() + "\")'>" + cKey.ToLower() + "</a></td></tr>";
                html += "<tr><td>Size</td><td>" + (CASC.CHashToSize.ContainsKey(cKey) ? CASC.CHashToSize[cKey] + " bytes" : "N/A") + "</td></tr>";
            }

            if (CASC.EncryptionStatuses.TryGetValue(filedataid, out var encryptionStatus))
            {
                var prettyEncryptionStatus = "";
                switch (encryptionStatus)
                {
                    case CASC.EncryptionStatus.EncryptedKnownKey:
                        prettyEncryptionStatus = "Encrypted (key is known)";
                        break;
                    case CASC.EncryptionStatus.EncryptedUnknownKey:
                        prettyEncryptionStatus = "Encrypted (key is unknown)";
                        break;
                    case CASC.EncryptionStatus.EncryptedMixed:
                        prettyEncryptionStatus = "Partially encrypted (some known keys, some unknown)";
                        break;
                    case CASC.EncryptionStatus.EncryptedButNot:
                        prettyEncryptionStatus = "Supposed to be encrypted, but isn't (duplicate with unencrypted file)";
                        break;
                }

                var usedKeys = CASC.EncryptedFDIDs[filedataid];
                html += "<tr><td>Encryption status</td><td>" + prettyEncryptionStatus + "<br>";
                html += "<table class='table table-sm table-inverse'>";
                html += "<thead><tr><th></th><th style='width: 10%'>Key</th><th style='width: 4%'>ID</th><th style='width: 22%'>First seen</th><th>Description</th></tr></thead>";

                var usedKeyInfo = new List<(ulong lookup, int ID, string FirstSeen, string Description)>();
                foreach (var key in usedKeys)
                {
                    if (KeyMetadata.KeyInfo.TryGetValue(key, out var keyInfo))
                    {
                        usedKeyInfo.Add((key, keyInfo.ID, keyInfo.FirstSeen, keyInfo.Description));
                    }
                    else
                    {
                        usedKeyInfo.Add((key, 0, "N/A", "No metadata known for this key, this is fine if this is a voice over file or a recently added key. Check back in a future version."));
                    }
                }

                var db2EncryptionMetaData = new Dictionary<ulong, int[]>();

                try
                {
                    if (
                        CASC.Types.ContainsKey(filedataid) && CASC.Types[filedataid].ToLower() == "db2" &&
                        CASC.Listfile.TryGetValue(filedataid, out var db2filename) && db2filename != ""
                        )
                    {
                        var storage = await dbcManager.GetOrLoad(Path.GetFileNameWithoutExtension(db2filename), CASC.BuildName);
                        db2EncryptionMetaData = storage.GetEncryptedIDs();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to get encrypted DB2 info for DB2 " + filedataid + ": " + e.Message);
                }

                foreach (var key in usedKeyInfo.OrderBy(x => x.ID))
                {
                    html += "<tr><td>" + (KeyService.HasKey(key.lookup) ? "<i style='color: green' class='fa fa-unlock'></i>" : "<i style='color: red' class='fa fa-lock'></i>") + "</td><td><a style='font-family: monospace;' target='_BLANK' href='/files/#search=encrypted%3A" + key.lookup.ToString("X16").PadLeft(16, '0') + "'>" + key.lookup.ToString("X16").PadLeft(16, '0') + "</a></td><td>" + key.ID + "</td><td>" + key.FirstSeen + "</td><td>" + key.Description + "</td></tr>";

                    if (db2EncryptionMetaData.TryGetValue(key.lookup, out var encryptedIDs))
                    {
                        html += "<tr><td colspan='4'>&nbsp;</td><td style='padding-left: 20px;'><b>" + encryptedIDs.Length;
                        if (KeyService.HasKey(key.lookup))
                        {
                            html += " <a href='/dbc/?dbc=" + Path.GetFileNameWithoutExtension(CASC.Listfile[filedataid]).ToLower() + "&build=" + CASC.BuildName + "#page=1&search=encrypted%3A" + key.lookup.ToString("X16").PadLeft(16, '0') + "' target='_BLANK' class='text-success'>available</a>";
                        }
                        else
                        {
                            html += " <span class='text-danger'>unavailable</span>";
                        }
                        html += " record(s). IDs:</b> ";
                        html += string.Join(", ", encryptedIDs);
                        html += "</td></tr>";
                    }
                }

                html += "</table></td></tr>";
            }
            else
            {
                html += "<tr><td>Encryption status</td><td>Not encrypted</td></tr>";
            }

            // File version
            var fileVersions = SQLiteDB.GetFileVersions(filedataid);
            if (fileVersions.Count > 0)
            {
                html += "<tr><td colspan='2'><b>Known versions</b></td></tr>";
                html += "<tr><td colspan='2'><table class='table table-sm table-striped'>";
                html += "<tr><th>Build</th><th>Contenthash</th><th><small><i>Locally unavailable downloads powered by <a href='https://wago.tools' target='_BLANK'>wago.tools</a></i></small></th></tr>";
                foreach (var fileVersion in fileVersions)
                {
                    if (CASC.CHashToFDID.TryGetValue(fileVersion.contentHash, out var fdidsWithChash) && fdidsWithChash.Contains(filedataid))
                    {
                        var dlFilename = CASC.Listfile.TryGetValue(filedataid, out var listfileName) ? Path.GetFileName(listfileName) : filedataid + ".unk";
                        html += "<tr><td>" + fileVersion.buildName + "</td><td style='font-family: monospace;'>" + fileVersion.contentHash.ToLower() + "</td><td><a href='/casc/fdid?fileDataID=" + filedataid + "&filename=" + HttpUtility.UrlEncode(dlFilename) + "' target='_BLANK' download>Download</a></td></tr>";
                    }
                    else
                    {
                        // Temp build filter, wago.tools does not have builds below 18379
                        var build = int.Parse(fileVersion.buildName.Split(".")[3]);

                        if (build > 18378)
                        {
                            html += "<tr><td>" + fileVersion.buildName + "</td><td style='font-family: monospace;'>" + fileVersion.contentHash.ToLower() + "</td><td><a href='https://wago.tools/api/casc/" + filedataid + "/?version=" + fileVersion.buildName + "&download' target='_BLANK' download>Download</a></td></tr>";
                        }
                        else
                        {
                            html += "<tr><td>" + fileVersion.buildName + "</td><td style='font-family: monospace;'>" + fileVersion.contentHash.ToLower() + "</td><td><i>Download not available</i></td></tr>";
                        }
                    }

                }
                html += "</table></td></tr>";
            }

            // Linked files
            var linkedParentFiles = SQLiteDB.GetParentFiles(filedataid);

            if (linkedParentFiles.Count > 0)
            {
                html += "<tr><td colspan='2'><b>Files linking to this file</b> (<i>Note, this can not scan on request, scan other files for them to show up here.)</i></td></tr>";
                html += "<tr><td colspan='2'><table class='table table-sm table-striped'>";
                html += "<tr><th>Link type</th><th>ID</th><th>Filename</th><th>Type</th></tr>";
                foreach (var linkedFile in linkedParentFiles)
                {
                    html += "<tr><td>" + linkedFile.linkType + "</td><td>" + linkedFile.fileDataID + " <a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer' onclick='fillModal(" + linkedFile.fileDataID + ")'><i class='fa fa-info-circle'></i></a></td><td>" + (CASC.Listfile.TryGetValue((int)linkedFile.fileDataID, out var linkedFilename) ? linkedFilename : "unknown/" + linkedFile.fileDataID + ".unk") + "</td><td>" + (CASC.Types.ContainsKey((int)linkedFile.fileDataID) ? CASC.Types[(int)linkedFile.fileDataID] : "unk") + "</td></tr>";
                }
                html += "</table></td></tr></table>";
            }

            if (CASC.Types.ContainsKey(filedataid) &&
    (CASC.Types[filedataid] == "m2" || CASC.Types[filedataid] == "wmo") && !Linker.existingParents.Contains(filedataid))
            {
                try
                {
                    switch (CASC.Types[filedataid])
                    {
                        case "m2":
                            Linker.LinkM2((uint)filedataid);
                            break;
                        case "wmo":
                            Linker.LinkWMO((uint)filedataid);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error generating links for file " + filedataid + ": " + e.Message + "\n" + e.StackTrace);
                }

                var linkedChildFiles = SQLiteDB.GetFilesByParent(filedataid);

                if (linkedChildFiles.Count > 0)
                {
                    html += "<tr><td colspan='2'><b>Files this file links to</b></td></tr>";
                    html += "<tr><td colspan='2'><table class='table table-sm table-striped'>";
                    html += "<tr><th>Link type</th><th>ID</th><th>Filename</th><th>Type</th></tr>";
                    foreach (var linkedFile in linkedChildFiles)
                    {
                        html += "<tr><td>" + linkedFile.linkType + "</td><td>" + linkedFile.fileDataID + " <a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer' onclick='fillModal(" + linkedFile.fileDataID + ")'><i class='fa fa-info-circle'></i></a></td><td>" + (CASC.Listfile.TryGetValue((int)linkedFile.fileDataID, out var linkedFilename) ? linkedFilename : "unknown/" + linkedFile.fileDataID + ".unk") + "</td><td>" + (CASC.Types.ContainsKey((int)linkedFile.fileDataID) ? CASC.Types[(int)linkedFile.fileDataID] : "unk") + "</td></tr>";
                    }
                    html += "</table></td></tr></table>";
                }
            }


            // TODO: Soundkits?
            // TODO: With SQLite, import manifests one time to DB and then implement version history again
            // Disable for now, slow to load manifests :<
            /*
            CASC.EnsureVersionHistoryLoaded();

            html += "<tr><td colspan='2'><b>Known versions</b></td></tr>";
            html += "<tr><td colspan='2'><table class='table table-sm'>";
            html += "<tr><th>Description</th><th>Build</th><th>Contenthash</th></tr>";
            if (CASC.VersionHistory.TryGetValue(filedataid, out var versions))
            {
                html += "<table class='table table-striped'><thead><tr><th>Build</th><th>Content hash</th></tr></thead>";
                foreach (var version in versions)
                {
                    html += "<tr><td>" + version.buildName + "</td><td><a href='#' data-toggle='modal' data-target='#chashModal' onClick='fillChashModal(\"" + version.contentHash.ToLower() + "\")'>" + version.contentHash.ToLower() + "</a></td></tr>";
                }
                html += "</table>";
            }
            else
            {
                html += "<td colspan='2'>No version history found for this file.</td>";
            }
            html += "</table></td></tr>";
            */

            // IDK how to do this in C# fast
            /*
            html += "<tr><td colspan='2'><b>Neighbouring files</b></td></tr>";
            html += "<tr><td colspan='2'><table class='table table-sm'>";
            html += "<tr><th>ID</th><th>Filename</th></tr>";
            
            html += "</table></td></tr></table>";
            */
            return html;
        }

        [Route("getVersion")]
        [HttpGet]
        public string GetVersion()
        {
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        [Route("diffFile")]
        [HttpGet]
        public string DiffFile(int fileDataID, string from, string to)
        {

            var html = "";
            if (CASC.Types.TryGetValue(fileDataID, out string fileType))
            {
                var textTypes = new List<string>() { "html", "htm", "lua", "json", "txt", "wtf", "toc", "xml", "xsd", "sbt" };

                if (fileType == "blp")
                {
                    html = "<ul class='nav nav-tabs' id='diffTabs' role='tablist'>";
                    html += "<li class='nav-item'>";
                    html += "<a class='nav-link active' id='sbs-tab' data-toggle='tab' href='#sbs' role='tab' aria-controls='sbs' aria-selected='true'>Side-by-Side</a>";
                    html += "</li>";
                    html += "<li class='nav-item'>";
                    html += "<a class='nav-link' id='toggle-tab' data-toggle='tab' href='#toggle' role='tab' aria-controls='toggle' aria-selected='false'>Switcher</a>";
                    html += "</li>";
                    //html += "<li class='nav-item'><a class='nav-link' id='imagediff-tab' data-toggle='tab' href='#imagediff' role='tab' aria-controls='imagediff' aria-selected='false'>Diff</a>";
                    //html += "</li>";
                    html += "</ul>";
                    html += "<div class='tab-content'>";
                    html += "<div class='tab-pane show active' id='sbs' role='tabpanel' aria-labelledby='sbs-tab'>";
                    html += "<div class='row'>";
                    html += "<div class='col-md-6' id='from-diff'>";
                    html += "<h3>Build " + from + " (Before)</h3>";
                    html += "<img id='fromImage' style='max-width: 100%;' src='/casc/blp2png?fileDataID=" + fileDataID + "&build=" + from + "'>";
                    html += "</div>";
                    html += "<div class='col-md-6' id='to-diff'>";
                    html += "<h3>Build " + to + " (After)</h3>";
                    html += "<img id='toImage' style='max-width: 100%;' src='/casc/blp2png?fileDataID=" + fileDataID + "&build=" + to + "'>";
                    html += "</div>";
                    html += "</div>";
                    html += "</div>";
                    html += "<div class='tab-pane' id='toggle' role='tabpanel' aria-labelledby='toggle-tab'>";
                    html += "<div id='toggle-content' data-current='from'><div class='col-md-6' id='from-diff'><h3>Build </h3><img style='max-width: 100%;' src=''></div></div><button class='btn btn-primary' id='toggle-button'>Switch</button>";
                    html += "</div>";
                    html += "</div>";
                    html += "<script type='text/javascript'>";
                    html += "$(document).ready(function() { $('#toggle-content').html($('#from-diff').html()); $('#toggle-button').click(function() { if(document.getElementById('toggle-content').dataset.current == 'from'){ $('#toggle-content').html($('#to-diff').html()); document.getElementById('toggle-content').dataset.current = 'to'; }else{ $('#toggle-content').html($('#from-diff').html()); document.getElementById('toggle-content').dataset.current = 'from'; }});});";
                    html += "</script>";
                }
                else if (textTypes.Contains(fileType))
                {


                    html = @"Note: Git is required to be installed on the system to generate text diffs<br>
    <link rel='stylesheet' type='text/css' href='/css/diff2html.min.css' />
    <script src='https://cdn.jsdelivr.net/npm/diff2html/bundles/js/diff2html.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/diff2html/bundles/js/diff2html-ui.min.js'></script>
    <script type='text/javascript' charset='utf-8'>
        $(document).ready(function() {
            $.get('/casc/diffText?fileDataID=";
                    html += fileDataID + "&from=" + from + "&to=" + to;

                    html += @"', function(data) {
                var diffHtml = Diff2Html.html(
                    data, {
                        inputFormat: 'diff',
                        drawFileList: false,
                        matching: 'lines',
                        outputFormat: 'side-by-side'
                    }
                    );
                document.getElementById('rawdiff').innerHTML = diffHtml;
            });
        });
    </script><div id='rawdiff'></div>";
                }
            }
            return html;
        }

        [Route("blp2png")]
        [HttpGet]
        public ActionResult Blp2Png(int fileDataID, string build)
        {
            var file = CASC.GetFileByID((uint)fileDataID, build);
            if (file == null)
                return NotFound();

            var blp = new BlpFile(file);
            var image = blp.GetImage(0);

            var ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Position = 0;
            return new FileStreamResult(ms, "image/png");
        }

        [Route("diffText")]
        [HttpGet]
        public string DiffText(int fileDataID, string from, string to)
        {
            var oldFile = CASC.GetFileByID((uint)fileDataID, from);
            var newFile = CASC.GetFileByID((uint)fileDataID, to);

            if (!Directory.Exists("temp/diffs/" + from))
                Directory.CreateDirectory("temp/diffs/" + from);

            using (var fs = new FileStream("temp/diffs/" + from + "/" + fileDataID, FileMode.Create))
            {
                oldFile.CopyTo(fs);
            }

            if (!Directory.Exists("temp/diffs/" + to))
                Directory.CreateDirectory("temp/diffs/" + to);

            using (var fs = new FileStream("temp/diffs/" + to + "/" + fileDataID, FileMode.Create))
            {
                newFile.CopyTo(fs);
            }

            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "git";
                p.StartInfo.Arguments = "diff --no-index temp/diffs/" + from + "/" + fileDataID + " temp/diffs/" + to + "/" + fileDataID;
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generating diff: " + e.Message);
                return "Error generating diff: " + e.Message;
            }
        }

        [Route("relinkFile")]
        [HttpGet]
        public string RelinkFile(uint fileDataID)
        {
            if (CASC.Types.TryGetValue((int)fileDataID, out var fileType))
            {
                if (fileType == "m2")
                    Linker.LinkM2(fileDataID, true);
                else if (fileType == "wmo")
                    Linker.LinkWMO(fileDataID, true);
            }

            return "";
        }

        [Route("startLinking")]
        [HttpGet]
        public string StartLinking()
        {
            Linker.Link();
            return "";
        }

        [Route("clearFileLinks")]
        [HttpGet]
        public string ClearLinks()
        {
            SQLiteDB.ClearLinks();
            return "";
        }

        [Route("clearFileHistory")]
        [HttpGet]
        public string ClearFileHistory()
        {
            SQLiteDB.ClearHistory();
            CASC.ClearFileHistory();
            return "";
        }

        [Route("generateFileHistory")]
        [HttpGet]
        public string GenerateFileHistory()
        {
            CASC.GenerateFileHistory();
            return "";
        }

        [Route("loadFileHistory")]
        [HttpGet]
        public string LoadFileHistory()
        {
            CASC.LoadFileHistory();
            return "";
        }

        [Route("importAllFileHistory")]
        [HttpGet]
        public string ImportAllFileHistory()
        {
            CASC.ImportAllFileHistory();
            return "";
        }

        [Route("importBuildIntoFileHistory")]
        [HttpGet]
        public string ImportBuildIntoFileHistory(string build)
        {
            SQLiteDB.ImportBuildIntoFileHistory(build);
            return "";
        }

        [Route("clearFileTypes")]
        [HttpGet]
        public void ClearFileTypes()
        {
            System.IO.File.Delete("cachedUnknowns.txt");
            CASC.Types.Clear();
            CASC.TypeMap.Clear();
        }
    }
}
