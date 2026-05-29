using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;

namespace YoutubeSummarizerWeb.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;

    public AuthService(HttpClient httpClient, AuthStateService authState)
    {
        _httpClient = httpClient;
        _authState = authState;
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

        return new AuthResult { Success = true, User = result.Data };
    }

    public async Task LogoutAsync()
    {
        _authState.ClearUser();
        try { await _httpClient.PostAsync("/api/auth/logout", null); }
        catch { }
    }

    public async Task RestoreSessionAsync()
    {
        if (_authState.IsLoggedIn)
            return;

        try
        {
            var response = await _httpClient.GetAsync("/api/auth/me");
            if (!response.IsSuccessStatusCode)
                return;

            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<AuthResponse>>(JsonOptions);
            if (result?.Status == true && result.Data != null)
                _authState.SetUser(result.Data);
        }
        catch { }
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
}
