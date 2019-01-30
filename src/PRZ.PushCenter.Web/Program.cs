using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                          .ConfigureAppConfiguration((context, builder) => ConfigureAppConfiguration(args, builder))
                          .UseStartup<Startup>()
                          .UseSerilog(ConfigureLogging)
                          .SuppressStatusMessages(true);
        }

        private static void ConfigureAppConfiguration(IReadOnlyList<string> args, IConfigurationBuilder builder)
        {
            if (args.Any() && !string.IsNullOrEmpty(args[0]))
            {
                builder.AddJsonFile(args[0], true);
            }
        }

        private static void ConfigureLogging(WebHostBuilderContext context, LoggerConfiguration config)
        {
            config.ReadFrom.Configuration(context.Configuration);
        }
    }
}