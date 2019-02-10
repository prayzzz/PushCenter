using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRZ.PushCenter.Bll.Push.Handler;
using PRZ.PushCenter.Bll.Subscriptions;
using PRZ.PushCenter.Web.Models;

namespace PRZ.PushCenter.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IEnumerable<IPushMessageHandler> _pushMessageHandlers;

        public WebhookController(IEnumerable<IPushMessageHandler> pushMessageHandlers,
                                 ILogger<WebhookController> logger)
        {
            _pushMessageHandlers = pushMessageHandlers;
            _logger = logger;
        }

        [HttpPost("grafana")]
        public async Task<IActionResult> Grafana([FromBody] GrafanaHookModel model)
        {
            _logger.LogDebug("Received Grafana webhook");

            var pushMessageDto = new PushMessageDto
            {
                Title = model.Title,
                Body = model.Message,
                Link = model.RuleUrl
            };

            foreach (var handler in _pushMessageHandlers)
            {
                if (handler.SubscriptionType == SubscriptionType.Server)
                {
                    await handler.Handle(pushMessageDto);
                }
            }

            return Ok();
        }
    }
}