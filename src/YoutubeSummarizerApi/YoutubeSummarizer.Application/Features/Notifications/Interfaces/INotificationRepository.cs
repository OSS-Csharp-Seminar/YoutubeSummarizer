using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Domain.Enums;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.Notifications.Interfaces
{
    public interface INotificationRepository
    {
        Task<(List<UserNotification> Items, int TotalCount)> GetPaginatedAsync(
            Guid userId, NotificationType? type, string? senderSearch,
            bool sortDescending, int page, int pageSize,
            CancellationToken cancellationToken = default);
        Task<UserNotification?> GetUserNotificationByIdAsync(Guid userNotificationId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddNotificationAsync(Notification notification, List<Guid> userIds, CancellationToken cancellationToken = default);
        Task UpdateUserNotificationAsync(UserNotification userNotification, CancellationToken cancellationToken = default);
        Task DeleteUserNotificationAsync(UserNotification userNotification, CancellationToken cancellationToken = default);
        Task<bool> HasRemainingRecipientsAsync(Guid notificationId, CancellationToken cancellationToken = default);
        Task DeleteNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default);
        Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<Guid>> GetNotificationIdsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAllUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task DeleteOrphanedNotificationsAsync(List<Guid> notificationIds, CancellationToken cancellationToken = default);
    }
}
