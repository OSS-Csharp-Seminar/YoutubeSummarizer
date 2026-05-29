using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;

namespace YoutubeSummarizer.Application.Features.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResponse<PaginatedResult<NotificationDto>>> GetNotificationsAsync(
            GetNotificationsQuery query, CancellationToken cancellationToken = default);
        Task<ServiceResponse<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> MarkAsReadAsync(Guid userNotificationId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> DismissAsync(Guid userNotificationId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> DismissAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> CreateNotificationAsync(
            CreateNotificationRequest request, List<Guid> userIds, CancellationToken cancellationToken = default);
    }
}
