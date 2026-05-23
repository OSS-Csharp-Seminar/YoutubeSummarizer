namespace YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos
{
    public class VideoMetadataDto
    {
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}
