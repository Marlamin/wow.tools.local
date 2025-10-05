using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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

            var results = SQLiteDB.GetCreatureNames(start, length, Request.Query.TryGetValue("search[value]", out var search) ? search : "");

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
        public DataTablesResult QuestTable(int draw, int start, int length)
        {
            //var questWDB = WDBReader.Read("C:\\World of Warcraft\\_retail_\\Cache\\WDB\\enUS\\questcache.wdb", "11.2.5.63534");
            var questWDB = WDBReader.Read("C:\\World of Warcraft\\_beta_\\Cache\\WDB\\enUS\\questcache.wdb", "12.0.0.63534");

            var result = new DataTablesResult()
            {
                draw = draw,
                recordsTotal = questWDB.entries.Count,
                recordsFiltered = questWDB.entries.Count,
                data = []
            };

            if (length == -1)
            {
                start = 0;
                length = 25;
            }

            var results = questWDB.entries.OrderBy(x => x.Key).Skip(start).Take(length);

            foreach (var res in results)
            {
                result.data.Add(
                [
                    res.Key.ToString(), // ID
                    res.Value["LogTitle"], // Filename 
                ]);
            }

            return result;
        }

        [Route("quest")]
        [HttpGet]
        public string Quest(int id)
        {
            //var questWDB = WDBReader.Read("C:\\World of Warcraft\\_retail_\\Cache\\WDB\\enUS\\questcache.wdb", "11.2.5.63534");
            var questWDB = WDBReader.Read("C:\\World of Warcraft\\_beta_\\Cache\\WDB\\enUS\\questcache.wdb", "12.0.0.63534");

            if (id == 0)
                return JsonSerializer.Serialize(questWDB.entries);
            else
                return JsonSerializer.Serialize(questWDB.entries.Where(x => x.Key == id));
        }
    }
}
