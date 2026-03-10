using DBCD.Providers;
using DBDefsLib.Structs;

namespace wow.tools.local.Providers
{
    public class EnumProvider : IEnumProvider
    {
        private GithubEnumProvider? githubEnumProvider = null;
        private FilesystemEnumProvider? filesystemEnumProvider = null;
        public List<MappingDefinition> Mappings { get; private set; }
        public bool isUsingBDBD = false;

        public EnumProvider()
        {
            // BDBD does not yet support enums/flags, fall back to github provider
            if (string.IsNullOrEmpty(SettingsManager.DefinitionDir) || !Directory.Exists(SettingsManager.DefinitionDir))
            {
                isUsingBDBD = true;
                githubEnumProvider = new GithubEnumProvider(true);
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
                    githubEnumProvider = new GithubEnumProvider();
                    Mappings = githubEnumProvider.Mappings;
                }
            }
        }
        public IReadOnlyDictionary<int?, EnumDefinition>? GetArrayEnumDefinitions(string tableName, string columnName)
        {
            if (isUsingBDBD)
                return githubEnumProvider?.GetArrayEnumDefinitions(tableName, columnName);
            else
                return filesystemEnumProvider?.GetArrayEnumDefinitions(tableName, columnName);
        }

        public EnumDefinition? GetEnumDefinition(string tableName, string columnName)
        {
            if (isUsingBDBD)
                return githubEnumProvider?.GetEnumDefinition(tableName, columnName);
            else
                return filesystemEnumProvider?.GetEnumDefinition(tableName, columnName);
        }

        public void ClearCache()
        {
            // This almost definitely isn't the way to do this
            if (isUsingBDBD)
            {
                githubEnumProvider = new(true);
                Mappings = githubEnumProvider.Mappings;
            }
            else
            {
                filesystemEnumProvider = new(Path.Combine(SettingsManager.DefinitionDir, "..", "meta", "mapping.dbdm"));
                Mappings = filesystemEnumProvider.Mappings;
            }
        }
    }
}
