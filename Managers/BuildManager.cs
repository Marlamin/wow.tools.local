using TACTSharp;
using wow.tools.local.Services;

namespace wow.tools.local.Managers
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
                    Builds[buildConfig] = LoadBuildByBuildConfig(buildConfig);

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

        public static BuildInstance LoadBuild(string product, string buildConfig = "", string cdnConfig = "", string productConfig = "")
        {
            // If we have a build config, try and load metadata from database
            if (!string.IsNullOrEmpty(buildConfig) && (string.IsNullOrEmpty(cdnConfig) || string.IsNullOrEmpty(productConfig)))
            {
                var buildMeta = SQLiteDB.GetBuildInfoByBuildConfig(buildConfig);
                if (buildMeta != null)
                {
                    if (string.IsNullOrEmpty(cdnConfig))
                        cdnConfig = buildMeta.cdnConfig;

                    if (string.IsNullOrEmpty(productConfig))
                        productConfig = buildMeta.productConfig;
                }
            }

            var buildInstance = new BuildInstance();

            // If any metadata is still missing, try and load it from the versions file on the CDN
            if (string.IsNullOrEmpty(buildConfig) || string.IsNullOrEmpty(cdnConfig) || string.IsNullOrEmpty(productConfig))
            {
                var versions = buildInstance.cdn.GetPatchServiceFile(product, "versions").Result;
                foreach (var line in versions.Split('\n'))
                {
                    if (!line.StartsWith(buildInstance.Settings.Region + "|"))
                        continue;

                    var splitLine = line.Split('|');

                    if (string.IsNullOrEmpty(buildConfig))
                        buildConfig = splitLine[1];

                    if (string.IsNullOrEmpty(cdnConfig))
                        cdnConfig = splitLine[2];

                    if (splitLine.Length >= 7 && !string.IsNullOrEmpty(splitLine[6]) && string.IsNullOrEmpty(productConfig))
                        productConfig = splitLine[6];
                }
            }

            // If any metadata is still missing, try and load it from local folder's .build.info (if available)
            if (!string.IsNullOrEmpty(SettingsManager.WoWFolder) && (string.IsNullOrEmpty(buildConfig) || string.IsNullOrEmpty(cdnConfig) || string.IsNullOrEmpty(productConfig)))
            {
                var buildInfoPath = Path.Combine(SettingsManager.WoWFolder, ".build.info");
                if (File.Exists(buildInfoPath))
                {
                    var buildInfo = new BuildInfo(buildInfoPath, buildInstance.Settings, buildInstance.cdn);

                    if (!buildInfo.Entries.Any(x => x.Product == product))
                    {
                        Console.WriteLine("No .build.info found for product " + product + ", falling back to online mode.");
                    }
                    else
                    {
                        var build = buildInfo.Entries.First(x => x.Product == product);

                        if (string.IsNullOrEmpty(buildConfig))
                            buildConfig = build.BuildConfig;

                        if (string.IsNullOrEmpty(cdnConfig))
                            cdnConfig = build.CDNConfig;

                        if (!string.IsNullOrEmpty(build.Armadillo))
                            buildInstance.cdn.ArmadilloKeyName = build.Armadillo;
                    }
                }
            }

            // If any metadata is still missing, try and load it from the versions file on the CDN
            if (string.IsNullOrEmpty(buildConfig) || string.IsNullOrEmpty(cdnConfig) || string.IsNullOrEmpty(productConfig))
            {
                var versions = buildInstance.cdn.GetPatchServiceFile(product, "versions").Result;
                foreach (var line in versions.Split('\n'))
                {
                    if (!line.StartsWith(buildInstance.Settings.Region + "|"))
                        continue;

                    var splitLine = line.Split('|');

                    if (string.IsNullOrEmpty(buildConfig))
                        buildConfig = splitLine[1];

                    if (string.IsNullOrEmpty(cdnConfig))
                        cdnConfig = splitLine[2];

                    if (splitLine.Length >= 7 && !string.IsNullOrEmpty(splitLine[6]) && string.IsNullOrEmpty(productConfig))
                        productConfig = splitLine[6];
                }
            }

            buildInstance.Settings.Product = product;
            buildInstance.Settings.BuildConfig = buildConfig;
            buildInstance.Settings.CDNConfig = cdnConfig;
            buildInstance.Settings.BaseDir = SettingsManager.WoWFolder;
            buildInstance.Settings.Locale = SettingsManager.TACTLocale;
            buildInstance.Settings.Region = SettingsManager.Region;
            buildInstance.Settings.RootMode = RootInstance.LoadMode.Full;

            if (!string.IsNullOrEmpty(SettingsManager.CDNFolder))
                buildInstance.Settings.CDNDir = SettingsManager.CDNFolder;

            if (SettingsManager.AdditionalCDNs.Length > 0 && !string.IsNullOrEmpty(SettingsManager.AdditionalCDNs[0]))
                buildInstance.Settings.AdditionalCDNs.AddRange(SettingsManager.AdditionalCDNs);

            var cdns = buildInstance.cdn.GetPatchServiceFile(product, "cdns").Result;
            foreach (var line in cdns.Split('\n'))
            {
                if (!line.StartsWith(buildInstance.Settings.Region + "|"))
                    continue;

                var splitLine = line.Split('|');
                var productDir = splitLine[1];

                buildInstance.cdn.ProductDirectory = productDir;
            }

            if (string.IsNullOrEmpty(buildInstance.cdn.ProductDirectory))
                buildInstance.cdn.ProductDirectory = "tpr/wow";

            buildInstance.cdn.OpenLocal();

            buildInstance.LoadConfigs(buildConfig, cdnConfig, productConfig);

            if (buildInstance.BuildConfig == null || buildInstance.CDNConfig == null)
                throw new Exception("Failed to load build configs");

            buildInstance.Load();

            if (buildInstance.Encoding == null || buildInstance.Root == null || buildInstance.Install == null || buildInstance.GroupIndex == null)
                throw new Exception("Failed to load build components");

            return buildInstance;
        }

        private static BuildInstance LoadBuildByBuildConfig(string buildConfig)
        {
            var buildMeta = SQLiteDB.GetBuildInfoByBuildConfig(buildConfig);
            if (buildMeta == null)
                throw new Exception($"Build config '{buildConfig}' not found in database, could not load build.");

            Console.WriteLine("Initializing TACTSharp instance for archived build " + buildMeta.version + "...");

            return LoadBuild(buildMeta.product, buildMeta.buildConfig, buildMeta.cdnConfig, buildMeta.productConfig);
        }
    }
}
