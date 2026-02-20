using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("map/")]
    public class MapController(IDBCManager dbcManager) : Controller
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;
        private readonly Dictionary<(string, int), List<int>> mapMaskCache = new Dictionary<(string, int), List<int>>();

        public struct MapInfo
        {
            public string ID;
            public string internalName;
            public string displayName;
            public uint wdtFileDataID;
        }

        [Route("tile")]
        [HttpGet]
        public FileContentResult Tile(uint fileDataID, int targetSize)
        {
            if (!CASC.FileExists(fileDataID))
            {
                var emptyImage = new Image<Rgba32>(targetSize, targetSize);
                var emptyPixels = new byte[targetSize * targetSize * 4];
                emptyImage.CopyPixelDataTo(emptyPixels);
                return new FileContentResult(emptyPixels, "application/octet-stream");
            }

            var blp = new BLPSharp.BLPFile(CASC.GetFileByID(fileDataID));

            int bestMipLevel = 0;
            for (int i = 0; i < blp.MipMapCount; i++)
            {
                var mipPixels = blp.GetPixels(i, out var mipW, out var mipH);
                if (mipW >= targetSize && mipH >= targetSize)
                    bestMipLevel = i;
                else
                    break;
            }

            var pixels = blp.GetPixels(bestMipLevel, out var w, out var h);
            var image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(pixels, w, h);
            var sortedImage = image.CloneAs<Rgba32>();
            var pixelBytes = new byte[targetSize * targetSize * 4];

            if (sortedImage.Width == targetSize)
            {
                sortedImage.CopyPixelDataTo(pixelBytes);
            }
            else
            {
                // Minimaps dont have mipmaps, so resize :( (maptextures do, so we support mips above anyways)
                sortedImage.Mutate(x => x.Resize(new Size(targetSize, targetSize)));
                sortedImage.CopyPixelDataTo(pixelBytes);
            }

            return new FileContentResult(pixelBytes, "application/octet-stream");
        }

        [Route("list")]
        [HttpGet]
        public async Task<List<MapInfo>> Info()
        {
            var list = new List<MapInfo>();
            var seenMaps = new HashSet<string>();

            var mapDB = await dbcManager.GetOrLoad("Map", CASC.BuildName);

            if (!mapDB.AvailableColumns.Contains("ID") || !mapDB.AvailableColumns.Contains("Directory") || !mapDB.AvailableColumns.Contains("MapName_lang"))
                throw new Exception("Unable to initialize map list, missing one of the required columns.");

            foreach (var entry in mapDB.Values)
            {
                uint wdtFileDataID = 0;
                if (mapDB.AvailableColumns.Contains("WdtFileDataID"))
                    wdtFileDataID = uint.Parse(entry["WdtFileDataID"].ToString()!);
                else
                    wdtFileDataID = CASC.GetFileDataIDByName("world/maps/" + entry["Directory"].ToString()!.ToLower() + "/" + entry["Directory"].ToString()!.ToLower() + ".wdt");

                if(!CASC.FileExists(wdtFileDataID))
                    continue;

                list.Add(new MapInfo()
                {
                    ID = entry["ID"].ToString()!,
                    internalName = entry["Directory"].ToString()!,
                    displayName = entry["MapName_lang"].ToString()!,
                    wdtFileDataID = wdtFileDataID
                });

                seenMaps.Add(entry["Directory"].ToString()!.ToLower());
            }

            var allMinimaps = Listfile.NameMap.Where(x => x.Value.StartsWith("world/minimaps", StringComparison.CurrentCultureIgnoreCase) && !x.Value.StartsWith("world/minimaps/wmo", StringComparison.CurrentCultureIgnoreCase)).ToDictionary(x => x.Key, x => x.Value);
            foreach (var minimapFile in allMinimaps)
            {
                if(!CASC.FileExists((uint)minimapFile.Key))
                    continue;

                var mapName = Path.GetDirectoryName(minimapFile.Value)!.Replace("world\\minimaps\\", "");
                if (seenMaps.Contains(mapName) || mapName.Contains('\\'))
                    continue;

                list.Add(new MapInfo()
                {
                    ID = mapName,
                    internalName = mapName,
                    displayName = mapName,
                    wdtFileDataID = 0
                });

                seenMaps.Add(mapName);
            }

            return list;
        }

        [Route("wdtMask")]
        [HttpGet]
        public List<int> GetWDTMask(string mapID, string directory, uint wdtFileDataID, byte layer = 0)
        {
            if (mapMaskCache.ContainsKey((mapID, layer)))
                return mapMaskCache[(mapID, layer)];

            var mask = new List<int>();
            Dictionary<string, int> allFiles;

            if (layer == 0)
                allFiles = Listfile.NameMap.Where(x => x.Value.StartsWith("world/minimaps/" + directory.ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToDictionary(x => x.Value, x => x.Key);
            else if (layer == 1)
                allFiles = Listfile.NameMap.Where(x => x.Value.StartsWith("world/maptextures/" + directory.ToLower(), StringComparison.CurrentCultureIgnoreCase) && !x.Value.EndsWith("_n.blp", StringComparison.CurrentCultureIgnoreCase)).ToDictionary(x => x.Value, x => x.Key);
            else if (layer == 2)
                allFiles = Listfile.NameMap.Where(x => x.Value.StartsWith("world/maptextures/" + directory.ToLower(), StringComparison.CurrentCultureIgnoreCase) && x.Value.EndsWith("_n.blp", StringComparison.CurrentCultureIgnoreCase)).ToDictionary(x => x.Value, x => x.Key);
            else
                throw new Exception("Unknown layer type");

            if (wdtFileDataID == 0)
            {
                // No shipped WDT, fall back to listfile-based minimap detection.
                for (byte x = 0; x < 64; x++)
                {
                    for (byte y = 0; y < 64; y++)
                    {
                        if (layer == 0)
                        {
                            if (allFiles.TryGetValue("world/minimaps/" + directory + "/map" + y.ToString().PadLeft(2, '0') + "_" + x.ToString().PadLeft(2, '0') + ".blp", out var fdid))
                                mask.Add(fdid);
                            else
                                mask.Add(0);
                        }
                        else if (layer == 1)
                        {
                            if (allFiles.TryGetValue("world/maptextures/" + directory + "/" + directory + "_" + y.ToString().PadLeft(2, '0') + "_" + x.ToString().PadLeft(2, '0') + ".blp", out var fdid))
                                mask.Add(fdid);
                            else
                                mask.Add(0);

                        }
                        else if (layer == 2)
                        {
                            if (allFiles.TryGetValue("world/maptextures/" + directory + "/" + directory + "_" + y.ToString().PadLeft(2, '0') + "_" + x.ToString().PadLeft(2, '0') + "_n.blp", out var fdid))
                                mask.Add(fdid);
                            else
                                mask.Add(0);
                        }
                    }
                }

                return mask;
            }

            var wdtName = Listfile.NameMap.Where(x => x.Value.EndsWith(".wdt") && x.Value.StartsWith(mapID.ToString())).FirstOrDefault().Value;

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
                                    bin.ReadBytes(20);
                                    var mapTextureFDID = bin.ReadUInt32();
                                    var mapTextureNFDID = bin.ReadUInt32();
                                    var minimapFDID = bin.ReadUInt32();

                                    if (layer == 0)
                                    {
                                        if (minimapFDID != 0)
                                        {
                                            mask.Add((int)minimapFDID);
                                        }
                                        else
                                        {
                                            var minimapName = "world/minimaps/" + directory.ToLower() + "/map" + y.ToString().PadLeft(2, '0') + "_" + x.ToString().PadLeft(2, '0') + ".blp";
                                            if (allFiles.TryGetValue(minimapName, out var fdid))
                                                mask.Add(fdid);
                                            else
                                                mask.Add(0);
                                        }
                                    }
                                    else if (layer == 1)
                                    {
                                        if (mapTextureFDID != 0)
                                        {
                                            mask.Add((int)mapTextureFDID);
                                        }
                                        else
                                        {
                                            var mapTextureName = "world/maptextures/" + directory.ToLower() + "/" + directory.ToLower() + "_" + y.ToString().PadLeft(2, '0') + "_" + x.ToString().PadLeft(2, '0') + ".blp";
                                            if (allFiles.TryGetValue(mapTextureName, out var fdid))
                                                mask.Add(fdid);
                                            else
                                                mask.Add(0);
                                        }
                                    }
                                    else if (layer == 2)
                                    {
                                        if (mapTextureNFDID != 0)
                                        {
                                            mask.Add((int)mapTextureNFDID);
                                        }
                                        else
                                        {
                                            var mapTextureNName = "world/maptextures/" + directory.ToLower() + "/" + directory.ToLower() + "_" + y.ToString().PadLeft(2, '0') + "_" + x.ToString().PadLeft(2, '0') + "_n.blp";
                                            if (allFiles.TryGetValue(mapTextureNName, out var fdid))
                                                mask.Add(fdid);
                                            else
                                                mask.Add(0);
                                        }
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
            mapMaskCache.Add((mapID, layer), mask);
            return mask;
        }
    }
}
