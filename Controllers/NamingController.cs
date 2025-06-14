using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using wow.tools.local.Services;
using wow.tools.Services;
using WoWFormatLib.FileReaders;
using WoWNamingLib;

namespace wow.tools.local.Controllers
{
    [Route("naming/")]
    [ApiController]
    public class NamingController : Controller
    {
        private readonly DBCManager dbcManager;

        public NamingController(IDBCManager dbcManager, IDBDProvider dbdProvider, IDBCProvider dbcProvider)
        {
            this.dbcManager = (DBCManager)dbcManager;

            if (!Namer.isInitialized || Namer.build != CASC.BuildName)
            {
                Namer.wowDir = SettingsManager.WoWFolder;
                Namer.localProduct = SettingsManager.WoWProduct;
                Namer.build = CASC.BuildName;
                Namer.cacheDir = "caches";

                if (((DBDProvider)dbdProvider).isUsingBDBD)
                    Namer.SetProviders(dbcProvider, ((DBDProvider)dbdProvider).GetBDBDStream());
                else
                    Namer.SetProviders(dbcProvider, dbdProvider);

                if (CASC.IsCASCLibInit)
                    Namer.SetCASC(ref CASC.cascHandler!, ref CASC.AvailableFDIDs);
                else if (CASC.IsTACTSharpInit)
                    Namer.SetTACT(ref CASC.buildInstance!, ref CASC.AvailableFDIDs);

                Namer.SetGetExpansionFunction(SQLiteDB.GetFirstVersionNumberByFileDataID);
                Namer.SetSetCreatureNameForFDIDFunction(SQLiteDB.SetCreatureNameForFDID);
                Namer.SetGetCreatureNameByDisplayIDFunction(SQLiteDB.GetCreatureNameByDisplayID);
                Namer.MergeLookups(CASC.LookupMap);

                // We need to read the listfile again here for now because the WTL listfile is unaware of files not in the build.
                InitListfile();
            }
        }

        static void InitListfile()
        {
            var FullListfile = new Dictionary<int, string>();
            foreach (var line in CASC.GetListfileLines())
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var splitLine = line.Split(";");
                var fdid = int.Parse(splitLine[0]);

                FullListfile.Add(fdid, splitLine[1]);
            }

            foreach (var entry in CASC.Types)
            {
                if (FullListfile.ContainsKey(entry.Key))
                    continue;

                if (entry.Value == "m2" || entry.Value == "wmo")
                {
                    FullListfile.Add(entry.Key, "models/" + entry.Key + "." + entry.Value);
                }
                //else
                //{
                //    FullListfile.Add(entry.Key, "unknown_unk_exp/" + entry.Key + "." + entry.Value);
                //}
            }

            Namer.SetInitialListfile(ref FullListfile);
        }


        [HttpGet]
        [Route("ssDebug")]
        public string SSDebug(uint pid)
        {
            return Namer.GetSceneScriptDebug(pid);
        }

        [HttpGet]
        [Route("ssCompileDebug")]
        public string SSCompileDebug(uint pid)
        {
            var compiledPackage = Namer.GetSceneScriptCompiledDebug(pid);
            return JsonConvert.SerializeObject(compiledPackage, Formatting.Indented);
        }

        [HttpGet]
        [Route("singleFile")]
        public string SingleFile(int id, string name)
        {
            CASC.Listfile[id] = name;
            Namer.placeholderNames.Remove(id);
            Namer.ForceRename.Add((uint)id);
            Namer.AddNewFile((uint)id, name, true, true);

            if (name.ToLower().EndsWith(".m2"))
            {
                Namer.NameM2s([(uint)id], false);
                Namer.NameCreatureDisplayInfo((uint)id);

                if (
                    name.StartsWith("models") ||
                    name.StartsWith("unkmaps") ||
                    name.Contains("autogen-names") ||
                    name.Contains(id.ToString()) ||
                    name.Contains("unk_exp") ||
                    name.Contains("tileset/unused") ||
                    string.IsNullOrEmpty(name)
                )
                {
                    if (!CASC.PlaceholderFiles.Contains(id))
                        CASC.PlaceholderFiles.Add(id);
                }
                else
                {
                    CASC.PlaceholderFiles.Remove(id);
                }
            }
            else if (name.ToLower().EndsWith(".wmo"))
            {
                Namer.NameWMO((uint)id);
            }

            return string.Join('\n', Namer.GetNewFiles().OrderBy(x => x.Key).Select(x => x.Key + ";" + x.Value));
        }

