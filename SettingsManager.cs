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