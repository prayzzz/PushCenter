using System.Threading.Tasks;
using PRZ.PushCenter.Bll.Subscriptions;

namespace PRZ.PushCenter.Bll.Push.Handler
{
    public class SmartHomePushMessageHandler : IPushMessageHandler
    {
        private const string IconPath = "/image/push-icons/smart-home.png";
        private readonly PushClient _pushClient;

        public SmartHomePushMessageHandler(PushClient pushClient)
        {
            _pushClient = pushClient;
        }

        public Task Handle(PushMessageDto messageDto)
        {
            var pushMessage = PushMessageBuilder.Build(messageDto.Title, messageDto.Body, IconPath, messageDto.Link);
            return _pushClient.Send(SubscriptionType, pushMessage);
        }

        public SubscriptionType SubscriptionType => SubscriptionType.SmartHome;
    }
}