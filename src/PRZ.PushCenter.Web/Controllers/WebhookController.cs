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

        public WebhookController(PushClient pushClient,
                                 ILogger<WebhookController> logger)
        {
            _pushClient = pushClient;
            _logger = logger;
        }

        [HttpPost("grafana")]
        public async Task<IActionResult> Grafana([FromBody] GrafanaHookModel model)
        {
            _logger.LogDebug("Received Grafana alert '{title}'", model.Title);

            var message = PushMessage.Create(model.Title, model.Message)
                                     .WithLink(model.RuleUrl)
                                     .WithImageUrl("/image/push-icons/grafana.png");

            await _pushClient.Send(SubscriptionType.Server, message);

            return Ok();
        }
    }
}