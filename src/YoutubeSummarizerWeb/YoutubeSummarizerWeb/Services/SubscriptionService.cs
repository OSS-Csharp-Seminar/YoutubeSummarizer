using System.Net.Http.Json;

namespace YoutubeSummarizerWeb.Services;

public class SubscriptionService
{
    private readonly HttpClient _httpClient;

    public SubscriptionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SubscribeResult> SubscribeAsync(string channelUrl, SummarizationStyle style)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/youtube-channels/subscribe");
        request.Content = JsonContent.Create(new { ChannelUrl = channelUrl, SummarizationStyle = (int)style });

        var response = await _httpClient.SendAsync(request);

        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<SubscribeResponse>>();
        if (result == null || !result.Status)
            return new SubscribeResult { Success = false, Error = result?.Message ?? "Failed to subscribe. Check the URL and try again." };

        return new SubscribeResult { Success = true, Data = result.Data };
    }

    public async Task<List<UserSubscription>> GetSubscriptionsAsync()
    {
        var response = await _httpClient.GetAsync("/api/youtube-channels/subscriptions");
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<UserSubscription>>>();
        return result?.Data ?? new List<UserSubscription>();
    }

    public async Task<UnsubscribeResult> UnsubscribeAsync(Guid subscriptionId)
    {
        var response = await _httpClient.DeleteAsync($"/api/youtube-channels/subscriptions/{subscriptionId}");
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        if (result == null || !result.Status)
            return new UnsubscribeResult { Success = false, Error = result?.Message ?? "Failed to unsubscribe." };
        return new UnsubscribeResult { Success = true };
    }
}

public enum SummarizationStyle
{
    Brief,
    Detailed,
    Scientific,
    Layman
}

public class SubscribeResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public SubscribeResponse? Data { get; set; }
}

public class SubscribeResponse
{
    public Guid YoutubeChannelId { get; set; }
    public string ChannelIdentifier { get; set; } = string.Empty;
    public string ChannelUrl { get; set; } = string.Empty;
    public SummarizationStyle SummarizationStyle { get; set; }
}

public class UserSubscription
{
    public Guid SubscriptionId { get; set; }
    public Guid YoutubeChannelId { get; set; }
    public string ChannelIdentifier { get; set; } = string.Empty;
    public string ChannelUrl { get; set; } = string.Empty;
    public SummarizationStyle SummarizationStyle { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class UnsubscribeResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}