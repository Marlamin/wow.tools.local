using System.Text.Json;
using System.Text.Json.Serialization;

namespace wow.tools.local.Utils
{
    public static class WTLJsonSettings
    {
        public static readonly JsonSerializerOptions Options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true,
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
