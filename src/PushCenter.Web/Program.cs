using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PushCenter.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureAppConfiguration((context, builder) => ConfigureAppConfiguration(args, builder))
                       .UseSerilog(ConfigureLogging)
                       .ConfigureWebHostDefaults(webHostBuilder =>
                       {
                           webHostBuilder.UseStartup<Startup>()
                                         .SuppressStatusMessages(true);
                       });
        }

        private static void ConfigureAppConfiguration(IReadOnlyList<string> args, IConfigurationBuilder builder)
        {
            // Add JSON File passed by arguments
            if (args.Any() && !string.IsNullOrEmpty(args[0])) builder.AddJsonFile(args[0], true);
        }

        private static void ConfigureLogging(HostBuilderContext context, LoggerConfiguration config)
        {
            config.ReadFrom.Configuration(context.Configuration);
        }
    }
}