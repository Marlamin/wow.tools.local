using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{

    [Route("wdb/")]
    [ApiController]
    public class WDBController : Controller
    {
        [Route("creatures")]
        [HttpGet]
        public DataTablesResult CreatureTable(int draw, int start, int length)
        {
            var totalCount = SQLiteDB.GetCreatureCount();
            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = []
            };

            var results = SQLiteDB.GetCreatureNames(start, length);

            if (length == -1)
            {
                start = 0;
                length = results.Count;
            }

            foreach (var res in results)
            {
                result.data.Add(
                    [
                        res.Key.ToString(), // ID
                        res.Value, // Filename 
                    ]);
            }

            return result;
        }
    }
}
