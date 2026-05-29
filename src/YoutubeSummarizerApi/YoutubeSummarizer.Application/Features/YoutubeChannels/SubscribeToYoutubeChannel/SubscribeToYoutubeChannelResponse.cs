using System;
using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel
{
    public class SubscribeToYoutubeChannelResponse
    {
        public Guid YoutubeChannelId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelUrl { get; set; } = string.Empty;
        public TranscriptSummarizationStyle SummarizationStyle { get; set; }
    }
}
