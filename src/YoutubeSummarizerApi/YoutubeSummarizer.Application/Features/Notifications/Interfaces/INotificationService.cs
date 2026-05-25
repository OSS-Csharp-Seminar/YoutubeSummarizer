using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;
using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResponse<PaginatedResult<NotificationDto>>> GetNotificationsAsync(
            NotificationType? type, string? senderSearch, bool sortDescending,
            int page, int pageSize, CancellationToken cancellationToken = default);
        Task<ServiceResponse<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> MarkAsReadAsync(Guid userNotificationId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> DismissAsync(Guid userNotificationId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> CreateGlobalNotificationAsync(
            CreateGlobalNotificationRequest request, string senderName, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> CreateNotificationAsync(
            CreateNotificationRequest request, List<Guid> userIds, CancellationToken cancellationToken = default);
    }
}
