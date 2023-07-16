namespace wow.tools.local
{
    public static class SettingsManager
    {
        public static string definitionDir;
        public static string listfileURL;
        public static string tactKeyURL;
        public static string? wowFolder;
        public static string dbcFolder;
        public static string wowProduct;
        public static string region;
        public static CASCLib.LocaleFlags locale;

        // supported command line flags and switches
        // flag syntax: -flag value || -flag=value or --switch
        private const string _wowFolderFlag = "-wowFolder";
        private const string _wowProductFlag = "-product";
        private const string _dbdFolderFlag = "-dbdFolder";
        private const string _dbcFolderFlag = "-dbcFolder";
        private const string _listfileURLFlag = "-listfileURL";
        private const string _tactKeyURLFlag = "-tactKeyURL";
        private const string _regionFlag = "-region";
        private const string _localeFlag = "-locale";

        // calling the double-hyphen args 'switch' instead of 'flag' because they don't have values
        private const string _debugSwitch = "--debug";

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
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error happened during config.json reading, make sure it is valid. Application will be unable to load correctly.");
                Console.WriteLine("Error message: " + e.Message);
                Console.ResetColor();
            }
        }

        public static void LoadSettings()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", optional: false, reloadOnChange: false).Build();
            definitionDir = config.GetSection("config")["definitionDir"];
            listfileURL = config.GetSection("config")["listfileURL"];
            tactKeyURL = config.GetSection("config")["tactKeyURL"];

            if (config.GetSection("config")["region"] != null)
            {
                region = config.GetSection("config")["region"];
            }
            else
            {
                region = "eu";
            }

            if (config.GetSection("config")["locale"] != null)
            {
                SetLocale(config.GetSection("config")["locale"]);
            }
            else
            {
                locale = CASCLib.LocaleFlags.enUS;
            }

            dbcFolder = config.GetSection("config")["dbcFolder"];
            wowFolder = config.GetSection("config")["wowFolder"];
            wowProduct = config.GetSection("config")["wowProduct"];
        }

        private static void SetLocale(string locValue)
        {
            if (locValue == null)
            {
                locale = CASCLib.LocaleFlags.enUS;
                return;
            }

            switch (locValue)
            {
                case "deDE":
                    locale = CASCLib.LocaleFlags.deDE;
                    break;
                case "enUS":
                    locale = CASCLib.LocaleFlags.enUS;
                    break;
                case "enGB":
                    locale = CASCLib.LocaleFlags.enGB;
                    break;
                case "ruRU":
                    locale = CASCLib.LocaleFlags.ruRU;
                    break;
                case "zhCN":
                    locale = CASCLib.LocaleFlags.zhCN;
                    break;
                case "zhTW":
                    locale = CASCLib.LocaleFlags.zhTW;
                    break;
                case "enTW":
                    locale = CASCLib.LocaleFlags.enTW;
                    break;
                case "esES":
                    locale = CASCLib.LocaleFlags.esES;
                    break;
                case "esMX":
                    locale = CASCLib.LocaleFlags.esMX;
                    break;
                case "frFR":
                    locale = CASCLib.LocaleFlags.frFR;
                    break;
                case "itIT":
                    locale = CASCLib.LocaleFlags.itIT;
                    break;
                case "koKR":
                    locale = CASCLib.LocaleFlags.koKR;
                    break;
                case "ptBR":
                    locale = CASCLib.LocaleFlags.ptBR;
                    break;
                case "ptPT":
                    locale = CASCLib.LocaleFlags.ptPT;
                    break;
                default:
                    Console.WriteLine("Locale " + locValue + " not found, defaulting to enUS");
                    locale = CASCLib.LocaleFlags.enUS;
                    break;
            }
        }

        private static void HandleFlag(string flag, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                switch(flag)
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
                    case (_listfileURLFlag):
                        listfileURL = value;
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
                wowFolder = null;
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