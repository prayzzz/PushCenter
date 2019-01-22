using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace PRZ.PushCenter.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>()
                          .UseSerilog(ConfigureLogging)
                          .SuppressStatusMessages(true);
        }

        private static void ConfigureLogging(WebHostBuilderContext context, LoggerConfiguration config)
        {
            config.ReadFrom.Configuration(context.Configuration);
        }
    }
}