using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.UserAccount.Dtos;

namespace YoutubeSummarizer.Application.Features.UserAccount.Interfaces
{
    public interface IUserAccountService
    {
        Task<ServiceResponse<List<BlacklistEntryDto>>> GetBlacklistEntriesAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<BlacklistEntryDto>> AddBlacklistEntryAsync(AddBlacklistEntryRequest request, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> RemoveBlacklistEntryAsync(string keyword, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> DeleteAccountAsync(DeleteAccountRequest request, CancellationToken cancellationToken = default);
    }
}
