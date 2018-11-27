using System;
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
            _logger.LogInformation($"Adding {subscriptionType} subscription for {pushSubscription.Endpoint}");
            
            if (_dbContext.Subscriptions.Any(s => s.Endpoint == pushSubscription.Endpoint))
            {
                throw new InvalidOperationException($"{subscriptionType} subscription for {pushSubscription.Endpoint} already known");
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

        public Task Delete(string endpoint, SubscriptionType subscriptionType)
        {
            _logger.LogInformation($"Adding {subscriptionType} subscription for {endpoint}");
            
            var subscriptions = _dbContext.Subscriptions.Where(s => s.Endpoint == endpoint && s.SubscriptionType == subscriptionType);

            _dbContext.Subscriptions.RemoveRange(subscriptions);
            return _dbContext.SaveChangesAsync();
        }
    }
}