using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PushServer.Abstractions.Services;
using PushServer.AzureNotificationHub;
using PushServer.WebPush;

namespace DigitPushService.Controllers
{
    [Route("api")]
    public class PushController : ControllerBase
    {
        private readonly IPushConfigurationManager pushConfigurationManager;

        public PushController(IPushConfigurationManager pushConfigurationManager)
        {
            this.pushConfigurationManager = pushConfigurationManager;
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
