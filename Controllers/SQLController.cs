using Microsoft.AspNetCore.Mvc;

namespace wow.tools.local.Controllers
{
    [Route("sql/")]
    [ApiController]
    public class SQLController() : Controller
    {
        [Route("query")]
        [HttpGet]
        public IActionResult Query(string query)
        {
            if (SettingsManager.ReadOnly)
                return Forbid();

            return Ok(Services.SQLiteDB.RunQuery(query));
        }
    }
}