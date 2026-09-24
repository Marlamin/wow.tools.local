using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Providers;

namespace wow.tools.local.Controllers
{
    [Route("dbc/labelColumns")]
    [ApiController]
    public class LabelController(IDBDProvider dbdProvider) : ControllerBase
    {
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;

        [HttpGet]
        public List<string> Get()
        {
            return dbdProvider.GetAllLabelColumns();
        }
    }
}
