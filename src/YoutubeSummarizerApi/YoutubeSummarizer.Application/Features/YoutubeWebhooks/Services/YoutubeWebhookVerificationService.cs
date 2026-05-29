using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Services
{
    public class YoutubeWebhookVerificationService : IYoutubeWebhookVerificationService
    {
        private readonly IYoutubeChannelRepository _channelRepo;

        public YoutubeWebhookVerificationService(IYoutubeChannelRepository channelRepo)
        {
            _channelRepo = channelRepo;
        }

        public async Task ConfirmSubscriptionAsync(string topicUrl, int leaseSeconds, CancellationToken cancellationToken = default)
        {
            var youtubeChannelId = ExtractChannelId(topicUrl);

            var channel = await _channelRepo.GetByYoutubeChannelIdAsync(youtubeChannelId, cancellationToken);
            if (channel is null)
                return;

            channel.IsWebhookSubscribed = true;
            channel.WebhookExpiresAtUtc = DateTime.UtcNow.AddSeconds(leaseSeconds);
            await _channelRepo.UpdateAsync(channel, cancellationToken);
        }

        private static string ExtractChannelId(string topicUrl)
        {
            var query = new Uri(topicUrl).Query.TrimStart('?');
            return query.Split('&')
                .Select(p => p.Split('='))
                .Where(p => p.Length == 2 && Uri.UnescapeDataString(p[0]) == "channel_id")
                .Select(p => Uri.UnescapeDataString(p[1]))
                .FirstOrDefault()
                ?? throw new ArgumentException("channel_id not found in topic URL.", nameof(topicUrl));
        }
    }
}
