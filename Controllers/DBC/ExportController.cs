using DBCD;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Collections.Generic;
using System.Net;
using wow.tools.local.Services;
using wow.tools.local;
using CASCLib;
using DBDefsLib;

namespace wow.tools.Local.Controllers
{
    using Parameters = IReadOnlyDictionary<string, string>;

    [Route("dbc/export")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private static readonly Parameters DefaultParameters = new Dictionary<string, string>();
        private static readonly char[] QuoteableChars = new char[] { ',', '"', '\r', '\n' };

        private readonly DBCManager dbcManager;

        public ExportController(IDBCManager dbcManager)
        {
            this.dbcManager = dbcManager as DBCManager;
        }

        private async Task<byte[]> GenerateCSVStream(IDBCDStorage storage, Parameters parameters, bool newLinesInStrings = true)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            
            if (storage.AvailableColumns.Length == 0)
            {
                throw new Exception("No columns found!");
            }

            // NOTE: if newLinesInStrings is obsolete then use StringToCSVCell in ctor
            Func<string, string> formatter = newLinesInStrings switch
            {
                true => StringToCSVCell,
                _ => StringToCSVCellSingleLine
            };

            var viewFilter = new DBCViewFilter(storage, parameters, formatter);

            using var exportStream = new MemoryStream();
            using var exportWriter = new StreamWriter(exportStream);

            // write header
            await exportWriter.WriteLineAsync(string.Join(",", GetColumnNames(storage)));

            // write records
            foreach (var item in viewFilter.GetRecords())
                await exportWriter.WriteLineAsync(string.Join(",", item));

            exportWriter.Flush();

            return exportStream.ToArray();
        }

        [Route("")]
        [Route("csv")]
        [HttpGet, HttpPost]
        public async Task<ActionResult> ExportCSV(string name, string build, bool useHotfixes = false, bool newLinesInStrings = true, LocaleFlags locale = LocaleFlags.All_WoW)
        {
            Console.WriteLine("Exporting DBC " + name + " as CSV for build " + build + " and locale " + locale);

            var parameters = DefaultParameters;

            if (Request.Method == "POST")
                parameters = Request.Form.ToDictionary(x => x.Key, x => (string)x.Value);

            try
            {
                var storage = await GetStorage(name, build, useHotfixes, locale);

                if (storage.Count == 0)
                {
                    return NoContent();
                }

                return new FileContentResult(await GenerateCSVStream(storage, parameters, newLinesInStrings), "application/octet-stream")
                {
                    FileDownloadName = Path.ChangeExtension(name, ".csv")
                };
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("DBC " + name + " for build " + build + " not found: " + e.Message);
                return NotFound();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during CSV generation for DBC " + name + " for build " + build + ": " + e.Message);
                return BadRequest();
            }
        }

