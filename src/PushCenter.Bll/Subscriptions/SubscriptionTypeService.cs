using System.Collections.Generic;
using PushCenter.Shared;

namespace PushCenter.Bll.Subscriptions
{
    public class SubscriptionTypeService
    {
        private static readonly Dictionary<int, string> SubscriptionTypes = new()
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