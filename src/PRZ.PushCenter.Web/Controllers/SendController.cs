using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRZ.PushCenter.Bll.Push;
using PRZ.PushCenter.Bll.Subscriptions;
using PRZ.PushCenter.Web.Models;

namespace PRZ.PushCenter.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendController : ControllerBase
    {
        private readonly ILogger<SendController> _logger;
        private readonly PushClient _pushClient;
        private readonly SubscriptionService _subscriptionService;

        public SendController(SubscriptionService subscriptionService,
                              PushClient pushClient,
                              ILogger<SendController> logger)
        {
            _subscriptionService = subscriptionService;
            _pushClient = pushClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromQuery] SubscriptionType type, [FromBody] SendNotificationModel model)
        {
            _logger.LogDebug("Received send-notification requests for type '{type}'", type);

            var message = BuildMessage(type, model);

            var subscriptions = _subscriptionService.Find(type);
            await _pushClient.Send(subscriptions, message);

            return Ok();
        }

        [HttpPost("toSubscriber")]
        public async Task<IActionResult> SendNotificationToSubscriber([FromQuery] SubscriptionType type,
                                                                      [FromBody] SendNotificationToSubscriberModel model)
        {
            _logger.LogDebug("Received send-notification requests for type '{type}'", type);

            var message = BuildMessage(type, model.SendNotificationModel);

            var subscription = _subscriptionService.Find(model.Endpoint, type);
            if (subscription == null)
            {
                _logger.LogError("Subscription not available for endpoint '{endpoint}' and type '{subscriptionType}'", model.Endpoint, type);
                return BadRequest();
            }

            await _pushClient.Send(subscription, message);
            return Ok();
        }

        private static PushMessage BuildMessage(SubscriptionType type, SendNotificationModel model)
        {
            var message = PushMessage.Create(model.Title, model.Body)
                                     .WithLink(model.Link)
                                     .WithImageForType(type)
                                     .WithUrgency(model.Urgency);

            if (model.TimeToLiveSeconds > 0)
            {
                message.WithTimeToLive(TimeSpan.FromSeconds(model.TimeToLiveSeconds));
            }

            return message;
        }
    }
}