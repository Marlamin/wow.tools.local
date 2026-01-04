using System.Runtime.InteropServices;
using TACTSharp;

namespace wow.tools.local
{
    public record WTLSetting
    {
        public required string Key { get; init; }
        public required string Description { get; init; }
        public required string Type { get; init; } // e.g. "string", "bool", "int", etc.
        public required string DefaultValue { get; init; } // default value for the setting
        public required string Value { get; set; }
        public bool Ephemeral { get; set; } // ephemeral settings shouldn't persist between sessions
    };


    public static class SettingsManager
    {
        public static readonly Dictionary<string, WTLSetting> Settings = new Dictionary<string, WTLSetting>
        {
            {"definitionDir", new WTLSetting { Key = "definitionDir", Value = string.Empty, Description = "Directory where the definition files (.dbd's) are stored. If not set they are automatically periodically downloaded from GitHub.", Type = "string", DefaultValue = string.Empty }},
            {"listfileURL", new WTLSetting { Key = "listfileURL", Value = string.Empty, Description = "URL to the CSV listfile. If you have the wowdev/wow-listfile repository locally, this can also be the path of the 'parts' directory.", Type = "string", DefaultValue = "https://github.com/wowdev/wow-listfile/releases/latest/download/community-listfile-withcapitals.csv" }},
            {"tactKeyURL", new WTLSetting { Key = "tactKeyURL", Value = string.Empty, Description = "URL to the TACT key TXT file.", Type = "string", DefaultValue = "https://github.com/wowdev/TACTKeys/raw/master/WoW.txt" }},
            {"wowFolder", new WTLSetting { Key = "wowFolder", Value = string.Empty, Description = "Path to the WoW installation folder.", Type = "string", DefaultValue = string.Empty }},
            {"dbcFolder", new WTLSetting { Key = "dbcFolder", Value = string.Empty, Description = "Path to the DBC folder.", Type = "string", DefaultValue = "dbcs" }},
            {"manifestFolder", new WTLSetting { Key = "manifestFolder", Value = "manifests", Description = "Folder where manifests are stored.", Type = "string", DefaultValue = "manifests" }},
            {"extractionDir", new WTLSetting { Key = "extractionDir", Value = "extract", Description = "Directory where files will be extracted to.", Type = "string", DefaultValue = "extract" }},
            {"cdnFolder", new WTLSetting { Key = "cdnFolder", Value = string.Empty, Description = "If you have a copy of the WoW CDN available locally, you can set this to the path of the directory containing a tpr/wow folder.", Type = "string", DefaultValue = string.Empty }},
            {"wowProduct", new WTLSetting { Key = "wowProduct", Value = string.Empty, Description = "The WoW product to use (e.g. 'wow', 'wowt', 'wow_classic', etc.).", Type = "string", DefaultValue = "wow" }},
            {"region", new WTLSetting { Key = "region", Value = "eu", Description = "The region to use (e.g. 'eu', 'us', etc.).", Type = "string", DefaultValue = "eu" }},
            {"showAllFiles", new WTLSetting { Key = "showAllFiles", Value = "false", Description = "Whether to show all files in WTL, including those not present in the loaded build.", Type = "bool", DefaultValue = "false" }},
            {"locale", new WTLSetting { Key = "locale", Value = "enUS", Description = "The locale to use (e.g. enUS, deDE, zhCN, etc.).", Type = "string", DefaultValue = "enUS" }},
            {"preferHighResTextures", new WTLSetting { Key = "preferHighResTextures", Value = "false", Description = "Whether to prefer high-res textures when available (Classic only).", Type = "bool", DefaultValue = "false" }},
            {"useTACTSharp", new WTLSetting { Key = "useTACTSharp", Value = "false", Description = "Whether to use TACTSharp for TACT operations. Uses CASCLib if disabled.", Type = "bool", DefaultValue = "true" }},
            {"additionalCDNs", new WTLSetting { Key = "additionalCDNs", Value = string.Empty, Description = "Additional CDN hosts to use for downloading files, separated by commas.", Type = "string", DefaultValue = string.Empty }},
            {"buildConfigFile", new WTLSetting { Key = "buildConfigFile", Value = string.Empty, Description = "Path to a build config file.", Type = "string", DefaultValue = string.Empty, Ephemeral = true }},
            {"cdnConfigFile", new WTLSetting { Key = "cdnConfigFile", Value = string.Empty, Description = "Path to a CDN config file.", Type = "string", DefaultValue = string.Empty, Ephemeral = true }},
            {"defaultFilesSearch", new WTLSetting { Key = "defaultFilesSearch", Value = string.Empty, Description = "Default search query for files page.", Type = "string", DefaultValue = string.Empty } },
            {"tagRepo", new WTLSetting { Key = "tagRepo", Value = string.Empty, Description = "Path to the wow-filetags repository.", Type = "string", DefaultValue = string.Empty } },
            {"readOnly", new WTLSetting { Key = "readOnly", Value = "false", Description = "Whether to operate in read-only mode. Disables various functionality.", Type = "bool", DefaultValue = "false" }},
        };

