using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Services
{
    public class YoutubeWebhookSubscriptionService : IYoutubeWebhookSubscriptionService
    {
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly IYoutubeWebSubClient _webSubClient;
        private readonly ILogger<YoutubeWebhookSubscriptionService> _logger;

        public YoutubeWebhookSubscriptionService(
            IYoutubeChannelRepository channelRepo,
            IYoutubeWebSubClient webSubClient,
            ILogger<YoutubeWebhookSubscriptionService> logger)
        {
            _channelRepo = channelRepo;
            _webSubClient = webSubClient;
            _logger = logger;
        }

        public async Task SubscribeAsync(Guid channelId, CancellationToken cancellationToken = default)
        {
            var channel = await _channelRepo.GetByIdAsync(channelId, cancellationToken)
                ?? throw new InvalidOperationException($"Channel {channelId} not found.");

            if (string.IsNullOrEmpty(channel.YoutubeChannelId))
                throw new InvalidOperationException("Cannot subscribe to webhook: YoutubeChannelId is unknown.");

            channel.LastWebhookSubscriptionAttemptUtc = DateTime.UtcNow;
            await _channelRepo.UpdateAsync(channel, cancellationToken);

            await _webSubClient.SubscribeAsync(channel.YoutubeChannelId, cancellationToken);
        }

        public async Task RefreshAllSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            var channels = await _channelRepo.GetAllAsync(cancellationToken);
            if (channels.Count == 0)
                return;

            _logger.LogInformation("Refreshing webhook subscriptions for {Count} channel(s).", channels.Count);

            foreach (var channel in channels)
            {
                try
                {
                    await SubscribeAsync(channel.Id, cancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogWarning(ex, "Failed to refresh webhook for channel {ChannelId}. Will retry later.", channel.Id);
                }
            }
        }

        public async Task UnsubscribeAsync(Guid channelId, CancellationToken cancellationToken = default)
        {
            var channel = await _channelRepo.GetByIdAsync(channelId, cancellationToken)
                ?? throw new InvalidOperationException($"Channel {channelId} not found.");

            if (string.IsNullOrEmpty(channel.YoutubeChannelId))
                return;

            await _webSubClient.UnsubscribeAsync(channel.YoutubeChannelId, cancellationToken);

            channel.IsWebhookSubscribed = false;
            channel.WebhookExpiresAtUtc = null;
            await _channelRepo.UpdateAsync(channel, cancellationToken);
        }
    }
}
