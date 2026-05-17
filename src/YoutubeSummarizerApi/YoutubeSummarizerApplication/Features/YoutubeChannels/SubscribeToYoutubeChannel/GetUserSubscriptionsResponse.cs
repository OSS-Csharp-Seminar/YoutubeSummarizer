using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel
{
    public class GetUserSubscriptionsResponse
    {
        public Guid SubscriptionId { get; set; }
        public Guid YoutubeChannelId { get; set; }
        public string ChannelIdentifier { get; set; } = string.Empty;
        public string ChannelUrl { get; set; } = string.Empty;
        public TranscriptSummarizationStyle SummarizationStyle { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}