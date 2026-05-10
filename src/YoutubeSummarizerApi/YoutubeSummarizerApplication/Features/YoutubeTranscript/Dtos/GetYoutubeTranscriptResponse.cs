using System.Text.Json.Serialization;

namespace YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos
{
    public class GetYoutubeTranscriptResponse
    {
        [JsonPropertyName("video_id")]
        public string VideoId { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public List<TranscriptSegmentDto> Transcript { get; set; } = [];

        public VideoMetadataDto? Metadata { get; set; }
    }
}
