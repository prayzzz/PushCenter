using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PushCenter.Bll;
using PushCenter.Bll.Common;
using PushCenter.Bll.Push;
using PushCenter.Bll.Subscriptions;
using PushCenter.Web.Common;
using Swashbuckle.AspNetCore.Swagger;

namespace PushCenter.Web
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

            services.AddScoped<SubscriptionService>();
            services.AddScoped<SubscriptionTypeService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "PushCenter" });
                c.EnableAnnotations();
            });

            services.AddResponseCompression();

            services.AddRouting(o =>
            {
                o.LowercaseUrls = true;
                o.LowercaseQueryStrings = true;
            });

            var mvc = services.AddMvc(options => { options.InputFormatters.Add(new TextPlainInputFormatter()); });
            mvc.SetCompatibilityVersion(CompatibilityVersion.Latest);
            mvc.AddMetrics();

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
