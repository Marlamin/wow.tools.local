using CASCLib;
using Microsoft.AspNetCore.StaticFiles;
using wow.tools.local.Services;

namespace wow.tools.local
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CASC.InitCasc(new BackgroundWorkerEx(), SettingsManager.wowFolder, SettingsManager.wowProduct);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception initializing CASC: " + e.Message);
            }

            try
            {
                CASC.LoadListfile();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception loading listfile: " + e.Message);
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