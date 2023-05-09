using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/reloadHotfixes")]
    [ApiController]
    public class ReloadHotfixesController : ControllerBase
    {
        private readonly DBDProvider dbdProvider;
        private readonly DBCManager dbcManager;

        public ReloadHotfixesController(IDBDProvider dbdProvider, IDBCManager dbcManager)
        {
            this.dbdProvider = dbdProvider as DBDProvider;
            this.dbcManager = dbcManager as DBCManager;
        }

        [HttpGet]
        public string Get()
        {
            HotfixManager.Clear();
            dbcManager.ClearCache();
            dbcManager.ClearHotfixCache();
            HotfixManager.LoadCaches();
            return "Reloaded hotfixes";
        }
    }
}