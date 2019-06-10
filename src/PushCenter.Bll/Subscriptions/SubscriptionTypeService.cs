using System.Collections.Generic;

namespace PushCenter.Bll.Subscriptions
{
    public class SubscriptionTypeService
    {
        private static readonly Dictionary<int, string> SubscriptionTypes = new Dictionary<int, string>
        {
            { (int) SubscriptionType.SmartHome, "SmartHome" },
            { (int) SubscriptionType.Server, "Server" }
        };

        public Dictionary<int, string> GetSubscriptionTypes()
        {
            return SubscriptionTypes;
        }
    }
}
