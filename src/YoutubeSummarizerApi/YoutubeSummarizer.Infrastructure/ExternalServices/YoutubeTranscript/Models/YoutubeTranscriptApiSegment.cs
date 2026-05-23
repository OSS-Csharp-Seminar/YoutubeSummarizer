namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeTranscript.Models
{
    public class YoutubeTranscriptApiSegment
    {
        public string Text { get; set; } = string.Empty;
        public decimal Start { get; set; }
        public decimal Duration { get; set; }
    }
}
