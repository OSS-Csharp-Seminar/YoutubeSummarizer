using YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos;

namespace YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces
{
    public interface IYoutubeTranscriptClient
    {
        Task<GetYoutubeTranscriptResponse> GetTranscriptAsync(GetYoutubeTranscriptRequest request, CancellationToken cancellationToken = default);
    }
}