        [Route("alltodisk")]
        [HttpGet]
        public async Task<bool> ExportAllToDisk()
        {
            Console.WriteLine("Exporting all DBCs from current build to disk");
           
            foreach (var dbname in dbcManager.GetDBCNames(CASC.BuildName))
            {
                try
                {
                    var cleanName = dbname.ToLower();
                    var filestream = CASC.GetFileByName("dbfilesclient/" + dbname + ".db2");

                    if (!Directory.Exists(Path.Combine(SettingsManager.dbcFolder, CASC.BuildName, "dbfilesclient")))
                    {
                        Directory.CreateDirectory(Path.Combine(SettingsManager.dbcFolder, CASC.BuildName, "dbfilesclient"));
                    }

                    using (var exportStream = new FileStream(Path.Combine(SettingsManager.dbcFolder, CASC.BuildName, "dbfilesclient", cleanName + ".db2"), FileMode.Create))
                    {
                        filestream.CopyTo(exportStream);
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Table " + dbname + " not found in build");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message + " occured when extracting DB2 " + dbname);
                }
            }

            return true;
        }
        
        [Route("all")]
        [HttpGet]
        public async Task<ActionResult> ExportAllCSV(string? build, bool useHotfixes = false, bool newLinesInStrings = true, LocaleFlags locale = LocaleFlags.All_WoW)
        {
            Console.WriteLine("Exporting all DBCs build " + build + " and locale " + locale);
            if (build == null)
                build = CASC.BuildName;
            
            using (var zip = new MemoryStream())
            {
                using (var archive = new ZipArchive(zip, ZipArchiveMode.Create))
                {
                    // TODO: Get list of DBCs for a specific build
                    foreach (var dbname in dbcManager.GetDBCNames(build))
                    {
                        try
                        {
                            var cleanName = dbname.ToLower();

                            var storage = await GetStorage(cleanName, build, useHotfixes, locale);

                            using (var exportStream = new MemoryStream(await GenerateCSVStream(storage, DefaultParameters, newLinesInStrings)))
                            {
                                var entryname = cleanName + ".csv";
                                var entry = archive.CreateEntry(entryname);
                                using (var entryStream = entry.Open())
                                {
                                    exportStream.CopyTo(entryStream);
                                }
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine("Table " + dbname + " not found in build " + build);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error " + e.Message + " occured when getting table " + dbname + " of build " + build);
                        }
                    }
                }

                return new FileContentResult(zip.ToArray(), "application/octet-stream")
                {
                    FileDownloadName = "alldbc-" + build + ".zip"
                };
            }
        }

        [Route("db2")]
        [HttpGet]
        public async Task<ActionResult> GetDB2ByTableName(string tableName, string fullBuild)
        {
            var provider = new DBCProvider();

            Console.WriteLine("Serving DB2 \"" + tableName + "\" for build " + fullBuild);

            try
            {

                if (fullBuild == CASC.BuildName)
                {
                    // Load from CASC
                    var fullFileName = "dbfilesclient/" + tableName + ".db2";
                    return new FileStreamResult(CASC.GetFileByName(fullFileName), "application/octet-stream")
                    {
                        FileDownloadName = Path.GetFileName(tableName.ToLower() + ".db2")
                    };
                }
                else
                {
                    var extension = "";

                    string fileName = Path.Combine(SettingsManager.dbcFolder, fullBuild, "dbfilesclient", $"{tableName}.db2");

                    if (System.IO.File.Exists(fileName))
                    {
                        extension = "db2";
                    }
                    else
                    {
                        fileName = Path.ChangeExtension(fileName, ".dbc");

                        if (!System.IO.File.Exists(fileName))
                            throw new FileNotFoundException($"Unable to find {tableName}, are DB2s for this build extracted?");

                        extension = "dbc";
                    }

                    using (var stream = provider.StreamForTableName(tableName, fullBuild))
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        return new FileContentResult(ms.ToArray(), "application/octet-stream")
                        {
                            FileDownloadName = Path.GetFileName(tableName.ToLower() + "." + extension)
                        };
                    }
                }
                    
            }
            catch (FileNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Table " + tableName + " not found for build " + fullBuild);
                Console.ResetColor();
                return NotFound();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error " + e.Message + " occured when serving file " + tableName + " for build " + fullBuild);
                Console.ResetColor();
            }

            return NotFound();
        }
        private async Task<IDBCDStorage> GetStorage(string name, string build, bool useHotfixes = false, LocaleFlags locale = LocaleFlags.All_WoW)
        {
            return await dbcManager.GetOrLoad(name, build, useHotfixes, locale);
        }

        private IEnumerable<string> GetColumnNames(IDBCDStorage storage)
        {
            var record = storage.Values.FirstOrDefault();

            if (record == null)
                yield break;

            for (var i = 0; i < storage.AvailableColumns.Length; ++i)
            {
                var name = storage.AvailableColumns[i];

                if (record[name] is Array array)
                {
                    // explode arrays by suffixing the ordinal
                    for (var j = 0; j < array.Length; j++)
                        yield return name + $"[{j}]";
                }
                else
                {
                    yield return name;
                }
            }
        }

        private static string StringToCSVCell(string str)
        {
            var mustQuote = str.IndexOfAny(QuoteableChars) > -1;
            if (mustQuote)
                return '"' + str.Replace("\"", "\"\"") + '"';

            return str;
        }

        private static string StringToCSVCellSingleLine(string str)
        {
            return StringToCSVCell(str.Replace("\n", "").Replace("\r", ""));
        }
    }
}