namespace YoutubeSummarizer.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<UserYoutubeChannelSubscription> Subscriptions { get; set; } = [];
        public List<BlacklistEntry> BlacklistEntries { get; set; } = [];
        public List<RefreshToken> RefreshTokens { get; set; } = [];
        public List<UserNotification> UserNotifications { get; set; } = [];
    }
}
