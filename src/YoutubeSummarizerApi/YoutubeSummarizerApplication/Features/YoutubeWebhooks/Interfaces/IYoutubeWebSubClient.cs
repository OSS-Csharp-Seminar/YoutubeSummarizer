namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IYoutubeWebSubClient
    {
        Task SubscribeAsync(string callbackUrl, string youtubeChannelId, CancellationToken cancellationToken = default);
    }
}
