using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces
{
    public interface IUserYoutubeChannelSubscriptionRepository
    {
        Task<bool> ExistsAsync(string userId, Guid youtubeChannelId, CancellationToken cancellationToken = default);
        Task AddAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default);
        Task<List<UserYoutubeChannelSubscription>> GetByYoutubeChannelIdAsync(Guid youtubeChannelId, CancellationToken cancellationToken = default);
        Task<List<UserYoutubeChannelSubscription>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserYoutubeChannelSubscription?> GetByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
        Task DeleteAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default);
    }
}
