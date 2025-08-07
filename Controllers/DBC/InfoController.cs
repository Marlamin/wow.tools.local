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
            public List<string[]> data { get; set; } = [];
            public string error { get; set; } = string.Empty;
        }

        private readonly DBCManager dbcManager = (DBCManager)dbcManager;

        [HttpGet]
        public DataTablesResult Get(string build)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var get in Request.Query)
                parameters.Add(get.Key, get.Value!);

            var draw = 0;
            if (parameters.TryGetValue("draw", out string? value))
                draw = int.Parse(value);

            var result = new DataTablesResult
            {
                draw = draw
            };

            if (!Directory.Exists(Path.Combine(SettingsManager.DBCFolder, build)))
            {
                result.error = "Could not find DBCs on disk for build " + build + "!";
                return result;
            }

            var db2s = dbcManager.GetDBCNames(build);

            result.data = [];

            foreach (var db2 in db2s)
            {
                Stream? fs = null;

                if (string.IsNullOrEmpty(build) || build == CASC.BuildName)
                {
                    if (CASC.DB2Exists("DBFilesClient/" + db2 + ".db2"))
                        fs = CASC.GetDB2ByName("DBFilesClient/" + db2 + ".db2");
                }
                else if (!string.IsNullOrEmpty(SettingsManager.DBCFolder))
                {
                    string directoryPath = Path.Combine(SettingsManager.DBCFolder, build, "dbfilesclient");
                    if (Directory.Exists(directoryPath))
                    {
                        // Try to either find a db2 or dbc file, ignoring casing to maintain identical behavior on all platforms
                        string? fileName = Directory.EnumerateFiles(directoryPath).FirstOrDefault(fn =>
                            Path.GetFileName(fn).Equals($"{db2}.db2", StringComparison.OrdinalIgnoreCase) ||
                            Path.GetFileName(fn).Equals($"{db2}.dbc", StringComparison.OrdinalIgnoreCase));

                        if (fileName != null)
                            fs = System.IO.File.OpenRead(fileName);
                    }
                }

                // No dbc/db2 found. No need to go any further.
                if (fs == null)
                {
                    result.error = "Could not find DBCs on disk for build " + build + "!";
                    return result;
                }


                using (var bin = new BinaryReader(fs))
                {
                    var db2Info = new List<string>();

                    var magic = new string(bin.ReadChars(4));

                    db2Info.Add(db2); // name

                    if (magic == "1SLC" || magic == "WDC2" || magic == "WDC3" || magic == "WDC4" || magic == "WDC5")
                    {
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
                    }
                    else if (magic == "WDC1")
                    {
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // fieldCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordSize
                        bin.ReadUInt32(); // stringTableSize
                        db2Info.Add(bin.ReadUInt32().ToString("X8")); // tableHash
                        db2Info.Add(bin.ReadUInt32().ToString("X8")); // layoutHash
                        db2Info.Add(bin.ReadInt32().ToString()); // minId
                        db2Info.Add(bin.ReadUInt32().ToString()); // maxId
                        db2Info.Add(bin.ReadInt32().ToString()); // locale
                        bin.ReadUInt32(); // copy table size
                        db2Info.Add(bin.ReadUInt16().ToString()); // flags
                        db2Info.Add(bin.ReadUInt16().ToString()); // idIndex
                        db2Info.Add(bin.ReadUInt32().ToString()); // totalFields
                        db2Info.Add("N/A"); // sectionCount, not in WDC1
                    }
                    else if (magic == "WDB6")
                    {
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // fieldCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordSize
                        bin.ReadUInt32(); // stringTableSize
                        db2Info.Add(bin.ReadUInt32().ToString("X8")); // tableHash
                        db2Info.Add(bin.ReadUInt32().ToString("X8")); // layoutHash
                        db2Info.Add(bin.ReadInt32().ToString()); // minId
                        db2Info.Add(bin.ReadUInt32().ToString()); // maxId
                        db2Info.Add(bin.ReadInt32().ToString()); // locale
                        bin.ReadUInt32(); // copy table size
                        db2Info.Add(bin.ReadUInt16().ToString()); // flags
                        db2Info.Add(bin.ReadUInt16().ToString()); // idIndex
                        db2Info.Add(bin.ReadUInt32().ToString()); // totalFields
                        db2Info.Add("N/A"); // sectionCount, not in WDC1
                    }
                    else if (magic == "WDB5")
                    {
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // fieldCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordSize
                        bin.ReadUInt32(); // stringTableSize
                        db2Info.Add(bin.ReadUInt32().ToString("X8")); // tableHash
                        db2Info.Add(bin.ReadUInt32().ToString("X8")); // layoutHash
                        db2Info.Add(bin.ReadInt32().ToString()); // minId
                        db2Info.Add(bin.ReadUInt32().ToString()); // maxId
                        db2Info.Add(bin.ReadInt32().ToString()); // locale
                        bin.ReadUInt32(); // copy table size
                        db2Info.Add(bin.ReadUInt16().ToString()); // flags
                        db2Info.Add(bin.ReadUInt16().ToString()); // idIndex
                        db2Info.Add("N/A"); // totalFields, not in <WDB5
                        db2Info.Add("N/A"); // sectionCount, not in <WDC2
                    }
                    else if (magic == "WDB4")
                    {
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // fieldCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordSize
                        bin.ReadUInt32(); // stringTableSize
                        db2Info.Add(bin.ReadUInt32().ToString("X8")); // tableHash
                        db2Info.Add("N/A"); // layoutHash, not available in <WDB4
                        bin.ReadUInt32();
                        bin.ReadUInt32();
                        db2Info.Add(bin.ReadInt32().ToString()); // minId
                        db2Info.Add(bin.ReadUInt32().ToString()); // maxId
                        db2Info.Add(bin.ReadInt32().ToString()); // locale
                        bin.ReadUInt32(); // copy table size
                        db2Info.Add(bin.ReadUInt16().ToString()); // flags
                        db2Info.Add("N/A"); // idIndex, not in <WDB4
                        db2Info.Add("N/A"); // totalFields, not in <WDB5
                        db2Info.Add("N/A"); // sectionCount, not in <WDC2
                    }
                    else if(magic == "WDB2" || magic == "WDB3")
                    {
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // fieldCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordSize
                        bin.ReadUInt32(); // stringTableSize
                        db2Info.Add(bin.ReadUInt32().ToString("X8")); // tableHash
                        db2Info.Add("N/A"); // layoutHash, not available in <WDB4
                        bin.ReadUInt32();
                        db2Info.Add(bin.ReadInt32().ToString()); // minId
                        db2Info.Add(bin.ReadUInt32().ToString()); // maxId
                        db2Info.Add(bin.ReadInt32().ToString()); // locale
                        bin.ReadUInt32(); // copy table size
                        db2Info.Add("N/A"); // flags
                        db2Info.Add("N/A"); // idIndex, not in <WDB4
                        db2Info.Add("N/A"); // totalFields, not in <WDB5
                        db2Info.Add("N/A"); // sectionCount, not in <WDC2
                    }else if(magic == "WDBC")
                    {
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // fieldCount
                        db2Info.Add(bin.ReadUInt32().ToString()); // recordSize
                        bin.ReadUInt32(); // stringTableSize
                        db2Info.Add("N/A");
                        db2Info.Add("N/A"); // layoutHash, not available in <WDB4
                        db2Info.Add("N/A");
                        db2Info.Add("N/A");
                        db2Info.Add("N/A");
                        db2Info.Add("N/A"); // flags
                        db2Info.Add("N/A"); // idIndex, not in <WDB4
                        db2Info.Add("N/A"); // totalFields, not in <WDB5
                        db2Info.Add("N/A"); // sectionCount, not in <WDC2
                    }

                    db2Info.Add(magic);

                    result.data.Add([.. db2Info]);
                }

                fs.Dispose();
            }

            return result;
        }
    }
}
