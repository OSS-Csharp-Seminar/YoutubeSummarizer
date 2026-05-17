using System.Net.Http.Json;
using System.Text.Json;

namespace YoutubeSummarizerWeb.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;
    private readonly ITokenStorageService _tokenStorage;

    public AuthService(HttpClient httpClient, AuthStateService authState, ITokenStorageService tokenStorage)
    {
        _httpClient = httpClient;
        _authState = authState;
        _tokenStorage = tokenStorage;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<AuthResponse>>(JsonOptions);
        if (result == null || !result.Status || result.Data == null)
            return new AuthResult { Success = false, Error = result?.Message ?? "Unexpected response from server." };

        await _tokenStorage.SaveTokensAsync(result.Data.AccessToken, result.Data.RefreshToken, result.Data);
        _authState.SetUser(result.Data);

        return new AuthResult { Success = true, User = result.Data };
    }

    public async Task<AuthResult> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        });

        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<AuthResponse>>(JsonOptions);
        if (result == null || !result.Status || result.Data == null)
            return new AuthResult { Success = false, Error = result?.Message ?? "Unexpected response from server." };

        await _tokenStorage.SaveTokensAsync(result.Data.AccessToken, result.Data.RefreshToken, result.Data);

        return new AuthResult { Success = true, User = result.Data };
    }

    public async Task LogoutAsync()
    {
        var refreshToken = await _tokenStorage.GetRefreshTokenAsync();

        _authState.ClearUser();
        await _tokenStorage.ClearAsync();

        if (refreshToken != null)
        {
            try { await _httpClient.PostAsJsonAsync("/api/auth/logout", new { RefreshToken = refreshToken }); }
            catch { }
        }
    }

    public async Task RestoreSessionAsync()
    {
        if (_authState.IsLoggedIn)
            return;

        var user = await _tokenStorage.GetUserAsync();
        if (user != null)
            _authState.SetUser(user);
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
}
