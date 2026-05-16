using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route("api/youtube-channels")]
    [Authorize]
    public class YoutubeChannelController : ControllerBase
    {
        private readonly IYoutubeChannelSubscriptionService _subscriptionService;

        public YoutubeChannelController(IYoutubeChannelSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(
            [FromBody] SubscribeToYoutubeChannelRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _subscriptionService.SubscribeAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
