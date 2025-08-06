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
    };


    public static class SettingsManager
    {
        public static readonly Dictionary<string, WTLSetting> Settings = new Dictionary<string, WTLSetting>
        {
            {"definitionDir", new WTLSetting { Key = "definitionDir", Value = string.Empty, Description = "Directory where the definition files are stored.", Type = "string", DefaultValue = string.Empty }},
            {"listfileURL", new WTLSetting { Key = "listfileURL", Value = string.Empty, Description = "URL to the listfile used for CASC operations.", Type = "string", DefaultValue = "https://github.com/wowdev/wow-listfile/releases/latest/download/community-listfile-withcapitals.csv" }},
            {"tactKeyURL", new WTLSetting { Key = "tactKeyURL", Value = string.Empty, Description = "URL to the TACT key file.", Type = "string", DefaultValue = "https://github.com/wowdev/TACTKeys/raw/master/WoW.txt" }},
            {"wowFolder", new WTLSetting { Key = "wowFolder", Value = string.Empty, Description = "Path to the WoW installation folder.", Type = "string", DefaultValue = string.Empty }},
            {"dbcFolder", new WTLSetting { Key = "dbcFolder", Value = string.Empty, Description = "Path to the DBC folder.", Type = "string", DefaultValue = "dbcs" }},
            {"manifestFolder", new WTLSetting { Key = "manifestFolder", Value = "manifests", Description = "Folder where manifests are stored.", Type = "string", DefaultValue = "manifests" }},
            {"extractionDir", new WTLSetting { Key = "extractionDir", Value = "extract", Description = "Directory where files will be extracted to.", Type = "string", DefaultValue = "extract" }},
            {"cdnFolder", new WTLSetting { Key = "cdnFolder", Value = string.Empty, Description = "Path to the CDN folder.", Type = "string", DefaultValue = string.Empty }},
            {"wowProduct", new WTLSetting { Key = "wowProduct", Value = string.Empty, Description = "The WoW product to use (e.g. 'wow', 'wowt', 'wow_classic', etc.).", Type = "string", DefaultValue = "wow" }},
            {"region", new WTLSetting { Key = "region", Value = "eu", Description = "The region to use (e.g. 'eu', 'us', etc.).", Type = "string", DefaultValue = "eu" }},
            {"showAllFiles", new WTLSetting { Key = "showAllFiles", Value = "false", Description = "Whether to show all files in WTL, including those not present in the loaded build.", Type = "bool", DefaultValue = "false" }},
            {"locale", new WTLSetting { Key = "locale", Value = "enUS", Description = "The locale to use.", Type = "string", DefaultValue = "enUS" }},
            {"preferHighResTextures", new WTLSetting { Key = "preferHighResTextures", Value = "false", Description = "Whether to prefer high-res textures when available (Classic).", Type = "bool", DefaultValue = "false" }},
            {"useTACTSharp", new WTLSetting { Key = "useTACTSharp", Value = "false", Description = "Whether to use TACTSharp for TACT operations.", Type = "bool", DefaultValue = "true" }},
            {"additionalCDNs", new WTLSetting { Key = "additionalCDNs", Value = string.Empty, Description = "Additional CDNs to use for downloading files, separated by commas.", Type = "string", DefaultValue = string.Empty }},
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

        // Bools
        public static bool ShowAllFiles { get => bool.Parse(Settings["showAllFiles"].Value); set => Settings["showAllFiles"].Value = value.ToString().ToLower(); }
        public static bool PreferHighResTextures { get => bool.Parse(Settings["preferHighResTextures"].Value); set => Settings["preferHighResTextures"].Value = value.ToString().ToLower(); }
        public static bool UseTACTSharp { get => bool.Parse(Settings["useTACTSharp"].Value); set => Settings["useTACTSharp"].Value = value.ToString().ToLower(); }

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
                        ValidateWowFolder();
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
            foreach (var setting in Settings)
                newConfigJson.config[setting.Key] = setting.Value.Value;

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
            ValidateWowFolder();
        }

        private static bool ValidateWowFolder()
        {
            if (string.IsNullOrEmpty(WoWFolder))
            {
                WoWFolder = string.Empty;
                return false;
            }
            else
            {
                if (!Directory.Exists(WoWFolder))
                {
                    return false;
                }
                else
                {
                    if (!File.Exists(Path.Combine(WoWFolder, ".build.info")))
                    {
                        WoWFolder = string.Empty;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
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
    }
}