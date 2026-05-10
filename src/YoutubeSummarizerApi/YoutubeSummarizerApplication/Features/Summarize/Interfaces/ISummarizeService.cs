using YoutubeSummarizer.Application.Features.Summarize.Dtos;

namespace YoutubeSummarizer.Application.Features.Summarize.Interfaces
{
    public interface ISummarizeService
    {
        Task<SummarizeResponse> SummarizeAsync(SummarizeRequest request, CancellationToken cancellationToken = default);
    }
}
