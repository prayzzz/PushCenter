using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRZ.PushCenter.Push.Handler;
using PRZ.PushCenter.Subscriptions;
using PRZ.PushCenter.Web.Models;

namespace PRZ.PushCenter.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendController : ControllerBase
    {
        private readonly IEnumerable<IPushMessageHandler> _pushMessageHandlers;

        public SendController(IEnumerable<IPushMessageHandler> pushMessageHandlers)
        {
            _pushMessageHandlers = pushMessageHandlers;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromQuery] SubscriptionType type, [FromBody] PushMessageModel message)
        {
            foreach (var handler in _pushMessageHandlers)
            {
                if (handler.SubscriptionType == type)
                {
                    await handler.Handle(new PushMessage(message.Title, message.Body));
                }
            }

            return Ok();
        }
    }
}