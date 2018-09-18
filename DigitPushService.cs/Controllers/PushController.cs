using System.Threading.Tasks;
using DigitPushService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PushServer.Abstractions;
using PushServer.Services;

namespace DigitPushService.Controllers
{
    [Route("api")]
    public class PushController : ControllerBase
    {
        private readonly IPushService pushService;
        private readonly ILogger<PushController> logger;

        public PushController(IPushService pushService,
            ILogger<PushController> logger)
        {
            this.pushService = pushService;
            this.logger = logger;
        }

        [Authorize("Service")]
        [HttpPost("{userId}/push")]
        public async Task<IActionResult> Push(string userId, [FromBody]PushRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                if (!string.IsNullOrEmpty(request.ChannelId))
                {
                    await pushService.Push(request.ChannelId, request.Payload, null);
                }
                else if (null != request.ChannelOptions)
                {
                    await pushService.Push(userId, request.ChannelOptions, request.Payload, null);
                }
                return StatusCode(201);
            }
            catch (PushConfigurationNotFoundException)
            {
                return NotFound();
            }
            catch (PushException ex)
            {
                logger.LogError("Error while sending push request", ex);
                throw;
            }
        }
    }
}
