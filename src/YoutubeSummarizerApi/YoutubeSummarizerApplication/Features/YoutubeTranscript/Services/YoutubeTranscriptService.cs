using YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;

namespace YoutubeSummarizer.Application.Features.YoutubeTranscript.Services
{
    public class YoutubeTranscriptService : IYoutubeTranscriptService
    {
        private readonly IYoutubeTranscriptClient _client;

        public YoutubeTranscriptService(IYoutubeTranscriptClient client)
        {
            _client = client;
        }

        public async Task<GetYoutubeTranscriptResponse> GetTranscriptAsync(GetYoutubeTranscriptRequest request, CancellationToken cancellationToken = default)
        {
            return await _client.GetTranscriptAsync(request, cancellationToken);
        }
    }
}
