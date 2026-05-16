namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IYoutubeWebhookSubscriptionService
    {
        Task SubscribeAsync(Guid youtubeChannelId, CancellationToken cancellationToken = default);
        Task RenewExpiringSubscriptionsAsync(CancellationToken cancellationToken = default);
    }
}
