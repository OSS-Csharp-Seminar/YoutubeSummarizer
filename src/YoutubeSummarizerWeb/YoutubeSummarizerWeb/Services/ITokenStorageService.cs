namespace YoutubeSummarizerWeb.Services;

public interface ITokenStorageService
{
    Task SaveTokensAsync(string accessToken, string refreshToken, AuthResponse user);
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task<AuthResponse?> GetUserAsync();
    Task UpdateTokensAsync(string accessToken, string refreshToken);
    Task ClearAsync();
}
