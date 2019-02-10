using PRZ.PushCenter.Common.Attributes;

namespace PRZ.PushCenter.Web.Models
{
    /// <summary>
    /// Based on <see cref="http://docs.grafana.org/alerting/notifications/#webhook"/>
    /// </summary>
    [JsonModel]
    public class GrafanaHookModel
    {
        public string Title { get; set; }
        public string RuleName { get; set; }
        public string RuleUrl { get; set; }
        public string Message { get; set; }
    }
}