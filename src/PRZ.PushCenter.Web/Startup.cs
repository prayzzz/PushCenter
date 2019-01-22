﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PRZ.PushCenter.Common;
using PRZ.PushCenter.Push;
using PRZ.PushCenter.Push.Handler;
using PRZ.PushCenter.Subscriptions;
using PRZ.PushCenter.Web.Common;
using Swashbuckle.AspNetCore.Swagger;

namespace PRZ.PushCenter.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<PushApiOptions>(_configuration.GetSection("PushCenter:PushApi"));

            services.AddDbContext<PushCenterDbContext>(o => o.UseSqlServer(_configuration.GetConnectionString("PushCenter")));

            services.AddHttpClient();

            services.AddScoped<PushClient>();
            services.AddScoped<IPushMessageHandler, DemoPushMessageHandler>();
            services.AddScoped<IPushMessageHandler, ServerPushMessageHandler>();
            services.AddScoped<IPushMessageHandler, SmartHomePushMessageHandler>();

            services.AddScoped<SubscriptionService>();
            services.AddScoped<SubscriptionTypeService>();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "PushCenter", Version = "v1" }); });

            services.AddResponseCompression();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.LogServerAddresses(_logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "PushCenter v1"); });
            }

            app.Use((context, next) =>
            {
                context.Response.Headers.Add("Service-Worker-Allowed", "/");
                return next.Invoke();
            });

            app.UseResponseCompression();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}