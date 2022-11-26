using CASCLib;
using DBCD.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using wow.tools.local.Services;
using wow.tools.Services;
namespace wow.tools.local.Controllers
{
    [Route("dbc/data")]
    [ApiController]
    public class DataController : ControllerBase
    {
        public class DataTablesResult
        {
            public int draw { get; set; }
            public int recordsFiltered { get; set; }
            public int recordsTotal { get; set; }
            public List<string[]> data { get; set; }
            public string error { get; set; }
        }

        private readonly DBDProvider dbdProvider;
        private readonly DBCManager dbcManager;

        public DataController(IDBDProvider dbdProvider, IDBCManager dbcManager)
        {
            this.dbdProvider = dbdProvider as DBDProvider;
            this.dbcManager = dbcManager as DBCManager;
        }

        // GET: data/
        [HttpGet]
        public string Get()
        {
            return "No DBC selected!";
        }

        // GET/POST: data/name
        [HttpGet("{name}"), HttpPost("{name}")]
        public async Task<DataTablesResult> Get(CancellationToken cancellationToken, string name, string build, int draw, int start, int length, bool useHotfixes = false, LocaleFlags locale = LocaleFlags.All_WoW)
        {
            var parameters = new Dictionary<string, string>();

            if (Request.Method == "POST")
            {
                // POST, what site uses
                foreach (var post in Request.Form)
                    parameters.Add(post.Key, post.Value);

                if (parameters.ContainsKey("draw"))
                    draw = int.Parse(parameters["draw"]);

                if (parameters.ContainsKey("start"))
                    start = int.Parse(parameters["start"]);

                if (parameters.ContainsKey("length"))
                    length = int.Parse(parameters["length"]);
            }
            else
            {
                // GET, backwards compatibility for scripts/users using this
                foreach (var get in Request.Query)
                    parameters.Add(get.Key, get.Value);
            }

            if (!parameters.TryGetValue("search[value]", out var searchValue) || string.IsNullOrWhiteSpace(searchValue))
            {
                Console.WriteLine("Serving data " + start + "," + length + " for dbc " + name + " (" + build + ") for draw " + draw);
            }
            else
            {
                Console.WriteLine("Serving data " + start + "," + length + " for dbc " + name + " (" + build + ") for draw " + draw + " with search " + searchValue);
            }

            var result = new DataTablesResult
            {
                draw = draw
            };

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var storage = await dbcManager.GetOrLoad(name, build, useHotfixes, locale);

                if (storage == null)
                {
                    throw new Exception("Definitions for this DB and version combination not found in definition cache!");
                }

                result.recordsTotal = storage.Values.Count();
                result.data = new List<string[]>();

                if (storage.Values.Count == 0 || storage.AvailableColumns.Length == 0)
                    return result;

                var viewFilter = new DBCViewFilter(storage, parameters, WebUtility.HtmlEncode);

                result.data = viewFilter.GetRecords(cancellationToken).ToList();
                result.recordsFiltered = result.data.Count;

                var takeLength = length;
                if ((start + length) > result.recordsFiltered)
                {
                    takeLength = result.recordsFiltered - start;
                }

                // Temp hackfix: If requested count is higher than the amount of filtered records an error occurs and all rows are returned crashing tabs for large DBs.
                if (takeLength < 0)
                {
                    start = 0;
                    takeLength = 0;
                }

                result.data = result.data.GetRange(start, takeLength);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured during serving data: " + e.Message);
                result.error = e.Message;
            }

            return result;
        }
    }
}