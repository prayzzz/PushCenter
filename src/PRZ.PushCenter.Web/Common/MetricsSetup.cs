using System;
using App.Metrics;
using App.Metrics.AspNetCore.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PRZ.PushCenter.Web.Common
{
    public static class MetricsSetup
    {
        private const string MetricsEndpointKey = "Kestrel:Endpoints:MetricHttp:Url";

        public static IServiceCollection AddPushCenterMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            var metricsEndpoint = configuration.GetValue<string>(MetricsEndpointKey);
            if (!string.IsNullOrEmpty(metricsEndpoint))
            {
                services.Configure<MetricsEndpointsHostingOptions>(o => o.MetricsEndpointPort = new Uri(metricsEndpoint).Port);
            }

            var metrics = new MetricsBuilder()
                          .OutputMetrics.AsInfluxDbLineProtocol()
                          .SampleWith.ForwardDecaying()
                          .TimeWith.StopwatchClock()
                          .Build();

            services.AddMetrics(metrics);
            services.AddMetricsEndpoints();
            services.AddMetricsTrackingMiddleware(o => o.ApdexTrackingEnabled = false);

            return services;
        }

        public static IApplicationBuilder UsePushCenterMetrics(this IApplicationBuilder app)
        {
            app.UseMetricsRequestTrackingMiddleware();
            app.UseMetricsErrorTrackingMiddleware();
            app.UseMetricsEndpoint();

            return app;
        }
    }
}