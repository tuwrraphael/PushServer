using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.PushConfiguration.Abstractions.Services;

namespace DigitPushService.Controllers
{
    [Route("api")]
    public class PushController : ControllerBase
    {
        private readonly IPushConfigurationStore pushConfigurationService;

        public PushController(IPushConfigurationStore pushConfigurationService)
        {
            this.pushConfigurationService = pushConfigurationService;
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
            await pushConfigurationService.RegisterAsync(User.GetId(), registration);
            return Ok();
        }

        [Authorize("User")]
        [HttpPost("me/push")]
        [Consumes("application/vnd+pushserver.webpush+json")]
        public async Task<IActionResult> Register([FromBody]WebPushChannelRegistration registration)
        {
            await pushConfigurationService.RegisterAsync(User.GetId(), registration);
            return Ok();
        }

        [Authorize("User")]
        [HttpGet("me/push")]
        public async Task<IActionResult> Get()
        {
            return Ok(await pushConfigurationService.GetAllAsync(User.GetId()));
        }

        [Authorize("User")]
        [HttpDelete("me/push/{configurationId}")]
        public async Task<IActionResult> Register(string configurationId)
        {
            var success = await pushConfigurationService.DeleteAsync(User.GetId(), configurationId);
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
            await pushConfigurationService.UpdateAsync(User.GetId(), configurationid, registration);
            return Ok();
        }

        [Authorize("User")]
        [HttpPut("me/push/{configurationid}")]
        [Consumes("application/vnd+pushserver.webpush+json")]
        public async Task<IActionResult> Update([FromQuery]string configurationid, [FromBody]WebPushChannelRegistration registration)
        {
            await pushConfigurationService.UpdateAsync(User.GetId(), configurationid, registration);
            return Ok();
        }
    }
}
