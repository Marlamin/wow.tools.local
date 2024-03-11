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
                var FullListfile = new Dictionary<int, string>();
                foreach (var line in System.IO.File.ReadAllLines("listfile.csv"))
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var splitLine = line.Split(";");
                    var fdid = int.Parse(splitLine[0]);

                    FullListfile.Add(fdid, splitLine[1]);
                }
                
                Namer.SetInitialListfile(ref FullListfile);
            }
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
            return string.Join('\n', Namer.GetNewFiles().OrderBy(x => x.Key).Select(x => x.Key + ";" + x.Value));
        }

        [HttpPost]
        [Route("start")]
        public string Start()
        {
            var form = Request.Form;

            var checkboxes = form["namers"];

            foreach(var selectedNamer in checkboxes)
            {
                switch(selectedNamer)
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
                            throw new FileNotFoundException("creaturecache.wdb not found");

                        Namer.NameVO(creatureCacheWDBFilename);
                        break;
                    case "WMO":
                        Namer.NameWMO();
                        break;
                    case "WWF":
                        Namer.NameWWF();
                        break;
                    default:
                        throw new Exception("Got unknown namer: " + selectedNamer);
                }
            }
            

            return string.Join('\n', Namer.GetNewFiles().OrderBy(x => x.Key).Select(x => x.Key + ";" + x.Value));
        }
    }
}
