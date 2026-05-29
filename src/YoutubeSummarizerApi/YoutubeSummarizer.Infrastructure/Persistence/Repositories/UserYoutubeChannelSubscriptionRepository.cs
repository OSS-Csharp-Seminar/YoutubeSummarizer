using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositories
{
    public class UserYoutubeChannelSubscriptionRepository : IUserYoutubeChannelSubscriptionRepository
    {
        private readonly ApplicationDbContext _db;

        public UserYoutubeChannelSubscriptionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> ExistsAsync(Guid userId, Guid channelId, CancellationToken cancellationToken = default)
            => _db.UserYoutubeChannelSubscriptions.AnyAsync(
                x => x.UserId == userId && x.ChannelId == channelId,
                cancellationToken);

        public async Task AddAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default)
        {
            _db.UserYoutubeChannelSubscriptions.Add(subscription);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<List<UserYoutubeChannelSubscription>> GetByYoutubeChannelIdAsync(Guid channelId, CancellationToken cancellationToken = default)
            => _db.UserYoutubeChannelSubscriptions
                .Include(x => x.YoutubeChannel)
                .Where(x => x.ChannelId == channelId)
                .ToListAsync(cancellationToken);

        public Task<List<UserYoutubeChannelSubscription>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => _db.UserYoutubeChannelSubscriptions
                .Include(x => x.YoutubeChannel)
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

        public async Task UpdateAsync(UserYoutubeChannelSubscription subscription, CancellationToken cancellationToken = default)
        {
            _db.UserYoutubeChannelSubscriptions.Update(subscription);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
