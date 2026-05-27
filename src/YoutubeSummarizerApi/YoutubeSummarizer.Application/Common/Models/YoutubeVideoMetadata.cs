namespace YoutubeSummarizer.Application.Common.Models
{
    public class YoutubeVideoMetadata
    {
        public string VideoId { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
    }
}
