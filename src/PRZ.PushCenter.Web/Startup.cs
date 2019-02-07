﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PRZ.PushCenter.Bll;
using PRZ.PushCenter.Bll.Common;
using PRZ.PushCenter.Bll.Push;
using PRZ.PushCenter.Bll.Push.Handler;
using PRZ.PushCenter.Bll.Subscriptions;
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
            services.Configure<BasicAuthenticationOptions>(_configuration.GetSection("PushCenter:BasicAuthentication"));

            services.AddDbContext<PushCenterDbContext>(o => o.UseSqlServer(_configuration.GetConnectionString("PushCenter")));

            services.AddHttpClient();

            services.AddScoped<PushClient>();
            services.AddScoped<IPushMessageHandler, ServerPushMessageHandler>();
            services.AddScoped<IPushMessageHandler, SmartHomePushMessageHandler>();

            services.AddScoped<SubscriptionService>();
            services.AddScoped<SubscriptionTypeService>();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "PushCenter", Version = "v1" }); });

            services.AddResponseCompression();
            services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            }).AddMetrics();

            services.AddAuthentication("BasicAuthentication")
                    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddPushCenterMetrics(_configuration);
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

            app.UsePushCenterMetrics();

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}