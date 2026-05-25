namespace YoutubeSummarizer.Application.Features.Notifications.Dtos
{
    public class CreateGlobalNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
