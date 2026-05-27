using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.UserAccount.Interfaces
{
    public interface IBlacklistEntryRepository
    {
        Task<List<BlacklistEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<BlacklistEntry>> GetByUserIdsAsync(List<Guid> userIds, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid userId, string keyword, CancellationToken cancellationToken = default);
        Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(BlacklistEntry entry, CancellationToken cancellationToken = default);
        Task<BlacklistEntry?> GetByKeyAsync(Guid userId, string keyword, CancellationToken cancellationToken = default);
        Task DeleteAsync(BlacklistEntry entry, CancellationToken cancellationToken = default);
        Task DeleteAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
