using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PRZ.PushCenter.Bll.Common;
using PRZ.PushCenter.Bll.Subscriptions;

namespace PRZ.PushCenter.Bll.Push
{
    public class PushClient : PushServiceClient
    {
        #region Metrics

        private static readonly CounterOptions MetricsSend = new CounterOptions
        {
            Name = "Bll_Push_Send"
        };

        #endregion

        private readonly ILogger<PushClient> _logger;
        private readonly IMetricsRoot _metrics;
        private readonly SubscriptionService _subscriptionService;
        private readonly VapidAuthentication _vapidAuthentication;

        public PushClient(SubscriptionService subscriptionService,
                          IHttpClientFactory client,
                          IOptions<PushApiOptions> options,
                          IMetricsRoot metrics,
                          ILogger<PushClient> logger) : base(client.CreateClient())
        {
            _logger = logger;
            _metrics = metrics;
            _subscriptionService = subscriptionService;

            _vapidAuthentication = new VapidAuthentication(options.Value.PublicKey, options.Value.PrivateKey)
            {
                Subject = options.Value.Subject
            };
        }

        public async Task Send(SubscriptionType subscriptionType, PushMessage pushMessage)
        {
            var subscriptions = _subscriptionService.Find(subscriptionType).Select(PushSubcriptionMapper.Map).ToList();

            _metrics.Measure.Counter.Increment(MetricsSend, subscriptions.Count);
            _logger.LogDebug("Sending PushMessage to {count} subscribers of type '{subscriptionType}'", subscriptions.Count, subscriptionType);

            foreach (var subscription in subscriptions)
            {
                try
                {
                    await RequestPushMessageDeliveryAsync(subscription, pushMessage, _vapidAuthentication);
                }
                catch (PushServiceClientException e)
                {
                    _logger.LogError(e,
                                     "Failed to send Notification to {count} subscribers of type '{subscriptionType}'",
                                     subscriptions.Count, subscriptionType);
                }
            }
        }
    }
}