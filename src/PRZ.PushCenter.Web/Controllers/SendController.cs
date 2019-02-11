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

        public SendController(PushClient pushClient,
                              ILogger<SendController> logger)
        {
            _pushClient = pushClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromQuery] SubscriptionType type, [FromBody] PushMessageModel model)
        {
            _logger.LogDebug("Received send-notification requests for type '{type}'", type);

            var message = PushMessage.Create(model.Title, model.Body)
                                     .WithLink(model.Link)
                                     .WithImageForType(type);

            await _pushClient.Send(type, message);

            return Ok();
        }
    }
}