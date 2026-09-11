using BLPSharp;
using Microsoft.AspNetCore.Mvc;
using NetVips;
using System.Text.Json;
using wow.tools.local.Managers;
using wow.tools.local.Services;
using WoWFormatLib.Structs.ADT;

namespace wow.tools.local.Controllers
{
    [Route("map/")]
    public class MapController(IDBCManager dbcManager) : Controller
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

        private readonly Dictionary<string, List<MapTile>> puzzleMapMaskCache = new Dictionary<string, List<MapTile>>();
        private readonly Dictionary<(string, int), List<int>> mapMaskCache = new Dictionary<(string, int), List<int>>();
        public struct MapInfo
        {
            public string ID;
            public string internalName;
            public string displayName;
            public uint wdtFileDataID;
        }

        public struct MapTile
        {
            public byte x;
            public byte y;

            public uint rootADT;
            public uint minimapTexture;
            public uint mapTexture;
            public uint mapTextureN;
        }

        [Route("clearCache")]
        [HttpGet]
        public ActionResult ClearCache()
        {
            puzzleMapMaskCache.Clear();
            mapMaskCache.Clear();
            return Ok();
        }

        private static NetVips.Image GetADTColorImage(uint fileDataID, int targetSize, string adtMethod = "")
        {
            if (string.IsNullOrEmpty(adtMethod))
                adtMethod = "mccv";

            var adt = CASC.GetFileByID(fileDataID)!;

            var outputImageSize = 128;
            var imageBytes = new byte[outputImageSize * outputImageSize * 4];

            // choosing not to use wowformatlib's full reader here for faster reading
            using (var bin = new BinaryReader(adt))
            {
                var mcnkI = 0;

                while (adt.Position < adt.Length)
                {
                    var chunkName = (ADTChunks)bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();

                    switch (chunkName)
                    {
                        case ADTChunks.MCNK:
                            var mcnkData = bin.ReadBytes((int)chunkSize);
                            using (var mcnkMS = new MemoryStream(mcnkData))
                            using (var mcnkBin = new BinaryReader(mcnkMS))
                            {
                                mcnkBin.ReadBytes(128); // mcnk header
                                while (mcnkMS.Position < mcnkMS.Length)
                                {
                                    var subChunkName = (ADTChunks)mcnkBin.ReadUInt32();
                                    var subChunkSize = mcnkBin.ReadUInt32();

                                    switch (subChunkName)
                                    {
                                        case ADTChunks.MCCV: // layer 3
                                            if (adtMethod == "mccv")
                                            {
                                                // calculate the mcnk row and col based on the index, 16x16 
                                                var mcnkRow = mcnkI / 16;
                                                var mcnkCol = mcnkI % 16;

                                                var mcnkPixelSize = outputImageSize / 16; // 8px per mcnk (1px per inner vertex)

                                                var mcnkXStart = mcnkCol * mcnkPixelSize;
                                                var mcnkyStart = mcnkRow * mcnkPixelSize;

                                                for (var i = 0; i < 17; i++)
                                                {
                                                    var isInnerVertice = (i % 2) != 0; // see mcvt on wiki
                                                    var columns = isInnerVertice ? 8 : 9;

                                                    // only care about inner vertice for MAXIMUM SPEED
                                                    if (!isInnerVertice)
                                                    {
                                                        mcnkBin.BaseStream.Position += columns * 4;
                                                        continue;
                                                    }

                                                    var innerRow = i / 2;
                                                    var pixelY = mcnkyStart + innerRow;

                                                    for (var j = 0; j < columns; j++)
                                                    {
                                                        var r = mcnkBin.ReadByte();
                                                        var g = mcnkBin.ReadByte();
                                                        var b = mcnkBin.ReadByte();
                                                        //var a = mcnkBin.ReadByte();
                                                        mcnkBin.BaseStream.Position += 1; // skip alpha

                                                        var pixelX = mcnkXStart + j;
                                                        var pixelByteOffset = ((pixelY * outputImageSize) + pixelX) * 4;
                                                        imageBytes[pixelByteOffset + 0] = b;
                                                        imageBytes[pixelByteOffset + 1] = g;
                                                        imageBytes[pixelByteOffset + 2] = r;
                                                        imageBytes[pixelByteOffset + 3] = 255; // idk if this is ever relevant
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                mcnkMS.Position += subChunkSize;
                                            }
                                            break;
                                        case ADTChunks.MCVT: // TODO: Heightmap for layer 4
                                        default:
                                            mcnkMS.Position += subChunkSize;
                                            break;
                                    }
                                }
                            }

                            mcnkI++;
                            break;
                        default:
                            adt.Position += chunkSize;
                            break;
                    }
                }
            }

            var adtImage = NetVips.Image.NewFromMemory(imageBytes, outputImageSize, outputImageSize, 4, Enums.BandFormat.Uchar);

            if (adtImage.Width != targetSize)
                adtImage = adtImage.Resize((double)targetSize / adtImage.Width);

            return adtImage;
        }

        [Route("tile")]
        [HttpGet]
        public FileStreamResult Tile(uint fileDataID, int targetSize, string adtMethod = "", string output = "raw")
        {
            if (!CASC.FileExists(fileDataID))
            {
                var emptyImage = Image.Black(targetSize, targetSize);
                var emptyPixels = new byte[targetSize * targetSize * 4];
                var emptyMS = new MemoryStream();
                emptyImage.WriteToStream(emptyMS, ".png");
                emptyMS.Position = 0;
                return new FileStreamResult(emptyMS, "image/png");
            }

            var type = "";
            if (!Listfile.Types.TryGetValue((int)fileDataID, out type))
                type = "blp"; // assume blp if unknown/unnamed

            if (type == "adt")
            {
                var adtImage = GetADTColorImage(fileDataID, targetSize, adtMethod);

                if (output == "raw")
                {
                    byte[] adtRawPixels = adtImage.WriteToMemory<byte>();
                    var adtms = new MemoryStream();
                    adtms.Write(adtRawPixels);
                    adtms.Position = 0;
                    return new FileStreamResult(adtms, "application/octet-stream");
                }
                else
                {
                    var adtms = new MemoryStream();
                    adtImage.WriteToStream(adtms, ".png");
                    adtms.Position = 0;
                    return new FileStreamResult(adtms, "image/png");
                }
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

            // TODO: I would prefer to ditch ImageSharp here
            var pixels = blp.GetPixels(bestMipLevel, out var w, out var h);
            using var raw = NetVips.Image.NewFromMemory(pixels, w, h, 4, Enums.BandFormat.Uchar);
            using var image = raw[2].Bandjoin(new[] { raw[1], raw[0], raw[3] }).Copy(interpretation: Enums.Interpretation.Srgb);

            var sortedImage = image.Copy();

            if (sortedImage.Width != targetSize)
                sortedImage = sortedImage.Resize((double)targetSize / sortedImage.Width);

            byte[] rawPixels = sortedImage.WriteToMemory<byte>();

            var ms = new MemoryStream();
            ms.Write(rawPixels);
            ms.Position = 0;
            return new FileStreamResult(ms, "application/octet-stream");
        }

        [Route("list")]
        [HttpGet]
        public async Task<ActionResult> Info()
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

                if (!CASC.FileExists(wdtFileDataID))
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
                if (!CASC.FileExists((uint)minimapFile.Key))
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

            var jsonOptions = new JsonSerializerOptions
            {
                IncludeFields = true
            };

            return Json(list, jsonOptions);
        }

        public List<int> CacheMask(string mapID, string directory, uint wdtFileDataID, byte layer = 0)
        {
            if (mapMaskCache.TryGetValue((mapID, layer), out var mask))
                return mask;

            mask = new List<int>();
            Dictionary<string, int> allFiles;

            if (layer == 0) // minimaps
                allFiles = Listfile.NameMap.Where(x => x.Value.StartsWith("world/minimaps/" + directory.ToLower(), StringComparison.OrdinalIgnoreCase)).ToDictionary(x => x.Value.ToLowerInvariant(), x => x.Key);
            else if (layer == 1) // maptextures
                allFiles = Listfile.NameMap.Where(x => x.Value.StartsWith("world/maptextures/" + directory.ToLower(), StringComparison.OrdinalIgnoreCase) && !x.Value.EndsWith("_n.blp", StringComparison.OrdinalIgnoreCase)).ToDictionary(x => x.Value.ToLowerInvariant(), x => x.Key);
            else if (layer == 2) // maptexture normals
                allFiles = Listfile.NameMap.Where(x => x.Value.StartsWith("world/maptextures/" + directory.ToLower(), StringComparison.OrdinalIgnoreCase) && x.Value.EndsWith("_n.blp", StringComparison.OrdinalIgnoreCase)).ToDictionary(x => x.Value.ToLowerInvariant(), x => x.Key);
            else if (layer == 3) // adt vertex colors
                allFiles = Listfile.NameMap.Where(x => x.Value.StartsWith("world/maps/" + directory.ToLower(), StringComparison.OrdinalIgnoreCase) && x.Value.EndsWith(".adt", StringComparison.OrdinalIgnoreCase) && !x.Value.EndsWith("_lod.adt", StringComparison.OrdinalIgnoreCase) && !x.Value.EndsWith("_obj0.adt", StringComparison.OrdinalIgnoreCase) && !x.Value.EndsWith("_obj1.adt", StringComparison.OrdinalIgnoreCase) && !x.Value.EndsWith("_tex0.adt", StringComparison.OrdinalIgnoreCase)).ToDictionary(x => x.Value.ToLowerInvariant(), x => x.Key);
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
                        else if (layer == 3 || layer == 4)
                        {
                            if (allFiles.TryGetValue("world/maps/" + directory + "/" + directory + "_" + y.ToString() + "_" + x.ToString() + ".adt", out var fdid))
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
                                    var rootADT = bin.ReadUInt32();
                                    bin.ReadBytes(16);
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
                                    else if (layer == 3 || layer == 4) // layer 3 is vertex colors, layer 4 will be heightmap
                                    {
                                        if (rootADT != 0)
                                        {
                                            mask.Add((int)rootADT);
                                        }
                                        else
                                        {
                                            var adtName = "world/maps/" + directory.ToLower() + "/" + directory.ToLower() + "_" + y.ToString() + "_" + x.ToString() + ".adt"; // no padding on adts
                                            if (allFiles.TryGetValue(adtName, out var fdid))
                                                mask.Add(fdid);
                                            else
                                                mask.Add(0);
                                        }
                                    }
                                }
                            }

                            break;
                        default:
                            //Console.WriteLine(string.Format("Found unknown header at offset {1} \"{0}\" while we should've already read them all!", chunkName.ToString("X"), position.ToString()));
                            break;
                    }
                }
            }
            mapMaskCache.Add((mapID, layer), mask);
            return mask;
        }

        [Route("wdtMask")]
        [HttpGet]
        public List<int> GetWDTMask(string mapID, string directory, uint wdtFileDataID, byte layer = 0)
        {
            return CacheMask(mapID, directory, wdtFileDataID, layer);
        }

        private static readonly int[] right = new[] { 0, 0, 0, 255 };
        private static readonly int[] in1 = new[] { 0, 0, 0, 0 };
        private static MemoryStream CompileMap(List<int> wdtMask, string mapName, sbyte min_x, sbyte min_y, sbyte max_x, sbyte max_y, byte layer = 0)
        {
            var ms = new MemoryStream();
            var blpRes = 512;

            Console.WriteLine("Compiling map " + mapName + " (" + wdtMask.Count + " tiles)");

            var emptyTile = Image.Black(blpRes, blpRes);
            var mask = emptyTile.Equal(right).BandAnd();
            emptyTile = mask.Ifthenelse(in1, emptyTile);

            Dictionary<string, Image> TileCache = [];

            var imageList = new List<Image>();

            if (layer == 0 || layer == 1 || layer == 2)
            {
                for (sbyte cur_x = 0; cur_x < 64; cur_x++)
                {
                    for (sbyte cur_y = 0; cur_y < 64; cur_y++)
                    {
                        if (cur_x > max_x || cur_y > max_y)
                            continue;

                        if (cur_x < min_x || cur_y < min_y)
                            continue;

                        Image? image;

                        var fdid = wdtMask[cur_x * 64 + cur_y];
                        if (fdid == 0)
                        {
                            image = emptyTile.Copy();
                            imageList.Add(image);
                            continue;
                        }

                        var minimapStream = CASC.GetFileByID((uint)fdid);

                        if (minimapStream == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unable to extract minimap tile " + fdid);
                            Console.ResetColor();
                            continue;
                        }

                        var blp = new BLPFile(minimapStream);
                        var pixels = blp.GetPixels(0, out var width, out var height);

                        try
                        {
                            ARGBColor8.ConvertToBGRA(pixels);
                            image = Image.NewFromMemory(pixels, width, height, 4, Enums.BandFormat.Uchar);
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed to create new image from BLP: " + e.Message);
                            Console.ResetColor();
                            continue;
                        }


                        if (image.Width != blpRes)
                        {
                            if (blpRes == 512 && image.Width == 256)
                            {
                                image = image.Resize(2, Enums.Kernel.Nearest);
                            }
                        }

                        imageList.Add(image);
                    }
                }
            }
            else if (layer == 3 || layer == 4)
            {
                for (sbyte cur_x = 0; cur_x < 64; cur_x++)
                {
                    for (sbyte cur_y = 0; cur_y < 64; cur_y++)
                    {
                        if (cur_x > max_x || cur_y > max_y)
                            continue;

                        if (cur_x < min_x || cur_y < min_y)
                            continue;

                        Image? image;

                        var fdid = wdtMask[cur_x * 64 + cur_y];
                        if (fdid == 0)
                        {
                            image = emptyTile.Copy();
                            imageList.Add(image);
                            continue;
                        }

                        try
                        {
                            image = GetADTColorImage((uint)fdid, blpRes, "mccv");
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Failed to create new image from ADT: " + e.Message);
                            Console.ResetColor();
                            continue;
                        }

                        imageList.Add(image);
                    }
                }
            }


            var timer = System.Diagnostics.Stopwatch.StartNew();

            // Generate compiled map
            Console.WriteLine("Joining " + imageList.Count + " tiles...");
            var compiled = Image.Arrayjoin(imageList.ToArray(), (max_x - min_x) + 1);
            Console.WriteLine("Done, took " + timer.ElapsedMilliseconds + "ms");
            timer.Restart();

            Console.WriteLine("Writing compiled map to stream...");
            compiled.WriteToStream(ms, ".png");
            Console.WriteLine("Done, took " + timer.ElapsedMilliseconds + "ms");

            ms.Position = 0;

            foreach (var img in TileCache.Values)
                img.Dispose();

            TileCache.Clear();
            compiled.Dispose();
            imageList.Clear();

            timer.Stop();

            return ms;
        }

        [Route("download")]
        [HttpGet]
        public FileStreamResult DownloadMap(string mapID, string directory, uint wdtFileDataID, byte layer = 0)
        {
            var wdtMask = CacheMask(mapID, directory, wdtFileDataID, layer);

            return new FileStreamResult(CompileMap(wdtMask, mapID, 0, 0, 63, 63, layer), "image/png")
            {
                FileDownloadName = mapID + ".png"
            };
        }
    }
}
