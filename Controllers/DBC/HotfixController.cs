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

            if(SQLiteDB.hotfixDBConn.State != System.Data.ConnectionState.Open)
                SQLiteDB.hotfixDBConn.Open();

            var totalHotfixesSQL = "SELECT COUNT(*) FROM wow_hotfixes";
            using var totalCmd = SQLiteDB.hotfixDBConn.CreateCommand();
            totalCmd.CommandText = totalHotfixesSQL;
            var totalHotfixes = (long)totalCmd.ExecuteScalar()!;
            result.recordsTotal = (int)totalHotfixes;
            result.recordsFiltered = result.recordsTotal;

            var numRecords = Request.QueryString.Value.Contains("length") ? int.Parse(Request.Query["length"]!) : 10;
            var startRecords = Request.QueryString.Value.Contains("start") ? int.Parse(Request.Query["start"]!) : 0;

            var sql = "SELECT * FROM wow_hotfixes ORDER BY firstdetected DESC, pushID DESC, tableName DESC, recordID DESC LIMIT @start, @length";
            using var cmd = SQLiteDB.hotfixDBConn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@start", startRecords);
            cmd.Parameters.AddWithValue("@length", numRecords);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var pushID = reader.GetInt32(0);
                var recordID = reader.GetInt32(1);
                var tableName = reader.GetString(2);
                var status = reader.GetInt32(3);
                var build = SQLiteDB.GetVersionByBuild(reader.GetInt32(4));
                if(string.IsNullOrEmpty(build))
                    build = "?"; // Question mark makes it just load current build

                var region = reader.GetInt32(5); // defunct, use hotfixpushxbuild instead
                var firstdetected = reader.GetString(6);

                result.data.Add(
                [
                    pushID.ToString(),
                    tableName,
                    recordID.ToString(),
                    build.ToString(),
                    status.ToString(),
                    firstdetected.ToString(),
                    CASC.DB2Map.ContainsKey("dbfilesclient/" + tableName.ToLower() + ".db2") ? "1" : "0"
                ]);
            }
            
            return result;
        }
    }
}
