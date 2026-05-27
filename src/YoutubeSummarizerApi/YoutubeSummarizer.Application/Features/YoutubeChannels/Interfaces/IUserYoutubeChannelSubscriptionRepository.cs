using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces
{
    public interface IUserYoutubeChannelSubscriptionRepository
    {
        Task<bool> ExistsAsync(Guid userId, Guid channelId, CancellationToken cancellationToken = default);
        Task AddAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default);
        Task<List<UserYoutubeChannelSubscription>> GetByYoutubeChannelIdAsync(Guid channelId, CancellationToken cancellationToken = default);
        Task<List<UserYoutubeChannelSubscription>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserYoutubeChannelSubscription?> GetByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
        Task DeleteAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default);
        Task UpdateAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default);
    }
}
