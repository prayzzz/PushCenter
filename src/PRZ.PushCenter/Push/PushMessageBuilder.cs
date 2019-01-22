using Lib.Net.Http.WebPush;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PRZ.PushCenter.Common.Attributes;

namespace PRZ.PushCenter.Push
{
    public static class PushMessageBuilder
    {
        private static readonly JsonSerializerSettings PushMessageSettings;

        static PushMessageBuilder()
        {
            PushMessageSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public static PushMessage Build(string message, string title, string iconUrl)
        {
            var model = new Model(message, title, iconUrl);
            return new PushMessage(JsonConvert.SerializeObject(model, PushMessageSettings));
        }

        [JsonModel]
        private class Model
        {
            public Model(string message, string title, string iconUrl)
            {
                Message = message;
                Title = title;
                IconUrl = iconUrl;
            }

            public string Message { get; }

            public string Title { get; }

            public string IconUrl { get; }
        }
    }
}