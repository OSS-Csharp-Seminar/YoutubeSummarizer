using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace YoutubeSummarizerWeb.Services;

public class TokenStorageService : ITokenStorageService
{
    private readonly ProtectedLocalStorage _storage;

    private const string AccessTokenKey = "ys_at";
    private const string RefreshTokenKey = "ys_rt";
    private const string UserKey = "ys_user";

    public TokenStorageService(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    public async Task SaveTokensAsync(string accessToken, string refreshToken, AuthResponse user)
    {
        await _storage.SetAsync(AccessTokenKey, accessToken);
        await _storage.SetAsync(RefreshTokenKey, refreshToken);
        await _storage.SetAsync(UserKey, new StoredUser(user.UserId, user.Email, user.FirstName, user.LastName));
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            var result = await _storage.GetAsync<string>(AccessTokenKey);
            return result.Success ? result.Value : null;
        }
        catch { return null; }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            var result = await _storage.GetAsync<string>(RefreshTokenKey);
            return result.Success ? result.Value : null;
        }
        catch { return null; }
    }

    public async Task<AuthResponse?> GetUserAsync()
    {
        try
        {
            var result = await _storage.GetAsync<StoredUser>(UserKey);
            if (!result.Success || result.Value is null) return null;

            var at = await GetAccessTokenAsync();
            var rt = await GetRefreshTokenAsync();

            return new AuthResponse
            {
                UserId = result.Value.UserId,
                Email = result.Value.Email,
                FirstName = result.Value.FirstName,
                LastName = result.Value.LastName,
                AccessToken = at ?? string.Empty,
                RefreshToken = rt ?? string.Empty
            };
        }
        catch { return null; }
    }

    public async Task UpdateTokensAsync(string accessToken, string refreshToken)
    {
        await _storage.SetAsync(AccessTokenKey, accessToken);
        await _storage.SetAsync(RefreshTokenKey, refreshToken);
    }

    public async Task ClearAsync()
    {
        await _storage.DeleteAsync(AccessTokenKey);
        await _storage.DeleteAsync(RefreshTokenKey);
        await _storage.DeleteAsync(UserKey);
    }

    private record StoredUser(Guid UserId, string Email, string FirstName, string LastName);
}