        private static CASCLib.LocaleFlags cascLocale;
        private static RootInstance.LocaleFlags tactLocale;

        // Strings
        public static string DefinitionDir { get => Settings["definitionDir"].Value; set => Settings["definitionDir"].Value = value; }
        public static string ListfileURL { get => Settings["listfileURL"].Value; set => Settings["listfileURL"].Value = value; }
        public static string TACTKeyURL { get => Settings["tactKeyURL"].Value; set => Settings["tactKeyURL"].Value = value; }
        public static string WoWFolder { get => Settings["wowFolder"].Value; set => Settings["wowFolder"].Value = value; }
        public static string DBCFolder { get => Settings["dbcFolder"].Value; set => Settings["dbcFolder"].Value = value; }
        public static string ManifestFolder { get => Settings["manifestFolder"].Value; set => Settings["manifestFolder"].Value = value; }
        public static string ExtractionDir { get => Settings["extractionDir"].Value; set => Settings["extractionDir"].Value = value; }
        public static string CDNFolder { get => Settings["cdnFolder"].Value; set => Settings["cdnFolder"].Value = value; }
        public static string WoWProduct { get => Settings["wowProduct"].Value; set => Settings["wowProduct"].Value = value; }
        public static string Region { get => Settings["region"].Value; set => Settings["region"].Value = value; }
        public static string BuildConfigFile { get => Settings["buildConfigFile"].Value; set => Settings["buildConfigFile"].Value = value; }
        public static string CDNConfigFile { get => Settings["cdnConfigFile"].Value; set => Settings["cdnConfigFile"].Value = value; }
        public static string DefaultFilesSearch { get => Settings["defaultFilesSearch"].Value; set => Settings["defaultFilesSearch"].Value = value; }
        public static string TagRepo { get => Settings["tagRepo"].Value; set => Settings["tagRepo"].Value = value; }

        // Bools
        public static bool ShowAllFiles { get => bool.Parse(Settings["showAllFiles"].Value); set => Settings["showAllFiles"].Value = value.ToString().ToLower(); }
        public static bool PreferHighResTextures { get => bool.Parse(Settings["preferHighResTextures"].Value); set => Settings["preferHighResTextures"].Value = value.ToString().ToLower(); }
        public static bool UseTACTSharp { get => bool.Parse(Settings["useTACTSharp"].Value); set => Settings["useTACTSharp"].Value = value.ToString().ToLower(); }
        public static bool ReadOnly { get => bool.Parse(Settings["readOnly"].Value); set => Settings["readOnly"].Value = value.ToString().ToLower(); }

        // Enums
        public static RootInstance.LocaleFlags TACTLocale { get => tactLocale; set => tactLocale = value; }
        public static CASCLib.LocaleFlags CASCLocale { get => cascLocale; set => cascLocale = value; }

        // Arrays
        public static string[] AdditionalCDNs
        {
            get => Settings["additionalCDNs"].Value.Split(',');
            set => Settings["additionalCDNs"].Value = string.Join(",", value);
        }

        static SettingsManager()
        {
            try
            {
                LoadSettings();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error happened during config.json reading, make sure it is valid. Application will be unable to load correctly.");
                Console.WriteLine("Error message: " + e.Message);
                Console.ResetColor();
            }
        }

