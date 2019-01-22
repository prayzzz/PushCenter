using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PRZ.PushCenter.Common;
using PRZ.PushCenter.Subscriptions;

namespace PRZ.PushCenter.Push
{
    public class PushClient : Lib.Net.Http.WebPush.PushServiceClient
    {
        private readonly ILogger<PushClient> _logger;
        private readonly SubscriptionService _subscriptionService;
        private readonly VapidAuthentication _vapidAuthentication;

        public PushClient(SubscriptionService subscriptionService,
                          ILogger<PushClient> logger,
                          IOptions<PushApiOptions> options,
                          IHttpClientFactory client) : base(client.CreateClient())
        {
            _subscriptionService = subscriptionService;
            _logger = logger;

            _vapidAuthentication = new VapidAuthentication(options.Value.PublicKey, options.Value.PrivateKey)
            {
                Subject = options.Value.Subject
            };
        }

        public async Task Send(SubscriptionType subscriptionType, Lib.Net.Http.WebPush.PushMessage pushMessage)
        {
            var subscriptions = _subscriptionService.Find(subscriptionType).Select(PushSubcriptionMapper.Map);

            foreach (var subscription in subscriptions)
            {
                try
                {
                    await RequestPushMessageDeliveryAsync(subscription, pushMessage, _vapidAuthentication);
                }
                catch (Lib.Net.Http.WebPush.PushServiceClientException e)
                {
                    _logger.LogError(e, "Failed to send Notification");
                }
            }
        }
    }
}