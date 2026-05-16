namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IYoutubeWebhookNotificationService
    {
        Task ProcessNotificationAsync(string youtubeChannelId, string videoId, CancellationToken cancellationToken = default);
    }
}