        public static void LoadSettings()
        {
            var cwd = Directory.GetCurrentDirectory();
            var appDir = AppDomain.CurrentDomain.BaseDirectory;

            string configPath = Path.Combine(cwd, "config.json");
            bool hasConfig = File.Exists(configPath);
            if (!hasConfig && File.Exists(Path.Combine(appDir, "config.json")))
                Environment.CurrentDirectory = appDir; // set the current directory to the app's directory if config.json is there

            if (hasConfig)
            {
                var config = new ConfigurationBuilder().SetBasePath(cwd).AddJsonFile("config.json", optional: false, reloadOnChange: false).Build();
                if (config.GetSection("config").Exists())
                    foreach (var setting in Settings)
                    {
                        setting.Value.Value = config.GetSection("config")[setting.Key] ?? setting.Value.DefaultValue;

                        if (setting.Key == "locale")
                            SetLocale(setting.Value.Value);
                    }
            }
            else
            {
                foreach (var setting in Settings)
                {
                    setting.Value.Value = setting.Value.DefaultValue;
                    if (setting.Key == "locale")
                        SetLocale(setting.Value.Value);

                    if (setting.Key == "wowFolder")
                    {
                        DetectWoWFolder();
                        if (!ValidateWowFolder(WoWFolder))
                            WoWFolder = string.Empty; // reset if the detected folder is invalid
                    }
                }
            }

            // After loading settings, always save them to ensure the config.json is valid and up-to-date (when e.g. settings are added/removed).
            SaveSettings();
        }

        public static void SaveSettings()
        {
            var cwd = Directory.GetCurrentDirectory();
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            bool hasConfig = File.Exists(Path.Combine(cwd, "config.json"));
            if (!hasConfig && File.Exists(Path.Combine(appDir, "config.json")))
                Environment.CurrentDirectory = appDir;

            var configPath = Path.Combine(Environment.CurrentDirectory, "config.json");
            dynamic newConfigJson = new System.Dynamic.ExpandoObject();
            newConfigJson.config = new Dictionary<string, string>();
            foreach (var setting in GetPersistentSettings())
                newConfigJson.config[setting.Key] = setting.Value;

            string json = System.Text.Json.JsonSerializer.Serialize(newConfigJson, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }

        private static void SetLocale(string locValue)
        {
            if (locValue == null)
            {
                cascLocale = CASCLib.LocaleFlags.enUS;
                tactLocale = RootInstance.LocaleFlags.enUS;
                return;
            }

            cascLocale = locValue switch
            {
                "deDE" => CASCLib.LocaleFlags.deDE,
                "enUS" => CASCLib.LocaleFlags.enUS,
                "enGB" => CASCLib.LocaleFlags.enGB,
                "ruRU" => CASCLib.LocaleFlags.ruRU,
                "zhCN" => CASCLib.LocaleFlags.zhCN,
                "zhTW" => CASCLib.LocaleFlags.zhTW,
                "enTW" => CASCLib.LocaleFlags.enTW,
                "esES" => CASCLib.LocaleFlags.esES,
                "esMX" => CASCLib.LocaleFlags.esMX,
                "frFR" => CASCLib.LocaleFlags.frFR,
                "itIT" => CASCLib.LocaleFlags.itIT,
                "koKR" => CASCLib.LocaleFlags.koKR,
                "ptBR" => CASCLib.LocaleFlags.ptBR,
                "ptPT" => CASCLib.LocaleFlags.ptPT,
                _ => throw new Exception("Invalid locale. Available locales: deDE, enUS, enGB, ruRU, zhCN, zhTW, enTW, esES, esMX, frFR, itIT, koKR, ptBR, ptPT"),
            };

            tactLocale = locValue switch
            {
                "deDE" => RootInstance.LocaleFlags.deDE,
                "enUS" => RootInstance.LocaleFlags.enUS,
                "enGB" => RootInstance.LocaleFlags.enGB,
                "ruRU" => RootInstance.LocaleFlags.ruRU,
                "zhCN" => RootInstance.LocaleFlags.zhCN,
                "zhTW" => RootInstance.LocaleFlags.zhTW,
                "enTW" => RootInstance.LocaleFlags.enTW,
                "esES" => RootInstance.LocaleFlags.esES,
                "esMX" => RootInstance.LocaleFlags.esMX,
                "frFR" => RootInstance.LocaleFlags.frFR,
                "itIT" => RootInstance.LocaleFlags.itIT,
                "koKR" => RootInstance.LocaleFlags.koKR,
                "ptBR" => RootInstance.LocaleFlags.ptBR,
                "ptPT" => RootInstance.LocaleFlags.ptPT,
                _ => throw new Exception("Invalid locale. Available locales: deDE, enUS, enGB, ruRU, zhCN, zhTW, enTW, esES, esMX, frFR, itIT, koKR, ptBR, ptPT"),
            };
        }

        private static void HandleFlag(string flag, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                foreach (var setting in Settings)
                    if (flag.Equals("-" + setting.Key, StringComparison.OrdinalIgnoreCase))
                        setting.Value.Value = value;
            }
        }

