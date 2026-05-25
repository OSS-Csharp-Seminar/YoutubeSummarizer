using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Admin.Dtos;

namespace YoutubeSummarizer.Application.Features.Admin.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResponse<List<AdminUserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> SendGlobalNotificationAsync(string title, string content, string senderName, CancellationToken cancellationToken = default);
    }
}
