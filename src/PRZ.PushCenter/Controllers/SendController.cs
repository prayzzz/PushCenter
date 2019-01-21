using System.Linq;
using System.Threading.Tasks;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PRZ.PushCenter.Common;
using PRZ.PushCenter.Subscriptions;

namespace PRZ.PushCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SendController> _logger;
        private readonly PushCenterOptions _options;
        private readonly PushServiceClient _pushServiceClient;
        private readonly SubscriptionService _subscriptionService;

        public SendController(SubscriptionService subscriptionService,
                              PushServiceClient pushServiceClient,
                              IHttpContextAccessor httpContextAccessor,
                              IOptions<PushCenterOptions> options,
                              ILogger<SendController> logger)
        {
            _subscriptionService = subscriptionService;
            _pushServiceClient = pushServiceClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _options = options.Value;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromQuery] SubscriptionType type, [FromBody] PushNotification notification)
        {
            var subscriptions = _subscriptionService.Find(type).Select(s =>
            {
                var pushSubscription = new PushSubscription();
                pushSubscription.Endpoint = s.Endpoint;
                pushSubscription.SetKey(PushEncryptionKeyName.Auth, s.Auth);
                pushSubscription.SetKey(PushEncryptionKeyName.P256DH, s.P256Dh);

                return pushSubscription;
            });

            var messageModel = new MessageModel
            {
                Title = notification.Title,
                Message = notification.Message,
                IconUrl = "/image/push-icons/smart-home.png"
            };

            var pushMessage = new PushMessage(JsonConvert.SerializeObject(messageModel));
            var auth = new VapidAuthentication(_options.PublicKey, _options.PrivateKey)
            {
                Subject = _options.Subject
            };

            foreach (var subscription in subscriptions)
            {
                try
                {
                    await _pushServiceClient.RequestPushMessageDeliveryAsync(subscription, pushMessage, auth);
                }
                catch (PushServiceClientException e)
                {
                    _logger.LogError(e, "Failed to send Notification");
                }
            }

            return Ok();
        }
    }

    public class PushNotification
    {
        public string Message { get; set; }

        public string Title { get; set; }
    }

    public class MessageModel
    {
        public string Message { get; set; }

        public string Title { get; set; }

        public string IconUrl { get; set; }
    }
}