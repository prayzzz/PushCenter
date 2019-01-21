using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Logging;

namespace PRZ.PushCenter.Subscriptions
{
    public class SubscriptionService
    {
        private readonly PushCenterDbContext _dbContext;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(PushCenterDbContext dbContext, ILogger<SubscriptionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Task Save(PushSubscription pushSubscription, SubscriptionType subscriptionType)
        {
            _logger.LogInformation($"Adding '{subscriptionType}' subscription for '{pushSubscription.Endpoint}'");

            if (_dbContext.Subscriptions.Any(s => s.Endpoint == pushSubscription.Endpoint && s.SubscriptionType == subscriptionType))
            {
                return Task.CompletedTask;
            }

            var subscription = new Subscription(
                pushSubscription.Endpoint,
                pushSubscription.GetKey(PushEncryptionKeyName.Auth),
                pushSubscription.GetKey(PushEncryptionKeyName.P256DH),
                subscriptionType
            );

            _dbContext.Add(subscription);
            return _dbContext.SaveChangesAsync();
        }

        public IEnumerable<SubscriptionType> Find(string endpoint)
        {
            return _dbContext.Subscriptions.Where(s => s.Endpoint == endpoint).Select(s => s.SubscriptionType);
        }

        public Task Delete(PushSubscription pushSubscription, SubscriptionType subscriptionType)
        {
            _logger.LogInformation($"Deleting '{subscriptionType}' subscription for '{pushSubscription.Endpoint}'");

            var subscriptions = _dbContext.Subscriptions
                                          .Where(s => s.Endpoint == pushSubscription.Endpoint && s.SubscriptionType == subscriptionType);

            _dbContext.Subscriptions.RemoveRange(subscriptions);
            return _dbContext.SaveChangesAsync();
        }
    }
}