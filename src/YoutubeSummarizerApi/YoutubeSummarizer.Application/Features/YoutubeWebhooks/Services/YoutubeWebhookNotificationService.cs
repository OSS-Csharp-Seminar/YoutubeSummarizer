using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Services
{
    public class YoutubeWebhookNotificationService : IYoutubeWebhookNotificationService
    {
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly IUserYoutubeChannelSubscriptionRepository _subscriptionRepo;
        private readonly ILogger<YoutubeWebhookNotificationService> _logger;

        public YoutubeWebhookNotificationService(
            IYoutubeChannelRepository channelRepo,
            IUserYoutubeChannelSubscriptionRepository subscriptionRepo,
            ILogger<YoutubeWebhookNotificationService> logger)
        {
            _channelRepo = channelRepo;
            _subscriptionRepo = subscriptionRepo;
            _logger = logger;
        }

        public async Task ProcessNotificationAsync(string youtubeChannelId, string videoId, CancellationToken cancellationToken = default)
        {
            var channel = await _channelRepo.GetByYoutubeChannelIdAsync(youtubeChannelId, cancellationToken);
            if (channel is null)
            {
                _logger.LogWarning("Received webhook notification for unknown channel {YoutubeChannelId}.", youtubeChannelId);
                return;
            }

            var subscriptions = await _subscriptionRepo.GetByYoutubeChannelIdAsync(channel.Id, cancellationToken);

            foreach (var subscription in subscriptions)
            {
                _logger.LogInformation(
                    "New video {VideoId} on channel {ChannelId} for user {UserId} with style {Style}. TODO: fetch transcript and summarize.",
                    videoId, youtubeChannelId, subscription.UserId, subscription.SummarizationStyle);
            }
        }
    }
}
