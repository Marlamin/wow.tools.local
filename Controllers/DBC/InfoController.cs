using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Services;

namespace wow.tools.local.Controllers
{
    [Route("dbc/info")]
    [ApiController]
    public class InfoController(IDBCManager dbcManager) : ControllerBase
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

        [HttpGet]
        public DataTablesResult Get(string build)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var get in Request.Query)
                parameters.Add(get.Key, get.Value);

            var draw = 0;
            if (parameters.TryGetValue("draw", out string? value))
                draw = int.Parse(value);

            var result = new DataTablesResult
            {
                draw = draw
            };

            if (!Directory.Exists(Path.Combine(SettingsManager.dbcFolder, build)))
            {
                result.error = "Could not find DBCs on disk for build " + build + "!";
                return result;
            }

            result.data = [];

            var db2s = dbcManager.GetDBCNames(build);

            foreach(var db2 in db2s)
            {
                Stream fs = new MemoryStream();

                if (string.IsNullOrEmpty(build) || build == CASC.BuildName)
                {
                    if (CASC.DB2Exists("DBFilesClient/" + db2 + ".db2"))
                        fs = CASC.GetDB2ByName("DBFilesClient/" + db2 + ".db2");
                }
                else if (!string.IsNullOrEmpty(SettingsManager.dbcFolder))
                {
                    string fileName = Path.Combine(SettingsManager.dbcFolder, build, "dbfilesclient", db2 + ".db2");

                    if (System.IO.File.Exists(fileName))
                        fs = System.IO.File.OpenRead(fileName);

                    fileName = Path.ChangeExtension(fileName, ".dbc");

                    if (System.IO.File.Exists(fileName))
                        fs = System.IO.File.OpenRead(fileName);
                }

                using (var bin = new BinaryReader(fs))
                {
                    var db2Info = new List<string>();


                    var magic = new string(bin.ReadChars(4));

                    db2Info.Add(db2); // name

                    if (magic == "WDC5")
                    {
                        bin.ReadUInt32();
                        bin.ReadBytes(128);
                    }

                    db2Info.Add(bin.ReadUInt32().ToString()); // recordCount
                    db2Info.Add(bin.ReadUInt32().ToString()); // fieldCount
                    db2Info.Add(bin.ReadUInt32().ToString()); // recordSize
                    bin.ReadUInt32(); // stringTableSize
                    db2Info.Add(bin.ReadUInt32().ToString("X8")); // tableHash
                    db2Info.Add(bin.ReadUInt32().ToString("X8")); // layoutHash
                    db2Info.Add(bin.ReadInt32().ToString()); // minId
                    db2Info.Add(bin.ReadUInt32().ToString()); // maxId
                    db2Info.Add(bin.ReadInt32().ToString()); // locale
                    db2Info.Add(bin.ReadUInt16().ToString()); // flags
                    db2Info.Add(bin.ReadUInt16().ToString()); // idIndex
                    db2Info.Add(bin.ReadUInt32().ToString()); // totalFields
                    bin.BaseStream.Position += 20;
                    db2Info.Add(bin.ReadUInt32().ToString()); // sectionCount

                    result.data.Add([.. db2Info]);
                }

                fs.Dispose();
            }

            return result;
        }
    }
}