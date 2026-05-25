using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task CreateAsync(User user);
        Task<List<Guid>> GetAllActiveUserIdsAsync(CancellationToken cancellationToken = default);
        Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
