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
    public class DataController(IDBCManager dbcManager) : ControllerBase
    {
        public class DataTablesResult
        {
            public int draw { get; set; }
            public int recordsFiltered { get; set; }
            public int recordsTotal { get; set; }
            public List<string[]> data { get; set; }
            public string error { get; set; }
        }

        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

        // GET: data/
        [HttpGet]
        public string Get()
        {
            return "No DBC selected!";
        }

        // GET/POST: data/name
        [HttpGet("{name}"), HttpPost("{name}")]
        public async Task<DataTablesResult> Get(string name, string build, int draw, int start, int length, CancellationToken cancellationToken, bool useHotfixes = false, LocaleFlags locale = LocaleFlags.All_WoW)
        {
            name = name.ToLower();
            if (string.IsNullOrEmpty(build) || build == "?" || build == "null")
                build = CASC.BuildName;

            var parameters = new Dictionary<string, string>();

            if (Request.Method == "POST")
            {
                // POST, what site uses
                foreach (var post in Request.Form)
                    parameters.Add(post.Key, post.Value);

                if (parameters.TryGetValue("draw", out string? drawString))
                    draw = int.Parse(drawString);

                if (parameters.TryGetValue("start", out string? startString))
                    start = int.Parse(startString);

                if (parameters.TryGetValue("length", out string? lengthString))
                    length = int.Parse(lengthString);
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

                result.recordsTotal = storage.Values.Count;
                result.data = [];

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