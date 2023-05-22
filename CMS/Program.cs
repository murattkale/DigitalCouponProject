using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    webBuilder.UseStartup<Startup>().CaptureStartupErrors(true).UseSetting("detailedErrors", "true");
                    webBuilder.UseSerilog((ctx, config) => { config.ReadFrom.Configuration(ctx.Configuration); });
                });
    }
}
