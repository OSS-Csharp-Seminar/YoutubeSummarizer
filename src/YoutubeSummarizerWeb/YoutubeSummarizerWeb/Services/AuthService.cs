using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace YoutubeSummarizerWeb.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;
    private readonly IJSRuntime _js;

    public AuthService(HttpClient httpClient, AuthStateService authState, IJSRuntime js)
    {
        _httpClient = httpClient;
        _authState = authState;
        _js = js;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return new AuthResult { Success = false, Error = error };
        }

        var data = await response.Content.ReadFromJsonAsync<AuthResponse>();
        _authState.SetUser(data!);
        await PersistSession(data!);
        return new AuthResult { Success = true, User = data };
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

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await ParseErrorAsync(response);
            return new AuthResult { Success = false, Error = errorMessage };
        }

        var data = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return new AuthResult { Success = true, User = data };
    }

    public async Task LogoutAsync()
    {
        _authState.ClearUser();
        await _js.InvokeVoidAsync("authCookie.delete");
    }

    public async Task RestoreSessionAsync()
    {
        if (_authState.IsLoggedIn)
            return;

        var json = await _js.InvokeAsync<string?>("authCookie.get");
        if (string.IsNullOrEmpty(json))
            return;

        var data = JsonSerializer.Deserialize<AuthResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (data != null)
            _authState.SetUser(data);
    }

    private static async Task<string> ParseErrorAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(body);

            if (doc.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
            {
                var firstMessage = errors.EnumerateArray()
                    .Select(e => e.TryGetProperty("message", out var m) ? m.GetString() : null)
                    .FirstOrDefault(m => m != null);
                if (firstMessage != null) return firstMessage;
            }

            if (doc.RootElement.TryGetProperty("message", out var msg))
                return msg.GetString() ?? "An error occurred.";
        }
        catch (JsonException) { }

        return "An error occurred.";
    }

    private async Task PersistSession(AuthResponse user)
    {
        var json = JsonSerializer.Serialize(new
        {
            user.UserId,
            user.Email,
            user.FullName,
            user.AccessToken
        });
        await _js.InvokeVoidAsync("authCookie.set", json, 1);
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public AuthResponse? User { get; set; }
}

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}
