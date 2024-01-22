using DBCD.Providers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using wow.tools.local.Services;
using wow.tools.Services;

namespace wow.tools.local
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddSingleton<IDBDProvider, DBDProvider>();
            services.AddSingleton<IDBCProvider, DBCProvider>();
            services.AddSingleton<IDBCManager, DBCManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseDefaultFiles();

            var extensionProvider = new FileExtensionContentTypeProvider();
            extensionProvider.Mappings.Add(".data", "application/octet-stream");

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = extensionProvider,
                OnPrepareResponse = sfrContext =>
                {
                    sfrContext.Context.Response.Headers.TryAdd("Expires", "-1");
                    sfrContext.Context.Response.Headers.TryAdd("Cache-Control", "no-cache, no-store");
                }
            });
        }
    }
}