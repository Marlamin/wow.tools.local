using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Managers;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/hotfixes")]
    [ApiController]
    public class HotfixController : Controller
    {
        [Route("list")]
        [HttpGet]
        public DataTablesResult GetHotfixesV2()
        {
            if (!Request.QueryString.HasValue)
                return new DataTablesResult
                {
                    draw = 0,
                    data = [],
                    recordsFiltered = 0,
                    recordsTotal = 0
                };

            var currentBuild = uint.Parse(CASC.BuildName.Split('.').Last());

            var result = new DataTablesResult
            {
                draw = Request.QueryString.Value.Contains("draw") ? int.Parse(Request.Query["draw"]!) : 0,
                data = []
            };

            if (SQLiteDB.hotfixDBConn.State != System.Data.ConnectionState.Open)
                SQLiteDB.hotfixDBConn.Open();
          
            // Filtering/searching
            var where = " ";

            var pushIDFilter = Request.Query.ContainsKey("columns[0][search][value]") ? Request.Query["columns[0][search][value]"]!.ToString() : "";
            var tableNameFilter = Request.Query.ContainsKey("columns[1][search][value]") ? Request.Query["columns[1][search][value]"]!.ToString() : "";
            var recordIDFilter = Request.Query.ContainsKey("columns[2][search][value]") ? Request.Query["columns[2][search][value]"]!.ToString() : "";
            var buildFilter = Request.Query.ContainsKey("columns[3][search][value]") ? Request.Query["columns[3][search][value]"]!.ToString() : "";
            var statusFilter = Request.Query.ContainsKey("columns[4][search][value]") ? Request.Query["columns[4][search][value]"]!.ToString() : "";
            var firstSeenFilter = Request.Query.ContainsKey("columns[5][search][value]") ? Request.Query["columns[5][search][value]"]!.ToString() : "";

            var parameters = new List<(string, string)>();

            if (!string.IsNullOrWhiteSpace(pushIDFilter) || !string.IsNullOrWhiteSpace(tableNameFilter) || !string.IsNullOrWhiteSpace(recordIDFilter) || !string.IsNullOrWhiteSpace(buildFilter) || !string.IsNullOrWhiteSpace(statusFilter) || !string.IsNullOrWhiteSpace(firstSeenFilter))
            {
                var conditions = new List<string>();

                if(!string.IsNullOrWhiteSpace(pushIDFilter))
                {
                    conditions.Add("pushID = @pushID");
                    parameters.Add(("@pushID", pushIDFilter));
                }

                if (!string.IsNullOrWhiteSpace(tableNameFilter))
                {
                    conditions.Add("tableName LIKE @tableName");
                    parameters.Add(("@tableName", $"%{tableNameFilter}%"));
                }

                if (!string.IsNullOrWhiteSpace(recordIDFilter))
                {
                    conditions.Add("recordID = @recordID");
                    parameters.Add(("@recordID", recordIDFilter));
                }

                if (!string.IsNullOrWhiteSpace(buildFilter))
                {
                    var splitBuild = buildFilter.Split('.');
                    if(splitBuild.Length == 4 && int.TryParse(splitBuild[3], out var buildNumber))
                    {
                        conditions.Add("build = @build");
                        parameters.Add(("@build", buildNumber.ToString()));
                    }
                }

                if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "-1")
                {
                    conditions.Add("isValid = @status");
                    parameters.Add(("@status", statusFilter));
                }

                if (!string.IsNullOrWhiteSpace(firstSeenFilter))
                {
                    conditions.Add("firstdetected LIKE @firstdetected");
                    parameters.Add(("@firstdetected", $"%{firstSeenFilter}%"));
                }

                if(parameters.Count > 0)
                {
                    where = " WHERE ";
                    where += string.Join(" AND ", conditions);
                }
            }

            // TODO: Ordering support
            var orderBy = " ORDER BY firstdetected DESC, pushID DESC, tableName DESC, recordID DESC";

            // Limits
            var numRecords = Request.QueryString.Value.Contains("length") ? int.Parse(Request.Query["length"]!) : 10;
            var startRecords = Request.QueryString.Value.Contains("start") ? int.Parse(Request.Query["start"]!) : 0;
            var limit = " LIMIT @start, @length";

            // Get full hotfix count
            var totalHotfixesSQL = "SELECT COUNT(*) FROM wow_hotfixes";
            using var totalCmd = SQLiteDB.hotfixDBConn.CreateCommand();
            totalCmd.CommandText = totalHotfixesSQL;

            var totalHotfixes = (long)totalCmd.ExecuteScalar()!;
            result.recordsTotal = (int)totalHotfixes;

            // Get filtered hotfix count
            var filteredHotfixesSQL = "SELECT COUNT(*) FROM wow_hotfixes" + where;
            using var filteredCmd = SQLiteDB.hotfixDBConn.CreateCommand();
            filteredCmd.CommandText = filteredHotfixesSQL;

            foreach(var (paramName, paramValue) in parameters)
                filteredCmd.Parameters.AddWithValue(paramName, paramValue);

            var filteredHotfixes = (long)filteredCmd.ExecuteScalar()!;
            result.recordsFiltered = (int)filteredHotfixes;

            // Get hotfixes
            var sql = "SELECT * FROM wow_hotfixes";
            using var cmd = SQLiteDB.hotfixDBConn.CreateCommand();
            cmd.CommandText = sql + where + orderBy + limit;

            cmd.Parameters.AddWithValue("@start", startRecords);
            cmd.Parameters.AddWithValue("@length", numRecords);

            foreach(var (paramName, paramValue) in parameters)
                cmd.Parameters.AddWithValue(paramName, paramValue);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var pushID = reader.GetInt32(0);
                var recordID = reader.GetInt32(1);
                var tableName = reader.GetString(2);
                var status = reader.GetInt32(3);
                var build = SQLiteDB.GetVersionByBuild(reader.GetInt32(4));
                if (string.IsNullOrEmpty(build))
                    build = "?"; // Question mark makes it just load current build

                var firstdetected = reader.GetString(6);

                result.data.Add(
                [
                    pushID.ToString(),
                    tableName,
                    recordID.ToString(),
                    build.ToString(),
                    status.ToString(),
                    firstdetected.ToString(),
                    Listfile.DB2Map.ContainsKey("dbfilesclient/" + tableName.ToLower() + ".db2") ? "1" : "0"
                ]);
            }

            return result;
        }

        [Route("downloadLatest")]
        [HttpGet]
        public IActionResult DownloadLatest(string branch)
        {
            HotfixManager.DownloadLatest(branch);
            return Ok();
        }
    }
}
