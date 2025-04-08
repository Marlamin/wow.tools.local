using DBCD;
using DBCD.Providers;
using wow.tools.local.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wow.tools.local;
using wow.tools.Services;

namespace wow.tools.local.Controllers.DBC
{
    [Route("dbc/header")]
    [ApiController]
    public class HeaderController(IDBCManager dbcManager, IDBDProvider dbdProvider) : ControllerBase
    {
        public class HeaderResult
        {
            public List<string> headers { get; set; }
            public Dictionary<string, string> fks { get; set; }
            public Dictionary<string, string> comments { get; set; }
            public List<string> unverifieds { get; set; }

            public Dictionary<string, List<string>> relationsToColumns { get; set; }
            public string error { get; set; }
        }

        private readonly DBCManager dbcManager = (DBCManager)dbcManager;
        private readonly DBDProvider dbdProvider = (DBDProvider)dbdProvider;

        // GET: api/DBC
        [HttpGet]
        public string Get()
        {
            return "No DBC selected!";
        }

        // GET: api/DBC/name
        [ResponseCache(NoStore = true, Duration = 0)]
        [HttpGet("{name}")]
        public async Task<HeaderResult> Get(string name, string build)
        {
            Console.WriteLine("Serving headers for " + name + " (" + build + ")");
            if (build == "?")
                build = CASC.BuildName;

            var result = new HeaderResult();
            try
            {
                var storage = await dbcManager.GetOrLoad(name, build);

                if (!dbdProvider.TryGetDefinition(name, out var definition))
                {
                    throw new KeyNotFoundException("Definition for " + name);
                }

                result.headers = [];
                result.fks = [];
                result.comments = [];
                result.unverifieds = [];

                if (storage.Values.Count == 0)
                {
                    for (var j = 0; j < storage.AvailableColumns.Length; ++j)
                    {
                        var fieldName = storage.AvailableColumns[j];
                        result.headers.Add(fieldName);

                        if (definition.columnDefinitions.TryGetValue(fieldName, out var columnDef))
                        {
                            if (!string.IsNullOrEmpty(columnDef.foreignTable))
                                result.fks.Add(fieldName, columnDef.foreignTable + "::" + columnDef.foreignColumn);

                            if (columnDef.comment != null)
                                result.comments.Add(fieldName, columnDef.comment);

                            if (!columnDef.verified)
                                result.unverifieds.Add(fieldName);
                        }
                    }
                }
                else
                {
                    foreach (DBCDRow item in storage.Values)
                    {
                        for (var j = 0; j < storage.AvailableColumns.Length; ++j)
                        {
                            string fieldName = storage.AvailableColumns[j];
                            var field = item[fieldName];

                            if (field is Array a)
                            {
                                for (var i = 0; i < a.Length; i++)
                                {
                                    result.headers.Add($"{fieldName}[{i}]");

                                    if (definition.columnDefinitions.TryGetValue(fieldName, out var columnDef))
                                    {
                                        if (!string.IsNullOrEmpty(columnDef.foreignTable))
                                            result.fks.Add($"{fieldName}[{i}]", columnDef.foreignTable + "::" + columnDef.foreignColumn);

                                        if (columnDef.comment != null)
                                            result.comments.Add($"{fieldName}[{i}]", columnDef.comment);

                                        if (!columnDef.verified)
                                            result.unverifieds.Add($"{fieldName}[{i}]");
                                    }
                                }
                            }
                            else
                            {
                                result.headers.Add(fieldName);

                                if (definition.columnDefinitions.TryGetValue(fieldName, out var columnDef))
                                {
                                    if (!string.IsNullOrEmpty(columnDef.foreignTable))
                                        result.fks.Add(fieldName, columnDef.foreignTable + "::" + columnDef.foreignColumn);

                                    if (columnDef.comment != null)
                                        result.comments.Add(fieldName, columnDef.comment);

                                    if (!columnDef.verified)
                                        result.unverifieds.Add(fieldName);
                                }
                            }
                        }

                        break;
                    }
                }

                result.relationsToColumns = [];

                foreach (var column in result.headers)
                {
                    var relationsToCol = dbdProvider.GetRelationsToColumn(name + "::" + column, true);
                    if (relationsToCol.Count > 0)
                    {
                        result.relationsToColumns.Add(column, relationsToCol);
                    }
                }
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