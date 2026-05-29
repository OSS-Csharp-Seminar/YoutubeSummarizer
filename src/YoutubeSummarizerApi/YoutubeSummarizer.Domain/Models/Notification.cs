using System;
using System.Collections.Generic;
using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Domain.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }

        public List<UserNotification> UserNotifications { get; set; } = new();
    }
}
