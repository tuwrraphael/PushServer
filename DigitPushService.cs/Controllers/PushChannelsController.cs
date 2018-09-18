using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PushServer.Abstractions.Services;
using PushServer.AzureNotificationHub;
using PushServer.WebPush;
using PushServer.WebPushApiClient;

namespace DigitPushService.Controllers
{
    [Route("api")]
    public class PushChannelsController : ControllerBase
    {
        private readonly IPushConfigurationManager pushConfigurationManager;
        private readonly IOptions<VapidAuthenticationOptions> vapidAuthenticationOptionsAccessor;

        public PushChannelsController(IPushConfigurationManager pushConfigurationManager)
        {
            this.pushConfigurationManager = pushConfigurationManager;
        }

        [Authorize("User")]
        [HttpPost("me/pushchannels")]
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
        [HttpPost("me/pushchannels")]
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
        [HttpGet("me/pushchannels")]
        public async Task<IActionResult> Get()
        {
            return Ok(await pushConfigurationManager.GetAllAsync(User.GetId()));
        }

        [Authorize("User")]
        [HttpDelete("me/pushchannels/{configurationId}")]
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
        [HttpPut("me/pushchannels/{configurationid}")]
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
        [HttpPut("me/pushchannels/{configurationid}")]
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
