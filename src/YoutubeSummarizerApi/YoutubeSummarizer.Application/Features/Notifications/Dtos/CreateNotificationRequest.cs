using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.Notifications.Dtos
{
    public class CreateNotificationRequest
    {
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
    }
}
