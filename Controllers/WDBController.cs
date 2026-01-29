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
            var result = new DataTablesResult()
            {
                draw = draw,
                data = []
            };

            if (SettingsManager.ReadOnly)
                return result;

            var questWDB = WDBReader.Read("C:\\World of Warcraft\\_retail_\\Cache\\WDB\\enUS\\questcache.wdb", CASC.BuildName);
            //var questWDB = WDBReader.Read("C:\\World of Warcraft\\_beta_\\Cache\\WDB\\enUS\\questcache.wdb", CASC.BuildName);

            result.recordsFiltered = questWDB.entries.Count;
            result.recordsTotal = questWDB.entries.Count;

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
            if (SettingsManager.ReadOnly)
                return "";

            var questWDB = WDBReader.Read("C:\\World of Warcraft\\_retail_\\Cache\\WDB\\enUS\\questcache.wdb", CASC.BuildName);
            //var questWDB = WDBReader.Read("C:\\World of Warcraft\\_beta_\\Cache\\WDB\\enUS\\questcache.wdb", CASC.BuildName);

            if (id == 0)
                return JsonSerializer.Serialize(questWDB.entries);
            else
                return JsonSerializer.Serialize(questWDB.entries.Where(x => x.Key == id));
        }


        [Route("creature")]
        [HttpGet]
        public string Creature(int id = 0, string format = "json")
        {
            if (SettingsManager.ReadOnly)
                return "";

            //var questWDB = WDBReader.Read("C:\\World of Warcraft\\_retail_\\Cache\\WDB\\enUS\\questcache.wdb", "11.2.7.64743");
            var creatureWDB = WDBReader.Read("C:\\World of Warcraft\\_beta_\\Cache\\WDB\\enUS\\creaturecache.wdb", CASC.BuildName);

            if (format == "json")
            {
                if (id == 0)
                    return JsonSerializer.Serialize(creatureWDB.entries);
                else
                    return JsonSerializer.Serialize(creatureWDB.entries.Where(x => x.Key == id));
            }
            else
            {
                var entries = creatureWDB.entries;
                if (id != 0)
                    entries = entries.Where(x => x.Key == id).ToDictionary(x => x.Key, x => x.Value);

                var tsv = "";
                foreach (var entry in entries.OrderBy(x => x.Key))
                {
                    tsv += entry.Key + "\t" + entry.Value["Name[0]"] + "\n";
                }
                return tsv;
            }

        }
    }
}
