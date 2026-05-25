using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.Services
{
    public class YoutubeChannelSubscriptionService : IYoutubeChannelSubscriptionService
    {
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly IUserYoutubeChannelSubscriptionRepository _subscriptionRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IYoutubeWebhookSubscriptionService _webhookSubscriptionService;

        public YoutubeChannelSubscriptionService(
            IYoutubeChannelRepository channelRepo,
            IUserYoutubeChannelSubscriptionRepository subscriptionRepo,
            ICurrentUserService currentUserService,
            IYoutubeWebhookSubscriptionService webhookSubscriptionService)
        {
            _channelRepo = channelRepo;
            _subscriptionRepo = subscriptionRepo;
            _currentUserService = currentUserService;
            _webhookSubscriptionService = webhookSubscriptionService;
        }

        public async Task<ServiceResponse<SubscribeToYoutubeChannelResponse>> SubscribeAsync(
            SubscribeToYoutubeChannelRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var identifier = YoutubeChannelUrlParser.ParseChannelIdentifier(request.ChannelUrl);

                var channel = await _channelRepo.GetByIdentifierAsync(identifier, cancellationToken);
                if (channel is null)
                {
                    string? youtubeChannelId = identifier.StartsWith("UC", StringComparison.OrdinalIgnoreCase) ? identifier : null;

                    channel = new YoutubeChannel
                    {
                        ChannelIdentifier = identifier,
                        ChannelUrl = request.ChannelUrl,
                        YoutubeChannelId = youtubeChannelId,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    await _channelRepo.AddAsync(channel, cancellationToken);

                    if (!string.IsNullOrEmpty(channel.YoutubeChannelId) && !channel.IsWebhookSubscribed)
                        await _webhookSubscriptionService.SubscribeAsync(channel.Id, cancellationToken);
                }

                var exists = await _subscriptionRepo.ExistsAsync(userId, channel.Id, cancellationToken);
                if (exists)
                    return ServiceResponse<SubscribeToYoutubeChannelResponse>.Failure("You are already subscribed to this channel.");

                await _subscriptionRepo.AddAsync(new UserYoutubeChannelSubscription
                {
                    UserId = userId,
                    YoutubeChannelId = channel.Id,
                    SummarizationStyle = request.SummarizationStyle,
                    CreatedAtUtc = DateTime.UtcNow
                }, cancellationToken);

                return ServiceResponse<SubscribeToYoutubeChannelResponse>.Success(
                    new SubscribeToYoutubeChannelResponse
                    {
                        YoutubeChannelId = channel.Id,
                        ChannelIdentifier = channel.ChannelIdentifier,
                        ChannelUrl = channel.ChannelUrl,
                        SummarizationStyle = request.SummarizationStyle
                    },
                    "Successfully subscribed to channel.");
            }
            catch
            {
                return ServiceResponse<SubscribeToYoutubeChannelResponse>.Failure("An error occurred.");
            }
        }
        public async Task<ServiceResponse<List<GetUserSubscriptionsResponse>>> GetUserSubscriptionsAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var subscriptions = await _subscriptionRepo.GetByUserIdAsync(userId, cancellationToken);

                var channelIds = subscriptions.Select(s => s.YoutubeChannelId).Distinct().ToList();
                var channels = await _channelRepo.GetByIdsAsync(channelIds, cancellationToken);
                var channelMap = channels.ToDictionary(c => c.Id);

                var result = new List<GetUserSubscriptionsResponse>();

                foreach (var sub in subscriptions)
                {
                    if (!channelMap.TryGetValue(sub.YoutubeChannelId, out var channel)) continue;

                    result.Add(new GetUserSubscriptionsResponse
                    {
                        SubscriptionId = sub.Id,
                        YoutubeChannelId = channel.Id,
                        ChannelIdentifier = channel.ChannelIdentifier,
                        ChannelUrl = channel.ChannelUrl,
                        SummarizationStyle = sub.SummarizationStyle,
                        CreatedAtUtc = sub.CreatedAtUtc
                    });
                }

                return ServiceResponse<List<GetUserSubscriptionsResponse>>.Success(result, "Subscriptions retrieved successfully.");
            }
            catch
            {
                return ServiceResponse<List<GetUserSubscriptionsResponse>>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<bool>> UnsubscribeAsync(
            Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();

                var subscription = await _subscriptionRepo.GetByIdAsync(subscriptionId, cancellationToken);
                if (subscription is null || subscription.UserId != userId)
                    return ServiceResponse<bool>.Failure("Subscription not found.");

                var channelId = subscription.YoutubeChannelId;

                await _subscriptionRepo.DeleteAsync(subscription, cancellationToken);

                var remaining = await _subscriptionRepo.GetByYoutubeChannelIdAsync(channelId, cancellationToken);
                if (remaining.Count == 0)
                {
                    var channel = await _channelRepo.GetByIdAsync(channelId, cancellationToken);
                    if (channel is not null)
                    {
                        if (channel.IsWebhookSubscribed)
                            await _webhookSubscriptionService.UnsubscribeAsync(channelId, cancellationToken);
                        await _channelRepo.DeleteAsync(channel, cancellationToken);
                    }
                }

                return ServiceResponse<bool>.Success(true, "Successfully unsubscribed.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("An error occurred.");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateSummarizationStyleAsync(
            Guid subscriptionId, UpdateSummarizationStyleRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();

                var subscription = await _subscriptionRepo.GetByIdAsync(subscriptionId, cancellationToken);
                if (subscription is null || subscription.UserId != userId)
                    return ServiceResponse<bool>.Failure("Subscription not found.");

                subscription.SummarizationStyle = request.SummarizationStyle;
                await _subscriptionRepo.UpdateAsync(subscription, cancellationToken);

                return ServiceResponse<bool>.Success(true, "Summarization style updated.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("An error occurred.");
            }
        }
    }
}
