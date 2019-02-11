using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PRZ.PushCenter.Bll.Subscriptions;
using PRZ.PushCenter.Common;

namespace PRZ.PushCenter.Bll.Push
{
    public class PushMessage
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private static readonly Dictionary<SubscriptionType, string> DefaultTypeIcon = new Dictionary<SubscriptionType, string>
        {
            { SubscriptionType.Server, "/image/push-icons/server.png" },
            { SubscriptionType.SmartHome, "/image/push-icons/smart-home.png" }
        };

        private readonly string _message;

        private readonly string _title;
        private string _iconUrl;
        private string _link;

        private PushMessage(string title, string message)
        {
            _title = title;
            _message = message;
        }

        public static PushMessage Create(string title, string message)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return new PushMessage(title, message);
        }

        public PushMessage WithLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                _link = link;
            }

            return this;
        }

        public PushMessage WithImageUrl(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                _iconUrl = imageUrl;
            }

            return this;
        }

        public PushMessage WithImageForType(SubscriptionType subscriptionType)
        {
            if (SubscriptionType.None != subscriptionType)
            {
                _iconUrl = DefaultTypeIcon.GetValueOrDefault(subscriptionType, null);
            }

            return this;
        }

        internal Lib.Net.Http.WebPush.PushMessage Build()
        {
            var model = new Model(_title, _message, _iconUrl, _link);
            return new Lib.Net.Http.WebPush.PushMessage(JsonConvert.SerializeObject(model, JsonSerializerSettings));
        }

        private class Model
        {
            public Model(string title, string message, string iconUrl, string link)
            {
                Message = message;
                Title = title;
                IconUrl = iconUrl;
                Link = link;
            }

            public string Message { get; }

            public string Title { get; }

            public string IconUrl { get; }

            public string Link { get; }
        }
    }
}