        private static void HandleSwitch(string flag)
        {
            if (!string.IsNullOrEmpty(flag))
            {
                switch (flag)
                {
                    case ("--debug"):
                        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
                        break;
                }
            }
        }

        private static void ProcessFlags(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string _arg = args[i];
                string _flag = _arg;
                string? _value = null;
                int _nextIndex = i + 1;

                // handle switches first, cause they're special
                if (_arg.StartsWith("--"))
                {
                    HandleSwitch(_arg);
                    continue;
                }

                // check if the value is specified with an equals sign, then break it up and set _value
                // TODO: currently eats any args containing an equal sign, which could cause us to eat flags that aren't meant for us. too bad
                if (_arg.Contains('='))
                {
                    string[] _argSplit = _arg.Split('=');
                    _flag = _argSplit[0];
                    _value = _argSplit[1];
                    HandleFlag(_flag, _value);
                    args[i] = "";
                    continue;
                }

                // check that the next index in args is valid
                if (_nextIndex < args.Length)
                {
                    // if flag was NOT specified with an equals sign, set it to the next index
                    if (_value == null)
                    {
                        _value = args[_nextIndex];
                        i++; // then skip the next item
                    }
                    HandleFlag(_flag, _value);
                    continue;
                }
            }
        }

        public static void ParseCommandLineArguments(string[] args)
        {
            ProcessFlags(args);

            // since this runs after LoadSettings() AND the command line arguments are parsed, I'm comfortable throwing this folder check here.
            if (!ValidateWowFolder(WoWFolder))
                WoWFolder = string.Empty; // reset if the detected folder is invalid
        }

