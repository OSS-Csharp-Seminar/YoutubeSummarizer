using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Enums;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepo;
        private readonly INotificationHubService _hubService;

        public NotificationService(
            INotificationRepository notificationRepo,
            ICurrentUserService currentUserService,
            IUserRepository userRepo,
            INotificationHubService hubService)
        {
            _notificationRepo = notificationRepo;
            _currentUserService = currentUserService;
            _userRepo = userRepo;
            _hubService = hubService;
        }

        public async Task<ServiceResponse<PaginatedResult<NotificationDto>>> GetNotificationsAsync(
            NotificationType? type, string? senderSearch, bool sortDescending,
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var (items, totalCount) = await _notificationRepo.GetPaginatedAsync(
                    userId, type, senderSearch, sortDescending, page, pageSize, cancellationToken);

                var mapped = new PaginatedResult<NotificationDto>(
                    items.Select(un => new NotificationDto
                    {
                        Id = un.Id,
                        Type = un.Notification.Type,
                        Title = un.Notification.Title,
                        Content = un.Notification.Content,
                        SenderName = un.Notification.SenderName,
                        IsRead = un.IsRead,
                        CreatedAtUtc = un.Notification.CreatedAtUtc
                    }).ToList(),
                    totalCount,
                    page,
                    pageSize
                );

                return ServiceResponse<PaginatedResult<NotificationDto>>.Success(mapped, "Notifications retrieved.");
            }
            catch
            {
                return ServiceResponse<PaginatedResult<NotificationDto>>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<int>> GetUnreadCountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var count = await _notificationRepo.GetUnreadCountAsync(userId, cancellationToken);
                return ServiceResponse<int>.Success(count, "Unread count retrieved.");
            }
            catch
            {
                return ServiceResponse<int>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<bool>> MarkAsReadAsync(Guid userNotificationId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var userNotification = await _notificationRepo.GetUserNotificationByIdAsync(userNotificationId, cancellationToken);
                if (userNotification is null || userNotification.UserId != userId)
                    return ServiceResponse<bool>.Failure("Notification not found.");

                userNotification.IsRead = true;
                await _notificationRepo.UpdateUserNotificationAsync(userNotification, cancellationToken);
                return ServiceResponse<bool>.Success(true, "Notification marked as read.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<bool>> MarkAllAsReadAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                await _notificationRepo.MarkAllAsReadAsync(userId, cancellationToken);
                return ServiceResponse<bool>.Success(true, "All notifications marked as read.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<bool>> DismissAsync(Guid userNotificationId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var userNotification = await _notificationRepo.GetUserNotificationByIdAsync(userNotificationId, cancellationToken);
                if (userNotification is null || userNotification.UserId != userId)
                    return ServiceResponse<bool>.Failure("Notification not found.");

                userNotification.IsDismissed = true;
                await _notificationRepo.UpdateUserNotificationAsync(userNotification, cancellationToken);
                return ServiceResponse<bool>.Success(true, "Notification dismissed.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<bool>> CreateGlobalNotificationAsync(
            CreateGlobalNotificationRequest request, string senderName, CancellationToken cancellationToken = default)
        {
            try
            {
                var userIds = await _userRepo.GetAllActiveUserIdsAsync(cancellationToken);

                var notification = new Notification
                {
                    Type = NotificationType.Global,
                    Title = request.Title,
                    Content = request.Content,
                    SenderName = senderName,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _notificationRepo.AddNotificationAsync(notification, userIds, cancellationToken);
                await _hubService.NotifyUsersAsync(userIds, cancellationToken);
                return ServiceResponse<bool>.Success(true, "Global notification sent.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<bool>> CreateNotificationAsync(
            CreateNotificationRequest request, List<Guid> userIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var notification = new Notification
                {
                    Type = request.Type,
                    Title = request.Title,
                    Content = request.Content,
                    SenderName = request.SenderName,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _notificationRepo.AddNotificationAsync(notification, userIds, cancellationToken);
                await _hubService.NotifyUsersAsync(userIds, cancellationToken);
                return ServiceResponse<bool>.Success(true, "Notification created.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("An error occurred.");
            }
        }
    }
}
