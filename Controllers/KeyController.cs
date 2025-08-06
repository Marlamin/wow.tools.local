using CASCLib;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("keys/")]
    [ApiController]
    public class KeyController(IDBCManager dbcManager) : Controller
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

        private class KeyInfoEntry
        {
            public int ID { get; set; }
            public string Lookup { get; set; } = "";
            public string Key { get; set; } = "";
            public string FirstSeen { get; set; } = "";
            public string Description { get; set; } = "";
        }

        [Route("info")]
        [HttpGet]
        public string Info()
        {
            KeyMetadata.ReloadKeys();

            var tklStorage = dbcManager.GetOrLoad("TactKeyLookup", CASC.BuildName, true).Result;

            var newKeysFound = false;
            foreach (dynamic tklRow in tklStorage.Values)
            {
                ulong key = BitConverter.ToUInt64(tklRow.TACTID);

                if (!KeyMetadata.KeyInfo.TryGetValue(key, out (int ID, string FirstSeen, string Description) metaData))
                {
                    KeyMetadata.KeyInfo.Add(
                        key,
                        ((int)tklRow.ID,
                        CASC.FullBuildName,
                        CASC.EncryptedFDIDs.Where(x => x.Value.Contains(key)).Select(x => x.Key).ToList().Count.ToString() + " file(s) as of " + CASC.BuildName)
                    );
                }
                else if (metaData.Description == "" || metaData.Description.Contains("file(s) as of"))
                {
                    var copy = metaData;
                    copy.Description = CASC.EncryptedFDIDs.Where(x => x.Value.Contains(key)).Select(x => x.Key).ToList().Count.ToString() + " file(s) as of " + CASC.BuildName;
                    KeyMetadata.KeyInfo[key] = copy;
                }
            }

            var tkStorage = dbcManager.GetOrLoad("TactKey", CASC.BuildName, true).Result;
            foreach (dynamic tkRow in tkStorage.Values)
            {
                foreach (var keyInfo in KeyMetadata.KeyInfo)
                {
                    if (keyInfo.Value.ID != (int)tkRow.ID)
                        continue;

                    if (WTLKeyService.HasKey(keyInfo.Key))
                        continue;

                    Console.WriteLine("Setting key " + (int)tkRow.ID + " from TactKey.db2");
                    WTLKeyService.SetKey(keyInfo.Key, tkRow.Key);
                    newKeysFound = true;
                }
            }

            if (Directory.Exists("caches"))
            {
                foreach (var file in Directory.GetFiles("caches", "*.bin", SearchOption.AllDirectories))
                {
                    var cache = new DBCacheParser(file);

                    foreach (var hotfix in cache.hotfixes)
                    {
                        if (hotfix.dataSize == 0) continue;
                        if (hotfix.tableHash == 0x021826BB)
                        {
                            using (var ms = new MemoryStream(hotfix.data))
                            using (var bin = new BinaryReader(ms))
                            {
                                bin.ReadCString(); // Text_lang
                                bin.ReadCString(); // Text1_lang

                                var remaining = bin.BaseStream.Length - bin.BaseStream.Position;
                                if (remaining == 44 || remaining == 45)
                                    continue;

                                bin.BaseStream.Position = bin.BaseStream.Length - 28;

                                if (bin.BaseStream.Position != bin.BaseStream.Length)
                                {
                                    var extraTableHash = bin.ReadUInt32();
                                    if (extraTableHash == 0xDF2F53CF)
                                    {
                                        var tactKeyLookup = bin.ReadUInt64();
                                        var tactKeyBytes = bin.ReadBytes(16);

                                        if (WTLKeyService.HasKey(tactKeyLookup))
                                            continue;

                                        WTLKeyService.SetKey(tactKeyLookup, tactKeyBytes);
                                        newKeysFound = true;
                                        Console.WriteLine("Found TACT Key " + string.Format("{0:X}", tactKeyLookup).PadLeft(16, '0') + " " + Convert.ToHexString(tactKeyBytes));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(SettingsManager.WoWFolder))
            {
                foreach (var file in Directory.GetFiles(SettingsManager.WoWFolder, "DBCache.bin", SearchOption.AllDirectories))
                {
                    var cache = new DBCacheParser(file);

                    foreach (var hotfix in cache.hotfixes)
                    {
                        if (hotfix.dataSize == 0) continue;

                        if (hotfix.tableHash == 0x021826BB)
                        {
                            using (var ms = new MemoryStream(hotfix.data))
                            using (var bin = new BinaryReader(ms))
                            {
                                bin.ReadCString(); // Text_lang
                                bin.ReadCString(); // Text1_lang

                                var remaining = bin.BaseStream.Length - bin.BaseStream.Position;
                                if (remaining == 44 || remaining == 45)
                                    continue;

                                bin.BaseStream.Position = bin.BaseStream.Length - 28;

                                if (bin.BaseStream.Position != bin.BaseStream.Length)
                                {

                                    var extraTableHash = bin.ReadUInt32();
                                    if (extraTableHash == 0xDF2F53CF)
                                    {
                                        var tactKeyLookup = bin.ReadUInt64();
                                        var tactKeyBytes = bin.ReadBytes(16);

                                        if (WTLKeyService.HasKey(tactKeyLookup))
                                            continue;

                                        WTLKeyService.SetKey(tactKeyLookup, tactKeyBytes);
                                        newKeysFound = true;
                                        Console.WriteLine("Found TACT Key " + string.Format("{0:X}", tactKeyLookup).PadLeft(16, '0') + " " + Convert.ToHexString(tactKeyBytes));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var keyInfos = new List<KeyInfoEntry>();

            foreach (var keyInfo in KeyMetadata.KeyInfo)
            {
                keyInfos.Add(
                   new KeyInfoEntry
                   {
                       ID = keyInfo.Value.ID,
                       Lookup = string.Format("{0:X}", keyInfo.Key).PadLeft(16, '0'),
                       Key = WTLKeyService.HasKey(keyInfo.Key) ? Convert.ToHexString(WTLKeyService.GetKey(keyInfo.Key)) : "",
                       FirstSeen = keyInfo.Value.FirstSeen,
                       Description = keyInfo.Value.Description
                   });
            }

            if (newKeysFound)
                CASC.RefreshEncryptionStatus();

            return JsonSerializer.Serialize(keyInfos.OrderBy(x => x.ID));
        }

        [Route("moreinfo")]
        [HttpGet]
        public string MoreInfo(string lookup)
        {
            CASC.EnsureCHashesLoaded();

            if (!ulong.TryParse(lookup, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var converted))
                return "";

            var output = "<table class='table table-striped table-bordered'>";

            var fdids = new HashSet<int>(CASC.EncryptedFDIDs.Where(kvp => kvp.Value.Contains(converted)).Select(kvp => kvp.Key));
            var files = CASC.Listfile.Where(p => fdids.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);

            output += "<tr><td style='width: 200px;'>Lookup</td><td>" + converted.ToString("X16") + " (" + converted + ")</td></tr>";
            output += "<tr><td style='width: 200px;'>Status</td><td>" + (WTLKeyService.HasKey(converted) ? "Available" : "Unavailable") + "</td></tr>";
            if (WTLKeyService.HasKey(converted))
                output += "<tr><td style='width: 200px;'>Key</td><td>" + Convert.ToHexString(WTLKeyService.GetKey(converted)) + "</td></tr>";

            output += "<tr><td style='width: 200px;'>File count</td><td>" + files.Count + "</td></tr>";

            if (files.Count == 0)
            {
                output += "</table>";
                return output;
            }
            output += "<tr><td colspan='2'>File list</td></tr>";
            output += "<tr><td colspan='2'><table class='table-bordered>";
            // DB2s first
            foreach (var file in files)
            {
                var filedataid = file.Key;

                long size = 0;
                List<byte[]> cKeys = [];
                if (CASC.FDIDToCHash.TryGetValue(filedataid, out var cKeyBytes))
                {
                    if (CASC.FDIDToExtraCHashes.TryGetValue(filedataid, out List<byte[]>? extraCHashes))
                    {
                        var cKey = Convert.ToHexStringLower(cKeyBytes);
                        cKeys.Add(cKeyBytes);

                        foreach (var extraCKey in extraCHashes)
                            cKeys.Add(extraCKey);

                        foreach (var cKeyB in cKeys)
                            if (CASC.CHashToSize.TryGetValue(Convert.ToHexStringLower(cKeyB), out size) && size != 0)
                                break;
                    }
                    else
                    {
                        var cKey = Convert.ToHexStringLower(cKeyBytes);
                        cKeys.Add(cKeyBytes);
                        CASC.CHashToSize.TryGetValue(cKey, out size);
                    }
                }

                if (CASC.Types.TryGetValue(file.Key, out string? fileType) && fileType == "db2")
                {
                    output += "<tr><td>" + file.Key + "</td><td>db2</td><td>" + file.Value + "</td><td>" + string.Join(", ", cKeys.Select(x => Convert.ToHexString(x)).ToList()) + "</td><td>" + size + " bytes</td></tr>";

                    var db2EncryptionMetaData = new Dictionary<ulong, int[]>();

                    try
                    {
                        var storage = dbcManager.GetOrLoad(Path.GetFileNameWithoutExtension(file.Value), CASC.BuildName).Result;
                        db2EncryptionMetaData = storage.GetEncryptedIDs();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unable to get encrypted DB2 info for DB2 " + filedataid + ": " + e.Message);
                    }

                    output += "<tr><td colspan='3'><table class='table table-bordered'>";
                    output += "<tr><td>Encrypted record IDs</td><td>" + string.Join(", ", db2EncryptionMetaData.Where(x => x.Key == converted).Select(x => x.Value).FirstOrDefault()!);

                    if (WTLKeyService.HasKey(converted))
                    {
                        output += " <a href='/dbc/?dbc=" + Path.GetFileNameWithoutExtension(CASC.Listfile[filedataid]).ToLower() + "&build=" + CASC.BuildName + "#page=1&search=encrypted%3A" + converted.ToString("X16").PadLeft(16, '0') + "' target='_BLANK' class='text-success'> (view)</a>";
                    }

                    output += "</td></tr>";
                    output += "</table></td></tr>";
                }
            }

            foreach (var file in files)
            {
                if (CASC.Types.TryGetValue(file.Key, out string? fileType) && fileType == "db2")
                    continue;

                var filedataid = file.Key;
                long size = 0;
                List<byte[]> cKeys = [];
                if (CASC.FDIDToCHash.TryGetValue(filedataid, out var cKeyBytes))
                {
                    if (CASC.FDIDToExtraCHashes.TryGetValue(filedataid, out List<byte[]>? extraCHashes))
                    {
                        var cKey = Convert.ToHexStringLower(cKeyBytes);
                        cKeys.Add(cKeyBytes);

                        foreach (var extraCKey in extraCHashes)
                            cKeys.Add(extraCKey);

                        CASC.CHashToSize.TryGetValue(cKey, out size);
                    }
                    else
                    {
                        var cKey = Convert.ToHexStringLower(cKeyBytes);
                        cKeys.Add(cKeyBytes);
                        CASC.CHashToSize.TryGetValue(cKey, out size);
                    }
                }

                if (string.IsNullOrEmpty(fileType) && (size == 6660 || size == 88612 || size == 175972))
                    fileType = "blp?";

                var filename = file.Value;
                if(string.IsNullOrEmpty(filename) && cKeyBytes != null)
                {
                    if(WoWNamingLib.Namers.ContentHashNamer.knownHashes.TryGetValue(Convert.ToHexStringLower(cKeyBytes), out var chashname))
                    {
                        filename = "Content hash name: " + chashname;
                    }
                }
                output += "<tr><td>" + file.Key + "</td><td>" + fileType + "</td><td>" + file.Value + "</td><td>" + string.Join(", ", cKeys.Select(x => Convert.ToHexString(x)).ToList()) + "</td><td>" + size + " bytes</td></tr>";
            }
            output += "</table></td></tr>";

            output += "</table>";
            return output;
        }
    }
}
