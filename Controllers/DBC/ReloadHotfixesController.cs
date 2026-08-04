using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Managers;

namespace wow.tools.local.Controllers
{
    [Route("dbc/reloadHotfixes")]
    [ApiController]
    public class ReloadHotfixesController(IDBCManager dbcManager) : ControllerBase
    {
        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

        [HttpGet]
        public async Task<string> Get()
        {
            HotfixManager.Clear();
            dbcManager.ClearCache();
            dbcManager.ClearHotfixCache();
            await HotfixManager.LoadCaches();
            return "Reloaded hotfixes";
        }
    }
}