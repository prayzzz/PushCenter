using System;
using System.Threading.Tasks;
using Lib.Net.Http.WebPush;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PushCenter.Bll.Common;
using PushCenter.Bll.Subscriptions;

namespace PushCenter.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly PushApiOptions _pushApiOptions;
        private readonly SubscriptionService _subscriptionService;
        private readonly SubscriptionTypeService _subscriptionTypeService;

        public SubscriptionController(SubscriptionService subscriptionService,
                                      SubscriptionTypeService subscriptionTypeService,
                                      IOptions<PushApiOptions> pushCenterOptions)
        {
            _subscriptionService = subscriptionService;
            _subscriptionTypeService = subscriptionTypeService;
            _pushApiOptions = pushCenterOptions.Value;
        }

        [HttpDelete]
        public async Task<IActionResult> Unsubscribe([FromQuery] SubscriptionType type,
                                                     [FromBody] PushSubscription subscription)
        {
            if (type == SubscriptionType.None)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            await _subscriptionService.Delete(subscription, type);

            return Ok();
        }

        [HttpGet("public-key")]
        public IActionResult GetPublicKey()
        {
            return Content(_pushApiOptions.PublicKey, "text/plain");
        }

        [HttpGet("subscription-types")]
        public IActionResult GetSubscriptionTypes()
        {
            return new JsonResult(_subscriptionTypeService.GetSubscriptionTypes());
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe([FromQuery] SubscriptionType type,
                                                   [FromBody] PushSubscription subscription)
        {
            if (type == SubscriptionType.None)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            await _subscriptionService.Save(subscription, type);

            return Ok();
        }

        [HttpGet]
        public IActionResult Find([FromQuery] string endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            return new JsonResult(_subscriptionService.Find(endpoint));
        }
    }
}
