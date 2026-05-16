using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeWebhooks;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route("api/webhooks/youtube")]
    public class YoutubeWebhooksController : ControllerBase
    {
        private readonly IYoutubeWebhookVerificationService _verificationService;
        private readonly IYoutubeWebhookNotificationService _notificationService;

        public YoutubeWebhooksController(
            IYoutubeWebhookVerificationService verificationService,
            IYoutubeWebhookNotificationService notificationService)
        {
            _verificationService = verificationService;
            _notificationService = notificationService;
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
            var (channelId, videoId) = YoutubeAtomXmlParser.Parse(xml);
            await _notificationService.ProcessNotificationAsync(channelId, videoId, cancellationToken);
            return Ok();
        }
    }
}
