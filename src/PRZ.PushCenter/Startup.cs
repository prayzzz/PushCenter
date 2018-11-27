using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PRZ.PushCenter.Common;
using PRZ.PushCenter.Subscriptions;
using Swashbuckle.AspNetCore.Swagger;

namespace PRZ.PushCenter
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<PushCenterOptions>(_configuration.GetSection("PushCenter"));

            services.AddDbContext<PushCenterDbContext>(o => o.UseSqlServer(_configuration.GetConnectionString("PushCenter")));
            services.AddScoped<SubscriptionService>();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "PushCenter", Version = "v1"}); });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "PushCenter V1"); });
            }


            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}