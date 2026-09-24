using wow.tools.local.Services;

namespace wow.tools.local
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // As the absolute first thing, ensure we're actually running in the correct dir
            if (!Directory.Exists("wwwroot"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: The current working directory does not contain the 'wwwroot' folder. Please run this application from the correct directory.");
                Console.ResetColor();
                return;
            }

            try
            {
                // this will override the config.json values if the relevant command line flags are present
                SettingsManager.ParseCommandLineArguments(args);

                CASC.InitTACT(SettingsManager.WoWFolder, SettingsManager.WoWProduct);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception initializing CASC: " + e.Message);
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Limits.MaxConcurrentConnections = 500;
                    serverOptions.Limits.MaxConcurrentUpgradedConnections = 500;
                })
                .UseStartup<Startup>();
            });
    }
}