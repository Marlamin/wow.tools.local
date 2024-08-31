using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;
using WoWFormatLib.FileReaders;

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

        [Route("quests")]
        [HttpGet]
        public string QuestDebug()
        {
            var questWDB = WDBReader.Read("C:\\World of Warcraft\\_retail_\\Cache\\WDB\\enUS\\questcache.wdb", "11.0.2.56196");
            return questWDB.ToString();
        }
    }
}
