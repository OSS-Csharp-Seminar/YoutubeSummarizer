using System;
using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Services
{
    public class YoutubeWebhookSubscriptionService : IYoutubeWebhookSubscriptionService
    {
        private readonly IYoutubeChannelRepository _channelRepo;
        private readonly IYoutubeWebSubClient _webSubClient;

        public YoutubeWebhookSubscriptionService(
            IYoutubeChannelRepository channelRepo,
            IYoutubeWebSubClient webSubClient)
        {
            _channelRepo = channelRepo;
            _webSubClient = webSubClient;
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

            await _webSubClient.UnsubscribeAsync(channel.YoutubeChannelId, cancellationToken);

            channel.IsWebhookSubscribed = false;
            channel.WebhookExpiresAtUtc = null;
            await _channelRepo.UpdateAsync(channel, cancellationToken);
        }
    }
}
