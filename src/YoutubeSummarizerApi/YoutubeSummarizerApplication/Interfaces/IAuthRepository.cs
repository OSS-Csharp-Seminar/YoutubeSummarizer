using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<User> CreateUserAsync(string firstName, string lastName, string email, string password);
    }
}
