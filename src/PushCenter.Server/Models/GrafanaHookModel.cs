using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PushCenter.Common.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PushCenter.Server.Models
{
    /// <summary>
    ///     Based on http://docs.grafana.org/alerting/notifications/
    /// </summary>
    [JsonModel]
    [SwaggerSchemaFilter(typeof(WebHookSchemaFilter))]
    public class GrafanaHookModel
    {
        public string Title { get; set; }
        public string RuleName { get; set; }
        public string RuleUrl { get; set; }
        public string Message { get; set; }
    }

    public class WebHookSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                [nameof(GrafanaHookModel.Title)] = new OpenApiString("My Message"),
                [nameof(GrafanaHookModel.RuleName)] = new OpenApiString("My Alert Rule"),
                [nameof(GrafanaHookModel.RuleUrl)] = new OpenApiString("http://my.grafana.instance"),
                [nameof(GrafanaHookModel.Message)] = new OpenApiString("My Panel Title")
            };
        }
    }
}