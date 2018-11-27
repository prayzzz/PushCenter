namespace PRZ.PushCenter.Subscriptions
{
    public class Subscription
    {
        public Subscription(string endpoint, string auth, string p256dh, SubscriptionType subscriptionType)
        {
            Endpoint = endpoint;
            Auth = auth;
            P256DH = p256dh;
            SubscriptionType = subscriptionType;
        }

        public string Endpoint { get; set; }

        public string Auth { get; set; }

        public SubscriptionType SubscriptionType { get; set; }

        // ReSharper disable once InconsistentNaming
        public string P256DH { get; set; }
    }
    
    public enum SubscriptionType
    {
        Demo,
        SmartHome
    }
}