using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Domain.Models
{
    public class UserYoutubeChannelSubscription
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid YoutubeChannelId { get; set; }
        public TranscriptSummarizationStyle SummarizationStyle { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
