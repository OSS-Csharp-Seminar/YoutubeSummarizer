using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeTranscript.Models
{
    public class YoutubeTranscriptApiResponse
    {
        [JsonPropertyName("video_id")]
        public string VideoId { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public List<YoutubeTranscriptApiSegment> Transcript { get; set; } = [];

        public YoutubeTranscriptApiMetadata? Metadata { get; set; }
    }
}
