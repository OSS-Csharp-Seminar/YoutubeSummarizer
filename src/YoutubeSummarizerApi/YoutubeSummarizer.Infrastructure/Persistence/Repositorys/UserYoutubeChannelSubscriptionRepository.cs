using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositorys
{
    public class UserYoutubeChannelSubscriptionRepository : IUserYoutubeChannelSubscriptionRepository
    {
        private readonly ApplicationDbContext _db;

        public UserYoutubeChannelSubscriptionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> ExistsAsync(string userId, Guid youtubeChannelId, CancellationToken cancellationToken = default)
            => _db.UserYoutubeChannelSubscriptions.AnyAsync(
                x => x.UserId == userId && x.YoutubeChannelId == youtubeChannelId,
                cancellationToken);

        public async Task AddAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default)
        {
            _db.UserYoutubeChannelSubscriptions.Add(subscription);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<List<UserYoutubeChannelSubscription>> GetByYoutubeChannelIdAsync(Guid youtubeChannelId, CancellationToken cancellationToken = default)
            => _db.UserYoutubeChannelSubscriptions
                .Where(x => x.YoutubeChannelId == youtubeChannelId)
                .ToListAsync(cancellationToken);

        public Task<List<UserYoutubeChannelSubscription>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
            => _db.UserYoutubeChannelSubscriptions
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
    
        public Task<UserYoutubeChannelSubscription?> GetByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
            => _db.UserYoutubeChannelSubscriptions
                .FirstOrDefaultAsync(x => x.Id == subscriptionId, cancellationToken);

        public async Task DeleteAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default)
        {
            _db.UserYoutubeChannelSubscriptions.Remove(subscription);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

}
