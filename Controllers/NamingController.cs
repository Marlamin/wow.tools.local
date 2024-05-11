using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;
using wow.tools.Services;
using WoWNamingLib;

namespace wow.tools.local.Controllers
{
    [Route("naming/")]
    [ApiController]
    public class NamingController : Controller
    {
        private readonly DBCManager dbcManager;

        public NamingController(IDBCManager dbcManager)
        {
            this.dbcManager = (DBCManager)dbcManager;

            if (!Namer.isInitialized)
            {
                Namer.wowDir = SettingsManager.wowFolder;
                Namer.localProduct = SettingsManager.wowProduct;
                Namer.build = CASC.BuildName;

                Namer.SetProviders(new DBCProvider(), new DBDProvider());
                Namer.SetCASC(ref CASC.cascHandler, ref CASC.AvailableFDIDs);

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
        [Route("singleFile")]
        public string SingleFile(int id, string name)
        {
            CASC.Listfile[id] = name;
            Namer.placeholderNames.Remove(id);
            Namer.ForceRename.Add((uint)id);
            Namer.AddNewFile((uint)id, name, true, true);
            Namer.NameM2s([(uint)id], false);
            Namer.NameCreatureDisplayInfo((uint)id);
            return string.Join('\n', Namer.GetNewFiles().OrderBy(x => x.Key).Select(x => x.Key + ";" + x.Value));
        }

        [HttpGet]
        [Route("clear")]
        public void Clear()
        {
            Namer.ClearNewFiles();
        }

        [HttpPost]
        [Route("start")]
        public string Start()
        {
            var form = Request.Form;

            var checkboxes = form["namers"];

            var namerOrder = new List<string> { "DB2", "Map", "WMO", "M2", "Anima", "BakedNPC", "CharCust", "Collectables", "ColorGrading", "CDI", "Emotes", "FSE", "GDI", "Interface", "ItemTex", "Music", "SoundKits", "SpellTex", "TerrainCubeMaps", "VO", "WWF", "ContentHashes" };
            checkboxes = checkboxes.OrderBy(x => namerOrder.IndexOf(x)).ToArray();

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
                        Namer.NameDBFilesClient(SettingsManager.definitionDir);
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
                        Namer.NameM2s([], true);
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
                        if (string.IsNullOrEmpty(SettingsManager.wowFolder))
                            break;

                        var creatureCacheWDBFilename = Path.Combine(SettingsManager.wowFolder, "_retail_", "Cache", "WDB", "enUS", "creaturecache.wdb");

                        if (!string.IsNullOrEmpty(form["creatureCacheWDBFilename"]))
                            creatureCacheWDBFilename = form["creatureCacheWDBFilename"];

                        if (!System.IO.File.Exists(creatureCacheWDBFilename))
                            break;

                        var textToSoundKitID = new Dictionary<string, List<uint>>();

                        foreach (var buildDir in Directory.GetDirectories(SettingsManager.dbcFolder))
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

                                foreach (var broadcastText in broadcastTextDB.Values)
                                {
                                    var soundKits = (uint[])broadcastText["SoundKitID"];

                                    if (!string.IsNullOrEmpty(broadcastText["Text_lang"].ToString()))
                                    {
                                        if (soundKits[0] != 0)
                                        {
                                            if (!textToSoundKitID.ContainsKey(broadcastText["Text_lang"].ToString()))
                                                textToSoundKitID.Add(broadcastText["Text_lang"].ToString(), new List<uint>());

                                            textToSoundKitID[broadcastText["Text_lang"].ToString()].Add(soundKits[0]);
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(broadcastText["Text1_lang"].ToString()))
                                    {
                                        if (soundKits[1] != 0)
                                        {
                                            if (!textToSoundKitID.ContainsKey(broadcastText["Text1_lang"].ToString()))
                                                textToSoundKitID.Add(broadcastText["Text1_lang"].ToString(), new List<uint>());

                                            textToSoundKitID[broadcastText["Text1_lang"].ToString()].Add(soundKits[1]);
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error loading BroadcastText for build " + buildDir + ": " + e.Message);
                            }
                        }

                        Namer.NameVO(creatureCacheWDBFilename, textToSoundKitID);
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
