using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Services
{
    public class YoutubeWebhookNotificationService : IYoutubeWebhookNotificationService
    {
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly IUserYoutubeChannelSubscriptionRepository _subscriptionRepo;
        private readonly IYoutubeWebhookNotificationParser _notificationParser;
        private readonly ILogger<YoutubeWebhookNotificationService> _logger;

        public YoutubeWebhookNotificationService(
            IYoutubeChannelRepository channelRepo,
            IUserYoutubeChannelSubscriptionRepository subscriptionRepo,
            IYoutubeWebhookNotificationParser notificationParser,
            ILogger<YoutubeWebhookNotificationService> logger)
        {
            _channelRepo = channelRepo;
            _subscriptionRepo = subscriptionRepo;
            _notificationParser = notificationParser;
            _logger = logger;
        }

        public async Task ProcessNotificationAsync(string payload, CancellationToken cancellationToken = default)
        {
            var notification = _notificationParser.Parse(payload);

            var channel = await _channelRepo.GetByYoutubeChannelIdAsync(notification.ChannelId, cancellationToken);
            if (channel is null)
            {
                _logger.LogWarning("Received webhook notification for unknown channel {YoutubeChannelId}.", notification.ChannelId);
                return;
            }

            var subscriptions = await _subscriptionRepo.GetByYoutubeChannelIdAsync(channel.Id, cancellationToken);

            foreach (var subscription in subscriptions)
            {
                _logger.LogInformation(
                    "New video {VideoId} on channel {ChannelId} for user {UserId} with style {Style}. TODO: fetch transcript and summarize.",
                    notification.VideoId, notification.ChannelId, subscription.UserId, subscription.SummarizationStyle);
            }
        }
    }
}
