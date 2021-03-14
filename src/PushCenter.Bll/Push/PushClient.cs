using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PushCenter.Bll.Common;
using PushCenter.Bll.Subscriptions;

namespace PushCenter.Bll.Push
{
    public class PushClient : PushServiceClient
    {
        private readonly PushCenterDbContext _dbContext;

        private readonly ILogger<PushClient> _logger;
        private readonly VapidAuthentication _vapidAuthentication;

        public PushClient(IHttpClientFactory client,
            IOptions<PushApiOptions> options,
            PushCenterDbContext dbContext,
            ILogger<PushClient> logger) : base(client.CreateClient())
        {
            _logger = logger;
            _dbContext = dbContext;

            _vapidAuthentication = new VapidAuthentication(options.Value.PublicKey, options.Value.PrivateKey)
            {
                Subject = options.Value.Subject
            };
        }

        public async Task Send(IEnumerable<Subscription> subscriptions, PushMessage pushMessage)
        {
            var pushSubs = subscriptions.Select(PushSubcriptionMapper.Map).ToList();

            _logger.LogDebug("Sending PushMessage to {Count} subscribers", pushSubs.Count);

            foreach (var subscription in pushSubs)
            {
                try
                {
                    await RequestPushMessageDeliveryAsync(subscription, pushMessage.Build(), _vapidAuthentication);
                }
                catch (PushServiceClientException e)
                {
                    _logger.LogError(e, "Failed to send Notification to '{Endpoint}'", subscription.Endpoint);

                    if (e.Message == "Gone")
                    {
                        _logger.LogInformation("Removing gone subscription '{Endpoint}'", subscription.Endpoint);
                        var toDelete = _dbContext.Subscriptions.Where(s => s.Endpoint == subscription.Endpoint);
                        _dbContext.RemoveRange(toDelete);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public Task Send([NotNull] Subscription subscription, PushMessage pushMessage)
        {
            return Send(new[] { subscription }, pushMessage);
        }
    }
}