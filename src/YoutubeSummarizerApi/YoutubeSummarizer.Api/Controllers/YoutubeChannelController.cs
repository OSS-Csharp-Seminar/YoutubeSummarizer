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
            var result = await _subscriptionService.SubscribeAsync(request, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetUserSubscriptions(CancellationToken cancellationToken)
        {
            var result = await _subscriptionService.GetUserSubscriptionsAsync(cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("subscriptions/{subscriptionId:guid}")]
        public async Task<IActionResult> Unsubscribe(Guid subscriptionId, CancellationToken cancellationToken)
        {
            var result = await _subscriptionService.UnsubscribeAsync(subscriptionId, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
