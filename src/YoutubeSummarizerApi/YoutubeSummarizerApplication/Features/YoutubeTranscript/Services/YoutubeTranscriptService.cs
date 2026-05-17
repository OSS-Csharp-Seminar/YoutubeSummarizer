using YoutubeSummarizer.Application.Common.Models;
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

        public async Task<ServiceResponse<GetYoutubeTranscriptResponse>> GetTranscriptAsync(GetYoutubeTranscriptRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _client.GetTranscriptAsync(request, cancellationToken);
                return ServiceResponse<GetYoutubeTranscriptResponse>.Success(result, "Transkript uspješno dohvaćen.");
            }
            catch
            {
                return ServiceResponse<GetYoutubeTranscriptResponse>.Failure("Nije moguće dohvatiti transkript.");
            }
        }
    }
}
