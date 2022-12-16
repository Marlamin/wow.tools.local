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
            LoadSettings();
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
                wowFolder = null;

            wowProduct = config.GetSection("config")["wowProduct"];
        }
    }
}