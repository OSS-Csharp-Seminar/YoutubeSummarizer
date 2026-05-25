using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Blacklist.Dtos;

namespace YoutubeSummarizer.Application.Features.Blacklist.Interfaces
{
    public interface IBlacklistService
    {
        Task<ServiceResponse<List<BlacklistedKeywordDto>>> GetKeywordsAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<BlacklistedKeywordDto>> AddKeywordAsync(AddKeywordRequest request, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> RemoveKeywordAsync(Guid id, CancellationToken cancellationToken = default);
    }
}