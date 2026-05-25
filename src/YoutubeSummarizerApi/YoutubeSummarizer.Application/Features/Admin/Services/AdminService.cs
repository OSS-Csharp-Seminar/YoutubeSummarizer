using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Admin.Dtos;
using YoutubeSummarizer.Application.Features.Admin.Interfaces;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.Admin.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserYoutubeChannelSubscriptionRepository _subRepo;
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly INotificationService _notificationService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IYoutubeWebhookSubscriptionService _webhookService;

        public AdminService(
            IUserRepository userRepo,
            IUserYoutubeChannelSubscriptionRepository subRepo,
            IYoutubeChannelRepository channelRepo,
            INotificationService notificationService,
            IRefreshTokenRepository refreshTokenRepo,
            IYoutubeWebhookSubscriptionService webhookService)
        {
            _userRepo = userRepo;
            _subRepo = subRepo;
            _channelRepo = channelRepo;
            _notificationService = notificationService;
            _refreshTokenRepo = refreshTokenRepo;
            _webhookService = webhookService;
        }

        public async Task<ServiceResponse<List<AdminUserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _userRepo.GetAllAsync(cancellationToken);
                var result = new List<AdminUserDto>();

                foreach (var user in users)
                {
                    var subs = await _subRepo.GetByUserIdAsync(user.Id, cancellationToken);
                    var channelIds = subs.Select(s => s.YoutubeChannelId).Distinct().ToList();
                    var channels = await _channelRepo.GetByIdsAsync(channelIds, cancellationToken);
                    var channelMap = channels.ToDictionary(c => c.Id);

                    var dto = new AdminUserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        Subscriptions = subs.Select(s =>
                        {
                            var channelDisplay = "Unknown";
                            if (channelMap.TryGetValue(s.YoutubeChannelId, out var ch))
                                channelDisplay = ch.ChannelIdentifier.StartsWith("@") ? ch.ChannelIdentifier : ch.ChannelUrl;

                            return new AdminUserSubscriptionDto
                            {
                                SubscriptionId = s.Id,
                                ChannelIdentifier = channelDisplay,
                                SummarizationStyle = s.SummarizationStyle.ToString()
                            };
                        }).ToList()
                    };

                    result.Add(dto);
                }

                return ServiceResponse<List<AdminUserDto>>.Success(result, "Users loaded successfully.");
            }
            catch
            {
                return ServiceResponse<List<AdminUserDto>>.Failure("Failed to load users.");
            }
        }

        public async Task<ServiceResponse<bool>> SendGlobalNotificationAsync(
            string title, string content, string senderName, CancellationToken cancellationToken = default)
        {
            try
            {
                var userIds = await _userRepo.GetAllActiveUserIdsAsync(cancellationToken);

                var request = new CreateNotificationRequest
                {
                    Type = NotificationType.Global,
                    Title = title,
                    Content = content,
                    SenderName = senderName
                };

                return await _notificationService.CreateNotificationAsync(request, userIds, cancellationToken);
            }
            catch
            {
                return ServiceResponse<bool>.Failure("Failed to send global notification.");
            }
        }

        public async Task<ServiceResponse<bool>> ToggleBanAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user is null)
                    return ServiceResponse<bool>.Failure("User not found.");

                user.IsActive = !user.IsActive;
                await _userRepo.UpdateAsync(user, cancellationToken);

                if (!user.IsActive)
                    await _refreshTokenRepo.RevokeAllByUserIdAsync(userId, cancellationToken);

                var status = user.IsActive ? "unbanned" : "banned";
                return ServiceResponse<bool>.Success(true, $"User {status} successfully.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("Failed to update user ban status.");
            }
        }

        public async Task<ServiceResponse<bool>> LogOutUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user is null)
                    return ServiceResponse<bool>.Failure("User not found.");

                await _refreshTokenRepo.RevokeAllByUserIdAsync(userId, cancellationToken);
                return ServiceResponse<bool>.Success(true, "User logged out successfully.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("Failed to log out user.");
            }
        }

        public async Task<ServiceResponse<bool>> CancelSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var subscription = await _subRepo.GetByIdAsync(subscriptionId, cancellationToken);
                if (subscription is null)
                    return ServiceResponse<bool>.Failure("Subscription not found.");

                var channelId = subscription.YoutubeChannelId;
                await _subRepo.DeleteAsync(subscription, cancellationToken);

                var remaining = await _subRepo.GetByYoutubeChannelIdAsync(channelId, cancellationToken);
                if (remaining.Count == 0)
                {
                    var channel = await _channelRepo.GetByIdAsync(channelId, cancellationToken);
                    if (channel is not null)
                    {
                        if (channel.IsWebhookSubscribed)
                            await _webhookService.UnsubscribeAsync(channelId, cancellationToken);
                        await _channelRepo.DeleteAsync(channel, cancellationToken);
                    }
                }

                return ServiceResponse<bool>.Success(true, "Subscription cancelled successfully.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("Failed to cancel subscription.");
            }
        }
    }
}
