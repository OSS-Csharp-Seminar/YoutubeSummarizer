using System;
using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Domain.Models
{
    public class UserYoutubeChannelSubscription
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ChannelId { get; set; }
        public TranscriptSummarizationStyle SummarizationStyle { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public YoutubeChannel YoutubeChannel { get; set; } = null!;
    }
}
