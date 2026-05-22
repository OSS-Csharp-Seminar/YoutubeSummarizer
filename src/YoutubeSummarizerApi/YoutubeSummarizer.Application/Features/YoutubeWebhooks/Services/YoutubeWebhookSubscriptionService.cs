using Microsoft.Extensions.Options;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Services
{
    public class YoutubeWebhookSubscriptionService : IYoutubeWebhookSubscriptionService
    {
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly IYoutubeWebSubClient _webSubClient;
        private readonly IOptionsMonitor<YoutubeWebhookSettings> _settings;

        public YoutubeWebhookSubscriptionService(
            IYoutubeChannelRepository channelRepo,
            IYoutubeWebSubClient webSubClient,
            IOptionsMonitor<YoutubeWebhookSettings> settings)
        {
            _channelRepo = channelRepo;
            _webSubClient = webSubClient;
            _settings = settings;
        }

        public async Task SubscribeAsync(Guid channelId, CancellationToken cancellationToken = default)
        {
            var channel = await _channelRepo.GetByIdAsync(channelId, cancellationToken)
                ?? throw new InvalidOperationException($"Channel {channelId} not found.");

            if (string.IsNullOrEmpty(channel.YoutubeChannelId))
                throw new InvalidOperationException("Cannot subscribe to webhook: YoutubeChannelId is unknown.");

            channel.LastWebhookSubscriptionAttemptUtc = DateTime.UtcNow;
            await _channelRepo.UpdateAsync(channel, cancellationToken);

            await _webSubClient.SubscribeAsync(_settings.CurrentValue.CallbackUrl, channel.YoutubeChannelId, cancellationToken);
        }

        public async Task RenewExpiringSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            var threshold = DateTime.UtcNow.AddDays(1);
            var channels = await _channelRepo.GetExpiringWebhookSubscriptionsAsync(threshold, cancellationToken);

            foreach (var channel in channels)
                await SubscribeAsync(channel.Id, cancellationToken);
        }

        public async Task UnsubscribeAsync(Guid channelId, CancellationToken cancellationToken = default)
        {
            var channel = await _channelRepo.GetByIdAsync(channelId, cancellationToken)
                ?? throw new InvalidOperationException($"Channel {channelId} not found.");

            if (string.IsNullOrEmpty(channel.YoutubeChannelId))
                return;

            await _webSubClient.UnsubscribeAsync(_settings.CurrentValue.CallbackUrl, channel.YoutubeChannelId, cancellationToken);

            channel.IsWebhookSubscribed = false;
            channel.WebhookExpiresAtUtc = null;
            await _channelRepo.UpdateAsync(channel, cancellationToken);
        }
    }
}
