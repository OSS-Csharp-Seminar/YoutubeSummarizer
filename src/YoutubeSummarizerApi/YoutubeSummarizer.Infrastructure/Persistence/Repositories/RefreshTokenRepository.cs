using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _db;

        public RefreshTokenRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            _db.RefreshTokens.Add(token);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
        }

        public async Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            _db.RefreshTokens.Update(token);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _db.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAtUtc == null && t.ExpiresAtUtc > DateTime.UtcNow)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAtUtc, DateTime.UtcNow), cancellationToken);
        }
    }
}
