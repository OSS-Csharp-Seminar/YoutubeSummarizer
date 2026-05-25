using System.Net.Http.Json;

namespace YoutubeSummarizerWeb.Client.Services;

public class BlacklistService
{
    private readonly HttpClient _httpClient;

    public BlacklistService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BlacklistedKeyword>> GetKeywordsAsync()
    {
        var response = await _httpClient.GetAsync("/api/blacklist");
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<BlacklistedKeyword>>>();
        return result?.Data ?? new List<BlacklistedKeyword>();
    }

    public async Task<AddKeywordResult> AddKeywordAsync(string keyword)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/blacklist", new { Keyword = keyword });
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<BlacklistedKeyword>>();
        if (result == null || !result.Status)
            return new AddKeywordResult { Success = false, Error = result?.Message ?? "Failed to add keyword." };
        return new AddKeywordResult { Success = true, Data = result.Data };
    }

    public async Task<RemoveKeywordResult> RemoveKeywordAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/api/blacklist/{id}");
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        if (result == null || !result.Status)
            return new RemoveKeywordResult { Success = false, Error = result?.Message ?? "Failed to remove keyword." };
        return new RemoveKeywordResult { Success = true };
    }
}

public class BlacklistedKeyword
{
    public Guid Id { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}

public class AddKeywordResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public BlacklistedKeyword? Data { get; set; }
}

public class RemoveKeywordResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}