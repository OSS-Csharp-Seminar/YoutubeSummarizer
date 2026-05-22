using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositorys
{
    public class YoutubeChannelRepository : IYoutubeChannelRepository
    {
        private readonly ApplicationDbContext _db;

        public YoutubeChannelRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<YoutubeChannel?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
            => _db.YoutubeChannels.FirstOrDefaultAsync(x => x.ChannelIdentifier == identifier, cancellationToken);

        public Task<YoutubeChannel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _db.YoutubeChannels.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public Task<List<YoutubeChannel>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
            => _db.YoutubeChannels.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        public Task<YoutubeChannel?> GetByYoutubeChannelIdAsync(string youtubeChannelId, CancellationToken cancellationToken = default)
            => _db.YoutubeChannels.FirstOrDefaultAsync(x => x.YoutubeChannelId == youtubeChannelId, cancellationToken);

        public Task<List<YoutubeChannel>> GetExpiringWebhookSubscriptionsAsync(DateTime thresholdUtc, CancellationToken cancellationToken = default)
            => _db.YoutubeChannels
                .Where(x => x.IsWebhookSubscribed && x.WebhookExpiresAtUtc <= thresholdUtc)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(YoutubeChannel channel, CancellationToken cancellationToken = default)
        {
            _db.YoutubeChannels.Add(channel);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(YoutubeChannel channel, CancellationToken cancellationToken = default)
        {
            _db.YoutubeChannels.Update(channel);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
