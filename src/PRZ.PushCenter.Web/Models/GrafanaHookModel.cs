using PRZ.PushCenter.Common.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PRZ.PushCenter.Web.Models
{
    /// <summary>
    ///     Based on <see cref="http://docs.grafana.org/alerting/notifications/#webhook" />
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
        public void Apply(Schema schema, SchemaFilterContext context)
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