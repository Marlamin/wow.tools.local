using DBCD.Providers;
using DBDefsLib;
using DBDefsLib.Constants;
using DBDefsLib.Structs;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Providers;

namespace wow.tools.local.Controllers.DBC
{
    [Route("dbc/meta")]
    [ApiController]
    public class MetaController(IEnumProvider enumProvider) : ControllerBase
    {
        private readonly EnumProvider enumProvider = (EnumProvider)enumProvider;
        private static bool EntryMatchesBuild(EnumEntry entry, Build currentBuild)
        {
            if (currentBuild == null || (entry.builds.Length == 0 && entry.buildRanges.Length == 0))
                return true;

            if (entry.builds.Any(b => b.Equals(currentBuild)))
                return true;

            return entry.buildRanges.Any(r => r.Contains(currentBuild));
        }

        [Route("getMappings")]
        [HttpGet]
        public async Task<List<MappingWithEntries>> GetMappings(string? tableName = null, string? build = null)
        {
            var mappings = string.IsNullOrEmpty(tableName)
                ? enumProvider.Mappings
                : enumProvider.Mappings.Where(x => x.tableName.ToLower() == tableName.ToLower()).ToList();

            return mappings.Select(m =>
            {
                List<EnumEntry>? entries = null;
                if (m.meta is MetaType.ENUM or MetaType.FLAGS)
                {
                    var def = enumProvider.GetEnumDefinition(m.tableName, m.columnName, m.arrIndex,
                        m.conditionalTable, m.conditionalColumn, m.conditionalValue);

                    if (build == null)
                    {
                        entries = def?.entries;
                    }
                    else
                    {
                        var dbdBuild = new Build(build);

                        if (def != null)
                        {
                            foreach (var entry in def?.entries!)
                            {
                                entries ??= new List<EnumEntry>();

                                if (!EntryMatchesBuild(entry, dbdBuild))
                                    continue;

                                entries.Add(entry);
                            }
                        }
                    }
                }

                return new MappingWithEntries(m.meta, m.tableName, m.columnName, m.arrIndex,
                    m.conditionalTable, m.conditionalColumn, m.conditionalValue, entries);
            }).ToList();
        }

        [Route("getMeta")]
        [HttpGet]
        public async Task<EnumDefinition?> GetMeta(string tableName, string columnName)
        {
            int? arrayIndex = null;

            if (columnName.Contains('['))
            {
                var columnSplit = columnName.Split('[');
                var splitArrayIndex = columnSplit[1][..^1];

                arrayIndex = int.Parse(splitArrayIndex);
                columnName = columnSplit[0];
            }

            Console.WriteLine($"Requesting Table: {tableName} and Column: {columnName} (Array: {arrayIndex})");
            return enumProvider.GetEnumDefinition(tableName, columnName, arrayIndex);
        }
    }

    public record MappingWithEntries(
        MetaType meta,
        string tableName,
        string columnName,
        int? arrIndex,
        string conditionalTable,
        string conditionalColumn,
        string conditionalValue,
        IList<EnumEntry>? entries   // null for Color/Date (meta 2/3)
    );
}
