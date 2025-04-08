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
        public string Get()
        {
            if (!dbdProvider.isUsingBDBD)
                Console.WriteLine("WARNING: You are using a local DBD definitions directory, updating can not be done through WTL itself.");

            // Reload manifest & defs
            DBDManifest.Load(true);
            int count = dbdProvider.LoadDefinitions(true);
            dbcManager.ClearCache();
            dbcManager.ClearHotfixCache();
            return "Reloaded " + count + " definitions and cleared DBC cache!";
        }
    }
}
