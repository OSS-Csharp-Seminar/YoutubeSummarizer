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
                        Id = Guid.NewGuid(),
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
                    return ServiceResponse<SubscribeToYoutubeChannelResponse>.Failure("Već ste pretplaćeni na ovaj kanal.");

                await _subscriptionRepo.AddAsync(new UserYoutubeChannelSubscription
                {
                    Id = Guid.NewGuid(),
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
                    "Uspješno ste se pretplatili na kanal.");
            }
            catch
            {
                return ServiceResponse<SubscribeToYoutubeChannelResponse>.Failure("Došlo je do greške.");
            }
        }
        public async Task<ServiceResponse<List<GetUserSubscriptionsResponse>>> GetUserSubscriptionsAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = _currentUserService.GetCurrentUserId();
                var subscriptions = await _subscriptionRepo.GetByUserIdAsync(userId, cancellationToken);

                var result = new List<GetUserSubscriptionsResponse>();

                foreach (var sub in subscriptions)
                {
                    var channel = await _channelRepo.GetByIdAsync(sub.YoutubeChannelId, cancellationToken);
                    if (channel is null) continue;

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

                return ServiceResponse<List<GetUserSubscriptionsResponse>>.Success(result, "Pretplate uspješno dohvaćene.");
            }
            catch
            {
                return ServiceResponse<List<GetUserSubscriptionsResponse>>.Failure("Došlo je do greške.");
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
                    return ServiceResponse<bool>.Failure("Pretplata nije pronađena.");

                var channelId = subscription.YoutubeChannelId;

                await _subscriptionRepo.DeleteAsync(subscription, cancellationToken);

                var remaining = await _subscriptionRepo.GetByYoutubeChannelIdAsync(channelId, cancellationToken);
                if (remaining.Count == 0)
                {
                    var channel = await _channelRepo.GetByIdAsync(channelId, cancellationToken);
                    if (channel is not null && channel.IsWebhookSubscribed)
                    {
                        await _webhookSubscriptionService.UnsubscribeAsync(channelId, cancellationToken);
                    }
                }

                return ServiceResponse<bool>.Success(true, "Uspješno ste se odjavili.");
            }
            catch
            {
                return ServiceResponse<bool>.Failure("Došlo je do greške.");
            }
        }
    }
}
