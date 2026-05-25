using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Features.Blacklist.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositories
{
    public class BlacklistRepository : IBlacklistRepository
    {
        private readonly ApplicationDbContext _db;

        public BlacklistRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<List<BlacklistedKeyword>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
            => _db.BlacklistedKeywords
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Keyword)
                .ToListAsync(cancellationToken);

        public Task<bool> ExistsAsync(string userId, string keyword, CancellationToken cancellationToken = default)
            => _db.BlacklistedKeywords.AnyAsync(
                x => x.UserId == userId && x.Keyword == keyword,
                cancellationToken);

        public Task<int> CountByUserIdAsync(string userId, CancellationToken cancellationToken = default)
            => _db.BlacklistedKeywords.CountAsync(x => x.UserId == userId, cancellationToken);

        public async Task AddAsync(BlacklistedKeyword keyword, CancellationToken cancellationToken = default)
        {
            _db.BlacklistedKeywords.Add(keyword);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<BlacklistedKeyword?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _db.BlacklistedKeywords.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task DeleteAsync(BlacklistedKeyword keyword, CancellationToken cancellationToken = default)
        {
            _db.BlacklistedKeywords.Remove(keyword);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}