using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/reloadHotfixes")]
    [ApiController]
    public class ReloadHotfixesController(IDBCManager dbcManager) : ControllerBase
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

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