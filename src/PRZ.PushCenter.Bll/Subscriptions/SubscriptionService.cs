using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using JetBrains.Annotations;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Logging;

namespace PRZ.PushCenter.Bll.Subscriptions
{
    public class SubscriptionService
    {
        private readonly PushCenterDbContext _dbContext;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IMetricsRoot _metrics;

        public SubscriptionService(PushCenterDbContext dbContext, IMetricsRoot metrics, ILogger<SubscriptionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _metrics = metrics;
        }

        public Task Save(PushSubscription pushSubscription, SubscriptionType subscriptionType)
        {
            _metrics.Measure.Counter.Increment(MetricsSubscribed);
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

        [CanBeNull]
        public Subscription Find(string endpoint, SubscriptionType type)
        {
            return _dbContext.Subscriptions.SingleOrDefault(s => s.Endpoint == endpoint && s.SubscriptionType == type);
        }

        public IEnumerable<SubscriptionType> Find(string endpoint)
        {
            return _dbContext.Subscriptions.Where(s => s.Endpoint == endpoint).Select(s => s.SubscriptionType);
        }

        public IEnumerable<Subscription> Find(SubscriptionType subscriptionType)
        {
            return _dbContext.Subscriptions.Where(s => s.SubscriptionType == subscriptionType);
        }

        public Task Delete(PushSubscription pushSubscription, SubscriptionType subscriptionType)
        {
            _metrics.Measure.Counter.Increment(MetricsDeleted);
            _logger.LogInformation($"Deleting '{subscriptionType}' subscription for '{pushSubscription.Endpoint}'");

            var subscriptions = _dbContext.Subscriptions
                                          .Where(s => s.Endpoint == pushSubscription.Endpoint && s.SubscriptionType == subscriptionType);

            _dbContext.Subscriptions.RemoveRange(subscriptions);
            return _dbContext.SaveChangesAsync();
        }

        #region Metrics

        private static readonly CounterOptions MetricsSubscribed = new CounterOptions
        {
            Name = "Bll_Subscriptions_Subscribed"
        };

        private static readonly CounterOptions MetricsDeleted = new CounterOptions
        {
            Name = "Bll_Subscriptions_Deleted"
        };

        #endregion
    }
}