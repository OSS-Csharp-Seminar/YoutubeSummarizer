using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace YoutubeSummarizerWeb.Client.Services;

public class AdminService
{
    private readonly HttpClient _httpClient;

    public AdminService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AdminUsersResult> GetAllUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/admin/users");
            if (!response.IsSuccessStatusCode)
                return new AdminUsersResult { Error = "Failed to load users." };

            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<AdminUserModel>>>();
            if (result is null || !result.Status)
                return new AdminUsersResult { Error = result?.Message ?? "Failed to load users." };

            return new AdminUsersResult { Users = result.Data ?? new() };
        }
        catch
        {
            return new AdminUsersResult { Error = "Failed to load users." };
        }
    }

    public async Task<AdminActionResult> CreateGlobalNotificationAsync(string title, string content)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/notifications/global");
            request.Content = JsonContent.Create(new { Title = title, Content = content });

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (result == null || !result.Status)
                return new AdminActionResult { Error = result?.Message ?? "Failed to send notification." };

            return new AdminActionResult();
        }
        catch
        {
            return new AdminActionResult { Error = "Failed to send notification." };
        }
    }

    public async Task<AdminActionResult> ToggleBanAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/admin/users/{userId}/toggle-ban", null);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (result == null || !result.Status)
                return new AdminActionResult { Error = result?.Message ?? "Failed to update ban status." };

            return new AdminActionResult { Message = result.Message };
        }
        catch
        {
            return new AdminActionResult { Error = "Failed to update ban status." };
        }
    }

    public async Task<AdminActionResult> LogOutUserAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/admin/users/{userId}/logout", null);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (result == null || !result.Status)
                return new AdminActionResult { Error = result?.Message ?? "Failed to log out user." };

            return new AdminActionResult { Message = result.Message };
        }
        catch
        {
            return new AdminActionResult { Error = "Failed to log out user." };
        }
    }

    public async Task<AdminActionResult> CancelSubscriptionAsync(Guid subscriptionId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/admin/subscriptions/{subscriptionId}");
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (result == null || !result.Status)
                return new AdminActionResult { Error = result?.Message ?? "Failed to cancel subscription." };

            return new AdminActionResult { Message = result.Message };
        }
        catch
        {
            return new AdminActionResult { Error = "Failed to cancel subscription." };
        }
    }

    public async Task<AdminActionResult> MockWebhookAsync(string videoUrl)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/admin/mock-webhook", new { VideoUrl = videoUrl });
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (result == null || !result.Status)
                return new AdminActionResult { Error = result?.Message ?? "Failed to send mock webhook." };

            return new AdminActionResult { Message = result.Message };
        }
        catch
        {
            return new AdminActionResult { Error = "Failed to send mock webhook." };
        }
    }
}

public class AdminUsersResult
{
    public List<AdminUserModel> Users { get; set; } = new();
    public string? Error { get; set; }
    public bool Success => Error is null;
}

public class AdminActionResult
{
    public string? Error { get; set; }
    public string? Message { get; set; }
    public bool Success => Error is null;
}

public class AdminUserModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<AdminUserSubscriptionModel> Subscriptions { get; set; } = new();
}

public class AdminUserSubscriptionModel
{
    public Guid SubscriptionId { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string SummarizationStyle { get; set; } = string.Empty;
}
