using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local.Controllers
{
    [Route("map/")]
    public class MapController : Controller
    {
        private readonly DBDProvider dbdProvider;
        private readonly DBCManager dbcManager;
        private Dictionary<string, List<int>> mapMaskCache = new();

        public struct MapInfo
        {
            public string ID;
            public string internalName;
            public string displayName;
            public uint wdtFileDataID;
        }

        public MapController(IDBDProvider dbdProvider, IDBCManager dbcManager)
        {
            this.dbdProvider = dbdProvider as DBDProvider;
            this.dbcManager = dbcManager as DBCManager;
        }

        [Route("list")]
        [HttpGet]
        public async Task<List<MapInfo>> Info()
        {
            var list = new List<MapInfo>();
            var seenMaps = new HashSet<string>();

            var mapDB = await dbcManager.GetOrLoad("Map", CASC.BuildName);

            if(!mapDB.AvailableColumns.Contains("Directory") || !mapDB.AvailableColumns.Contains("MapName_lang") || !mapDB.AvailableColumns.Contains("WdtFileDataID"))
                throw new Exception("Unable to initialize map list, missing one of the required columns.");

            foreach(var entry in mapDB.Values)
            {
                list.Add(new MapInfo()
                {
                    ID = entry["ID"].ToString(),
                    internalName = entry["Directory"].ToString(),
                    displayName = entry["MapName_lang"].ToString(),
                    wdtFileDataID = uint.Parse(entry["WdtFileDataID"].ToString())
                });

                seenMaps.Add(entry["Directory"].ToString().ToLower());
            }

            var allMinimaps = CASC.Listfile.Values.Where(x => x.ToLower().StartsWith("world/minimaps") && !x.ToLower().StartsWith("world/minimaps/wmo")).Select(x => Path.GetDirectoryName(x).Replace("world\\minimaps\\", "")).Distinct();
            Console.WriteLine();
            foreach(var minimap in allMinimaps)
            {
                if (seenMaps.Contains(minimap) || minimap.Contains("\\"))
                    continue;

                list.Add(new MapInfo()
                {
                    ID = minimap,
                    internalName = minimap,
                    displayName = minimap,
                    wdtFileDataID = 0
                });
            }

            return list;
        }

        [Route("wdtMask")]
        [HttpGet]
        public List<int> GetWDTMask(string mapID, string directory, uint wdtFileDataID)
        {
          //  if (mapMaskCache.ContainsKey(mapID))
          //      return mapMaskCache[mapID];

            var mask = new List<int>();
            var allMinimaps = CASC.Listfile.Where(x => x.Value.ToLower().StartsWith("world/minimaps/" + directory.ToLower())).ToDictionary(x => x.Value, x => x.Key);

            if (wdtFileDataID == 0)
            {
                // No shipped WDT, fall back to listfile-based minimap detection.
                for (byte x = 0; x < 64; x++)
                {
                    for (byte y = 0; y < 64; y++)
                    {
                        if (allMinimaps.TryGetValue("world/minimaps/" + directory + "/map" + y.ToString().PadLeft(2, '0') + "_" + x.ToString().PadLeft(2, '0') + ".blp", out var fdid))
                            mask.Add(fdid);
                        else
                            mask.Add(0);
                    }
                }
                
                return mask;
            }

            var wdtName = CASC.Listfile.Where(x => x.Value.EndsWith(".wdt") && x.Value.StartsWith(mapID.ToString())).FirstOrDefault().Value;

            var wdt = CASC.GetFileByID(wdtFileDataID);

            if (wdt == null)
                return mask;

            using (var bin = new BinaryReader(wdt))
            {
                long position = 0;
                while (position < wdt.Length)
                {
                    wdt.Position = position;

                    var chunkName = bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();

                    position = wdt.Position + chunkSize;

                    switch (chunkName)
                    {
                        case 'M' << 24 | 'A' << 16 | 'I' << 8 | 'D' << 0:
                            for (byte x = 0; x < 64; x++)
                            {
                                for (byte y = 0; y < 64; y++)
                                {
                                    bin.ReadBytes(28);
                                    var minimapFDID = bin.ReadUInt32();

                                    if (minimapFDID != 0)
                                    {
                                        mask.Add((int)minimapFDID);
                                    }
                                    else
                                    {
                                        var minimapName = "world/minimaps/" + directory.ToLower() + "/map" + y.ToString().PadLeft(2, '0') + "_" + x.ToString().PadLeft(2, '0') + ".blp";
                                        if (allMinimaps.TryGetValue(minimapName, out var fdid))
                                            mask.Add(fdid);
                                        else
                                            mask.Add(0);
                                    }
                                }
                            }
                            break;
                        default:
                            Console.WriteLine(string.Format("Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkName.ToString("X"), position.ToString()));
                            break;
                    }
                }
            }
            mapMaskCache.Add(mapID, mask);
            return mask;
        }
    }
}
