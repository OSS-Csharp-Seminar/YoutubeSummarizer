using System.Security.Claims;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IJwtService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(string ipAddress);
        ClaimsPrincipal? ValidateTokenWithoutLifetime(string token);
    }
}
