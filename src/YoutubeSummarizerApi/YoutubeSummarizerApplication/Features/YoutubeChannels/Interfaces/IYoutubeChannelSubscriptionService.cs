using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces
{
    public interface IYoutubeChannelSubscriptionService
    {
        Task<ServiceResponse<SubscribeToYoutubeChannelResponse>> SubscribeAsync(
            SubscribeToYoutubeChannelRequest request,
            CancellationToken cancellationToken = default);
    }
}
