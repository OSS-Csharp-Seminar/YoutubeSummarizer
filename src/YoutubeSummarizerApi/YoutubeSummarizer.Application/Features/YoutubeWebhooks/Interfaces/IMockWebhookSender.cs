namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IMockWebhookSender
    {
        Task<bool> SendAsync(string callbackUrl, string atomXml, CancellationToken cancellationToken = default);
    }
}
