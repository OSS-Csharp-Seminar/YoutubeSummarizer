using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;
using YoutubeSummarizer.Domain.Enums;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _db;

        public NotificationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<(List<UserNotification> Items, int TotalCount)> GetPaginatedAsync(
            Guid userId, NotificationType? type, string? senderSearch,
            bool sortDescending, int page, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _db.UserNotifications
                .Include(x => x.Notification)
                .Where(x => x.UserId == userId && !x.IsDismissed);

            if (type.HasValue)
                query = query.Where(x => x.Notification.Type == type.Value);

            if (!string.IsNullOrWhiteSpace(senderSearch))
                query = query.Where(x => x.Notification.SenderName.Contains(senderSearch));

            var totalCount = await query.CountAsync(cancellationToken);

            query = sortDescending
                ? query.OrderByDescending(x => x.Notification.CreatedAtUtc)
                : query.OrderBy(x => x.Notification.CreatedAtUtc);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public Task<UserNotification?> GetUserNotificationByIdAsync(Guid userNotificationId, CancellationToken cancellationToken = default)
            => _db.UserNotifications
                .Include(x => x.Notification)
                .FirstOrDefaultAsync(x => x.Id == userNotificationId, cancellationToken);

        public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
            => _db.UserNotifications.CountAsync(x => x.UserId == userId && !x.IsRead && !x.IsDismissed, cancellationToken);

        public async Task AddNotificationAsync(Notification notification, List<Guid> userIds, CancellationToken cancellationToken = default)
        {
            notification.UserNotifications = userIds.Select(uid => new UserNotification
            {
                UserId = uid
            }).ToList();

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateUserNotificationAsync(UserNotification userNotification, CancellationToken cancellationToken = default)
        {
            _db.UserNotifications.Update(userNotification);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _db.UserNotifications
                .Where(x => x.UserId == userId && !x.IsRead && !x.IsDismissed)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), cancellationToken);
        }
    }
}
