using PRZ.PushCenter.Common.Attributes;

namespace PRZ.PushCenter.Web.Models
{
    [JsonModel]
    public class SendNotificationToSubscriberModel
    {
        public string Endpoint { get; set; }

        public SendNotificationModel SendNotificationModel { get; set; }
    }
}