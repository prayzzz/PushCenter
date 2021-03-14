using System.ComponentModel.DataAnnotations;
using PushCenter.Bll.Push;
using PushCenter.Common.Attributes;

namespace PushCenter.Server.Models
{
    [JsonModel]
    public class SendNotificationModel
    {
        [Required] public string Body { get; set; }

        [Required] public string Title { get; set; }

        public string Link { get; set; }

        public Urgency Urgency { get; set; }

        public int TimeToLiveSeconds { get; set; }
    }
}