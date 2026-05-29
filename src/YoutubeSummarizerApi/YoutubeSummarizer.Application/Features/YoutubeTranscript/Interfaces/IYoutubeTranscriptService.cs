using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos;

namespace YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces
{
    public interface IYoutubeTranscriptService
    {
        Task<ServiceResponse<GetYoutubeTranscriptResponse>> GetTranscriptAsync(GetYoutubeTranscriptRequest request, CancellationToken cancellationToken = default);
    }
}
