using System.Collections.Generic;

namespace YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos
{
    public class GetYoutubeTranscriptResponse
    {
        public string VideoId { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public List<TranscriptSegmentDto> Transcript { get; set; } = [];

        public VideoMetadataDto? Metadata { get; set; }
    }
}
