using System.Text.Json.Serialization;

namespace YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos
{
    public class VideoMetadataDto
    {
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; } = string.Empty;

        [JsonPropertyName("author_url")]
        public string AuthorUrl { get; set; } = string.Empty;

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}
