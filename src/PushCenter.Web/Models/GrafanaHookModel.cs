using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using PushCenter.Common.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PushCenter.Web.Models
{
    /// <summary>
    ///     Based on <see cref="http://docs.grafana.org/alerting/notifications/#webhook" />
    /// </summary>
    [JsonModel]
    [SwaggerSchemaFilter(typeof(WebHookSchemaFilter))]
    public class GrafanaHookModel : IOpenApiAny
    {
        public string Title { get; set; }
        public string RuleName { get; set; }
        public string RuleUrl { get; set; }
        public string Message { get; set; }

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteAny(this);
        }

        public AnyType AnyType { get; } = AnyType.Object;
    }

    public class WebHookSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new GrafanaHookModel
            {
                Message = "My Message",
                RuleName = "My Alert Rule",
                RuleUrl = "http://my.grafana.instance",
                Title = "My Panel Title"
            };
        }
    }
}
