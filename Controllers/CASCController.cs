using CASCLib;
using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using wow.tools.local.Services;
using wow.tools.Services;
using WoWFormatLib.FileProviders;
using WoWFormatLib.FileReaders;
using WoWFormatLib.Structs.WDT;
using WoWNamingLib;
using WoWNamingLib.Namers;

namespace wow.tools.local.Controllers
{
    [Route("casc/")]
    [ApiController]
    public class CASCController(IDBCManager dbcManager, IDBCProvider dbcProvider) : Controller
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;
        private readonly DBCProvider dbcProvider = (DBCProvider)dbcProvider;
        private static readonly Dictionary<string, string> RibbitCache = new();

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
                if (CASC.TryGetEKeysByCKey(contenthash.FromHexString().ToMD5(), out var eKey))
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

        [Route("dumpInstall")]
        [HttpGet]
        public bool DumpInstall()
        {
            foreach (var entry in CASC.InstallEntries)
            {
                if (CASC.TryGetEKeysByCKey(entry.MD5, out var eKey))
                {
                    using (var ms = new MemoryStream())
                    {
                        var fileName = Path.DirectorySeparatorChar != '\\' ? entry.Name.Replace("\\", Path.DirectorySeparatorChar.ToString()) : entry.Name;
                        var directoryName = System.IO.Path.GetDirectoryName(fileName) ?? string.Empty;
                        var outputDir = Path.Combine(SettingsManager.extractionDir, CASC.BuildName, directoryName);
                        Directory.CreateDirectory(outputDir);

                        var fileStream = CASC.GetFileByEKey(eKey.Keys[0], eKey.Size);
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

        [Route("builds")]
        [HttpPost]
        public DataTablesResult Builds(bool remote = false)
        {
            var result = new DataTablesResult();

            if (Request.Method == "POST" && Request.Form.TryGetValue("draw", out var drawValue) && int.TryParse(drawValue, out var draw))
            {
                result.draw = draw;
                result.data = [];
            }

            if (!remote && SettingsManager.wowFolder != null && System.IO.File.Exists(Path.Combine(SettingsManager.wowFolder, ".build.info")))
            {
                foreach (var availableBuild in CASC.AvailableBuilds)
                {
                    var splitVersion = availableBuild.Version.Split(".");
                    var patch = splitVersion[0] + "." + splitVersion[1] + "." + splitVersion[2];
                    var build = splitVersion[3];

                    var isActive = CASC.CurrentProduct == availableBuild.Product;

                    // It's possible that we're not actually on the same build as the product is on when loading custom configs with TACTSharp. Double check.
                    if (isActive && CASC.IsTACTSharpInit)
                        isActive = availableBuild.BuildConfig == CASC.buildInstance.Settings.BuildConfig && availableBuild.CDNConfig == CASC.buildInstance.Settings.CDNConfig;

                    var hasManifest = System.IO.File.Exists(Path.Combine(SettingsManager.manifestFolder, patch + "." + build + ".txt"));
                    var hasDBCs = Directory.Exists(Path.Combine(SettingsManager.dbcFolder, patch + "." + build, "dbfilesclient"));
                    result.data.Add([patch, build, availableBuild.Product, availableBuild.Folder, availableBuild.BuildConfig, availableBuild.CDNConfig, isActive.ToString(), hasManifest.ToString(), hasDBCs.ToString()]);
                }

                result.data = [.. result.data.OrderBy(x => x[0])];
                result.recordsTotal = result.data.Count;
                result.recordsFiltered = result.data.Count;
            }

            if (remote)
            {
                var ribbitClient = new Ribbit.Protocol.Client("us.version.battle.net", 1119);
                RibbitCache["v1/summary"] = ribbitClient.Request("v1/summary").ToString();

                var builds = new Ribbit.Parsing.BPSV(RibbitCache["v1/summary"]);
                foreach (var product in builds.data)
                {
                    // Skip products with no versions
                    if (product[2] != "" || !product[0].StartsWith("wow"))
                        continue;

                    var endPoint = "v1/products/" + product[0] + "/versions";
                    if (!RibbitCache.TryGetValue(endPoint, out var cachedResult))
                    {
                        cachedResult = ribbitClient.Request(endPoint).ToString();
                        RibbitCache[endPoint] = cachedResult;
                    }

                    foreach (var line in cachedResult.Split("\n"))
                    {
                        var splitLine = line.Split('|');

                        if (splitLine[0] != "us")
                            continue;

                        var splitVersion = splitLine[5].Split(".");
                        var patch = splitVersion[0] + "." + splitVersion[1] + "." + splitVersion[2];
                        var build = splitVersion[3];

                        var isActive = CASC.CurrentProduct == product[0] && CASC.IsOnline;
                        // It's possible that we're not actually on the same build as the product is on when loading custom configs with TACTSharp. Double check.
                        if (isActive && CASC.IsTACTSharpInit)
                            isActive = splitLine[1] == CASC.buildInstance.Settings.BuildConfig && splitLine[2] == CASC.buildInstance.Settings.CDNConfig;

                        var hasManifest = System.IO.File.Exists(Path.Combine(SettingsManager.manifestFolder, patch + "." + build + ".txt"));
                        var hasDBCs = Directory.Exists(Path.Combine(SettingsManager.dbcFolder, patch + "." + build, "dbfilesclient"));

                        result.data.Add([patch, build, product[0], splitLine[1], splitLine[2], isActive.ToString(), hasManifest.ToString(), hasDBCs.ToString()]);
                    }

                    // sort by build
                    result.data = result.data.OrderByDescending(x => x[1]).ToList();
                }
            }

            return result;
        }

        [Route("switchProduct")]
        [HttpGet]
        public bool SwitchProduct(string product, bool isOnline = false)
        {
            if (SettingsManager.useTACTSharp)
                CASC.InitTACT(isOnline ? "" : SettingsManager.wowFolder, product);
            else
                CASC.InitCasc(isOnline ? "" : SettingsManager.wowFolder, product);

            // Don't respond until things are done loading
            while (true)
            {
                if (SettingsManager.useTACTSharp && CASC.IsTACTSharpInit)
                    return true;

                if (!SettingsManager.useTACTSharp && CASC.IsCASCLibInit)
                    return true;
            }
        }

        [Route("switchConfigs")]
        [HttpGet]
        public bool SwitchConfigs(string buildconfig, string cdnconfig)
        {
            if (!SettingsManager.useTACTSharp)
                return false;

            CASC.InitTACT(null, "wow", buildconfig, cdnconfig);

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
            return [.. cachedManifests.OrderByDescending(x => int.Parse(x.Split(".")[3]))];
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

            List<int> unknownFiles = CASC.AvailableFDIDs.Except(CASC.Types.Where(x => x.Value != "unk").Select(x => x.Key)).ToList();

            if (knownUnknowns.Count > 0)
            {
                foreach (var knownUnknown in knownUnknowns)
                    CASC.SetFileType(knownUnknown.Key, knownUnknown.Value);

                unknownFiles = CASC.AvailableFDIDs.Except(CASC.Types.Where(x => x.Value != "unk").Select(x => x.Key)).ToList();
            }

            try
            {
                var mfdStorage = await dbcManager.GetOrLoad("ModelFileData", CASC.BuildName);
                foreach (dynamic mfdEntry in mfdStorage.Values)
                {
                    var fdid = (int)mfdEntry.FileDataID;

                    // Skip these for now -- contains M3s
                    if (mfdEntry.ModelResourcesID == 0)
                    {
                        Console.WriteLine("Skipping MFD => M2 mapping for " + fdid + " for having ModelResourcesID 0, likely an M3 file.");
                        continue;
                    }

                    if (fdid == 5569152 || fdid == 5916032 || fdid == 6022679) // M3, hopefully these get separate out at some point in ModelFileData through a flag or something
                        continue;

                    if (!CASC.Types.TryGetValue(fdid, out string? value) || value == "unk")
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
                var tfdStorage = await dbcManager.GetOrLoad("TextureFileData", CASC.BuildName);
                foreach (dynamic tfdEntry in tfdStorage.Values)
                {
                    var fdid = (int)tfdEntry.FileDataID;
                    if (!CASC.Types.TryGetValue(fdid, out string? value) || value == "unk")
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
                var mfdStorage = await dbcManager.GetOrLoad("MovieFileData", CASC.BuildName);
                foreach (dynamic mfdEntry in mfdStorage.Values)
                {
                    var fdid = (int)mfdEntry.ID;
                    if (!CASC.Types.TryGetValue(fdid, out string? value) || value == "unk")
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
                if (CASC.FileExists(1375802))
                {
                    var mp3Storage = await dbcManager.GetOrLoad("ManifestMP3", CASC.BuildName);
                    foreach (dynamic mp3Entry in mp3Storage.Values)
                    {
                        var fdid = (int)mp3Entry.ID;
                        if (!CASC.Types.TryGetValue(fdid, out string? value) || value == "unk")
                        {
                            knownUnknowns.TryAdd(fdid, "mp3");
                            unknownFiles.Remove(fdid);
                            CASC.SetFileType(fdid, "mp3");
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
                            default:
                                break;
                        }

                        if (magicString.StartsWith("ID3") || (magic[0] == 0xFF && magic[1] == 0xFB))
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

            try
            {
                var skStorage = await dbcManager.GetOrLoad("SoundKitEntry", CASC.BuildName);
                foreach (dynamic skEntry in skStorage.Values)
                {
                    var fdid = (int)skEntry.FileDataID;
                    if (!CASC.Types.TryGetValue(fdid, out string? value) || value == "unk")
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

            System.IO.File.WriteAllLines("cachedUnknowns.txt", knownUnknowns.Where(x => x.Value != "unk").Select(x => x.Key + ";" + x.Value));
            Console.WriteLine("Finished unknown file analysis");
            return true;
        }

        [Route("diff")]
        [HttpGet]
        public async Task<ActionResult> DiffManifests(string from, string to)
        {
            if (BuildDiffCache.Get(from, to, out ApiDiff diff))
            {
                return Json(new
                {
                    added = diff.Added.Count(),
                    modified = diff.Modified.Count(),
                    removed = diff.Removed.Count(),
                    data = diff.All.ToArray()
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
                        type = CASC.Types.TryGetValue(entry.Key, out string? type) ? type : (!string.IsNullOrEmpty(filename) ? Path.GetExtension(file).Replace(".", "").ToLower() : "unk"),
                        encryptedStatus = CASC.EncryptionStatuses.TryGetValue(entry.Key, out CASC.EncryptionStatus encStatus) ? encStatus.ToString() : ""
                    };
                };
            }

            var rootFromEntries = (await System.IO.File.ReadAllLinesAsync(Path.Combine(SettingsManager.manifestFolder, from + ".txt"))).Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);
            var rootToEntries = (await System.IO.File.ReadAllLinesAsync(Path.Combine(SettingsManager.manifestFolder, to + ".txt"))).Select(x => x.Split(";")).ToDictionary(x => int.Parse(x[0]), x => x[1]);

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

                // DB2 files are special, we need to ignore the string in header (if current build, obviously)
                var type = "unk";
                if (CASC.Types.TryGetValue(entry, out string? value))
                    type = value;

                if (type == "db2")
                {
                    if (CASC.Listfile.TryGetValue(entry, out var filename))
                    {
                        var basename = Path.GetFileNameWithoutExtension(filename);

                        try
                        {
                            var fromDB2 = dbcProvider.StreamForTableName(basename, from);
                            var fromDB2Header = new byte[4];
                            await fromDB2.ReadExactlyAsync(fromDB2Header);

                            var toDB2 = dbcProvider.StreamForTableName(basename, to);
                            var toDB2Header = new byte[4];
                            await toDB2.ReadExactlyAsync(toDB2Header);

                            if (MemoryMarshal.Read<int>(fromDB2Header) == 0x35434457 && MemoryMarshal.Read<int>(fromDB2Header) == 0x35434457)
                            {
                                fromDB2.Position = 136;
                                toDB2.Position = 136;

                                var remainingFromBytes = new byte[fromDB2.Length - 136];
                                await fromDB2.ReadExactlyAsync(remainingFromBytes);

                                var remainingToBytes = new byte[toDB2.Length - 136];
                                await toDB2.ReadExactlyAsync(remainingToBytes);

                                if (!remainingFromBytes.SequenceEqual(remainingToBytes))
                                    modifiedFiles.Add(new KeyValuePair<int, string>(entry, rootToEntries[entry]));

                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed to compare DB2 " + basename + " for entry " + entry + ": " + e.Message);
                        }
                    }
                }

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
                Added = addedFiles.Select(toAddedDiffEntryDelegate),
                Removed = removedFiles.Select(toRemovedDiffEntryDelegate),
                Modified = modifiedFiles.Select(toModifiedDiffEntryDelegate)
            };

            Console.WriteLine($"Added: {diff.Added.Count()}, removed: {diff.Removed.Count()}, modified: {diff.Modified.Count()}, common: {commonEntries.Count()}");

            BuildDiffCache.Add(from, to, diff);

            return Json(new
            {
                added = diff.Added.Count(),
                modified = diff.Modified.Count(),
                removed = diff.Removed.Count(),
                data = diff.All.ToArray()
            });
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
                html += "<tr><td>" + filedataid + "</td><td>" + (CASC.Listfile.TryGetValue(filedataid, out var filename) ? filename : "N/A") + "</td></tr>";
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

            html += "<tr><td>Type</td><td>" + (CASC.Types.TryGetValue(filedataid, out string? value) ? value : "unk") + "</td></tr>";

            if (CASC.FDIDToCHash.TryGetValue(filedataid, out var cKey))
            {
                html += "<tr><td>Content hash (MD5)</td><td style='font-family: monospace;'><a href='#' data-bs-toggle='modal' data-bs-target='#chashModal' onClick='fillChashModal(\"" + cKey.ToLower() + "\")'>" + cKey.ToLower() + "</a></td></tr>";
                html += "<tr><td>Size</td><td>" + (CASC.CHashToSize.TryGetValue(cKey, out long size) ? size + " bytes" : "N/A") + "</td></tr>";
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
                        CASC.Types.TryGetValue(filedataid, out string? fileType) && fileType.Equals("db2", StringComparison.CurrentCultureIgnoreCase) &&
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
                    html += "<tr><td>" + (WTLKeyService.HasKey(key.lookup) ? "<i style='color: green' class='fa fa-unlock'></i>" : "<i style='color: red' class='fa fa-lock'></i>") + "</td><td><a style='font-family: monospace;' target='_BLANK' href='/files/#search=encrypted%3A" + key.lookup.ToString("X16").PadLeft(16, '0') + "'>" + key.lookup.ToString("X16").PadLeft(16, '0') + "</a></td><td>" + key.ID + "</td><td>" + key.FirstSeen + "</td><td>" + key.Description + "</td></tr>";

                    if (db2EncryptionMetaData.TryGetValue(key.lookup, out var encryptedIDs))
                    {
                        html += "<tr><td colspan='4'>&nbsp;</td><td style='padding-left: 20px;'><b>" + encryptedIDs.Length;
                        if (WTLKeyService.HasKey(key.lookup))
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

            if (CASC.Types.TryGetValue(filedataid, out string? type) && (type == "m2" || type == "wmo"))
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
                        html += "<tr><td><b>Generated name (spell)</b></td><td>" + spellNameClean + "</td></tr>";
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
                    html += "<tr><td>" + version.buildName + "</td><td><a href='#' data-bs-toggle='modal' data-bs-target='#chashModal' onClick='fillChashModal(\"" + version.contentHash.ToLower() + "\")'>" + version.contentHash.ToLower() + "</a></td></tr>";
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
            return System.Reflection.Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
        }

        [Route("diffFile")]
        [HttpGet]
        public string DiffFile(int fileDataID, string from, string to, bool json = false)
        {
            var html = "";
            if (CASC.Types.TryGetValue(fileDataID, out var fileType))
            {
                var textTypes = new List<string>() { "html", "htm", "lua", "json", "txt", "wtf", "toc", "xml", "xsd", "sbt" };

                if (fileType == "blp")
                {
                    html = "<ul class='nav nav-tabs' id='diffTabs' role='tablist'>";
                    html += "<li class='nav-item'>";
                    html += "<a class='nav-link active' id='sbs-tab' data-bs-toggle='tab' href='#sbs' role='tab' aria-controls='sbs' aria-selected='true'>Side-by-Side</a>";
                    html += "</li>";
                    html += "<li class='nav-item'>";
                    html += "<a class='nav-link' id='toggle-tab' data-bs-toggle='tab' href='#toggle' role='tab' aria-controls='toggle' aria-selected='false'>Switcher</a>";
                    html += "</li>";
                    //html += "<li class='nav-item'><a class='nav-link' id='imagediff-tab' data-bs-toggle='tab' href='#imagediff' role='tab' aria-controls='imagediff' aria-selected='false'>Diff</a>";
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
                else if (textTypes.Contains(fileType) || json == true)
                {
                    html = @"Note: Git is required to be installed on the system to generate text diffs<br>
    <link rel='stylesheet' type='text/css' href='/css/diff2html.min.css' />
    <script src='https://cdn.jsdelivr.net/npm/diff2html/bundles/js/diff2html.min.js'></script>
    <script src='https://cdn.jsdelivr.net/npm/diff2html/bundles/js/diff2html-ui.min.js'></script>
    <script type='text/javascript' charset='utf-8'>
        $(document).ready(function() {
            $.get('";

                    if (json)
                        html += "/casc/jsonDiff";
                    else
                        html += "/casc/diffText";

                    html += "?fileDataID=";
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

            var blp = new BLPSharp.BLPFile(file);
            var pixels = blp.GetPixels(0, out var w, out var h);
            var image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(pixels, w, h);

            var ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Position = 0;
            return new FileStreamResult(ms, "image/png");
        }

        [Route("diffText")]
        [HttpGet]
        public string DiffText(int fileDataID, string from, string to)
        {
            if (!Directory.Exists("temp/diffs/" + from))
                Directory.CreateDirectory("temp/diffs/" + from);

            var oldFile = CASC.GetFileByID((uint)fileDataID, from);
            if (oldFile == null)
                return "Error loading file " + fileDataID + " from build";

            using (var fs = new FileStream("temp/diffs/" + from + "/" + fileDataID, FileMode.Create))
            {
                oldFile.CopyTo(fs);
            }

            if (!Directory.Exists("temp/diffs/" + to))
                Directory.CreateDirectory("temp/diffs/" + to);

            var newFile = CASC.GetFileByID((uint)fileDataID, to);
            if (newFile == null)
                return "Error loading file " + fileDataID + " from build";

            using (var fs = new FileStream("temp/diffs/" + to + "/" + fileDataID, FileMode.Create))
            {
                newFile.CopyTo(fs);
            }

            try
            {
                Process p = new();
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
            Linker.existingParents.Clear();
            return "";
        }

        [Route("clearFileHistory")]
        [HttpGet]
        public string ClearFileHistory()
        {
            SQLiteDB.ClearHistory();
            CASC.VersionHistory.Clear();
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

        [Route("checkFiles")]
        [HttpPost]
        public async Task<string> CheckFiles()
        {
            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8))
                rawContent = await reader.ReadToEndAsync();

            var filenames = rawContent.Split("\n").ToList();
            var unknownFDIDs = CASC.Listfile.Where(x => x.Value == "").Select(x => x.Key).ToList();
            var reverseLookup = CASC.LookupMap.ToDictionary(x => x.Value, x => x.Key);
            var hasher = new Jenkins96();
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
        public async Task<string> CheckFilesFromFile(string file = "")
        {
            var filenames = System.IO.File.ReadAllLines(file);
            var unknownFDIDs = CASC.Listfile.Where(x => x.Value == "").Select(x => x.Key).ToList();
            var reverseLookup = CASC.LookupMap.ToDictionary(x => x.Value, x => x.Key);
            var hasher = new Jenkins96();
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
            return CASC.Listfile.Where(x => x.Value.Equals(search, StringComparison.CurrentCultureIgnoreCase)).Any();
        }

        [Route("directoryAC")]
        [HttpGet]
        public List<string> DirectoryAC(string search)
        {
            return CASC.Listfile.Values
                .Where(x => !string.IsNullOrEmpty(x) && x.StartsWith(search, StringComparison.CurrentCultureIgnoreCase) && (x.ToLower().EndsWith(".m2") || x.ToLower().EndsWith(".wmo")))
                .Select(x => x.Replace('\\', '/'))
                .DistinctBy(x => x.ToLower()).OrderByDescending(x => x).Take(20).ToList();
        }

        [Route("json")]
        [HttpGet]
        public string Json(uint fileDataID, string? build)
        {
            build ??= CASC.BuildName;

            var supportedTypes = new List<string> { "wdt", "wmo", "m2", "adt" };

            if (!(CASC.Types.TryGetValue((int)fileDataID, out var fileType) && supportedTypes.Contains(fileType)))
            {
                return "Unsupported file type or file not found";
            }

            if (!FileProvider.HasProvider(build))
            {
                if (build == CASC.BuildName)
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
                else
                {
                    var wago = new WagoFileProvider();
                    wago.SetBuild(build);
                    FileProvider.SetProvider(wago, build);
                }
            }

            FileProvider.SetDefaultBuild(build);

            switch (fileType)
            {
                case "wdt":
                    var wdtReader = new WDTReader();
                    wdtReader.LoadWDT(fileDataID);
                    return JsonConvert.SerializeObject(wdtReader.wdtfile, Formatting.Indented);
                case "wmo":
                    var wmoReader = new WMOReader();
                    var wmo = wmoReader.LoadWMO(fileDataID);
                    for (var i = 0; i < wmo.group.Length; i++)
                    {
                        wmo.group[i].mogp.indices = [];
                        wmo.group[i].mogp.vertices = [];
                        wmo.group[i].mogp.normals = [];
                        wmo.group[i].mogp.textureCoords = [];
                    }
                    return JsonConvert.SerializeObject(wmo, Formatting.Indented, new StringEnumConverter());
                case "adt":
                    var adtReader = new ADTReader();
                    adtReader.LoadADT(MPHDFlags.adt_has_height_texturing | MPHDFlags.adt_has_height_texturing, fileDataID, 0, 0, false);
                    for (var i = 0; i < adtReader.adtfile.chunks.Length; i++)
                    {
                        adtReader.adtfile.chunks[i].vertices = new WoWFormatLib.Structs.ADT.MCVT();
                        adtReader.adtfile.chunks[i].normals.normal_0 = [];
                        adtReader.adtfile.chunks[i].normals.normal_1 = [];
                        adtReader.adtfile.chunks[i].normals.normal_2 = [];
                        adtReader.adtfile.chunks[i].colors.color = [];
                    }
                    return JsonConvert.SerializeObject(adtReader.adtfile, Formatting.Indented, new StringEnumConverter());
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
                    return JsonConvert.SerializeObject(m2Reader.model, Formatting.Indented, new StringEnumConverter());
                default:
                    throw new Exception("Unsupported file type");
            }
        }

        [Route("jsonDiff")]
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
    }
}
