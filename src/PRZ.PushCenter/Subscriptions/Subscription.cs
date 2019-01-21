namespace PRZ.PushCenter.Subscriptions
{
    public class Subscription : Entity
    {
        public Subscription(string endpoint, string auth, string p256Dh, SubscriptionType subscriptionType)
        {
            Endpoint = endpoint;
            Auth = auth;
            P256Dh = p256Dh;
            SubscriptionType = subscriptionType;
        }

        public string Endpoint { get; private set; }

        public string Auth { get; private set; }

        public string P256Dh { get; private set; }

        public SubscriptionType SubscriptionType { get; private set; }
    }
}