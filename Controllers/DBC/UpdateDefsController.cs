using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local.Controllers.DBC
{
    [Route("dbc/updateDefs")]
    [ApiController]
    public class UpdateDefsController(IDBDProvider dbdProvider, IDBCManager dbcManager) : ControllerBase
    {
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

        [HttpGet]
        public async Task<string> Get()
        {
            using (var client = new HttpClient())
            {
                var stream = await client.GetStreamAsync("https://github.com/wowdev/WoWDBDefs/archive/refs/heads/master.zip");
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    ms.Position = 0;
                    using (var archive = new ZipArchive(ms, ZipArchiveMode.Read))
                    {
                        archive.ExtractToDirectory("WoWDBDefs_tmp");
                        Directory.Delete("WoWDBDefs", true);
                        Directory.Move("WoWDBDefs_tmp/WoWDBDefs-master", "WoWDBDefs");
                        Directory.Delete("WoWDBDefs_tmp", true);
                    }
                }
            }

            // Reload defs
            int count = dbdProvider.LoadDefinitions();
            dbcManager.ClearCache();
            dbcManager.ClearHotfixCache();
            return "Reloaded " + count + " definitions and cleared DBC cache!";
        }
    }
}
