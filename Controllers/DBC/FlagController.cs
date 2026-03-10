using DBCD.Providers;
using DBDefsLib.Structs;
using Microsoft.AspNetCore.Mvc;
using wow.tools.local.Providers;

namespace wow.tools.local.Controllers.DBC
{
    [Route("dbc/flag")]
    [ApiController]
    public class FlagController(IEnumProvider enumProvider) : ControllerBase
    {
        private readonly EnumProvider enumProvider = (EnumProvider)enumProvider;

        [Route("get")]
        [HttpGet]
        public async Task<EnumDefinition?> GetFlagsByColumn(string tableName, string columnName)
        {
            if (columnName.Contains('['))
            {
                var columnSplit = columnName.Split('[');
                var splitColumnName = columnSplit[0];
                var splitArrayIndex = columnSplit[1].Substring(0, columnSplit[1].Length - 1);
                var definition = enumProvider.GetArrayEnumDefinitions(tableName, splitColumnName);
                return definition != null && definition.TryGetValue(int.Parse(splitArrayIndex), out var enumDef) ? enumDef : null;
            }
            else
            {
                return enumProvider.GetEnumDefinition(tableName, columnName);
            }
        }
    }
}
