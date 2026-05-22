using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel
{
    public class SubscribeToYoutubeChannelRequest
    {
        public string ChannelUrl { get; set; } = string.Empty;
        public TranscriptSummarizationStyle SummarizationStyle { get; set; }
    }
}
