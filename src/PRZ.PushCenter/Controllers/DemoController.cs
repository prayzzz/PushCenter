using System.Threading.Tasks;
using Lib.Net.Http.WebPush;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PRZ.PushCenter.Common;
using PRZ.PushCenter.Subscriptions;

namespace PRZ.PushCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        private const SubscriptionType Type = SubscriptionType.Demo;

        private readonly SubscriptionService _subscriptionService;
        private readonly PushCenterOptions _pushCenterOptions;

        public DemoController(SubscriptionService subscriptionService, IOptions<PushCenterOptions> pushCenterOptions)
        {
            _subscriptionService = subscriptionService;
            _pushCenterOptions = pushCenterOptions.Value;
        }

        [HttpGet("public-key")]
        public ContentResult GetPublicKey()
        {
            return Content(_pushCenterOptions.PublicKey, "text/plain");
        }

        [HttpPost("subscription")]
        public async Task<IActionResult> Save([FromBody] PushSubscription subscription)
        {
            await _subscriptionService.Save(subscription, Type);

            return NoContent();
        }

        [HttpDelete("subscription")]
        public async Task<IActionResult> Delete(string endpoint)
        {
            await _subscriptionService.Delete(endpoint, Type);

            return NoContent();
        }
    }
}