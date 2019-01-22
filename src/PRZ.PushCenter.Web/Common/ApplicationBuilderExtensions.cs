using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PRZ.PushCenter.Web.Common
{
    public static class ApplicationBuilderExtensions
    {
        public static void LogServerAddresses(this IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.ApplicationServices.GetService<IApplicationLifetime>().ApplicationStarted.Register(() =>
            {
                var hostingEnvironment = app.ApplicationServices.GetService<IHostingEnvironment>();
                logger.LogInformation("Hosting environment: {EnvironmentName}", hostingEnvironment.EnvironmentName);
                logger.LogInformation("Content root path: {ContentRootPath}", hostingEnvironment.ContentRootPath);

                var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
                if (serverAddressesFeature != null)
                {
                    logger.LogInformation("Now listening on: {Url}", string.Join(", ", serverAddressesFeature.Addresses));
                }
            });
        }
    }
}