using System.Net.Http.Json;

namespace YoutubeSummarizerWeb.Services;

public class AuthRefreshService : IAuthRefreshService
{
    private readonly ITokenStorageService _tokenStorage;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthRefreshService(ITokenStorageService tokenStorage, IHttpClientFactory httpClientFactory)
    {
        _tokenStorage = tokenStorage;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> TryRefreshTokenAsync()
    {
        var accessToken = await _tokenStorage.GetAccessTokenAsync();
        var refreshToken = await _tokenStorage.GetRefreshTokenAsync();

        if (accessToken == null || refreshToken == null)
            return false;

        try
        {
            var client = _httpClientFactory.CreateClient("RefreshClient");
            var response = await client.PostAsJsonAsync("/api/auth/refresh-token", new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });

            if (!response.IsSuccessStatusCode)
            {
                await _tokenStorage.ClearAsync();
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<RefreshResponse>>();
            if (result == null || !result.Status || result.Data == null)
            {
                await _tokenStorage.ClearAsync();
                return false;
            }

            await _tokenStorage.UpdateTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
            return true;
        }
        catch
        {
            await _tokenStorage.ClearAsync();
            return false;
        }
    }

    private record RefreshResponse(string AccessToken, string RefreshToken);
}
