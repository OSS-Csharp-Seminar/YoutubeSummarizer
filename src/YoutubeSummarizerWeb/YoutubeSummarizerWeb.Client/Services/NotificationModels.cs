using System;
using System.Collections.Generic;

namespace YoutubeSummarizerWeb.Client.Services;

public enum NotificationType
{
    NewVideo,
    Global
}

public class NotificationItem
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class GetNotificationsResponse
{
    public List<NotificationItem> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class NotificationsResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public GetNotificationsResponse? Data { get; set; }
}

