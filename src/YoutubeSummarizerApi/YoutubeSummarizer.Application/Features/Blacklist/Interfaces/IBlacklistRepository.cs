using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.Blacklist.Interfaces
{
    public interface IBlacklistRepository
    {
        Task<List<BlacklistedKeyword>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string userId, string keyword, CancellationToken cancellationToken = default);
        Task<int> CountByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task AddAsync(BlacklistedKeyword keyword, CancellationToken cancellationToken = default);
        Task<BlacklistedKeyword?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(BlacklistedKeyword keyword, CancellationToken cancellationToken = default);
    }
}