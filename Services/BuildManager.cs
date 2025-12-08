using TACTSharp;

namespace wow.tools.local.Services
{
    public static class BuildManager
    {
        private static Dictionary<string, BuildInstance> Builds = new();
        private static Dictionary<string, string> VersionToConfig = new();

        private static readonly object BuildLock = new();

        public static BuildInstance GetBuildByConfig(string buildConfig)
        {
            if (string.IsNullOrEmpty(buildConfig))
                throw new ArgumentException("Build name cannot be null or empty.", nameof(buildConfig));

            lock (BuildLock)
            {
                if (!Builds.TryGetValue(buildConfig, out var buildInstance))
                    Builds[buildConfig] = LoadBuild(buildConfig);

                return Builds[buildConfig];
            }
        }

        public static BuildInstance GetBuildByVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentException("Version cannot be null or empty.", nameof(version));

            lock (BuildLock)
            {
                if (!VersionToConfig.TryGetValue(version, out var buildConfig))
                {
                    var buildMeta = SQLiteDB.GetBuildInfoByVersion(version);
                    if (buildMeta == null)
                        throw new Exception($"Version '{version}' not found in database, could not load build.");

                    VersionToConfig[version] = buildMeta.buildConfig;
                    buildConfig = buildMeta.buildConfig;
                }

                return GetBuildByConfig(buildConfig);
            }
        }

        private static BuildInstance LoadBuild(string buildConfig)
        {
            var buildMeta = SQLiteDB.GetBuildInfoByBuildConfig(buildConfig);
            if (buildMeta == null)
                throw new Exception($"Build config '{buildConfig}' not found in database, could not load build.");

            Console.WriteLine("Initializing TACTSharp instance for " + buildMeta.version + "...");
            var buildInstance = new BuildInstance();

            buildInstance.Settings.Product = buildMeta.product;
            buildInstance.Settings.BuildConfig = buildMeta.buildConfig;
            buildInstance.Settings.CDNConfig = buildMeta.cdnConfig;
            buildInstance.Settings.Locale = SettingsManager.TACTLocale;
            buildInstance.Settings.Region = SettingsManager.Region;
            buildInstance.Settings.RootMode = RootInstance.LoadMode.Normal;
            buildInstance.Settings.CDNDir = SettingsManager.CDNFolder;

            if (SettingsManager.AdditionalCDNs.Length > 0 && !string.IsNullOrEmpty(SettingsManager.AdditionalCDNs[0]))
                buildInstance.Settings.AdditionalCDNs.AddRange(SettingsManager.AdditionalCDNs);

            if (!string.IsNullOrEmpty(SettingsManager.WoWFolder))
                buildInstance.Settings.BaseDir = SettingsManager.WoWFolder;

            buildInstance.LoadConfigs(buildConfig, buildMeta.cdnConfig);

            if (buildInstance.BuildConfig == null || buildInstance.CDNConfig == null)
                throw new Exception("Failed to load build configs");

            buildInstance.Load();

            if (buildInstance.Encoding == null || buildInstance.Root == null || buildInstance.Install == null || buildInstance.GroupIndex == null)
                throw new Exception("Failed to load build components");

            return buildInstance;
        }
    }
}
