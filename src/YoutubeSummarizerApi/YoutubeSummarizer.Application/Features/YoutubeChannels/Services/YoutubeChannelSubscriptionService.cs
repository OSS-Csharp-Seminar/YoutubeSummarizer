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
        private readonly IYoutubeMetadataClient _metadataClient;

        public YoutubeChannelSubscriptionService(
            IYoutubeChannelRepository channelRepo,
            IUserYoutubeChannelSubscriptionRepository subscriptionRepo,
            ICurrentUserService currentUserService,
            IYoutubeWebhookSubscriptionService webhookSubscriptionService,
            IYoutubeMetadataClient metadataClient)
        {
            _channelRepo = channelRepo;
            _subscriptionRepo = subscriptionRepo;
            _currentUserService = currentUserService;
            _webhookSubscriptionService = webhookSubscriptionService;
            _metadataClient = metadataClient;
        }

        public async Task<ServiceResponse<SubscribeToYoutubeChannelResponse>> SubscribeAsync(
            SubscribeToYoutubeChannelRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();

                var metadata = await _metadataClient.GetChannelMetadataAsync(request.ChannelUrl, cancellationToken);

                var channel = await _channelRepo.GetByYoutubeChannelIdAsync(metadata.ChannelId, cancellationToken);
                if (channel is null)
                {
                    channel = new YoutubeChannel
                    {
                        YoutubeChannelId = metadata.ChannelId,
                        ChannelName = metadata.ChannelName,
                        ChannelUrl = request.ChannelUrl,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    await _channelRepo.AddAsync(channel, cancellationToken);

                    if (!channel.IsWebhookSubscribed)
                        await _webhookSubscriptionService.SubscribeAsync(channel.Id, cancellationToken);
                }

                var exists = await _subscriptionRepo.ExistsAsync(userId, channel.Id, cancellationToken);
                if (exists)
                    return ServiceResponse<SubscribeToYoutubeChannelResponse>.Failure("You are already subscribed to this channel.");

                await _subscriptionRepo.AddAsync(new UserYoutubeChannelSubscription
                {
                    UserId = userId,
                    ChannelId = channel.Id,
                    SummarizationStyle = request.SummarizationStyle,
                    CreatedAtUtc = DateTime.UtcNow
                }, cancellationToken);

                return ServiceResponse<SubscribeToYoutubeChannelResponse>.Success(
                    new SubscribeToYoutubeChannelResponse
                    {
                        YoutubeChannelId = channel.Id,
                        ChannelName = channel.ChannelName,
                        ChannelUrl = channel.ChannelUrl,
                        SummarizationStyle = request.SummarizationStyle
                    },
                    "Successfully subscribed to channel.");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is HttpRequestException)
            {
                return ServiceResponse<SubscribeToYoutubeChannelResponse>.Failure("Could not resolve channel information. Please check the URL.");
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

                var result = subscriptions.Select(sub => new GetUserSubscriptionsResponse
                {
                    SubscriptionId = sub.Id,
                    YoutubeChannelId = sub.YoutubeChannel.Id,
                    ChannelName = sub.YoutubeChannel.ChannelName,
                    ChannelUrl = sub.YoutubeChannel.ChannelUrl,
                    SummarizationStyle = sub.SummarizationStyle,
                    CreatedAtUtc = sub.CreatedAtUtc
                }).ToList();

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

                var channelId = subscription.ChannelId;

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
