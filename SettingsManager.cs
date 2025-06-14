using CASCLib;
using TACTSharp;

namespace wow.tools.local
{
    public static class SettingsManager
    {
        private static string definitionDir = string.Empty;
        private static string listfileURL = string.Empty;
        private static string tactKeyURL = string.Empty;
        private static string wowFolder = string.Empty;
        private static string dbcFolder = string.Empty;
        private static string manifestFolder = "manifests";
        private static string extractionDir = "extract";
        private static string cdnFolder = string.Empty;
        private static string wowProduct = string.Empty;
        private static string region = "eu";

        private static CASCLib.LocaleFlags cascLocale;
        private static RootInstance.LocaleFlags tactLocale;
        private static bool showAllFiles = false;
        private static bool preferHighResTextures = false;
        private static bool useTACTSharp = false;
        private static string[] additionalCDNs = Array.Empty<string>();

        // supported command line flags and switches
        // flag syntax: -flag value || -flag=value or --switch
        private const string _wowFolderFlag = "-wowFolder";
        private const string _wowProductFlag = "-product";
        private const string _dbdFolderFlag = "-dbdFolder";
        private const string _dbcFolderFlag = "-dbcFolder";
        private const string _manifestFolderFlag = "-manifestFolder";
        private const string _cdnFolderFlag = "-cdnFolder";
        private const string _listfileURLFlag = "-listfileURL";
        private const string _tactKeyURLFlag = "-tactKeyURL";
        private const string _regionFlag = "-region";
        private const string _localeFlag = "-locale";
        private const string _showAllFilesFlag = "-showAllFiles";
        private const string _extractionDirFlag = "-extractionDir";
        private const string _preferHighResTexturesFlag = "-preferHighResTextures";
        private const string _useTACTSharpFlag = "-useTACTSharp";
        private const string _additionalCDNsFlag = "-additionalCDNs";

        // calling the double-hyphen args 'switch' instead of 'flag' because they don't have values
        private const string _debugSwitch = "--debug";

        public static string DefinitionDir { get => definitionDir; private set => definitionDir = value; }
        public static string ListfileURL { get => listfileURL; private set => listfileURL = value; }
        public static string TACTKeyURL { get => tactKeyURL; private set => tactKeyURL = value; }
        public static string WoWFolder { get => wowFolder; private set => wowFolder = value; }
        public static string DBCFolder { get => dbcFolder; private set => dbcFolder = value; }
        public static string ManifestFolder { get => manifestFolder; private set => manifestFolder = value; }
        public static string ExtractionDir { get => extractionDir; private set => extractionDir = value; }
        public static string CDNFolder { get => cdnFolder; private set => cdnFolder = value; }
        public static string WoWProduct { get => wowProduct; private set => wowProduct = value; }
        public static string Region { get => region; private set => region = value; }
        public static bool ShowAllFiles { get => showAllFiles; private set => showAllFiles = value; }
        public static RootInstance.LocaleFlags TACTLocale { get => tactLocale; private set => tactLocale = value; }
        public static LocaleFlags CASCLocale { get => cascLocale; private set => cascLocale = value; }
        public static bool PreferHighResTextures { get => preferHighResTextures; private set => preferHighResTextures = value; }
        public static bool UseTACTSharp { get => useTACTSharp; private set => useTACTSharp = value; }
        public static string[] AdditionalCDNs { get => additionalCDNs; private set => additionalCDNs = value; }

        // to add a new flag, add a new const string above
        //    then add a new case to the switch statement in either HandleFlag or HandleSwitch with the functionality you want

        // TODO:
        // make it easier to associate flags with values and add new flags
        // flags are currently case-sensitive, probably not best practice. easy to fix, i'm just being lazy

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

