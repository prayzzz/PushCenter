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
    public class SendController : ControllerBase
    {
        private readonly ILogger<SendController> _logger;
        private readonly IEnumerable<IPushMessageHandler> _pushMessageHandlers;

        public SendController(IEnumerable<IPushMessageHandler> pushMessageHandlers,
                              ILogger<SendController> logger)
        {
            _pushMessageHandlers = pushMessageHandlers;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromQuery] SubscriptionType type, [FromBody] PushMessageModel message)
        {
            _logger.LogDebug("Received send-notification requests for type '{type}'", type);

            var pushMessageDto = new PushMessageDto
            {
                Title = message.Title,
                Body = message.Body,
                Link = string.Empty
            };

            foreach (var handler in _pushMessageHandlers)
            {
                if (handler.SubscriptionType == type)
                {
                    await handler.Handle(pushMessageDto);
                }
            }

            return Ok();
        }
    }
}