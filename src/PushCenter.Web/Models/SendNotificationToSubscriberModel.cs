using PushCenter.Common.Attributes;

namespace PushCenter.Web.Models
{
    [JsonModel]
    public class SendNotificationToSubscriberModel
    {
        public string Endpoint { get; set; }

        public SendNotificationModel SendNotificationModel { get; set; }
    }
}
