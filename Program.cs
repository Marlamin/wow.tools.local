using CASCLib;
using Microsoft.AspNetCore.StaticFiles;
using wow.tools.local.Services;

namespace wow.tools.local
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            try
            {
                var keyRes = await CASC.LoadKeys();
                if (!keyRes)
                    throw new Exception("Failed to load TACT keys");

                CASC.InitCasc(SettingsManager.wowFolder, SettingsManager.wowProduct);
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