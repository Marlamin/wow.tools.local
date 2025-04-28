using CASCLib;
using Microsoft.AspNetCore.Mvc;
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
                                if(remaining == 44 || remaining == 45)
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

            if (SettingsManager.wowFolder != null)
            {
                foreach (var file in Directory.GetFiles(SettingsManager.wowFolder, "DBCache.bin", SearchOption.AllDirectories))
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
    }
}
