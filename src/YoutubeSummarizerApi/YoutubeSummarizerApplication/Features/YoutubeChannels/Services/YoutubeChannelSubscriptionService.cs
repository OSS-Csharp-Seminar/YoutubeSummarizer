using YoutubeSummarizer.Application.Common.Interfaces;
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

        public async Task<SubscribeToYoutubeChannelResponse> SubscribeAsync(
            SubscribeToYoutubeChannelRequest request,
            CancellationToken cancellationToken = default)
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
                throw new InvalidOperationException("User is already subscribed to this channel.");

            await _subscriptionRepo.AddAsync(new UserYoutubeChannelSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                YoutubeChannelId = channel.Id,
                SummarizationStyle = request.SummarizationStyle,
                CreatedAtUtc = DateTime.UtcNow
            }, cancellationToken);

            return new SubscribeToYoutubeChannelResponse
            {
                YoutubeChannelId = channel.Id,
                ChannelIdentifier = channel.ChannelIdentifier,
                ChannelUrl = channel.ChannelUrl,
                SummarizationStyle = request.SummarizationStyle
            };
        }
    }
}
