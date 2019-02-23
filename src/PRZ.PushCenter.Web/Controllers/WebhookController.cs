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
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly PushClient _pushClient;
        private readonly SubscriptionService _subscriptionService;

        public WebhookController(SubscriptionService subscriptionService,
                                 PushClient pushClient,
                                 ILogger<WebhookController> logger)
        {
            _subscriptionService = subscriptionService;
            _pushClient = pushClient;
            _logger = logger;
        }

        [HttpPost("grafana")]
        public async Task<IActionResult> Grafana([FromBody] GrafanaHookModel model)
        {
            _logger.LogDebug("Received Grafana alert '{title}'", model.Title);

            var message = PushMessage.Create(model.Title, model.Message)
                                     .WithLink(model.RuleUrl)
                                     .WithImageUrl("/image/push-icons/grafana.png")
                                     .WithUrgency(Urgency.High);

            var subscriptions = _subscriptionService.Find(SubscriptionType.Server);
            await _pushClient.Send(subscriptions, message);

            return Ok();
        }
    }
}