using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using wow.tools.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/relations")]
    [ApiController]
    public class RelationshipController(IDBDProvider dbdProvider) : ControllerBase
    {
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;

        [HttpGet]
        public Dictionary<string, List<string>> Get()
        {
            return dbdProvider.GetAllRelations();
        }

        [HttpGet("{foreignColumn}")]
        public List<string> Get(string foreignColumn)
        {
            return dbdProvider.GetRelationsToColumn(foreignColumn);
        }
    }
}