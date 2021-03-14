using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PushCenter.Bll;
using Serilog;

namespace PushCenter.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PushCenterDbContext>();
                db.Database.Migrate();
            }

            host.Run();
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureAppConfiguration((_, builder) => ConfigureAppConfiguration(args, builder))
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