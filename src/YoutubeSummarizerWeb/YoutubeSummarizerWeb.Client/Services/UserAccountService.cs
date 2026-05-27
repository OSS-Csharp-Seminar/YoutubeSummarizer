using System.Net.Http.Json;

namespace YoutubeSummarizerWeb.Client.Services;

public class UserAccountService
{
    private readonly HttpClient _httpClient;

    public UserAccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BlacklistEntry>> GetBlacklistEntriesAsync()
    {
        var response = await _httpClient.GetAsync("/api/account/blacklist");
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<BlacklistEntry>>>();
        return result?.Data ?? new List<BlacklistEntry>();
    }

    public async Task<AddBlacklistEntryResult> AddBlacklistEntryAsync(string keyword)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/account/blacklist", new { Keyword = keyword });
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<BlacklistEntry>>();
        if (result == null || !result.Status)
            return new AddBlacklistEntryResult { Success = false, Error = result?.Message ?? "Failed to add keyword." };
        return new AddBlacklistEntryResult { Success = true, Keyword = result.Data?.Keyword };
    }

    public async Task<RemoveBlacklistEntryResult> RemoveBlacklistEntryAsync(string keyword)
    {
        var response = await _httpClient.DeleteAsync($"/api/account/blacklist?keyword={Uri.EscapeDataString(keyword)}");
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        if (result == null || !result.Status)
            return new RemoveBlacklistEntryResult { Success = false, Error = result?.Message ?? "Failed to remove keyword." };
        return new RemoveBlacklistEntryResult { Success = true };
    }

    public async Task<DeleteAccountResult> DeleteAccountAsync(string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/account")
        {
            Content = JsonContent.Create(new { Password = password })
        };
        var response = await _httpClient.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        if (result == null || !result.Status)
            return new DeleteAccountResult { Success = false, Error = result?.Message ?? "Failed to delete account." };
        return new DeleteAccountResult { Success = true };
    }
}

public class BlacklistEntry
{
    public string Keyword { get; set; } = string.Empty;
}

public class AddBlacklistEntryResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? Keyword { get; set; }
}

public class RemoveBlacklistEntryResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class DeleteAccountResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}
