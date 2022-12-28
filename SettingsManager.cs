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
                switch (config.GetSection("config")["locale"])
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
                        Console.WriteLine("Locale " + config.GetSection("config")["locale"] + " not found, defaulting to enUS");
                        locale = CASCLib.LocaleFlags.enUS;
                        break;
                }
            }
            else
            {
                locale = CASCLib.LocaleFlags.enUS;
            }

            dbcFolder = config.GetSection("config")["dbcFolder"];
            
            wowFolder = config.GetSection("config")["wowFolder"];
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
                    if(!File.Exists(Path.Combine(wowFolder, ".build.info")))
                    {
                        throw new FileNotFoundException("Unable to find .build.info in WoW directory. Make sure you selected the root WoW directory and not a subfolder.");
                    }
                }
            }


            wowProduct = config.GetSection("config")["wowProduct"];
        }
    }
}