        [HttpGet]
        [Route("singleVO")]
        public string SingleVO(int id, string name)
        {
            var result = Namer.NameSingleVO(id, name);

            if (!string.IsNullOrWhiteSpace(result))
                CASC.Listfile[id] = result;

            return result;
        }

        [HttpGet]
        [Route("clear")]
        public void Clear()
        {
            Namer.ClearNewFiles();
            InitListfile();
        }

        [HttpGet]
        [Route("getNewFiles")]
        public string GetNewFiles()
        {
            return string.Join('\n', Namer.GetNewFiles().OrderBy(x => x.Key).Select(x => x.Key + ";" + x.Value));
        }

        [HttpPost]
        [Route("start")]
        public string Start()
        {
            var form = Request.Form;

            var checkboxes = form["namers"];
            var overrideVO = form.ContainsKey("overrideVO") && form["overrideVO"] == "on";
            Namer.AllowCaseRenames = form.ContainsKey("allowCaseRenames") && form["allowCaseRenames"] == "on";

            var namerOrder = new List<string> { "DB2", "Map", "WMO", "M2", "Anima", "BakedNPC", "CharCust", "Collectables", "ColorGrading", "CDI", "Emotes", "FSE", "GDI", "Interface", "ItemTex", "Music", "SoundKits", "SpellTex", "TerrainCubeMaps", "VO", "WWF", "ContentHashes" };
            checkboxes = checkboxes.OrderBy(x => namerOrder.IndexOf(x!)).ToArray();

            var buildMap = new Dictionary<uint, string>();
            if (Directory.Exists(SettingsManager.ManifestFolder))
            {
                foreach (var file in Directory.GetFiles(SettingsManager.ManifestFolder, "*.txt"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var splitFilename = fileName.Split('.');
                    if (splitFilename.Length != 4)
                        continue;

                    buildMap.Add(uint.Parse(splitFilename[3]), fileName);
                }
            }

            foreach (var selectedNamer in checkboxes)
            {
                Console.WriteLine("Naming " + selectedNamer);
                switch (selectedNamer)
                {
                    case "Anima":
                        Namer.NameAnima();
                        break;
                    case "BakedNPC":
                        Namer.NameBakedNPC();
                        break;
                    case "CharCust":
                        Namer.NameCharCust();
                        break;
                    case "Collectables":
                        Namer.NameCollectable();
                        break;
                    case "ColorGrading":
                        Namer.NameColorGrading();
                        break;
                    case "CDI":
                        Namer.NameCreatureDisplayInfo();
                        break;
                    case "DB2":
                        Namer.NameDBFilesClient(SettingsManager.DefinitionDir);
                        break;
                    case "Emotes":
                        Namer.NameEmotes();
                        break;
                    case "FSE":
                        Namer.NameFullScreenEffect();
                        break;
                    case "GDI": // Not NOD
                        Namer.NameGODisplayInfo();
                        break;
                    case "Interface":
                        Namer.NameInterface();
                        break;
                    case "ItemTex":
                        Namer.NameItemTexture();
                        break;
                    case "M2":
                        var goDIDToFDID = new Dictionary<uint, uint>();
                        var goDisplayInfoDB = dbcManager.GetOrLoad("GameObjectDisplayInfo", CASC.BuildName, true).Result;
                        foreach (var goDisplayInfo in goDisplayInfoDB.Values)
                        {
                            goDIDToFDID.Add(uint.Parse(goDisplayInfo["ID"].ToString()!), uint.Parse(goDisplayInfo["FileDataID"].ToString()!));
                        }

                        var goNames = new Dictionary<uint, string>();

                        if (!string.IsNullOrWhiteSpace(SettingsManager.WoWFolder))
                        {
                            foreach (var gobjectCacheFile in Directory.GetFiles(SettingsManager.WoWFolder, "gameobjectcache.wdb", SearchOption.AllDirectories))
                            {
                                var flavorDir = new DirectoryInfo(gobjectCacheFile).Parent!.Parent!.Parent!.Parent!.Name;

                                // Don't bother with Classic or non-Flavor directories
                                if (flavorDir.Contains("classic") || !flavorDir.StartsWith('_'))
                                    continue;

                                // Skip if we don't have a build for this flavor
                                if (!CASC.AvailableBuilds.Where(x => x.Folder == flavorDir).Any())
                                    continue;

                                var productVersionByFlavor = CASC.AvailableBuilds.Where(x => x.Folder == flavorDir).First().Version;

                                if (productVersionByFlavor == null)
                                    continue;

                                Console.WriteLine("Loading " + gobjectCacheFile + " for " + productVersionByFlavor);

                                var gobjectCache = WDBReader.Read(gobjectCacheFile, productVersionByFlavor);
                                foreach (var entry in gobjectCache.entries)
                                {
                                    if (entry.Value.TryGetValue("GameObjectDisplayID", out var displayID))
                                    {
                                        var displayIDButInt = uint.Parse(displayID);
                                        if (entry.Value.TryGetValue("Name[0]", out var name) && !name.Contains(' ') && (name.Contains("10") || name.Contains("11")))
                                        {
                                            goNames.TryAdd(displayIDButInt, name);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (var gobjectCacheFile in Directory.GetFiles("caches", "gameobjectcache*", SearchOption.AllDirectories))
                        {
                            uint build = 0;

                            using (var ms = new MemoryStream(System.IO.File.ReadAllBytes(gobjectCacheFile)))
                            using (var bin = new BinaryReader(ms))
                            {
                                bin.ReadUInt32();
                                build = bin.ReadUInt32();
                            }

                            if (buildMap.TryGetValue(build, out var buildName))
                            {
                                Console.WriteLine("Loading " + gobjectCacheFile + " for " + buildName);
                                var gobjectCache = WDBReader.Read(gobjectCacheFile, buildName);
                                foreach (var entry in gobjectCache.entries)
                                {

                                    if (entry.Value.TryGetValue("GameObjectDisplayID", out var displayID))
                                    {
                                        var displayIDButInt = uint.Parse(displayID);
                                        if (entry.Value.TryGetValue("Name[0]", out var name) && !name.Contains(' ') && (name.Contains("10") || name.Contains("11")))
                                        {
                                            goNames.TryAdd(displayIDButInt, name);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No full build name found for build " + build);
                            }
                        }

                        var fdidToObjectName = new Dictionary<uint, string>();
                        foreach (var goName in goNames)
                        {
                            if (goDIDToFDID.TryGetValue(goName.Key, out var fdid))
                            {
                                var currentName = Path.GetFileNameWithoutExtension(Namer.IDToNameLookup[(int)fdid]);
                                if (currentName != goName.Value)
                                {
                                    Console.WriteLine("FDID " + fdid + " current name: " + currentName + ", official name '" + goName.Value + "'");
                                    fdidToObjectName.Add(fdid, goName.Value);
                                }
                            }
                            else
                            {
                                Console.WriteLine("GoDisplayID " + goName.Key + " with name '" + goName.Value + "' is not known in GameObjectDisplayInfo.db2, skipping..");
                            }
                        }

                        Namer.NameM2s([], true, fdidToObjectName);
                        break;
                    case "Map":
                        Namer.NameMap();
                        break;
                    case "Music":
                        Namer.NameMusic();
                        break;
                    case "SoundKits":
                        Namer.NameSound();
                        break;
                    case "SpellTex":
                        Namer.NameSpellTextures();
                        break;
                    case "TerrainCubeMaps":
                        Namer.NameTerrainMaterial();
                        break;
                    case "VO":
                        if (string.IsNullOrEmpty(SettingsManager.WoWFolder))
                            break;

                        try
                        {
                            var creatureXDIDB = dbcManager.GetOrLoad("CreatureXDisplayInfo", CASC.BuildName).Result;
                            if(creatureXDIDB.AvailableColumns.Contains("CreatureDisplayInfoID") && creatureXDIDB.AvailableColumns.Contains("CreatureID"))
                            {
                                foreach (var creatureXDisplayInfo in creatureXDIDB.Values)
                                {
                                    SQLiteDB.InsertOrUpdateDisplayIDToCreatureID(int.Parse(creatureXDisplayInfo["CreatureDisplayInfoID"].ToString()!), int.Parse(creatureXDisplayInfo["CreatureID"].ToString()!));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error loading CreatureXDisplayInfo: " + e.Message);
                        }

                        var currentCreatureNames = SQLiteDB.GetCreatureNames();

                        if (!string.IsNullOrEmpty(SettingsManager.WoWFolder))
                        {
                            foreach (var creatureCacheFile in Directory.GetFiles(SettingsManager.WoWFolder, "creaturecache.wdb", SearchOption.AllDirectories))
                            {
                                var flavorDir = new DirectoryInfo(creatureCacheFile).Parent!.Parent!.Parent!.Parent!.Name;

                                // Don't bother with Classic or non-Flavor directories
                                if (flavorDir.Contains("classic") || !flavorDir.StartsWith('_'))
                                    continue;

                                // Skip if we don't have a build for this flavor
                                if (!CASC.AvailableBuilds.Where(x => x.Folder == flavorDir).Any())
                                    continue;

                                var productVersionByFlavor = CASC.AvailableBuilds.Where(x => x.Folder == flavorDir).First().Version;

                                if (productVersionByFlavor == null)
                                    continue;

                                Console.WriteLine("Loading " + creatureCacheFile + " for " + productVersionByFlavor);

                                var creatureCache = WDBReader.Read(creatureCacheFile, productVersionByFlavor);
                                foreach (var entry in creatureCache.entries)
                                {
                                    if (!currentCreatureNames.TryGetValue(entry.Key, out var currentCreatureName))
                                    {
                                        Console.WriteLine("Discovered new creature: " + entry.Value["Name[0]"] + " (" + entry.Key + ")");
                                        SQLiteDB.InsertOrUpdateCreature((int)entry.Key, entry.Value["Name[0]"], creatureCache.buildInfo.build);
                                        currentCreatureNames[entry.Key] = entry.Value["Name[0]"];
                                    }
                                    else if (currentCreatureName != entry.Value["Name[0]"] && creatureCache.clientBuild > SQLiteDB.creatureCache[(int)entry.Key])
                                    {
                                        Console.WriteLine("Updating creature name: " + currentCreatureName + " => " + entry.Value["Name[0]"] + " (" + entry.Key + ")");
                                        SQLiteDB.InsertOrUpdateCreature((int)entry.Key, entry.Value["Name[0]"], creatureCache.buildInfo.build);
                                        currentCreatureNames[entry.Key] = entry.Value["Name[0]"];
                                    }

                                    for (var i = 0; i < int.Parse(entry.Value["NumCreatureDisplays"]); i++)
                                    {
                                        SQLiteDB.InsertOrUpdateDisplayIDToCreatureID(int.Parse(entry.Value["CreatureDisplayInfoID[" + i + "]"]), (int)entry.Key);
                                    }
                                }
                            }
                        }

                        foreach (var creatureCacheFile in Directory.GetFiles("caches", "creaturecache*", SearchOption.AllDirectories))
                        {
                            uint build = 0;

                            using (var ms = new MemoryStream(System.IO.File.ReadAllBytes(creatureCacheFile)))
                            using (var bin = new BinaryReader(ms))
                            {
                                bin.ReadUInt32();
                                build = bin.ReadUInt32();
                            }

                            if (buildMap.TryGetValue(build, out var buildName))
                            {
                                Console.WriteLine("Loading " + creatureCacheFile + " for " + buildName);
                                var creatureCache = WDBReader.Read(creatureCacheFile, buildName);
                                foreach (var entry in creatureCache.entries)
                                {
                                    if (!currentCreatureNames.TryGetValue(entry.Key, out var currentCreatureName))
                                    {
                                        Console.WriteLine("Discovered new creature: " + entry.Value["Name[0]"] + " (" + entry.Key + ")");
                                        SQLiteDB.InsertOrUpdateCreature((int)entry.Key, entry.Value["Name[0]"], creatureCache.buildInfo.build);
                                        currentCreatureNames[entry.Key] = entry.Value["Name[0]"];
                                    }
                                    else if (currentCreatureName != entry.Value["Name[0]"] && creatureCache.clientBuild > SQLiteDB.creatureCache[(int)entry.Key])
                                    {
                                        Console.WriteLine("Updating creature name: " + currentCreatureName + " => " + entry.Value["Name[0]"] + " (" + entry.Key + ")");
                                        SQLiteDB.InsertOrUpdateCreature((int)entry.Key, entry.Value["Name[0]"], creatureCache.buildInfo.build);
                                        currentCreatureNames[entry.Key] = entry.Value["Name[0]"];
                                    }

                                    for (var i = 0; i < int.Parse(entry.Value["NumCreatureDisplays"]); i++)
                                    {
                                        SQLiteDB.InsertOrUpdateDisplayIDToCreatureID(int.Parse(entry.Value["CreatureDisplayInfoID[" + i + "]"]), (int)entry.Key);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No full build name found for build " + build);
                            }
                        }

                        if (!string.IsNullOrEmpty(form["creatureCacheWDBFilename"]) && System.IO.File.Exists(form["creatureCacheWDBFilename"]))
                        {
                            var creatureCache = WDBReader.Read(form["creatureCacheWDBFilename"], CASC.BuildName);
                            foreach (var entry in creatureCache.entries)
                            {
                                if (!currentCreatureNames.TryGetValue(entry.Key, out var currentCreatureName))
                                {
                                    Console.WriteLine("Discovered new creature: " + entry.Value["Name[0]"] + " (" + entry.Key + ")");
                                    SQLiteDB.InsertOrUpdateCreature((int)entry.Key, entry.Value["Name[0]"], creatureCache.buildInfo.build);
                                    currentCreatureNames[entry.Key] = entry.Value["Name[0]"];
                                }
                                else if (currentCreatureName != entry.Value["Name[0]"] && creatureCache.clientBuild > SQLiteDB.creatureCache[(int)entry.Key])
                                {
                                    Console.WriteLine("Updating creature name: " + currentCreatureName + " => " + entry.Value["Name[0]"] + " (" + entry.Key + ")");
                                    SQLiteDB.InsertOrUpdateCreature((int)entry.Key, entry.Value["Name[0]"], creatureCache.buildInfo.build);
                                    currentCreatureNames[entry.Key] = entry.Value["Name[0]"];
                                }

                                for (var i = 0; i < int.Parse(entry.Value["NumCreatureDisplays"]); i++)
                                {
                                    SQLiteDB.InsertOrUpdateDisplayIDToCreatureID(int.Parse(entry.Value["CreatureDisplayInfoID[" + i + "]"]), (int)entry.Key);
                                }
                            }
                        }

                        foreach (var buildDir in Directory.GetDirectories(SettingsManager.DBCFolder))
                        {
                            if (!System.IO.File.Exists(Path.Combine(buildDir, "dbfilesclient", "BroadcastText.db2")))
                                continue;

                            var buildName = Path.GetFileName(buildDir);

                            // Skip expansions lower than DF
                            if (short.Parse(buildName.Split(".")[0]) < 10)
                                continue;

                            try
                            {
                                var broadcastTextDB = dbcManager.GetOrLoad("BroadcastText", buildName, true).Result;

                                if(broadcastTextDB.AvailableColumns.Contains("Text_lang") && broadcastTextDB.AvailableColumns.Contains("Text1_lang") && broadcastTextDB.AvailableColumns.Contains("SoundKitID"))
                                {
                                    foreach (var broadcastText in broadcastTextDB.Values)
                                    {
                                        var soundKits = (uint[])broadcastText["SoundKitID"];
                                        if (soundKits.Length > 0)
                                        {
                                            SQLiteDB.InsertOrUpdateBroadcastText(int.Parse(broadcastText["ID"].ToString()!), broadcastText["Text_lang"].ToString()!, broadcastText["Text1_lang"].ToString()!, (int)soundKits[0], (int)soundKits[1], int.Parse(buildName.Split(".")[3]));
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error loading BroadcastText for build " + buildDir + ": " + e.Message);
                            }
                        }

                        Namer.NameVO(SQLiteDB.GetCreatureNames(), SQLiteDB.GetTextToSoundKitIDs(), SQLiteDB.GetCreatureToFDIDMap(), SQLiteDB.GetBroadcastTextIDToSoundKitIDs(), overrideVO);
                        break;
                    case "WMO":
                        Namer.NameWMO();
                        break;
                    case "WWF":
                        Namer.NameWWF();
                        break;
                    case "ContentHashes":
                        CASC.EnsureCHashesLoaded();
                        Namer.NameByContentHashes(CASC.FDIDToCHash);
                        break;
                    default:
                        throw new Exception("Got unknown namer: " + selectedNamer);
                }
                Console.WriteLine("Finished naming " + selectedNamer);
            }

            return string.Join('\n', Namer.GetNewFiles().OrderBy(x => x.Key).Select(x => x.Key + ";" + x.Value));
        }
    }
}
