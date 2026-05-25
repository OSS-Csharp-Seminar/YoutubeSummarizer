namespace YoutubeSummarizer.Domain.Models
{
    public class UserNotification
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public bool IsRead { get; set; }
        public bool IsDismissed { get; set; }

        public Notification Notification { get; set; } = null!;
    }
}
