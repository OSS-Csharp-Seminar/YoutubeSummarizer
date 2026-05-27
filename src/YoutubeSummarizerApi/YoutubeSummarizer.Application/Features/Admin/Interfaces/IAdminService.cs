using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Admin.Dtos;

namespace YoutubeSummarizer.Application.Features.Admin.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResponse<List<AdminUserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> SendGlobalNotificationAsync(string title, string content, string senderName, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> ToggleBanAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> LogOutUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> CancelSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> MockWebhookAsync(string videoUrl, CancellationToken cancellationToken = default);
    }
}
