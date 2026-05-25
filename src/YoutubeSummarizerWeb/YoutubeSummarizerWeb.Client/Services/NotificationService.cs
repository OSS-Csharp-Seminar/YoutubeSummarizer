using System.Net.Http.Json;

namespace YoutubeSummarizerWeb.Client.Services;

public class NotificationService
{
    private readonly HttpClient _httpClient;

    public event Action? OnUnreadCountChanged;

    public NotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<NotificationsResult> GetNotificationsAsync(
        NotificationType? type, string? senderSearch, bool sortDescending, int page, int pageSize)
    {
        var url = $"/api/notifications?sortDescending={sortDescending}&page={page}&pageSize={pageSize}";
        if (type.HasValue)
            url += $"&type={type.Value}";
        if (!string.IsNullOrWhiteSpace(senderSearch))
            url += $"&senderSearch={Uri.EscapeDataString(senderSearch)}";

        var response = await _httpClient.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetNotificationsResponse>>();
        if (result == null || !result.Status || result.Data == null)
            return new NotificationsResult { Success = false, Error = result?.Message ?? "Failed to load notifications." };
        return new NotificationsResult { Success = true, Data = result.Data };
    }

    public async Task<int> GetUnreadCountAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/notifications/unread-count");
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<int>>();
            return result?.Data ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task<bool> MarkAsReadAsync(Guid id)
    {
        var response = await _httpClient.PutAsync($"/api/notifications/{id}/read", null);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        if (result?.Status == true)
            OnUnreadCountChanged?.Invoke();
        return result?.Status == true;
    }

    public async Task<bool> MarkAllAsReadAsync()
    {
        var response = await _httpClient.PutAsync("/api/notifications/read-all", null);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        if (result?.Status == true)
            OnUnreadCountChanged?.Invoke();
        return result?.Status == true;
    }

    public async Task<bool> DismissAsync(Guid id)
    {
        var response = await _httpClient.PutAsync($"/api/notifications/{id}/dismiss", null);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        if (result?.Status == true)
            OnUnreadCountChanged?.Invoke();
        return result?.Status == true;
    }

    public async Task<GlobalNotificationResult> CreateGlobalNotificationAsync(string title, string content)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/notifications/global");
        request.Content = JsonContent.Create(new { Title = title, Content = content });

        var response = await _httpClient.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        if (result == null || !result.Status)
            return new GlobalNotificationResult { Success = false, Error = result?.Message ?? "Failed to send notification." };
        OnUnreadCountChanged?.Invoke();
        return new GlobalNotificationResult { Success = true };
    }

}
