using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces
{
    public interface IYoutubeChannelSubscriptionService
    {
        Task<ServiceResponse<SubscribeToYoutubeChannelResponse>> SubscribeAsync(
            SubscribeToYoutubeChannelRequest request,
            CancellationToken cancellationToken = default);
        Task<ServiceResponse<List<GetUserSubscriptionsResponse>>> GetUserSubscriptionsAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> UnsubscribeAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> UpdateSummarizationStyleAsync(Guid subscriptionId, UpdateSummarizationStyleRequest request, CancellationToken cancellationToken = default);
    }
}
