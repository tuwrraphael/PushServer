using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PushServer.Abstractions.Services;
using PushServer.AzureNotificationHub;
using PushServer.Services;
using PushServer.WebPush;
using PushServer.WebPushApiClient;

namespace DigitPushService.Controllers
{
    [Route("api")]
    public class PushController : ControllerBase
    {
        private readonly IPushConfigurationManager pushConfigurationManager;
        private readonly IPushService pushService;
        private readonly IOptions<VapidAuthenticationOptions> vapidAuthenticationOptionsAccessor;

        public PushController(IPushConfigurationManager pushConfigurationManager,
            IPushService pushService, IOptions<VapidAuthenticationOptions> vapidAuthenticationOptionsAccessor)
        {
            this.pushConfigurationManager = pushConfigurationManager;
            this.pushService = pushService;
            this.vapidAuthenticationOptionsAccessor = vapidAuthenticationOptionsAccessor;
        }

        [HttpGet("stuff")]
        public async Task<IActionResult> Stuff(string configId)
        {
            await pushService.Push(configId, null, new PushServer.Models.PushOptions());
            return Ok();
        }

        [Authorize("User")]
        [HttpPost("me/push")]
        [Consumes("application/vnd+pushserver.azurenotificationhub+json")]
        public async Task<IActionResult> Register([FromBody]AzureNotificationHubPushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await pushConfigurationManager.RegisterAsync(User.GetId(), registration);
            return Ok();
        }

        [Authorize("User")]
        [HttpPost("me/push")]
        [Consumes("application/vnd+pushserver.webpush+json")]
        public async Task<IActionResult> Register([FromBody]WebPushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await pushConfigurationManager.RegisterAsync(User.GetId(), registration);
            return Ok();
        }

        [Authorize("User")]
        [HttpGet("me/push")]
        public async Task<IActionResult> Get()
        {
            return Ok(await pushConfigurationManager.GetAllAsync(User.GetId()));
        }

        [Authorize("User")]
        [HttpDelete("me/push/{configurationId}")]
        public async Task<IActionResult> Delete(string configurationId)
        {
            var success = await pushConfigurationManager.DeleteAsync(User.GetId(), configurationId);
            if (success)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize("User")]
        [HttpPut("me/push/{configurationid}")]
        [Consumes("application/vnd+pushserver.azurenotificationhub+json")]
        public async Task<IActionResult> Update([FromQuery]string configurationid, [FromBody]AzureNotificationHubPushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await pushConfigurationManager.UpdateAsync(User.GetId(), configurationid, registration);
            return Ok();
        }

        [Authorize("User")]
        [HttpPut("me/push/{configurationid}")]
        [Consumes("application/vnd+pushserver.webpush+json")]
        public async Task<IActionResult> Update([FromQuery]string configurationid, [FromBody]WebPushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await pushConfigurationManager.UpdateAsync(User.GetId(), configurationid, registration);
            return Ok();
        }
    }
}
