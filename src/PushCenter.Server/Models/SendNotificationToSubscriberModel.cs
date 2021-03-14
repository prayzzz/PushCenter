using PushCenter.Common.Attributes;

namespace PushCenter.Server.Models
{
    [JsonModel]
    public class SendNotificationToSubscriberModel
    {
        public string Endpoint { get; set; }

        public SendNotificationModel SendNotificationModel { get; set; }
    }
}