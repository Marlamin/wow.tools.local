using CASCLib;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("keys/")]
    [ApiController]
    public class KeyController : Controller
    {
        private readonly DBCManager dbcManager;

        public KeyController(IDBCManager dbcManager)
        {
            this.dbcManager = dbcManager as DBCManager;
        }

        private class KeyInfoEntry
        {
            public int ID { get; set; }
            public string Lookup { get; set; }
            public string Key { get; set; }
            public string FirstSeen { get; set; }
            public string Description { get; set; }
        }

        [Route("info")]
        [HttpGet]
        public string Info()
        {
            var tklStorage = dbcManager.GetOrLoad("TactKeyLookup", CASC.BuildName, true).Result;

            foreach (dynamic tklRow in tklStorage.Values)
            {
                ulong key = BitConverter.ToUInt64(tklRow.TACTID);

                if (!KeyMetadata.KeyInfo.ContainsKey(key))
                {
                    KeyMetadata.KeyInfo.Add(
                        key,
                        ((int)tklRow.ID,
                        CASC.GetFullBuild(),
                        CASC.EncryptedFDIDs.Where(x => x.Value.Contains(key)).Select(x => x.Key).ToList().Count.ToString() + " file(s) as of " + CASC.BuildName)
                    );
                }
                else if (KeyMetadata.KeyInfo[key].Description == "" || KeyMetadata.KeyInfo[key].Description.Contains("file(s) as of"))
                {
                    var copy = KeyMetadata.KeyInfo[key];
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

                    if (!CASC.KnownKeys.Contains(keyInfo.Key))
                        CASC.KnownKeys.Add(keyInfo.Key);

                    if (KeyService.HasKey(keyInfo.Key))
                        continue;


                    Console.WriteLine("Setting key " + (int)tkRow.ID + " from TactKey.db2");
                    KeyService.SetKey(keyInfo.Key, tkRow.Key);
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
                                bin.ReadUInt32();  // ID
                                bin.ReadUInt32();  // LanguageID
                                bin.ReadUInt32();  // ConditionID
                                bin.ReadUInt16();  // EmotesID
                                bin.ReadByte();    // Flags
                                bin.ReadUInt32();  // ChatBubbleDurationMs
                                bin.ReadUInt32();  // VoiceOverPriorityID
                                bin.ReadUInt32();  // SoundKitID[0]
                                bin.ReadUInt32();  // SoundKitID[1]
                                bin.ReadUInt16();  // EmoteID[0]
                                bin.ReadUInt16();  // EmoteID[1]
                                bin.ReadUInt16();  // EmoteID[2]
                                bin.ReadUInt16();  // EmoteDelay[0]
                                bin.ReadUInt16();  // EmoteDelay[1]
                                bin.ReadUInt16();  // EmoteDelay[2]
                                if (bin.BaseStream.Position != bin.BaseStream.Length)
                                {

                                    var extraTableHash = bin.ReadUInt32();
                                    if (extraTableHash == 0xDF2F53CF)
                                    {
                                        var tactKeyLookup = bin.ReadUInt64();
                                        var tactKeyBytes = bin.ReadBytes(16);

                                        if (KeyService.HasKey(tactKeyLookup))
                                            continue;

                                        CASC.KnownKeys.Add(tactKeyLookup);

                                        KeyService.SetKey(tactKeyLookup, tactKeyBytes);

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
                                bin.ReadUInt32();  // ID
                                bin.ReadUInt32();  // LanguageID
                                bin.ReadUInt32();  // ConditionID
                                bin.ReadUInt16();  // EmotesID
                                bin.ReadByte();    // Flags
                                bin.ReadUInt32();  // ChatBubbleDurationMs
                                bin.ReadUInt32();  // VoiceOverPriorityID
                                bin.ReadUInt32();  // SoundKitID[0]
                                bin.ReadUInt32();  // SoundKitID[1]
                                bin.ReadUInt16();  // EmoteID[0]
                                bin.ReadUInt16();  // EmoteID[1]
                                bin.ReadUInt16();  // EmoteID[2]
                                bin.ReadUInt16();  // EmoteDelay[0]
                                bin.ReadUInt16();  // EmoteDelay[1]
                                bin.ReadUInt16();  // EmoteDelay[2]
                                if (bin.BaseStream.Position != bin.BaseStream.Length)
                                {

                                    var extraTableHash = bin.ReadUInt32();
                                    if (extraTableHash == 0xDF2F53CF)
                                    {
                                        var tactKeyLookup = bin.ReadUInt64();
                                        var tactKeyBytes = bin.ReadBytes(16);

                                        if (KeyService.HasKey(tactKeyLookup))
                                            continue;

                                        CASC.KnownKeys.Add(tactKeyLookup);

                                        KeyService.SetKey(tactKeyLookup, tactKeyBytes);

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
                       Key = KeyService.HasKey(keyInfo.Key) ? Convert.ToHexString(KeyService.GetKey(keyInfo.Key)) : "",
                       FirstSeen = keyInfo.Value.FirstSeen,
                       Description = keyInfo.Value.Description
                   });
            }

            return JsonSerializer.Serialize(keyInfos.OrderBy(x => x.ID));
        }
    }
}
