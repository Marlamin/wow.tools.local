using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Managers;
using wow.tools.local.Providers;

namespace wow.tools.local.Controllers
{
    [Route("dbc/reloadDefs")]
    [ApiController]
    public class ReloadDefsController(IDBDProvider dbdProvider, IDBCManager dbcManager, IEnumProvider enumProvider) : ControllerBase
    {
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;
        private readonly EnumProvider enumProvider = (EnumProvider)enumProvider;

        [HttpGet]
        public string Get()
        {
            enumProvider.ClearCache();
            int count = dbdProvider.LoadDefinitions();
            dbcManager.ClearCache();
            dbcManager.ClearHotfixCache();
            HotfixManager.Clear();
            return "Reloaded " + count + " definitions and cleared DBC cache!";
        }
    }
}