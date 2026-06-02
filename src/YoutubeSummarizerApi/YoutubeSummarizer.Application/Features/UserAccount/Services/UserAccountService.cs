using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;
using YoutubeSummarizer.Application.Features.UserAccount.Dtos;
using YoutubeSummarizer.Application.Features.UserAccount.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.UserAccount.Services
{
    public class UserAccountService : IUserAccountService
    {
        private const int MaxKeywordsPerUser = 50;

        private readonly IBlacklistEntryRepository _blacklistRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthRepository _authRepo;
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IUserYoutubeChannelSubscriptionRepository _subRepo;
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly IYoutubeWebhookSubscriptionService _webhookService;
        private readonly ILogger<UserAccountService> _logger;

        public UserAccountService(
            IBlacklistEntryRepository blacklistRepo,
            ICurrentUserService currentUserService,
            IAuthRepository authRepo,
            IUserRepository userRepo,
            IRefreshTokenRepository refreshTokenRepo,
            INotificationRepository notificationRepo,
            IUserYoutubeChannelSubscriptionRepository subRepo,
            IYoutubeChannelRepository channelRepo,
            IYoutubeWebhookSubscriptionService webhookService,
            ILogger<UserAccountService> logger)
        {
            _blacklistRepo = blacklistRepo;
            _currentUserService = currentUserService;
            _authRepo = authRepo;
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _notificationRepo = notificationRepo;
            _subRepo = subRepo;
            _channelRepo = channelRepo;
            _webhookService = webhookService;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<BlacklistEntryDto>>> GetBlacklistEntriesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetCurrentUserId();
            var entries = await _blacklistRepo.GetByUserIdAsync(userId, cancellationToken);
            var dtos = entries.Select(e => new BlacklistEntryDto { Keyword = e.Keyword }).ToList();
            return ServiceResponse<List<BlacklistEntryDto>>.Success(dtos, "Blacklist entries retrieved successfully.");
        }

        public async Task<ServiceResponse<BlacklistEntryDto>> AddBlacklistEntryAsync(AddBlacklistEntryRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetCurrentUserId();
            var normalized = request.Keyword.Trim().ToLowerInvariant();

            var count = await _blacklistRepo.CountByUserIdAsync(userId, cancellationToken);
            if (count >= MaxKeywordsPerUser)
                return ServiceResponse<BlacklistEntryDto>.Failure($"You cannot add more than {MaxKeywordsPerUser} blacklisted keywords.");

            var exists = await _blacklistRepo.ExistsAsync(userId, normalized, cancellationToken);
            if (exists)
                return ServiceResponse<BlacklistEntryDto>.Failure("This keyword is already in your blacklist.");

            var entry = new BlacklistEntry
            {
                UserId = userId,
                Keyword = normalized
            };

            await _blacklistRepo.AddAsync(entry, cancellationToken);

            return ServiceResponse<BlacklistEntryDto>.Success(
                new BlacklistEntryDto { Keyword = entry.Keyword },
                "Keyword added to blacklist.");
        }

        public async Task<ServiceResponse<bool>> RemoveBlacklistEntryAsync(string keyword, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetCurrentUserId();
            var normalized = keyword.Trim().ToLowerInvariant();
            var entry = await _blacklistRepo.GetByKeyAsync(userId, normalized, cancellationToken);

            if (entry is null)
                return ServiceResponse<bool>.Failure("Keyword not found.");

            await _blacklistRepo.DeleteAsync(entry, cancellationToken);
            return ServiceResponse<bool>.Success(true, "Keyword removed from blacklist.");
        }

        public async Task<ServiceResponse<bool>> DeleteAccountAsync(DeleteAccountRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.GetCurrentUserId();
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
                return ServiceResponse<bool>.Failure("User not found.");

            var passwordValid = await _authRepo.CheckPasswordAsync(user.Email, request.Password);
            if (!passwordValid)
                return ServiceResponse<bool>.Failure("Incorrect password.");

            var subscriptions = await _subRepo.GetByUserIdAsync(userId, cancellationToken);
            var channelIds = subscriptions.Select(s => s.ChannelId).Distinct().ToList();

            foreach (var sub in subscriptions)
                await _subRepo.DeleteAsync(sub, cancellationToken);

            foreach (var channelId in channelIds)
            {
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
            }

            await _blacklistRepo.DeleteAllByUserIdAsync(userId, cancellationToken);
            await _notificationRepo.DeleteAllUserNotificationsAsync(userId, cancellationToken);
            await _refreshTokenRepo.DeleteAllByUserIdAsync(userId, cancellationToken);
            await _userRepo.DeleteAsync(user, cancellationToken);

            return ServiceResponse<bool>.Success(true, "Account deleted successfully.");
        }
    }
}
