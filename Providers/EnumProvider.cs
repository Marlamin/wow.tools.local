using DBCD.Providers;
using DBDefsLib;
using DBDefsLib.Structs;

namespace wow.tools.local.Providers
{
    public class EnumProvider : IEnumProvider
    {
        private FilesystemEnumProvider? filesystemEnumProvider = null;
        public List<MappingDefinition> Mappings { get; private set; }
        public Dictionary<string, EnumDefinition> EnumDefinitions { get; private set; }
        public bool isUsingBDBD = false;

        public EnumProvider()
        {
            // BDBD does not yet support enums/flags, fall back to github provider
            if (string.IsNullOrEmpty(SettingsManager.DefinitionDir) || !Directory.Exists(SettingsManager.DefinitionDir))
            {
                isUsingBDBD = true;
                LoadBDBD();
            }
            else
            {
                var mappingPath = Path.Combine(SettingsManager.DefinitionDir, "..", "meta", "mapping.dbdm");
                if (File.Exists(mappingPath))
                {
                    filesystemEnumProvider = new FilesystemEnumProvider(mappingPath);
                    Mappings = filesystemEnumProvider.Mappings;
                }
                else
                {
                    isUsingBDBD = true;
                    LoadBDBD();
                }
            }
        }

        private void LoadBDBD(bool clearCache = false)
        {
            Console.WriteLine("Loading definitions from BDBD file");
            using (var fs = DBDProvider.GetBDBDStream(clearCache))
            {
                var bdbd = BDBDReader.Read(fs);
                Mappings = bdbd.enumMappings;
                EnumDefinitions = bdbd.enumDefinitions;

                Console.WriteLine("Loaded " + Mappings.Count + " enum mappings and " + EnumDefinitions.Count + " enum definitions from BDBD file!");
            }
        }

        public EnumDefinition? GetEnumDefinition(string tableName, string columnName, int? arrayIndex = null,
            string? conditionalTable = null, string? conditionalColumn = null, string? conditionalValue = null)
        {
            if (isUsingBDBD)
            {
                var relevantMappings = Mappings.Where(m => m.tableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && m.columnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                if (!relevantMappings.Any())
                    return null;

                // If conditional context supplied, try conditional key first
                if (!string.IsNullOrEmpty(conditionalTable))
                {
                    var conditionalMapping = relevantMappings.Where(m => m.conditionalTable.Equals(conditionalTable, StringComparison.OrdinalIgnoreCase)
                                && m.conditionalColumn.Equals(conditionalColumn, StringComparison.OrdinalIgnoreCase)
                                && m.conditionalValue.Equals(conditionalValue, StringComparison.OrdinalIgnoreCase));

                    if (conditionalMapping.Any() && EnumDefinitions.TryGetValue(conditionalMapping.First().metaValue, out var enumDefinition))
                        return enumDefinition;
                }

                // Fall back to unconditional (handles arrayIndex fallback too)
                if (arrayIndex.HasValue && relevantMappings.Where(m => m.arrIndex == arrayIndex.Value).Any())
                {
                    if (EnumDefinitions.TryGetValue(relevantMappings.Where(m => m.arrIndex == arrayIndex.Value).First().metaValue, out var enumDefinition))
                        return enumDefinition;
                }
                else
                {
                    if (EnumDefinitions.TryGetValue(relevantMappings.First().metaValue, out var enumDefinition))
                        return enumDefinition;
                    else
                        return null;
                }

                return null;
            }
            else
            {
                return filesystemEnumProvider?.GetEnumDefinition(tableName, columnName, arrayIndex, conditionalTable, conditionalColumn, conditionalValue);
            }
        }

        public void ClearCache()
        {
            if (isUsingBDBD)
            {
                LoadBDBD(true);
            }
            else
            {
                // This almost definitely isn't the way to do this
                filesystemEnumProvider = new(Path.Combine(SettingsManager.DefinitionDir, "..", "meta", "mapping.dbdm"));
                Mappings = filesystemEnumProvider.Mappings;
            }
        }
    }
}
