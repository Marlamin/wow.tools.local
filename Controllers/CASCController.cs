using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using NetVips;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Web;
using wow.tools.local.Managers;
using wow.tools.local.Providers;
using wow.tools.local.Services;
using WoWFormatLib.FileProviders;
using WoWFormatLib.FileReaders;
using WoWFormatLib.Structs.M2;
using WoWFormatLib.Structs.WDT;
using WoWNamingLib;
using WoWNamingLib.Namers;
using static TACTSharp.RootInstance;

namespace wow.tools.local.Controllers
{
    [Route("casc/")]
    [ApiController]
    public class CASCController(IDBCManager dbcManager, IDBCProvider dbcProvider) : Controller
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;
        private readonly DBCProvider dbcProvider = (DBCProvider)dbcProvider;
        private static readonly Lock moreInfoInitLock = new Lock();

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
                if (Listfile.Types.ContainsKey((int)fileDataID))
                    filename += "." + Listfile.Types[(int)fileDataID];
            }

            var file = CASC.GetFileByID(fileDataID, build);
            if (file == null)
                return NotFound();

            return new FileStreamResult(file, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        [Route("chash")]
        [HttpGet]
        public ActionResult CHash(string contenthash, string filename = "", string build = "")
        {
            if (build == "")
                build = CASC.BuildName;

            if (string.IsNullOrEmpty(filename))
                filename = contenthash + ".unk";

            try
            {
                if (CASC.TryGetEKeysByCKey(new MD5(Convert.FromHexString(contenthash)), out var eKey))
                {
                    using (var ms = new MemoryStream())
                    {
                        return new FileStreamResult(CASC.GetFileByEKey(eKey.Keys[0], eKey.Size)!, "application/octet-stream")
                        {
                            FileDownloadName = filename
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error extracting file by chash: " + e.Message);
            }

            return NotFound();
        }

        [Route("commonCHashes")]
        [HttpGet]
        public ActionResult CommonCHashes()
        {
            var hashesWithUsages = new Dictionary<string, (string, int)>();
            CASC.EnsureCHashesLoaded();
            foreach (var hash in WoWNamingLib.Namers.ContentHashNamer.knownHashes)
            {
                hashesWithUsages[hash.Key] = (hash.Value, 0);
                if (CASC.CHashToFDID.TryGetValue(hash.Key.ToUpperInvariant(), out var ckeyUses))
                {
                    hashesWithUsages[hash.Key] = (hashesWithUsages[hash.Key].Item1, ckeyUses.Count);
                }
            }

            var jsonSerializerSettings = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return Json(hashesWithUsages, jsonSerializerSettings);
        }

        [Route("dumpInstall")]
        [HttpGet]
        public bool DumpInstall()
        {
            foreach (var entry in CASC.InstallEntries)
            {
                if (CASC.TryGetEKeysByCKey(new MD5(entry.md5), out var eKey))
                {
                    using (var ms = new MemoryStream())
                    {
                        var fileName = Path.DirectorySeparatorChar != '\\' ? entry.name.Replace("\\", Path.DirectorySeparatorChar.ToString()) : entry.name;
                        var directoryName = System.IO.Path.GetDirectoryName(fileName) ?? string.Empty;
                        var outputDir = Path.Combine(SettingsManager.ExtractionDir, CASC.BuildName, directoryName);
                        Directory.CreateDirectory(outputDir);

                        var fileStream = CASC.GetFileByEKey(eKey.Keys[0], eKey.Size);
                        if (fileStream == null)
                        {
                            Console.WriteLine($"Failed to extract {fileName} from install entries, file not found in CASC.");
                            continue;
                        }

                        fileStream.CopyTo(ms);
                        ms.Position = 0;

                        System.IO.File.WriteAllBytes(Path.Combine(outputDir, Path.GetFileName(fileName)), ms.ToArray());
                    }
                }
            }

            return true;
        }

        [Route("buildname")]
        [HttpGet]
        public string BuildName()
        {
            return CASC.BuildName;
        }

        [Route("switchProduct")]
        [HttpGet]
        public bool SwitchProduct(string product)
        {
            if (SettingsManager.ReadOnly)
                return false;

            CASC.InitTACT(SettingsManager.WoWFolder, product);

            // Don't respond until things are done loading
            while (true)
            {
                if (CASC.IsTACTSharpInit)
                    return true;
            }
        }

        [Route("switchConfigs")]
        [HttpGet]
        public bool SwitchConfigs(string product, string buildconfig, string cdnconfig)
        {
            if (SettingsManager.ReadOnly)
                return false;

            CASC.InitTACT(SettingsManager.WoWFolder, product, buildconfig, cdnconfig);

            // Don't respond until things are done loading
            while (true)
            {
                if (CASC.IsTACTSharpInit)
                    return true;
            }
        }

        [Route("updateListfile")]
        [HttpGet]
        public bool UpdateListfile()
        {
            if (SettingsManager.ReadOnly)
                return false;

            BuildDiffCache.Invalidate();
            Listfile.Load(true);
            return true;
        }

        [Route("exportListfile")]
        [HttpGet]
        public bool ExportListfile()
        {
            if (SettingsManager.ReadOnly)
                return false;

            Listfile.Export();
            return true;
        }

        [Route("updateLookups")]
        [HttpGet]
        public bool UpdateLookups()
        {
            if (SettingsManager.ReadOnly)
                return false;

            Listfile.LoadLookups(true);
            return true;
        }

        [Route("exportLookups")]
        [HttpGet]
        public bool ExportLookups()
        {
            if (SettingsManager.ReadOnly)
                return false;

            Listfile.ExportLookups();
            return true;
        }

        [Route("updateTACTKeys")]
        [HttpGet]
        public bool UpdateTACTKeys()
        {
            if (SettingsManager.ReadOnly)
                return false;

            WTLKeyService.LoadKeys(true);
            return true;
        }

        [Route("exportTACTKeys")]
        [HttpGet]
        public bool ExportTACTKeys()
        {
            if (SettingsManager.ReadOnly)
                return false;

            WTLKeyService.ExportTACTKeys();
            return true;
        }

        [Route("listManifests")]
        [HttpGet]
        public List<string> ListManifests()
        {
            return [.. ManifestManager.GetManifestVersions().OrderByDescending(x => int.Parse(x.Split(".")[3]))];
        }

        [Route("analyzeUnknown")]
        [HttpGet]
        public async Task<bool> AnalyzeUnknown()
        {
            Console.WriteLine("Analyzing unknown files");
            var knownUnknowns = new Dictionary<int, string>();

            if (System.IO.File.Exists("cachedUnknowns.txt"))
            {
                knownUnknowns = System.IO.File.ReadAllLines("cachedUnknowns.txt").Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);

                // Remove old M3 related placeholder types: m3strtbl, m3shlib, m3matlib
                foreach (var key in knownUnknowns.Keys.ToList())
                {
                    if (knownUnknowns[key] == "m3strtbl" || knownUnknowns[key] == "m3shlib" || knownUnknowns[key] == "m3matlib")
                        knownUnknowns.Remove(key);
                }
            }

            List<int> unknownFiles = CASC.AvailableFDIDs.Except(Listfile.Types.Where(x => x.Value != "unk").Select(x => x.Key)).ToList();

            if (knownUnknowns.Count > 0)
            {
                foreach (var knownUnknown in knownUnknowns)
                    Listfile.SetFileType(knownUnknown.Key, knownUnknown.Value);

                unknownFiles = CASC.AvailableFDIDs.Except(Listfile.Types.Where(x => x.Value != "unk").Select(x => x.Key)).ToList();
            }

            try
            {
                var tfdStorage = await dbcManager.GetOrLoad("TextureFileData", CASC.BuildName);
                foreach (dynamic tfdEntry in tfdStorage.Values)
                {
                    var fdid = (int)tfdEntry.FileDataID;
                    if (!Listfile.Types.TryGetValue(fdid, out string? value) || value == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "blp");
                        unknownFiles.Remove(fdid);
                        Listfile.SetFileType(fdid, "blp");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with TextureFileData:" + e.Message);
            }

            try
            {
                var mfdStorage = await dbcManager.GetOrLoad("MovieFileData", CASC.BuildName);
                foreach (dynamic mfdEntry in mfdStorage.Values)
                {
                    var fdid = (int)mfdEntry.ID;
                    if (!Listfile.Types.TryGetValue(fdid, out string? value) || value == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "avi");
                        unknownFiles.Remove(fdid);
                        Listfile.SetFileType(fdid, "avi");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with MovieFileData:" + e.Message);
            }

            try
            {
                var movieStorage = await dbcManager.GetOrLoad("Movie", CASC.BuildName);
                foreach (dynamic movieEntry in movieStorage.Values)
                {
                    var audioFDID = (int)movieEntry.AudioFileDataID;
                    if (audioFDID != 0)
                    {
                        if (!Listfile.Types.TryGetValue(audioFDID, out string? value) || value == "unk")
                        {
                            knownUnknowns.TryAdd(audioFDID, "mp3");
                            unknownFiles.Remove(audioFDID);
                            Listfile.SetFileType(audioFDID, "mp3");
                        }
                    }

                    var subtitleFDID = (int)movieEntry.SubtitleFileDataID;
                    if (subtitleFDID != 0)
                    {
                        if (!Listfile.Types.TryGetValue(subtitleFDID, out string? value) || value == "unk")
                        {
                            var format = (int)movieEntry.SubtitleFileFormat;
                            var subtitleType = "srt";
                            if (format == 118)
                                subtitleType = "srt";
                            else if (format == 7)
                                subtitleType = "sbt";
                            else
                            {
                                Console.WriteLine("Unknown subtitle format " + format);
                                continue;
                            }

                            knownUnknowns.TryAdd(subtitleFDID, subtitleType);
                            unknownFiles.Remove(subtitleFDID);
                            Listfile.SetFileType(subtitleFDID, subtitleType);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with Movie:" + e.Message);
            }

            try
            {
                if (CASC.FileExists(1375802))
                {
                    var mp3Storage = await dbcManager.GetOrLoad("ManifestMP3", CASC.BuildName);
                    foreach (dynamic mp3Entry in mp3Storage.Values)
                    {
                        var fdid = (int)mp3Entry.ID;
                        if (!Listfile.Types.TryGetValue(fdid, out string? value) || value == "unk")
                        {
                            knownUnknowns.TryAdd(fdid, "mp3");
                            unknownFiles.Remove(fdid);
                            Listfile.SetFileType(fdid, "mp3");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with ManifestMP3:" + e.Message);
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
                    if (CASC.EncryptionStatuses.TryGetValue(unknownFile, out CASC.EncryptionStatus value) && value == CASC.EncryptionStatus.EncryptedUnknownKey)
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
                                        type = "pvdata";
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
                                bin.ReadBytes(8); // length + ver
                                var secondChunk = bin.ReadBytes(4);
                                var subChunk = Encoding.ASCII.GetString(secondChunk);
                                switch (subChunk)
                                {
                                    case "RDHM": // ADT root
                                        var newLength = bin.ReadInt32();
                                        bin.ReadBytes(newLength);
                                        var thirdChunk = bin.ReadBytes(4);
                                        var subSubChunk = Encoding.ASCII.GetString(thirdChunk);
                                        if (subSubChunk == "EFMM")
                                        {
                                            type = "wdt";
                                        }
                                        else
                                        {
                                            type = "adt";
                                        }
                                        break;
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
                                    case "3LPM":
                                    case "IMVP": // WDT MPV
                                    case "GOFV": // WDT FOGS
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
                            case "M3DT":
                            case "M3SI":
                            case "M3ST":
                            case "MES3":
                            case "M3CL":
                            case "M3SV":
                            case "M3XF":
                                type = "m3";
                                break;
                            case "m3SL":
                            case "m3SH":
                            case "m3SP":
                            case "m3ST":
                            case "m3SS":
                            case "m3MD":
                            case "m3S2":
                            case "m3M2":
                            case "m3DB":
                            case "m3ML":
                                type = "mtl3lib";
                                break;
                            case "*QIL":
                                type = "liq";
                                break;
                            case "<?xm":
                                type = "xml";
                                break;
                            case "???1":
                                type = "srt";
                                break;
                            case "<Ui ":
                                type = "xml";
                                break;
                            case "## T":
                                type = "toc";
                                break;
                            case "#pra":
                                type = "hlsl";
                                break;
                            case "?PNG":
                                type = "png";
                                break;
                            case "NIBA":
                                type = "nib";
                                break;
                            case "APPL":
                                type = "PkgInfo";
                                break;
                            default:
                                break;
                        }

                        if (magic[0] == 0xCA && magic[1] == 0xFE && magic[2] == 0xBA && magic[3] == 0xBE)
                            type = "macbin";

                        if (magic[0] == 'M' && magic[1] == 'Z')
                            type = "exe";

                        if (magicString.StartsWith("ID3") || (magic[0] == 0xFF && magic[1] == 0xFB))
                            type = "mp3";

                        if (type == "unk")
                            Console.WriteLine((uint)unknownFile + " - Unknown magic " + magicString + " (" + Convert.ToHexString(magic) + ")");

                        lock (typeLock)
                        {
                            Listfile.SetFileType(unknownFile, type);
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

            try
            {
                var mfdStorage = await dbcManager.GetOrLoad("ModelFileData", CASC.BuildName);
                foreach (dynamic mfdEntry in mfdStorage.Values)
                {
                    var fdid = (int)mfdEntry.FileDataID;

                    // Likely an encrypted file if we got this far, these could also be M3s but just assume they are M2s for now.
                    if (!Listfile.Types.TryGetValue(fdid, out string? value) || value == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "m2");
                        unknownFiles.Remove(fdid);
                        Listfile.SetFileType(fdid, "m2");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with ModelFileData:" + e.Message);
            }

            try
            {
                var skStorage = await dbcManager.GetOrLoad("SoundKitEntry", CASC.BuildName);
                foreach (dynamic skEntry in skStorage.Values)
                {
                    var fdid = (int)skEntry.FileDataID;
                    if (!Listfile.Types.TryGetValue(fdid, out string? value) || value == "unk")
                    {
                        knownUnknowns.TryAdd(fdid, "ogg");
                        unknownFiles.Remove(fdid);
                        Listfile.SetFileType(fdid, "ogg");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during type guessing with SoundKitEntry:" + e.Message);
            }

            System.IO.File.WriteAllLines("cachedUnknowns.txt", knownUnknowns.Where(x => x.Value != "unk").Select(x => x.Key + ";" + x.Value));
            Console.WriteLine("Finished unknown file analysis");

            BuildDiffCache.Invalidate();

            return true;
        }

        [Route("diff")]
        [HttpGet]
        public async Task<ActionResult> DiffManifests(string from, string to, string type = "normal")
        {
            var oldDiff = new ApiDiff
            {
                Added = Array.Empty<DiffEntry>(),
                Removed = Array.Empty<DiffEntry>(),
                Modified = Array.Empty<DiffEntry>()
            };

            var jsonOptions = new JsonSerializerOptions()
            {
                IncludeFields = true
            };

            if (BuildDiffCache.Get(from, to, out ApiDiff diff))
            {
                if (type == "normal")
                {
                    return Json(new
                    {
                        added = diff.Added.Count(),
                        modified = diff.Modified.Count(),
                        removed = diff.Removed.Count(),
                        data = diff.All.ToArray()
                    }, jsonOptions);
                }
                else
                {
                    oldDiff = diff;
                }
            }

            var allListfileNames = Listfile.GetAllNames();

            Func<KeyValuePair<int, string>, DiffEntry> toDiffEntry(DiffAction action)
            {
                return delegate (KeyValuePair<int, string> entry)
                {
                    var file = allListfileNames.TryGetValue(entry.Key, out var filename) ? filename : "";

                    return new DiffEntry
                    {
                        action = action,
                        filename = file,
                        id = entry.Key,
                        md5 = entry.Value.ToLower(),
                        type = Listfile.Types.TryGetValue(entry.Key, out string? type) ? type : (!string.IsNullOrEmpty(filename) ? Path.GetExtension(file).Replace(".", "").ToLower() : "unk"),
                        encryptedStatus = CASC.EncryptionStatuses.TryGetValue(entry.Key, out CASC.EncryptionStatus encStatus) ? encStatus : CASC.EncryptionStatus.Unknown
                    };
                };
            }

            var rawFromEntries = new List<(uint FileDataID, byte[] MD5)>();
            var rawToEntries = new List<(uint FileDataID, byte[] MD5)>();

            if (type == "normal")
            {
                var fromTask = ManifestManager.GetEntriesForVersionAsync(from);
                var toTask = ManifestManager.GetEntriesForVersionAsync(to);
                await Task.WhenAll(fromTask, toTask);
                rawFromEntries = fromTask.Result;
                rawToEntries = toTask.Result;
            }
            else
            {
                TACTSharp.BuildInstance fromBuild;
                TACTSharp.BuildInstance toBuild;

                if (from == CASC.BuildName)
                    fromBuild = CASC.buildInstance!;
                else
                    fromBuild = BuildManager.GetBuildByVersion(from);

                if (to == CASC.BuildName)
                    toBuild = CASC.buildInstance!;
                else
                    toBuild = BuildManager.GetBuildByVersion(to);

                foreach (var entry in fromBuild.Root!.GetAvailableFDIDs())
                {
                    var entries = fromBuild.Root.GetEntriesByFDID(entry);
                    var anMD5 = entries[0].md5;
                    var encodingEntries = fromBuild.Encoding!.FindContentKey(anMD5);
                    rawFromEntries.Add((entry, encodingEntries[0].ToArray()));
                }

                foreach (var entry in toBuild.Root!.GetAvailableFDIDs())
                {
                    var entries = toBuild.Root.GetEntriesByFDID(entry);
                    var anMD5 = entries[0].md5;
                    var encodingEntries = toBuild.Encoding!.FindContentKey(anMD5);
                    rawToEntries.Add((entry, encodingEntries[0].ToArray()));
                }
            }

            var fromDict = rawFromEntries.ToDictionary(x => (int)x.FileDataID, x => Convert.ToHexString(x.MD5));
            var toDict = rawToEntries.ToDictionary(x => (int)x.FileDataID, x => Convert.ToHexString(x.MD5));

            var fromEntries = fromDict.Keys.ToHashSet();
            var toEntries = toDict.Keys.ToHashSet();

            var commonEntries = fromEntries.Intersect(toEntries).ToHashSet();
            var removedEntries = fromEntries.Except(commonEntries);
            var addedEntries = toEntries.Except(commonEntries);

            var addedFiles = addedEntries.Select(entry => new KeyValuePair<int, string>(entry, toDict[entry]));
            var removedFiles = removedEntries.Select(entry => new KeyValuePair<int, string>(entry, fromDict[entry]));
            var modifiedFiles = new ConcurrentBag<KeyValuePair<int, string>>();

            dbcProvider.LoadFromBuildManager = true;

            Parallel.ForEach(commonEntries, entry =>
            {
                // DB2 files are special, we need to ignore the string in header (if current build, obviously)
                var type = "unk";
                if (Listfile.Types.TryGetValue(entry, out string? value))
                    type = value;

                if (type == "db2")
                {
                    if (Listfile.NameMap.TryGetValue(entry, out var filename))
                    {
                        var basename = Path.GetFileNameWithoutExtension(filename);

                        try
                        {
                            var fromDB2 = dbcProvider.StreamForTableName(basename, from);
                            var toDB2 = dbcProvider.StreamForTableName(basename, to);

                            if (fromDB2.Length != toDB2.Length)
                            {
                                modifiedFiles.Add(new KeyValuePair<int, string>(entry, toDict[entry]));
                                return;
                            }

                            var fromDB2Header = new byte[4];
                            fromDB2.ReadExactly(fromDB2Header);

                            var toDB2Header = new byte[4];
                            toDB2.ReadExactly(toDB2Header);

                            if (MemoryMarshal.Read<int>(fromDB2Header) == 0x35434457 && MemoryMarshal.Read<int>(toDB2Header) == 0x35434457)
                            {
                                fromDB2.Position = 136;
                                toDB2.Position = 136;

                                var remainingFromBytes = new byte[fromDB2.Length - 136];
                                fromDB2.ReadExactly(remainingFromBytes);

                                var remainingToBytes = new byte[toDB2.Length - 136];
                                toDB2.ReadExactly(remainingToBytes);

                                if (!remainingFromBytes.SequenceEqual(remainingToBytes))
                                    modifiedFiles.Add(new KeyValuePair<int, string>(entry, toDict[entry]));

                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed to compare DB2 " + basename + " for entry " + entry + ": " + e.Message);
                        }
                    }
                }

                var originalFile = fromDict[entry];
                var patchedFile = toDict[entry];

                if (originalFile != patchedFile)
                    modifiedFiles.Add(new KeyValuePair<int, string>(entry, patchedFile));
            });

            dbcProvider.LoadFromBuildManager = false;

            var toAddedDiffEntryDelegate = toDiffEntry(DiffAction.Added);
            var toRemovedDiffEntryDelegate = toDiffEntry(DiffAction.Removed);
            var toModifiedDiffEntryDelegate = toDiffEntry(DiffAction.Modified);

            DiffEntry[] addedDiffEntries;
            DiffEntry[] removedDiffEntries;
            DiffEntry[] modifiedDiffEntries;

            if (type == "normal")
            {
                addedDiffEntries = addedFiles.Select(toAddedDiffEntryDelegate).ToArray();
                removedDiffEntries = removedFiles.Select(toRemovedDiffEntryDelegate).ToArray();
                modifiedDiffEntries = modifiedFiles.Select(toModifiedDiffEntryDelegate).ToArray();
            }
            else
            {
                var oldAdded = oldDiff.Added.Select(x => x.id).ToHashSet();
                var oldRemoved = oldDiff.Removed.Select(x => x.id).ToHashSet();
                var oldModified = oldDiff.Modified.Select(x => x.id).ToHashSet();

                addedDiffEntries = addedFiles.Where(x => !oldAdded.Contains(x.Key)).Select(toAddedDiffEntryDelegate).ToArray();
                removedDiffEntries = removedFiles.Where(x => !oldRemoved.Contains(x.Key)).Select(toRemovedDiffEntryDelegate).ToArray();
                modifiedDiffEntries = modifiedFiles.Where(x => !oldModified.Contains(x.Key)).Select(toModifiedDiffEntryDelegate).ToArray();
            }

            diff = new ApiDiff
            {
                Added = addedDiffEntries,
                Removed = removedDiffEntries,
                Modified = modifiedDiffEntries
            };

            Console.WriteLine($"Added: {addedDiffEntries.Length}, removed: {removedDiffEntries.Length}, modified: {modifiedDiffEntries.Length}, common: {commonEntries.Count}");

            if (type == "normal")
                BuildDiffCache.Add(from, to, diff);

            var allEntries = new DiffEntry[addedDiffEntries.Length + removedDiffEntries.Length + modifiedDiffEntries.Length];
            addedDiffEntries.CopyTo(allEntries, 0);
            removedDiffEntries.CopyTo(allEntries, addedDiffEntries.Length);
            modifiedDiffEntries.CopyTo(allEntries, addedDiffEntries.Length + removedDiffEntries.Length);

            return Json(new
            {
                added = addedDiffEntries.Length,
                modified = modifiedDiffEntries.Length,
                removed = removedDiffEntries.Length,
                data = allEntries
            }, jsonOptions);
        }

        [Route("samehashes")]
        [HttpGet]
        public string SameHashes(string chash)
        {
            CASC.EnsureCHashesLoaded();

            var html = "The table below lists files that are identical in content to the requested file.<br><table class='table table-striped'><thead><tr><th>ID</th><th>Name (if available)</th></tr></thead>";

            var filedataids = CASC.GetSameFiles(chash).Order();
            foreach (var filedataid in filedataids)
            {
                html += "<tr><td><a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' data-bs-toggle='modal' data-bs-target='#moreInfoModal' data-tooltip='file' data-id='" + filedataid + "' onclick='fillModal(" + filedataid + ")'>" + filedataid + "</a></td><td>" + (Listfile.NameMap.TryGetValue(filedataid, out var filename) ? filename : "N/A") + "</td></tr>";
            }
            html += "</table>";

            return html;
        }

        [Route("hashbyid")]
        [HttpGet]
        public (string, int) HashByID(int filedataid)
        {
            CASC.EnsureCHashesLoaded();

            var allCKeys = CASC.GetCKeysAndFlagsByFDID(filedataid);
            if (allCKeys.Count > 0)
            {
                var primaryCKey = Convert.ToHexStringLower(CASC.GetPreferredCKey(allCKeys));
                var filedataids = CASC.GetSameFiles(primaryCKey).Order();

                return (primaryCKey, filedataids.Count());
            }
            return ("N/A", 0);
        }

        [Route("suggestCHashes")]
        public static string SuggestCHashes()
        {
            var suggestions = "";

            var knownHashes = WoWNamingLib.Namers.ContentHashNamer.knownHashes.Keys.ToHashSet();
            var knownNames = WoWNamingLib.Namers.ContentHashNamer.knownHashes.Values.ToHashSet();

            var search = "type:blp,!maptextures,available,!baked,multiuse,lookupmatch,!character,!interface,!minimaps,!maps,!_lod";
            var listfileResults = Listfile.DoSearch(Listfile.NameMap, search);

            var filesToCheck = new Dictionary<string, int>();
            foreach (var result in listfileResults)
            {
                var basename = Path.GetFileNameWithoutExtension(result.Value);
                if (!filesToCheck.ContainsKey(basename))
                    filesToCheck.Add(basename, result.Key);
            }

            foreach (var fileToCheck in filesToCheck)
            {
                var basename = fileToCheck.Key;
                var fdid = fileToCheck.Value;
                var cKeys = CASC.GetCKeysAndFlagsByFDID(fdid);
                if (cKeys.Count > 0)
                {
                    var primaryCKey = Convert.ToHexStringLower(CASC.GetPreferredCKey(cKeys));
                    if(!knownHashes.Contains(primaryCKey) && !knownNames.Contains(basename, StringComparer.OrdinalIgnoreCase))
                    {
                        knownHashes.Add(primaryCKey);
                        knownNames.Add(basename);
                        suggestions += "{\"" + primaryCKey + "\", \"" + basename + "\"},\n";
                    }
                }
            }

            return suggestions;
        }

        [Route("knownChashes")]
        [HttpGet]
        public Dictionary<string, string> KnownCHashes()
        {
            return WoWNamingLib.Namers.ContentHashNamer.knownHashes;
        }

        [Route("moreinfo")]
        [HttpGet]
        public async Task<string> MoreInfo(int filedataid)
        {
            moreInfoInitLock.Enter();

            CASC.EnsureCHashesLoaded();

            // Yes, generating HTML here is ugly but that's how the old system worked and I can't be arsed to redo it.
            var html = "<div style='float: right'><a class='btn btn-sm btn-primary' id='fileRelinkButton' onClick='relinkFile(" + filedataid + ")'>Recrawl file links</a></div><table style='clear: both' class='table table-striped'><thead><tr><th></th><th></th></tr></thead>";
            html += "<tr><td>FileDataID</td><td>" + filedataid + "</td></tr>";
            html += "<tr><td>Filename</td><td>" + (Listfile.NameMap.TryGetValue(filedataid, out var filename) ? filename : "unknown/" + filedataid + ".unk") + "</td></tr>";
            html += "<tr><td>Lookup</td>";

            if (Listfile.LookupMap.TryGetValue(filedataid, out var lookup))
            {
                var hasher = new TACTSharp.Jenkins96();
                html += "<td>" + lookup.ToString("X16");

                if (Listfile.NameMap.TryGetValue(filedataid, out var lookupFilename) && lookupFilename != "")
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

            html += "<tr><td>Type</td><td>" + (Listfile.Types.TryGetValue(filedataid, out string? value) ? value : "unk") + "</td></tr>";

            var allCKeys = CASC.GetCKeysAndFlagsByFDID(filedataid);
            if (allCKeys.Count > 0)
            {
                var primaryCKey = Convert.ToHexStringLower(CASC.GetPreferredCKey(allCKeys));

                html += "<tr><td>Content Hashes</td><td>";
                html += "<table class='table table-sm table-inverse'>";
                html += "<thead><tr><th>MD5/CKey</th><th>LocaleFlags</th><th>ContentFlags</th><th>&nbsp;</th></tr></thead>";

                foreach (var extraCKeyEntry in allCKeys)
                {
                    var extraCKeyHex = Convert.ToHexStringLower(extraCKeyEntry.cKey);

                    html += "<tr>";
                    html += "<td style='font-family: monospace;'><a href='#' data-bs-toggle='modal' data-bs-target='#chashModal' onClick='fillChashModal(\"" + extraCKeyHex + "\")'>" + extraCKeyHex + "</a>";
                    if (primaryCKey == extraCKeyHex && allCKeys.Count > 1)
                        html += " (preferred)";
                    html += "</td>";
                    html += "<td>" + extraCKeyEntry.localeFlags + "</td>";
                    html += "<td>" + extraCKeyEntry.contentFlags + "</td>";
                    html += "<td><a href='/casc/chash?contenthash=" + extraCKeyHex + "&filename=" + filedataid + ".bytes'>download</a></td>";
                    html += "</tr>";
                }

                html += "</table>";

                html += "</td></tr>";

                html += "<tr><td>Size</td><td>" + (CASC.CHashToSize.TryGetValue(primaryCKey, out var size) ? size + " bytes" : "N/A");

                if (allCKeys.Count > 1)
                    html += " (for preferred version)";

                html += "</td></tr>";
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

                var usedKeys = CASC.EncryptedFDIDs[filedataid].Distinct();

                html += "<tr><td>Encryption status</td><td>" + prettyEncryptionStatus + "<br>";
                html += "<table class='table table-sm table-inverse'>";
                html += "<thead><tr><th></th><th style='width: 10%'>Key</th><th style='width: 4%'>ID</th><th style='width: 22%'>First seen</th><th>Description</th></tr></thead>";

                var usedKeyInfo = new List<(ulong lookup, int ID, string FirstSeen, string Description, int totalFiles)>();
                foreach (var key in usedKeys)
                {
                    var affectedFiles = CASC.EncryptedFDIDs.Where(kvp => kvp.Value.Contains(key)).Select(kvp => kvp.Key).Count();
                    if (KeyMetadata.KeyInfo.TryGetValue(key, out var keyInfo))
                    {
                        usedKeyInfo.Add((key, keyInfo.ID, keyInfo.FirstSeen, keyInfo.Description, affectedFiles));
                    }
                    else
                    {
                        usedKeyInfo.Add((key, 0, "N/A", "No metadata known for this key, this is fine if this is a voice over file or a recently added key. Check back in a future version.", affectedFiles));
                    }
                }

                var db2EncryptionMetaData = new Dictionary<ulong, int[]>();

                try
                {
                    if (
                        Listfile.Types.TryGetValue(filedataid, out string? fileType) && fileType.Equals("db2", StringComparison.CurrentCultureIgnoreCase) &&
                        Listfile.NameMap.TryGetValue(filedataid, out var db2filename) && db2filename != ""
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
                    html += "<tr><td>" + (WTLKeyService.HasKey(key.lookup) ? "<i style='color: green' class='fa fa-unlock'></i>" : "<i style='color: red' class='fa fa-lock'></i>") + "</td><td><a style='font-family: monospace;' target='_BLANK' href='/files/#search=encrypted%3A" + key.lookup.ToString("X16").PadLeft(16, '0') + "'>" + key.lookup.ToString("X16").PadLeft(16, '0') + "</a></td><td>" + key.ID + "</td><td>" + key.FirstSeen + "</td><td>" + key.Description + " <small><i>(" + key.totalFiles + " total files)</i></small></td></tr>";

                    if (db2EncryptionMetaData.TryGetValue(key.lookup, out var encryptedIDs))
                    {
                        html += "<tr><td colspan='4'>&nbsp;</td><td style='padding-left: 20px;'><b>" + encryptedIDs.Length;
                        if (WTLKeyService.HasKey(key.lookup))
                        {
                            html += " <a href='/dbc/?dbc=" + Path.GetFileNameWithoutExtension(Listfile.NameMap[filedataid]).ToLower() + "&build=" + CASC.BuildName + "#page=1&search=encrypted%3A" + key.lookup.ToString("X16").PadLeft(16, '0') + "' target='_BLANK' class='text-success'>available</a>";
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
                        var dlFilename = Listfile.NameMap.TryGetValue(filedataid, out var listfileName) ? Path.GetFileName(listfileName) : filedataid + ".unk";
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
                    html += "<tr><td>" + linkedFile.linkType + "</td>";
                    html += "<td><a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' data-bs-toggle='modal' data-bs-target='#moreInfoModal' data-tooltip='file' data-id='" + linkedFile.fileDataID + "' onclick='fillModal(" + linkedFile.fileDataID + ")'>" + linkedFile.fileDataID + "</a> <a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer' onclick='fillModal(" + linkedFile.fileDataID + ")'><i class='fa fa-info-circle'></i></a></td><td>" + (Listfile.NameMap.TryGetValue((int)linkedFile.fileDataID, out var linkedFilename) ? linkedFilename : "unknown/" + linkedFile.fileDataID + ".unk") + "</td><td>" + (Listfile.Types.ContainsKey((int)linkedFile.fileDataID) ? Listfile.Types[(int)linkedFile.fileDataID] : "unk") + "</td></tr>";
                }
                html += "</table></td></tr>";
            }

            if (Listfile.Types.TryGetValue(filedataid, out string? type) && (type == "m2" || type == "wmo"))
            {
                if (!Linker.existingParents.Contains(filedataid))
                {
                    try
                    {
                        switch (value)
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
                }

                if (type == "m2" && Namer.isInitialized)
                {
                    if (Model.spellNames.Count == 0)
                        Model.LoadSpellMap();

                    if (Model.spellNamesClean.TryGetValue((uint)filedataid, out var spellNameClean))
                    {
                        html += "<tr><td><b>Generated name (spell)</b></td><td onclick='copyToClipboard(\"" + spellNameClean + "\")'>" + spellNameClean + "</td></tr>";
                    }

                    if (Model.spellNames.TryGetValue((uint)filedataid, out var spellEntries))
                    {
                        html += "<tr><td colspan='2'><b>Spells using this model</b></td></tr>";
                        html += "<tr><td colspan='2'><table class='table table-sm table-striped'>";
                        foreach (var spellEntry in spellEntries)
                        {
                            html += "<tr><td style='width: 80px;'>" + spellEntry.SpellID + "</td><td style='width: 100px; font-size: 12px;'><a target='_BLANK' href='https://wowdb.com/spells/" + spellEntry.SpellID + "'>WoWDB</a> - <a target='_BLANK' href='https://wowhead.com/spell=" + spellEntry.SpellID + "'>WH</a></td><td>" + spellEntry.SpellName + "</td></tr>";
                        }
                        html += "</table></td></tr>";
                    }
                }

                if (type == "wmo" && Namer.isInitialized)
                {
                    var wmoFile = CASC.GetFileByID((uint)filedataid);
                    if (wmoFile != null)
                    {
                        var bin = new BinaryReader(wmoFile);
                        var magic = bin.ReadUInt32();
                        if(magic != 0)
                        {
                            bin.BaseStream.Position = 0;
                            var reader = new WMOReader();
                            var parsedWMO = reader.LoadWMO(wmoFile);

                            html += "<tr><td colspan='2'><b>Group names for this WMO</b></td></tr>";
                            html += "<tr><td colspan='2'><table class='table table-sm table-striped'>";
                            foreach (var mogn in parsedWMO.groupNames)
                            {
                                html += "<tr><td>" + mogn.name + "</td></tr>";
                            }
                            html += "</table></td></tr>";
                        }
                    }
                }
            }

            var linkedChildFiles = SQLiteDB.GetFilesByParent(filedataid);

            if (linkedChildFiles.Count > 0)
            {
                html += "<tr><td colspan='2'><b>Files this file links to</b></td></tr>";
                html += "<tr><td colspan='2'><table class='table table-sm table-striped'>";
                html += "<tr><th>Link type</th><th>ID</th><th>Filename</th><th>Type</th></tr>";
                foreach (var linkedFile in linkedChildFiles)
                {
                    html += "<tr><td>" + linkedFile.linkType + "</td><td><a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' data-bs-toggle='modal' data-bs-target='#moreInfoModal' data-tooltip='file' data-id='" + linkedFile.fileDataID + "' onclick='fillModal(" + linkedFile.fileDataID + ")'>" + linkedFile.fileDataID + "</a> <a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer' onclick='fillModal(" + linkedFile.fileDataID + ")'><i class='fa fa-info-circle'></i></a></td><td>" + (Listfile.NameMap.TryGetValue((int)linkedFile.fileDataID, out var linkedFilename) ? linkedFilename : "unknown/" + linkedFile.fileDataID + ".unk") + "</td><td>" + (Listfile.Types.ContainsKey((int)linkedFile.fileDataID) ? Listfile.Types[(int)linkedFile.fileDataID] : "unk") + "</td></tr>";
                }
                html += "</table></td></tr></table>";
            }

            // TODO: Soundkits?

            html += "<tr><td colspan='2'><b>Neighbouring files</b></td></tr>";
            html += "<tr><td colspan='2'><table class='table table-sm'>";
            html += "<tr><th>ID</th><th>Filename</th></tr>";

            var listfileAsKeys = Listfile.NameMap.Keys.ToList();
            listfileAsKeys.Sort();
            var fileDataIDPosition = listfileAsKeys.IndexOf(filedataid);

            for (int i = -5; i < 6; i++)
            {
                int idx = fileDataIDPosition + i;
                if (idx < 0 || idx >= listfileAsKeys.Count)
                    continue;

                var fdid = listfileAsKeys[idx];
                if (Listfile.NameMap.TryGetValue(fdid, out var listfileName))
                    if (fdid == filedataid)
                        html += "<tr><td style='color: red'><b><a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' data-bs-toggle='modal' data-bs-target='#moreInfoModal' data-tooltip='file' data-id='" + fdid + "' onclick='fillModal(" + fdid + ")'>" + fdid + "</a></b></td><td style='color: red'><b>" + listfileName + "</b></td></tr>";
                    else
                        html += "<tr><td><a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' data-bs-toggle='modal' data-bs-target='#moreInfoModal' data-tooltip='file' data-id='" + fdid + "' onclick='fillModal(" + fdid + ")'>" + fdid + "</a></td><td>" + listfileName + "</td></tr>";
            }

            html += "</table></td></tr></table>";
            moreInfoInitLock.Exit();
            return html;
        }

        [Route("getVersion")]
        [HttpGet]
        public string GetVersion()
        {
            return System.Reflection.Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
        }

        [Route("diffFile")]
        [HttpGet]
        public string DiffFile(int fileDataID, string from, string to)
        {
            var textTypes = new List<string>() { "html", "htm", "lua", "json", "txt", "wtf", "toc", "xml", "xsd", "sbt", "hlsl" };
            var jsonTypes = new List<string>() { "m2", "wmo", /*"wdt",*/ "adt", "m3", "tex" };
            var imageTypes = new List<string>() { "blp", "png", };

            var html = "<ul class='nav nav-tabs' id='diffTabs' role='tablist'>";
            var js = @"      
            var d2hConfig = {
                drawFileList: false,
                fileListToggle: false,
                fileListStartVisible: false,
                fileContentToggle: false,
                matching: 'lines',
                outputFormat: 'side-by-side',
                inputFormat: 'diff',
                synchronisedScroll: true,
                highlight: true,
                renderNothingWhenEmpty: false,
              };
            ";

            if (!Listfile.Types.TryGetValue(fileDataID, out var fileType))
                fileType = "unk";

            var hasActiveTab = false;

            if (imageTypes.Contains(fileType))
            {
                hasActiveTab = true;
                html += "<li class='nav-item'>";
                html += "<a class='nav-link active' id='sbs-tab' data-bs-toggle='tab' href='#sbs' role='tab' aria-controls='sbs' aria-selected='true'>Side-by-Side</a>";
                html += "</li>";
                html += "<li class='nav-item'>";
                html += "<a class='nav-link' id='toggle-tab' data-bs-toggle='tab' href='#toggle' role='tab' aria-controls='toggle' aria-selected='false'>Switcher</a>";
                html += "</li>";
            }

            if (jsonTypes.Contains(fileType))
            {
                hasActiveTab = true;
                html += "<li class='nav-item'>";
                html += "<a class='nav-link active' id='json-tab' data-bs-toggle='tab' href='#json' role='tab' aria-controls='json' aria-selected='true'>JSON</a>";
                html += "</li>";
            }

            if (textTypes.Contains(fileType))
            {
                hasActiveTab = true;
                html += "<li class='nav-item'>";
                html += "<a class='nav-link active' id='text-tab' data-bs-toggle='tab' href='#text' role='tab' aria-controls='text' aria-selected='true'>Text</a>";
                html += "</li>";
            }

            html += "<li class='nav-item'>";
            html += "<a class='nav-link" + (!hasActiveTab ? " active" : "") + "' id='hex-tab' data-bs-toggle='tab' href='#hex' role='tab' aria-controls='hex' aria-selected='" + (!hasActiveTab ? " true" : "false") + "'>Raw (hex)</a>";
            html += "</li>";

            html += "</ul>";

            html += "<div class='tab-content' style='min-height: 256px;'>";
            if (imageTypes.Contains(fileType))
            {
                // side by side tab
                html += "<div class='tab-pane active' id='sbs' role='tabpanel' aria-labelledby='sbs-tab'>";
                html += "<div class='row'>";
                html += "<div class='col-md-6' id='from-diff'>";
                html += "<h3>Build " + from + " (Before)</h3>";

                if (fileType == "blp")
                {
                    html += "<img id='fromImage' style='max-width: 100%;' src='/casc/blp2png?fileDataID=" + fileDataID + "&build=" + from + "'>";
                    html += "</div>";
                    html += "<div class='col-md-6' id='to-diff'>";
                    html += "<h3>Build " + to + " (After)</h3>";
                    html += "<img id='toImage' style='max-width: 100%;' src='/casc/blp2png?fileDataID=" + fileDataID + "&build=" + to + "'>";
                }
                else
                {
                    html += "<img id='fromImage' style='max-width: 100%;' src='/casc/file?fileDataID=" + fileDataID + "&build=" + from + "'>";
                    html += "</div>";
                    html += "<div class='col-md-6' id='to-diff'>";
                    html += "<h3>Build " + to + " (After)</h3>";
                    html += "<img id='toImage' style='max-width: 100%;' src='/casc/file?fileDataID=" + fileDataID + "&build=" + to + "'>";
                }

                html += "</div>";
                html += "</div>";
                html += "</div>";

                // toggle tab
                html += "<div class='tab-pane' id='toggle' role='tabpanel' aria-labelledby='toggle-tab'>";
                html += "<div id='toggle-content' data-current='from'>";
                html += "<div class='col-md-6' id='from-diff'>";
                html += "<h3>Build " + from + " (Before)</h3>";

                if (fileType == "blp")
                {
                    html += "<img id='fromImage' style='max-width: 100%;' src='/casc/blp2png?fileDataID=" + fileDataID + "&build=" + from + "'>";
                }
                else
                {
                    html += "<img id='fromImage' style='max-width: 100%;' src='/casc/file?fileDataID=" + fileDataID + "&build=" + from + "'>";
                }

                html += "</div>";
                html += "</div>";
                html += "<button class='btn btn-primary' id='toggle-button' onclick='switchImages()'>Switch</button>";
                html += "</div>";
                js += @"
                    function switchImages(){ 
                        let toggleDiv = document.getElementById('toggle-content');
                        let fromHTML = document.getElementById('from-diff').innerHTML;
                        let toHTML = document.getElementById('to-diff').innerHTML;
                        
                        if(toggleDiv.dataset.current == 'from'){ 
                            toggleDiv.innerHTML = toHTML; 
                            toggleDiv.dataset.current = 'to'; 
                        } else { 
                            toggleDiv.innerHTML = fromHTML; 
                            toggleDiv.dataset.current = 'from'; 
                        }
                    }
                    ";
            }

            if (jsonTypes.Contains(fileType))
            {
                if (!Listfile.NameMap.TryGetValue(fileDataID, out var filename))
                    filename = "";

                html += "<div class='tab-pane active' id='json' role='tabpanel' aria-labelledby='json-tab'>";
                html += "Note: Git is required to be installed on the system (and in PATH) to generate JSON diffs.<br>";

                if (string.IsNullOrEmpty(filename))
                    html += "<div class='alert alert-warning'>This file is a file with multiple subtypes (e.g. WDT or ADT) with an unknown filename. The file will be loaded as the main type (root WDT or root ADT), any subtypes will show incorrect information below or cause other issues.</div>";

                html += "<div id='json-content' style='max-height: 100vh'>";
                html += "<i class='fa fa-spin fa-spinner'></i>";
                html += "</div>";

                js += @"
                    fetch('/casc/diffJSON?fileDataID=" + fileDataID + "&from=" + from + "&to=" + to + @"')
                        .then(response => response.text())
                        .then(data => {
                        try{
                            if(data.length > 10000000)
                                throw new Error('Too much data');

                            var diff2htmlUi = new Diff2HtmlUI(document.getElementById('json-content'), data, d2hConfig);
                            diff2htmlUi.draw();
                            diff2htmlUi.highlightCode();
                        } catch (error) {
                            document.getElementById('json-content').innerHTML = '<div class=\'alert alert-danger\'>A client-side error occurred while generating this diff (it may be too much data): ' + error.message + '</div>';
                        }
                    });
            ";

                html += "</div>";
            }

            if (textTypes.Contains(fileType))
            {
                html += "<div class='tab-pane active' id='text' role='tabpanel' aria-labelledby='text-tab'>";
                html += "Note: Git is required to be installed on the system (and in PATH) to generate text diffs.<br>";
                html += "<div id='text-content'>";
                html += "<i class='fa fa-spin fa-spinner'></i>";
                html += "</div>";
                html += "</div>";

                js += @"
                        fetch('/casc/diffText?fileDataID=" + fileDataID + "&from=" + from + "&to=" + to + @"')
                        .then(response => response.text())
                        .then(data => {
                            try{
                                if(data.length > 10000000)
                                    throw new Error('Too much data');

                                var diff2htmlUi = new Diff2HtmlUI(document.getElementById('text-content'), data, d2hConfig);
                                diff2htmlUi.draw();
                                diff2htmlUi.highlightCode();
                            } catch (error) {
                                document.getElementById('text-content').innerHTML = '<div class=\'alert alert-danger\'>A client-side error occurred while generating this diff (it may be too much data): ' + error.message + '</div>';
                            }
                        });
                ";
            }

            html += "<div class='tab-pane" + (!hasActiveTab ? " active" : "") + "' id='hex' role='tabpanel' aria-labelledby='hex-tab'>";
            html += "Note: Git is required to be installed on the system (and in PATH) to generate raw hex diffs.<br>";
            html += "<div id='hex-content' style='max-height: 100vh'>";
            html += "<i class='fa fa-spin fa-spinner'></i>";
            html += "</div>";
            html += "</div>";

            if (!hasActiveTab)
                js += "document.addEventListener('DOMContentLoaded', () => {";
            else
                js += "document.getElementById('hex-tab').addEventListener('click', () => {";

            js += @"
                fetch('/casc/diffHex?fileDataID=" + fileDataID + "&from=" + from + "&to=" + to + @"')
                    .then(response => response.text())
                    .then(data => {
                        try{
                            if(data.length > 10000000)
                                throw new Error('Too much data');

                            var diff2htmlUi = new Diff2HtmlUI(document.getElementById('hex-content'), data, d2hConfig);
                            diff2htmlUi.draw();
                            diff2htmlUi.highlightCode();
                        } catch (error) {
                            document.getElementById('hex-content').innerHTML = '<div class=\'alert alert-danger\'>A client-side error occurred while generating this diff (it may be too much data): ' + error.message + '</div>';
                        }
                });
           ";

            js += "});";

            html += "</div>";

            html += "<script type='text/javascript'>" + js + "</script>";

            return html;
        }

        [Route("blp2png")]
        [HttpGet]
        public ActionResult Blp2Png(int fileDataID, string build = "")
        {
            if (string.IsNullOrEmpty(build) || build == "?")
                build = CASC.BuildName;

            var file = CASC.GetFileByID((uint)fileDataID, build);
            if (file == null)
                return NotFound();

            // check encrypted
            var first4 = new byte[4];
            file.ReadExactly(first4, 0, 4);
            if (first4[0] == 0 && first4[1] == 0 && first4[2] == 0 && first4[3] == 0)
                return NotFound();

            file.Position = 0;

            var blp = new BLPSharp.BLPFile(file);
            var pixels = blp.GetPixels(0, out var w, out var h);

            using var raw = NetVips.Image.NewFromMemory(pixels, w, h, 4, Enums.BandFormat.Uchar);
            using var image = raw[2].Bandjoin(new[] { raw[1], raw[0], raw[3] }).Copy(interpretation: Enums.Interpretation.Srgb);

            var ms = new MemoryStream();
            image.WriteToStream(ms, ".png");
            ms.Position = 0;
            return new FileStreamResult(ms, "image/png");
        }

        [Route("diffText")]
        [HttpGet]
        public string DiffText(int fileDataID, string from, string to)
        {
            if (!Directory.Exists("temp/diffs/" + from))
                Directory.CreateDirectory("temp/diffs/" + from);

            if (!Listfile.Types.TryGetValue(fileDataID, out var fileType))
                fileType = "txt";

            var oldFile = CASC.GetFileByID((uint)fileDataID, from);
            if (oldFile == null)
                return "Error loading file " + fileDataID + " from build";

            using (var fs = new FileStream("temp/diffs/" + from + "/" + fileDataID + "." + fileType, FileMode.Create))
            {
                oldFile.CopyTo(fs);
            }

            if (!Directory.Exists("temp/diffs/" + to))
                Directory.CreateDirectory("temp/diffs/" + to);

            var newFile = CASC.GetFileByID((uint)fileDataID, to);
            if (newFile == null)
                return "Error loading file " + fileDataID + " from build";

            using (var fs = new FileStream("temp/diffs/" + to + "/" + fileDataID + "." + fileType, FileMode.Create))
            {
                newFile.CopyTo(fs);
            }

            try
            {
                Process p = new();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "git";
                p.StartInfo.Arguments = "diff --no-index temp/diffs/" + from + "/" + fileDataID + "." + fileType + " temp/diffs/" + to + "/" + fileDataID + "." + fileType;
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

        [Route("reloadBuilds")]
        [HttpGet]
        public bool ReloadBuilds()
        {
            CASC.LoadBuildInfo();
            return true;
        }

        [Route("relinkFile")]
        [HttpGet]
        public string RelinkFile(uint fileDataID)
        {
            if (SettingsManager.ReadOnly)
                return "";

            if (Listfile.Types.TryGetValue((int)fileDataID, out var fileType))
            {
                if (fileType == "m2")
                    Linker.LinkM2(fileDataID, true);
                else if (fileType == "wmo")
                    Linker.LinkWMO(fileDataID, true);
                else if(fileType == "wdt")
                    Linker.LinkWDT((int)fileDataID, true);
            }

            return "";
        }

        [Route("startLinking")]
        [HttpGet]
        public string StartLinking(bool fullRun = false)
        {
            if (SettingsManager.ReadOnly)
                return "";

            Linker.Link(fullRun);
            return "";
        }

        [Route("clearFileLinks")]
        [HttpGet]
        public string ClearLinks()
        {
            if (SettingsManager.ReadOnly)
                return "";

            SQLiteDB.ClearLinks();
            Linker.existingParents.Clear();
            return "";
        }

        [Route("clearFileHistory")]
        [HttpGet]
        public string ClearFileHistory()
        {
            if (SettingsManager.ReadOnly)
                return "";

            SQLiteDB.ClearHistory();
            CASC.VersionHistory.Clear();
            return "";
        }

        [Route("generateFileHistory")]
        [HttpGet]
        public string GenerateFileHistory()
        {
            if (SettingsManager.ReadOnly)
                return "";

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
            if (SettingsManager.ReadOnly)
                return "";

            CASC.ImportAllFileHistory();
            return "";
        }

        [Route("importBuildIntoFileHistory")]
        [HttpGet]
        public string ImportBuildIntoFileHistory(string build)
        {
            if (SettingsManager.ReadOnly)
                return "";

            SQLiteDB.ImportBuildIntoFileHistory(build);
            return "";
        }

        [Route("convertManifests")]
        [HttpGet]
        public string ConvertManifests()
        {
            if (SettingsManager.ReadOnly)
                return "";

            ManifestManager.ConvertAllTxtToWtlm();
            return "";
        }

        [Route("clearFileTypes")]
        [HttpGet]
        public void ClearFileTypes()
        {
            if (SettingsManager.ReadOnly)
                return;

            System.IO.File.Delete("cachedUnknowns.txt");
            Listfile.Types.Clear();
            Listfile.TypeMap.Clear();
        }

        [Route("checkFiles")]
        [HttpPost]
        public async Task<string> CheckFiles()
        {
            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8))
                rawContent = await reader.ReadToEndAsync();

            var filenames = rawContent.Split("\n").ToList();
            var unknownFDIDs = Listfile.NameMap.Where(x => x.Value == "").Select(x => x.Key).ToList();
            var reverseLookup = Listfile.LookupMap.ToDictionary(x => x.Value, x => x.Key);
            var hasher = new TACTSharp.Jenkins96();
            var results = new List<string>();
            foreach (var filename in filenames)
            {
                var lookup = hasher.ComputeHash(filename.Trim());
                if (reverseLookup.TryGetValue(lookup, out var unkFDID))
                {
                    results.Add(unkFDID + ";" + filename);
                }
            }
            return string.Join('\n', results);
        }

        [Route("checkFilesFromFile")]
        [HttpGet]
        public string CheckFilesFromFile(string file = "")
        {
            if (SettingsManager.ReadOnly)
                return "";

            var filenames = System.IO.File.ReadAllLines(file);
            var unknownFDIDs = Listfile.NameMap.Where(x => x.Value == "").Select(x => x.Key).ToList();
            var reverseLookup = Listfile.LookupMap.ToDictionary(x => x.Value, x => x.Key);
            var hasher = new TACTSharp.Jenkins96();
            var results = new List<string>();
            foreach (var filename in filenames)
            {
                var lookup = hasher.ComputeHash(filename.Trim());
                if (reverseLookup.TryGetValue(lookup, out var unkFDID))
                {
                    results.Add(unkFDID + ";" + filename);
                }
            }
            return string.Join('\n', results);
        }

        [Route("fileCheck")]
        [HttpGet]
        public bool FileCheck(string search)
        {
            return Listfile.NameMap.Where(x => x.Value.Equals(search, StringComparison.CurrentCultureIgnoreCase)).Any();
        }

        [Route("directoryAC")]
        [HttpGet]
        public List<string> DirectoryAC(string search)
        {
            return Listfile.NameMap.Values
                .Where(x => !string.IsNullOrEmpty(x) && x.StartsWith(search, StringComparison.CurrentCultureIgnoreCase) && (x.ToLower().EndsWith(".m2") || x.ToLower().EndsWith(".wmo")))
                .Select(x => x.Replace('\\', '/'))
                .DistinctBy(x => x.ToLower()).OrderByDescending(x => x).Take(20).ToList();
        }

        [Route("json")]
        [HttpGet]
        public string Json(uint fileDataID, string? build, string? overrideCKey = "", string? overrideType = "")
        {
            build ??= CASC.BuildName;

            var supportedTypes = new List<string> { "wmo", "m2", "adt", "bls", "m3", "gfat", /*"wdt",*/ "wdl", "tex" };

            if (!(Listfile.Types.TryGetValue((int)fileDataID, out var fileType) && supportedTypes.Contains(fileType)))
            {
                return "Unsupported file type or file not found";
            }

            if (!FileProvider.HasProvider(build))
            {
                if (build == CASC.BuildName)
                {
                    if (CASC.IsTACTSharpInit)
                    {
                        var tact = new TACTSharpFileProvider();
                        tact.InitTACT(CASC.buildInstance);
                        FileProvider.SetProvider(tact, CASC.BuildName);
                    }
                }
                else
                {
                    try
                    {
                        var dbBuild = BuildManager.GetBuildByVersion(build);
                        var tact = new TACTSharpFileProvider();
                        tact.InitTACT(dbBuild);
                        FileProvider.SetProvider(tact, build);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error initializing TACTSharp for build " + build + ": " + e.Message);
                        var wago = new WagoFileProvider();
                        wago.SetBuild(build);
                        FileProvider.SetProvider(wago, build);
                    }
                }
            }

            FileProvider.SetDefaultBuild(build);

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true,
                IncludeFields = true
            };

            switch (!string.IsNullOrEmpty(overrideType) ? overrideType : fileType)
            {
                // TODO: Tuple conversion for System.Text.JSON
                //case "wdt":
                //    var wdtReader = new WDTReader();
                //    wdtReader.LoadWDT(fileDataID);
                //    return JsonSerializer.Serialize(wdtReader.wdtfile, options);
                case "wdl":
                    var wdlReader = new WDLReader();
                    wdlReader.LoadWDL(fileDataID);
                    return JsonSerializer.Serialize(wdlReader.wdlfile, options);
                case "tex":
                    var texReader = new TEXReader();
                    var tex = texReader.LoadTEX(fileDataID);
                    return JsonSerializer.Serialize(tex, options);
                case "wmo":
                    var wmoReader = new WMOReader();
                    var wmo = wmoReader.LoadWMO(fileDataID);
                    if(wmo.group != null)
                    {
                        for (var i = 0; i < wmo.group.Length; i++)
                        {
                            wmo.group[i].mogp.indices = [];
                            wmo.group[i].mogp.vertices = [];
                            wmo.group[i].mogp.normals = [];
                            wmo.group[i].mogp.textureCoords = [];
                            wmo.group[i].mogp.colors = [];
                            wmo.group[i].mogp.colors2 = [];
                            wmo.group[i].mogp.colors3 = [];
                            wmo.group[i].mogp.bspIndices = [];
                            wmo.group[i].mogp.bspNodes = [];
                            wmo.group[i].mogp.materialInfo = [];
                        }
                    }
                    return JsonSerializer.Serialize(wmo, options);
                case "adt":
                    var adtReader = new ADTReader();

                    uint obj0FDID = 0;
                    uint tex0FDID = 0;

                    if (Listfile.NameMap.TryGetValue((int)fileDataID, out var adtName) && !string.IsNullOrEmpty(adtName))
                    {
                        obj0FDID = Listfile.NameMap.Select(x => (Key: x.Key, Value: x.Value))
                            .Where(x => !string.IsNullOrEmpty(x.Value) && x.Value.EndsWith("_obj0.adt") && x.Value.StartsWith(adtName.Replace(".adt", "")))
                            .Select(x => (uint)x.Key).FirstOrDefault();

                        tex0FDID = Listfile.NameMap.Select(x => (Key: x.Key, Value: x.Value))
                            .Where(x => !string.IsNullOrEmpty(x.Value) && x.Value.EndsWith("_tex0.adt") && x.Value.StartsWith(adtName.Replace(".adt", "")))
                            .Select(x => (uint)x.Key).FirstOrDefault();
                    }

                    if (obj0FDID == 0 || tex0FDID == 0)
                        adtReader.LoadADT(fileDataID, MPHDFlags.adt_has_height_texturing | MPHDFlags.adt_has_height_texturing);
                    else
                        adtReader.LoadADT(MPHDFlags.adt_has_height_texturing | MPHDFlags.adt_has_height_texturing, fileDataID, obj0FDID, tex0FDID, true);

                    if (adtReader.adtfile.chunks != null)
                    {
                        for (var i = 0; i < adtReader.adtfile.chunks.Length; i++)
                        {
                            adtReader.adtfile.chunks[i].vertices = new WoWFormatLib.Structs.ADT.MCVT();
                            adtReader.adtfile.chunks[i].normals.normal_0 = [];
                            adtReader.adtfile.chunks[i].normals.normal_1 = [];
                            adtReader.adtfile.chunks[i].normals.normal_2 = [];
                            adtReader.adtfile.chunks[i].colors.color = [];
                        }
                    }

                    return JsonSerializer.Serialize(adtReader.adtfile, options);
                case "m2":
                    var m2Reader = new M2Reader();
                    m2Reader.LoadM2(fileDataID);
                    // lets maybe not return all m2 data
                    m2Reader.model.vertices = [];
                    if (m2Reader.model.skins != null)
                    {
                        for (var i = 0; i < m2Reader.model.skins.Length; i++)
                        {
                            m2Reader.model.skins[i].indices = [];
                            m2Reader.model.skins[i].triangles = [];
                            m2Reader.model.skins[i].properties = [];
                        }
                    }
                    return JsonSerializer.Serialize(m2Reader.model, options);
                case "m3":
                    var m3Reader = new M3Reader();
                    m3Reader.LoadM3(fileDataID);
                    return JsonSerializer.Serialize(m3Reader.model, options);
                case "bls":
                    var blsReader = new BLSReader();

                    if (!string.IsNullOrEmpty(overrideCKey))
                        blsReader.LoadBLS(Convert.FromHexString(overrideCKey));
                    else
                        blsReader.LoadBLS(fileDataID);

                    //var extractDir = Path.Combine("extract", "bls", fileDataID.ToString());
                    //var baseName = fileDataID.ToString();

                    //if (Listfile.NameMap.TryGetValue((int)fileDataID, out var shaderFileName) && !string.IsNullOrEmpty(shaderFileName))
                    //{
                    //    baseName = fileDataID.ToString() + " (" + shaderFileName.Replace("shaders/", "").Replace(".bls", "").Replace("/", "-") + ")";
                    //    extractDir = Path.Combine("extract", "bls", baseName);
                    //}

                    //if (!Directory.Exists(extractDir))
                    //    Directory.CreateDirectory(extractDir);

                    var json = JsonSerializer.Serialize(blsReader.shaderFile, options);
                    //System.IO.File.WriteAllText(Path.Combine(Path.Combine("extract", "bls"), baseName + ".json"), json);
                    //var shaderIndex = 0;
                    //foreach(var decompressedShader in blsReader.shaderFile.decompressedShaders)
                    //{
                    //    System.IO.File.WriteAllBytes(Path.Combine(extractDir, "shader_" + shaderIndex++ + ".bytes"), decompressedShader);
                    //}
                    return json;
                case "gfat":
                    var gfatReader = new GFATReader();
                    var gfat = gfatReader.LoadGFAT(fileDataID);

                    if (!string.IsNullOrEmpty(overrideCKey))
                        gfat = gfatReader.LoadGFAT(Convert.FromHexString(overrideCKey));

                    return JsonSerializer.Serialize(gfat, options);
                default:
                    throw new Exception("Unsupported file type");
            }
        }

        [Route("hex")]
        [HttpGet]
        public string Hex(uint fileDataID, string? build)
        {
            build ??= CASC.BuildName;

            if (!FileProvider.HasProvider(build))
            {
                if (build == CASC.BuildName)
                {
                    if (CASC.IsTACTSharpInit)
                    {
                        var tact = new TACTSharpFileProvider();
                        tact.InitTACT(CASC.buildInstance);
                        FileProvider.SetProvider(tact, CASC.BuildName);
                    }
                }
                else
                {
                    try
                    {
                        var dbBuild = BuildManager.GetBuildByVersion(build);
                        var tact = new TACTSharpFileProvider();
                        tact.InitTACT(dbBuild);
                        FileProvider.SetProvider(tact, build);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error initializing TACTSharp for build " + build + ": " + e.Message);
                        var wago = new WagoFileProvider();
                        wago.SetBuild(build);
                        FileProvider.SetProvider(wago, build);
                    }
                }
            }

            FileProvider.SetDefaultBuild(build);

            using (var stream = FileProvider.OpenFile(fileDataID))
            {
                var hex = new StringBuilder();
                var ascii = new StringBuilder();
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < stream.Length; i++)
                    {
                        // Cut off at 1MB
                        if (i > 1024 * 1024)
                        {
                            hex.AppendLine().Append("... (file too large to display fully)");
                            break;
                        }

                        if (i % 16 == 0)
                        {
                            if (i != 0)
                            {
                                hex.Append("  ").Append(ascii).AppendLine();
                                ascii.Clear();
                            }

                            hex.Append($"{i:X8}: ");
                        }

                        var rawByte = reader.ReadByte();
                        hex.Append($"{rawByte:X2} ");
                        ascii.Append(rawByte >= 32 && rawByte <= 126 ? (char)rawByte : '.');
                    }

                    // append leftovers
                    if (ascii.Length > 0)
                    {
                        int padding = 16 - (int)(stream.Length % 16);
                        if (padding < 16)
                        {
                            hex.Append(new string(' ', padding * 3));
                        }
                        hex.Append("  ").Append(ascii);
                    }
                }
                return HttpUtility.HtmlEncode(hex.ToString());
            }
        }

        [Route("diffJSON")]
        [HttpGet]
        public string JsonDiff(uint fileDataID, string from, string to)
        {
            var oldJson = Json(fileDataID, from);
            var newJson = Json(fileDataID, to);

            if (!Directory.Exists("temp/diffs/" + from))
                Directory.CreateDirectory("temp/diffs/" + from);

            System.IO.File.WriteAllText("temp/diffs/" + from + "/" + fileDataID + ".json", oldJson);

            if (!Directory.Exists("temp/diffs/" + to))
                Directory.CreateDirectory("temp/diffs/" + to);

            System.IO.File.WriteAllText("temp/diffs/" + to + "/" + fileDataID + ".json", newJson);

            try
            {
                Process p = new();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "git";
                p.StartInfo.Arguments = "diff --no-index temp/diffs/" + from + "/" + fileDataID + ".json" + " temp/diffs/" + to + "/" + fileDataID + ".json";
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

        [Route("diffHex")]
        [HttpGet]
        public string HexDiff(uint fileDataID, string from, string to)
        {
            var oldJson = Hex(fileDataID, from);
            var newJson = Hex(fileDataID, to);

            if (!Directory.Exists("temp/diffs/" + from))
                Directory.CreateDirectory("temp/diffs/" + from);

            System.IO.File.WriteAllText("temp/diffs/" + from + "/" + fileDataID + ".hexdump", oldJson);

            if (!Directory.Exists("temp/diffs/" + to))
                Directory.CreateDirectory("temp/diffs/" + to);

            System.IO.File.WriteAllText("temp/diffs/" + to + "/" + fileDataID + ".hexdump", newJson);

            try
            {
                Process p = new();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "git";
                p.StartInfo.Arguments = "diff --no-index temp/diffs/" + from + "/" + fileDataID + ".hexdump" + " temp/diffs/" + to + "/" + fileDataID + ".hexdump";
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

        [Route("commonBLPs")]
        [HttpGet]
        public Dictionary<string, List<int>> CommonBLPs()
        {
            CASC.EnsureCHashesLoaded();
            var blpIDs = Listfile.TypeMap["blp"];
            var commonCHashes = CASC.CHashToFDID.Where(x => x.Value.Count > 2).OrderByDescending(x => x.Value.Count).ToDictionary();

            var chashesToSkip = new HashSet<string> { "93eb33c44532ea7e4f62666417beaa6a", "77beda3cb2c5709fc953c9d21e1d2414", "ef3ae8b80605064fadc0515b10c82ef2" }; // empty maptextures, minimaps
            var result = new Dictionary<string, List<int>>();
            foreach (var entry in commonCHashes)
            {
                if (chashesToSkip.Contains(entry.Key.ToLowerInvariant()))
                    continue;

                if (blpIDs.Contains(entry.Value[0]))
                    result[entry.Key] = entry.Value;
            }


            return result;
        }
    }
}
