using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRZ.PushCenter.Bll.Push.Handler;
using PRZ.PushCenter.Web.Models;

namespace PRZ.PushCenter.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly ServerPushMessageHandler _serverPushMessageHandler;

        public WebhookController(ServerPushMessageHandler serverPushMessageHandler,
                                 ILogger<WebhookController> logger)
        {
            _serverPushMessageHandler = serverPushMessageHandler;
            _logger = logger;
        }

        [HttpPost("grafana")]
        public async Task<IActionResult> Grafana([FromBody] GrafanaHookModel model)
        {
            _logger.LogDebug("Received Grafana alert '{title}'", model.Title);

            var pushMessageDto = new PushMessageDto
            {
                Title = model.Title,
                Body = model.Message,
                Link = model.RuleUrl,
                Icon = "/image/push-icons/grafana.png"
            };

            await _serverPushMessageHandler.Handle(pushMessageDto);

            return Ok();
        }
    }
}