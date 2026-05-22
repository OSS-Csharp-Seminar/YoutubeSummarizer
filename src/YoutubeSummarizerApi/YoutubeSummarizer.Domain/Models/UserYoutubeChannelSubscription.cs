using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Domain.Models
{
    public class UserYoutubeChannelSubscription
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid YoutubeChannelId { get; set; }
        public TranscriptSummarizationStyle SummarizationStyle { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
