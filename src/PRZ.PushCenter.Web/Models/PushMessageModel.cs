using PRZ.PushCenter.Common.Attributes;

namespace PRZ.PushCenter.Web.Models
{
    [JsonModel]
    public class PushMessageModel
    {
        public string Body { get; set; }

        public string Title { get; set; }
    }
}