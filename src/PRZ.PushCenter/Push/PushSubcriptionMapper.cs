using Lib.Net.Http.WebPush;
using PRZ.PushCenter.Subscriptions;

namespace PRZ.PushCenter.Push
{
    public static class PushSubcriptionMapper
    {
        public static PushSubscription Map(Subscription subscription)
        {
            var pushSubscription = new PushSubscription();
            pushSubscription.Endpoint = subscription.Endpoint;
            pushSubscription.SetKey(PushEncryptionKeyName.Auth, subscription.Auth);
            pushSubscription.SetKey(PushEncryptionKeyName.P256DH, subscription.P256Dh);
            return pushSubscription;
        }
    }
}