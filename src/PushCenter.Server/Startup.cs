using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PushCenter.Bll;
using PushCenter.Bll.Common;
using PushCenter.Bll.Push;
using PushCenter.Bll.Subscriptions;
using PushCenter.Server.Common;

namespace PushCenter.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<PushApiOptions>(_configuration.GetSection("PushCenter:PushApi"));

            services.AddDbContext<PushCenterDbContext>(
                                                       o => o.UseNpgsql(
                                                                        _configuration.GetConnectionString("PushCenter"),
                                                                        b => b.MigrationsAssembly("PushCenter.Server")
                                                                       )
                                                      );

            services.AddHttpClient();

            services.AddScoped<PushClient>();

            services.AddScoped<SubscriptionService>();
            services.AddScoped<SubscriptionTypeService>();

            services.AddResponseCompression();

            services.AddControllers(o => o.InputFormatters.Add(new TextPlainInputFormatter()))
                    .AddNewtonsoftJson();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "PushCenter" });
                options.EnableAnnotations();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddRouting(o =>
            {
                o.LowercaseUrls = true;
                o.LowercaseQueryStrings = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.LogServerAddresses(logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "PushCenter"); });
            }

            app.Use((context, next) =>
            {
                context.Response.Headers.Add("Service-Worker-Allowed", "/");
                return next.Invoke();
            });

            app.UseResponseCompression();

            app.UseBlazorFrameworkFiles();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}