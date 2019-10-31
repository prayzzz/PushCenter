using System;
using System.Collections.Generic;
using Lib.Net.Http.WebPush;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PushCenter.Bll.Subscriptions;

namespace PushCenter.Bll.Push
{
    public enum Urgency
    {
        None = 0,
        VeryLow = 1,
        Low = 2,
        Normal = 3,
        High = 4
    }

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
        private TimeSpan? _timeToLive;
        private Urgency _urgency;

        private PushMessage(string title, string message)
        {
            _title = title;
            _message = message;
            _urgency = Urgency.Normal;
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

        public PushMessage WithUrgency(Urgency urgency)
        {
            if (_urgency != Urgency.None)
            {
                _urgency = urgency;
            }

            return this;
        }

        public PushMessage WithTimeToLive(TimeSpan timeTolive)
        {
            _timeToLive = timeTolive;

            return this;
        }

        internal Lib.Net.Http.WebPush.PushMessage Build()
        {
            var model = new Model(_title, _message, _iconUrl, _link);
            var message = new Lib.Net.Http.WebPush.PushMessage(JsonConvert.SerializeObject(model, JsonSerializerSettings));
            message.TimeToLive = _timeToLive?.Seconds;

            switch (_urgency)
            {
                case Urgency.VeryLow:
                    message.Urgency = PushMessageUrgency.VeryLow;
                    break;
                case Urgency.Low:
                    message.Urgency = PushMessageUrgency.Low;
                    break;
                case Urgency.Normal:
                    message.Urgency = PushMessageUrgency.Normal;
                    break;
                case Urgency.High:
                    message.Urgency = PushMessageUrgency.High;
                    break;
            }

            return message;
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
