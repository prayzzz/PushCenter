using System.Threading.Tasks;
using PRZ.PushCenter.Bll.Subscriptions;

namespace PRZ.PushCenter.Bll.Push.Handler
{
    public class ServerPushMessageHandler : IPushMessageHandler
    {
        private const string IconPath = "/image/push-icons/server.png";
        private readonly PushClient _pushClient;

        public ServerPushMessageHandler(PushClient pushClient)
        {
            _pushClient = pushClient;
        }

        public Task Handle(PushMessageDto messageDto)
        {
            var pushMessage = PushMessageBuilder.Build(messageDto.Title, messageDto.Body, messageDto.Icon ?? IconPath, messageDto.Link);
            return _pushClient.Send(SubscriptionType, pushMessage);
        }

        public SubscriptionType SubscriptionType => SubscriptionType.Server;
    }
}