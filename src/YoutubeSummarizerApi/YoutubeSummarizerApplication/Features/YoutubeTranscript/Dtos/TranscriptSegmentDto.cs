namespace YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos
{
    public class TranscriptSegmentDto
    {
        public string Text { get; set; } = string.Empty;
        public decimal Start { get; set; }
        public decimal Duration { get; set; }
    }
}
