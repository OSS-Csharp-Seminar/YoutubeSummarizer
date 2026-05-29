using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Features.UserAccount.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositories
{
    public class BlacklistEntryRepository : IBlacklistEntryRepository
    {
        private readonly ApplicationDbContext _db;

        public BlacklistEntryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<List<BlacklistEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => _db.BlacklistEntries
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Keyword)
                .ToListAsync(cancellationToken);

        public Task<List<BlacklistEntry>> GetByUserIdsAsync(List<Guid> userIds, CancellationToken cancellationToken = default)
            => _db.BlacklistEntries
                .Where(x => userIds.Contains(x.UserId))
                .ToListAsync(cancellationToken);

        public Task<bool> ExistsAsync(Guid userId, string keyword, CancellationToken cancellationToken = default)
            => _db.BlacklistEntries.AnyAsync(
                x => x.UserId == userId && x.Keyword == keyword,
                cancellationToken);

        public Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => _db.BlacklistEntries.CountAsync(x => x.UserId == userId, cancellationToken);

        public async Task AddAsync(BlacklistEntry entry, CancellationToken cancellationToken = default)
        {
            _db.BlacklistEntries.Add(entry);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<BlacklistEntry?> GetByKeyAsync(Guid userId, string keyword, CancellationToken cancellationToken = default)
            => _db.BlacklistEntries.FirstOrDefaultAsync(
                x => x.UserId == userId && x.Keyword == keyword,
                cancellationToken);

        public async Task DeleteAsync(BlacklistEntry entry, CancellationToken cancellationToken = default)
        {
            _db.BlacklistEntries.Remove(entry);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _db.BlacklistEntries
                .Where(x => x.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
