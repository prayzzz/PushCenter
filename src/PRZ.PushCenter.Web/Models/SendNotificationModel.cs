using System.ComponentModel.DataAnnotations;
using PRZ.PushCenter.Bll.Push;
using PRZ.PushCenter.Common.Attributes;

namespace PRZ.PushCenter.Web.Models
{
    [JsonModel]
    public class SendNotificationModel
    {
        [Required]
        public string Body { get; set; }

        [Required]
        public string Title { get; set; }

        public string Link { get; set; }

        public Urgency Urgency { get; set; }

        public int TimeToLiveSeconds { get; set; }
    }
}