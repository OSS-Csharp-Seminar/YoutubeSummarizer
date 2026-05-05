using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
