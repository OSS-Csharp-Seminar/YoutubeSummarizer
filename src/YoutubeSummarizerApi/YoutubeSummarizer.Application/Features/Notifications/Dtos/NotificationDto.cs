using System;
using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.Notifications.Dtos
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
