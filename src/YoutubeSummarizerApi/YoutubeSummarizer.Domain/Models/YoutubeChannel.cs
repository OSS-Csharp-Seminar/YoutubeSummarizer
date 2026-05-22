namespace YoutubeSummarizer.Domain.Models
{
    public class YoutubeChannel
    {
        public Guid Id { get; set; }
        public string ChannelIdentifier { get; set; } = string.Empty;
        public string ChannelUrl { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? YoutubeChannelId { get; set; }
        public bool IsWebhookSubscribed { get; set; }
        public DateTime? WebhookExpiresAtUtc { get; set; }
        public DateTime? LastWebhookSubscriptionAttemptUtc { get; set; }
    }
}
