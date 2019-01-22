using System.Threading.Tasks;
using PRZ.PushCenter.Subscriptions;

namespace PRZ.PushCenter.Push.Handler
{
    public class DemoPushMessageHandler : IPushMessageHandler
    {
        private const string IconPath = "/image/push-icons/demo.png";
        private readonly PushClient _pushClient;

        public DemoPushMessageHandler(PushClient pushClient)
        {
            _pushClient = pushClient;
        }

        public Task Handle(PushMessage message)
        {
            return _pushClient.Send(SubscriptionType, PushMessageBuilder.Build(message.Title, message.Body, IconPath));
        }

        public SubscriptionType SubscriptionType => SubscriptionType.Demo;
    }
}