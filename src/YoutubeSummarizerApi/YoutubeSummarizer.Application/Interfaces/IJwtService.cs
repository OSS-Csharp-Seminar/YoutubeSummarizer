using System.Security.Claims;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IJwtService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(User user, IList<string> roles);
        RefreshToken GenerateRefreshToken(string ipAddress);
        ClaimsPrincipal? ValidateTokenWithoutLifetime(string token);
    }
}
