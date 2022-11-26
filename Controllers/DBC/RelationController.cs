using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/relations")]
    [ApiController]
    public class RelationshipController : ControllerBase
    {
        private readonly DBDProvider dbdProvider;

        public RelationshipController(IDBDProvider dbdProvider, IDBCManager dbcManager)
        {
            this.dbdProvider = dbdProvider as DBDProvider;
        }

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