            IConfigurationRoot config;
            var cwd = Directory.GetCurrentDirectory();
            var appDir = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                config = new ConfigurationBuilder().SetBasePath(cwd).AddJsonFile("config.json", optional: false, reloadOnChange: false).Build();
            }
            catch (Exception)
            { // if we can't find config.json in the cwd, try the app's directory instead. This is to support launching from the command line, which might not be started in the app's directory
              // this could cause soft errors in case of misconfiguration, but too bad
                config = new ConfigurationBuilder().SetBasePath(appDir).AddJsonFile("config.json", optional: false, reloadOnChange: false).Build();
                Environment.CurrentDirectory = appDir;
            }

            definitionDir = config.GetSection("config")["definitionDir"] ?? string.Empty;
            listfileURL = config.GetSection("config")["listfileURL"] ?? string.Empty;
            tactKeyURL = config.GetSection("config")["tactKeyURL"] ?? string.Empty;

            region = config.GetSection("config")["region"] ?? "eu";

            string? localeValue = config.GetSection("config")["locale"];
            if (localeValue != null)
            {
                SetLocale(localeValue);
            }
            else
            {
                cascLocale = CASCLib.LocaleFlags.enUS;
                tactLocale = RootInstance.LocaleFlags.enUS;
            }

            manifestFolder = config.GetSection("config")["manifestFolder"] ?? "manifests";

            dbcFolder = config.GetSection("config")["dbcFolder"] ?? string.Empty;
            wowFolder = config.GetSection("config")["wowFolder"] ?? string.Empty;
            cdnFolder = config.GetSection("config")["cdnFolder"] ?? string.Empty;
            wowProduct = config.GetSection("config")["wowProduct"] ?? string.Empty;
            showAllFiles = config.GetSection("config").GetValue<bool>("showAllFiles");
            extractionDir = config.GetSection("config")["extractionDir"] ?? string.Empty;
            preferHighResTextures = config.GetSection("config").GetValue<bool>("preferHighResTextures");
            useTACTSharp = config.GetSection("config").GetValue<bool>("useTACTSharp");
            additionalCDNs = config.GetSection("config")["additionalCDNs"]?.Split(',') ?? Array.Empty<string>();
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
                switch (flag)
                {
                    case (_wowFolderFlag):
                        wowFolder = value;
                        break;
                    case (_wowProductFlag):
                        wowProduct = value;
                        break;
                    case (_dbdFolderFlag):
                        definitionDir = value;
                        break;
                    case (_dbcFolderFlag):
                        dbcFolder = value;
                        break;
                    case (_manifestFolderFlag):
                        manifestFolder = value;
                        break;
                    case (_cdnFolderFlag):
                        cdnFolder = value;
                        break;
                    case (_listfileURLFlag):
                        ListfileURL = value;
                        break;
                    case (_tactKeyURLFlag):
                        tactKeyURL = value;
                        break;
                    case (_regionFlag):
                        region = value;
                        break;
                    case (_localeFlag):
                        SetLocale(value);
                        break;
                    case (_showAllFilesFlag):
                        showAllFiles = bool.Parse(value);
                        break;
                    case (_extractionDirFlag):
                        extractionDir = value;
                        break;
                    case (_preferHighResTexturesFlag):
                        preferHighResTextures = bool.Parse(value);
                        break;
                    case (_useTACTSharpFlag):
                        useTACTSharp = bool.Parse(value);
                        break;
                    case (_additionalCDNsFlag):
                        additionalCDNs = value.Split(',');
                        break;
                }
            }
        }

        private static void HandleSwitch(string flag)
        {
            if (!string.IsNullOrEmpty(flag))
            {
                switch (flag)
                {
                    case (_debugSwitch):
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

        private static void ValidateWowFolder()
        {
            if (string.IsNullOrEmpty(wowFolder))
            {
                wowFolder = string.Empty;
            }
            else
            {
                if (!Directory.Exists(wowFolder))
                {
                    throw new DirectoryNotFoundException("Could not find folder " + wowFolder);
                }
                else
                {
                    if (!File.Exists(Path.Combine(wowFolder, ".build.info")))
                    {
                        throw new FileNotFoundException("Unable to find .build.info in WoW directory. Make sure you selected the root WoW directory and not a subfolder.");
                    }
                }
            }
        }
    }
}