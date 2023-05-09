using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/reloadHotfixes")]
    [ApiController]
    public class ReloadHotfixesController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            HotfixManager.Clear();
            HotfixManager.LoadCaches();
            return "Reloaded hotfixes";
        }
    }
}