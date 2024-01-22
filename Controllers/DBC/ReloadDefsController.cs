using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/reloadDefs")]
    [ApiController]
    public class ReloadDefsController(IDBDProvider dbdProvider, IDBCManager dbcManager) : ControllerBase
    {
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

        [HttpGet]
        public string Get()
        {
            int count = dbdProvider.LoadDefinitions();
            dbcManager.ClearCache();
            dbcManager.ClearHotfixCache();
            HotfixManager.Clear();
            return "Reloaded " + count + " definitions and cleared DBC cache!";
        }
    }
}