        public static (bool, string) ValidateSetting(string key, string value)
        {
            switch (key)
            {

                case "wowProduct":
                    if (string.IsNullOrEmpty(value) || !value.StartsWith("wow", StringComparison.OrdinalIgnoreCase))
                        return (false, "Product is empty or does not start with 'wow'");
                    else
                        return (true, string.Empty);
                case "wowFolder":
                    return (!string.IsNullOrEmpty(value) && !ValidateWowFolder(value) ? (false, "Invalid WoW folder path or .build.info file not found.") : (true, string.Empty));
                case "cdnFolder":
                    if (!string.IsNullOrEmpty(value) && !Directory.Exists(value))
                        return (false, "Directory does not exist");
                    else
                        return (true, string.Empty);
                case "locale":
                    var validLocales = new[] { "deDE", "enUS", "enGB", "ruRU", "zhCN", "zhTW", "enTW", "esES", "esMX", "frFR", "itIT", "koKR", "ptBR", "ptPT" };
                    if (validLocales.Contains(value))
                    {
                        SetLocale(value);
                        return (true, string.Empty);
                    }
                    else
                        return (false, "Invalid locale. Available locales: deDE, enUS, enGB, ruRU, zhCN, zhTW, enTW, esES, esMX, frFR, itIT, koKR, ptBR, ptPT");
                case "region":
                    var validRegions = new[] { "eu", "us", "kr", "cn", "tw" };
                    if (validRegions.Contains(value))
                        return (true, string.Empty);
                    else
                        return (false, "Invalid region. Available regions: eu, us, kr, cn, tw");
                case "definitionDir":
                    if (string.IsNullOrEmpty(value))
                        return (true, string.Empty);
                    else if (Directory.Exists(value) && File.Exists(Path.Combine(value, "Map.dbd")))
                        return (true, string.Empty);
                    else
                        return (false, "Invalid path to definitions directory (must contain a Map.dbd)");
                case "listfileURL":
                    if (Uri.TryCreate(value, UriKind.Absolute, out var listfileURIResult) && (listfileURIResult.Scheme == Uri.UriSchemeHttp || listfileURIResult.Scheme == Uri.UriSchemeHttps) && value.EndsWith(".csv"))
                        return (true, string.Empty);
                    else
                        if (Directory.Exists(value) && File.Exists(Path.Combine(value, "placeholder.csv")))
                        return (true, string.Empty);
                    else
                        return (false, "Invalid listfile URL (must be a valid link and end in .csv) or invalid path to parts (directory must contain placeholder.csv)");
                case "tactKeyURL":
                    if (Uri.TryCreate(value, UriKind.Absolute, out var tcURIResult) && (tcURIResult.Scheme == Uri.UriSchemeHttp || tcURIResult.Scheme == Uri.UriSchemeHttps) && (value.EndsWith(".txt") || value.EndsWith(".csv")))
                        return (true, string.Empty);
                    else
                        return (false, "Invalid TACT key list URL (must end in .txt or .csv)");
                case "additionalCDNs":
                    if (string.IsNullOrEmpty(value))
                        return (true, string.Empty);
                    else
                    {
                        var cdns = value.Split(',');
                        foreach (var cdn in cdns)
                        {
                            if (string.IsNullOrWhiteSpace(cdn) || cdn.StartsWith("http"))
                                return (false, "Invalid CDN host: " + cdn + "(must not contain http/https or slashes, only a host name)");
                        }
                        return (true, string.Empty);
                    }
                case "dbcFolder":
                case "manifestFolder":
                case "extractionDir":
                    if (string.IsNullOrEmpty(value))
                        return (false, "Folder must be set but does not have to exist (will be created)");
                    else
                        return (true, string.Empty);
                case "showAllFiles":
                case "useTACTSharp":
                case "preferHighResTextures":
                case "readOnly":
                    if (bool.TryParse(value, out _))
                        return (true, string.Empty);
                    else
                        return (false, "Value must be a boolean (true/false)");
                case "buildConfigFile":
                    if (File.Exists(value))
                        return (true, string.Empty);
                    else
                        return (false, "Specified build config file does not exist");
                case "cdnConfigFile":
                    if (File.Exists(value))
                        return (true, string.Empty);
                    else
                        return (false, "Specified CDN config file does not exist");
                case "defaultFilesSearch":
                    return (true, string.Empty);
                case "tagRepo":
                    if (string.IsNullOrEmpty(value))
                        return (true, string.Empty);
                    else if (Directory.Exists(value) && File.Exists(Path.Combine(value, "meta", "tags.csv")))
                        return (true, string.Empty);
                    else
                        return (false, "Invalid path to wow-filetags repo directory (must contain a meta/tags.csv)");
            }

            return (true, "Not checked");
        }

        private static bool ValidateWowFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return false;
            }
            else
            {
                if (!Directory.Exists(folder))
                {
                    return false;
                }
                else
                {
                    return File.Exists(Path.Combine(folder, ".build.info"));
                }
            }
        }

        private static void DetectWoWFolder()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            // Try registry!
            var wowPath = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Blizzard Entertainment\World of Warcraft", "InstallPath", null) as string;
            if (!string.IsNullOrEmpty(wowPath))
            {
                // Check if this is a subfolder in a shared installation, if so go up one level
                if (wowPath.EndsWith("_\\"))
                {
                    var pathInfo = new DirectoryInfo(wowPath);
                    wowPath = pathInfo.Parent!.FullName;
                }

                WoWFolder = wowPath;
            }
        }

        public static List<WTLSetting> GetPersistentSettings()
        {
            // don't include ephemeral settings in the persistent settings
            return Settings.Values.Where(x => !x.Ephemeral).ToList();
        }
    }
}