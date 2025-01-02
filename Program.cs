using wow.tools.local.Services;

namespace wow.tools.local
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var keyRes = CASC.LoadKeys();
                if (!keyRes)
                    throw new Exception("Failed to load TACT keys");

                // this will override the config.json values if the relevant command line flags are present
                SettingsManager.ParseCommandLineArguments(args);

#if DEBUG
                //if(SettingsManager.wowFolder == null)
                //{
                //    CASC.InitTACT(SettingsManager.wowProduct);
                //}
                //else
                //{
#endif
                    CASC.InitCasc(SettingsManager.wowFolder, SettingsManager.wowProduct);
#if DEBUG
                //}
#endif
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