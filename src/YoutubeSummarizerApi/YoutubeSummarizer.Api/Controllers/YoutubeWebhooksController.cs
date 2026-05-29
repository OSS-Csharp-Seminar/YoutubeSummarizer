using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route(YoutubeWebhookCallbackPath.Value)]
    public class YoutubeWebhooksController : ControllerBase
    {
        private readonly IYoutubeWebhookVerificationService _verificationService;
        private readonly IWebhookPayloadQueue _payloadQueue;

        public YoutubeWebhooksController(
            IYoutubeWebhookVerificationService verificationService,
            IWebhookPayloadQueue payloadQueue)
        {
            _verificationService = verificationService;
            _payloadQueue = payloadQueue;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Verify(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.topic")] string topic,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.lease_seconds")] int leaseSeconds,
            CancellationToken cancellationToken)
        {
            if (mode == "subscribe")
                await _verificationService.ConfirmSubscriptionAsync(topic, leaseSeconds, cancellationToken);

            return Content(challenge, "text/plain");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Notify(CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(Request.Body);
            var xml = await reader.ReadToEndAsync(cancellationToken);
            await _payloadQueue.EnqueueAsync(xml, cancellationToken);
            return Ok();
        }
    }
}
