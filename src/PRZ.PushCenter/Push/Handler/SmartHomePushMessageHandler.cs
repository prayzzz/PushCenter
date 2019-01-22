using System.Threading.Tasks;
using PRZ.PushCenter.Subscriptions;

namespace PRZ.PushCenter.Push.Handler
{
    public class SmartHomePushMessageHandler : IPushMessageHandler
    {
        private const string IconPath = "/image/push-icons/smart-home.png";
        private readonly PushClient _pushClient;

        public SmartHomePushMessageHandler(PushClient pushClient)
        {
            _pushClient = pushClient;
        }

        public Task Handle(PushMessage message)
        {
            return _pushClient.Send(SubscriptionType, PushMessageBuilder.Build(message.Title, message.Body, IconPath));
        }

        public SubscriptionType SubscriptionType => SubscriptionType.SmartHome;
    }
}