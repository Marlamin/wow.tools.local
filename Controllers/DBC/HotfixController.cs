using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/hotfixes")]
    [ApiController]
    public class HotfixController : Controller
    {
        [Route("list")]
        [HttpGet]
        public DataTablesResult GetHotfixes()
        {
            if (HotfixManager.dbcacheParsers.Count == 0)
                HotfixManager.LoadCaches();

            var allHotfixes = HotfixManager.dbcacheParsers.Select(x => x.Value.SelectMany(y => y.hotfixes).ToList());
            var uniquePushes = allHotfixes.SelectMany(x => x).Where(x => x.pushID != -1).GroupBy(x => x.pushID).OrderByDescending(x => x.First().pushID).OrderByDescending(x => HotfixManager.pushIDDetected[x.Key]).ToList();
            Console.WriteLine("Found " + uniquePushes.Count + " unique pushes");

            var result = new DataTablesResult();
            result.data = new List<List<string>>();
            var numRecords = Request.QueryString.Value.Contains("length") ? int.Parse(Request.Query["length"]) : 10;
            var startRecords = Request.QueryString.Value.Contains("start") ? int.Parse(Request.Query["start"]) : 0;

            result.draw = Request.QueryString.Value.Contains("draw") ? int.Parse(Request.Query["draw"]) : 0;

            foreach(var uniquePush in uniquePushes)
            {
                foreach(var hotfix in uniquePush)
                {
                    if(!HotfixManager.tableNames.TryGetValue(hotfix.tableHash, out var tableName))
                    {
                        tableName = "Unknown";
                    }

                    result.data.Add(new List<string>
                    {
                        hotfix.pushID.ToString(),
                        tableName,
                        hotfix.recordID.ToString(),
                        "?",
                        hotfix.status.ToString(),
                        HotfixManager.pushIDDetected[hotfix.pushID].ToString(),
                        CASC.ListfileReverse.ContainsKey("dbfilesclient/" + tableName.ToLower() + ".db2") ? "1" : "0"
                    });
                }
            }

            result.recordsFiltered = result.data.Count;
            result.data = result.data.Skip(startRecords).Take(numRecords).ToList();
            result.recordsTotal = result.data.Count;

            return result;
        }
    }
}
