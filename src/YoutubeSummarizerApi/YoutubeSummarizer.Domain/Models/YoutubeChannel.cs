namespace YoutubeSummarizer.Domain.Models
{
    public class YoutubeChannel
    {
        public Guid Id { get; set; }
        public string YoutubeChannelId { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelUrl { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsWebhookSubscribed { get; set; }
        public DateTime? WebhookExpiresAtUtc { get; set; }
        public DateTime? LastWebhookSubscriptionAttemptUtc { get; set; }

        public List<UserYoutubeChannelSubscription> Subscriptions { get; set; } = [];
    }
}
