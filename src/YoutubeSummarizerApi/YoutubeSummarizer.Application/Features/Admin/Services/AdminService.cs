using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Admin.Dtos;
using YoutubeSummarizer.Application.Features.Admin.Interfaces;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Dtos;
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
        private readonly IYoutubeMetadataClient _metadataClient;
        private readonly IMockWebhookSender _mockWebhookSender;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IUserRepository userRepo,
            IUserYoutubeChannelSubscriptionRepository subRepo,
            IYoutubeChannelRepository channelRepo,
            INotificationService notificationService,
            IRefreshTokenRepository refreshTokenRepo,
            IYoutubeWebhookSubscriptionService webhookService,
            IYoutubeMetadataClient metadataClient,
            IMockWebhookSender mockWebhookSender,
            ILogger<AdminService> logger)
        {
            _userRepo = userRepo;
            _subRepo = subRepo;
            _channelRepo = channelRepo;
            _notificationService = notificationService;
            _refreshTokenRepo = refreshTokenRepo;
            _webhookService = webhookService;
            _metadataClient = metadataClient;
            _mockWebhookSender = mockWebhookSender;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<AdminUserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _userRepo.GetAllWithSubscriptionsAsync(cancellationToken);

                var result = users.Select(user => new AdminUserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Subscriptions = user.Subscriptions.Select(s => new AdminUserSubscriptionDto
                    {
                        SubscriptionId = s.Id,
                        ChannelName = s.YoutubeChannel.ChannelName,
                        SummarizationStyle = s.SummarizationStyle.ToString()
                    }).ToList()
                }).ToList();

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

                var channelId = subscription.ChannelId;
                await _subRepo.DeleteAsync(subscription, cancellationToken);

                var remaining = await _subRepo.GetByYoutubeChannelIdAsync(channelId, cancellationToken);
                if (remaining.Count == 0)
                {
                    var channel = await _channelRepo.GetByIdAsync(channelId, cancellationToken);
                    if (channel is not null)
                    {
                        if (channel.IsWebhookSubscribed)
                        {
                            try
                            {
                                await _webhookService.UnsubscribeAsync(channelId, cancellationToken);
                            }
                            catch (Exception ex) when (ex is not OperationCanceledException)
                            {
                                _logger.LogWarning(ex, "Failed to unsubscribe webhook for channel {ChannelId}. It will expire at the hub.", channelId);
                            }
                        }
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

        public async Task<ServiceResponse<bool>> MockWebhookAsync(string videoUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var metadata = await _metadataClient.GetVideoMetadataAsync(videoUrl, cancellationToken);

                var publishedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var atomXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<feed xmlns:yt=""http://www.youtube.com/xml/schemas/2015"" xmlns=""http://www.w3.org/2005/Atom"">
  <link rel=""hub"" href=""https://pubsubhubbub.appspot.com""/>
  <link rel=""self"" href=""https://www.youtube.com/xml/feeds/videos.xml?channel_id={metadata.ChannelId}""/>
  <title>YouTube video feed</title>
  <updated>{publishedAt}</updated>
  <entry>
    <id>yt:video:{metadata.VideoId}</id>
    <yt:videoId>{metadata.VideoId}</yt:videoId>
    <yt:channelId>{metadata.ChannelId}</yt:channelId>
    <title>{System.Security.SecurityElement.Escape(metadata.Title)}</title>
    <link rel=""alternate"" href=""https://www.youtube.com/watch?v={metadata.VideoId}""/>
    <author>
      <name>{System.Security.SecurityElement.Escape(metadata.ChannelName)}</name>
      <uri>https://www.youtube.com/channel/{metadata.ChannelId}</uri>
    </author>
    <published>{publishedAt}</published>
    <updated>{publishedAt}</updated>
  </entry>
</feed>";

                _logger.LogInformation("Sending mock webhook");

                var result = await _mockWebhookSender.SendAsync(atomXml, cancellationToken);

                return result switch
                {
                    MockWebhookResult.Success => ServiceResponse<bool>.Success(true, $"Mock webhook sent for \"{metadata.Title}\" by {metadata.ChannelName}."),
                    MockWebhookResult.NotConfigured => ServiceResponse<bool>.Failure("Webhook callback URL is not configured. Is ngrok running?"),
                    MockWebhookResult.EndpointError => ServiceResponse<bool>.Failure("Webhook endpoint returned an error."),
                    _ => ServiceResponse<bool>.Failure("Unknown mock webhook result.")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send mock webhook");
                return ServiceResponse<bool>.Failure($"Failed to send mock webhook: {ex.Message}");
            }
        }
    }
}
