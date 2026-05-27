namespace YoutubeSummarizer.Application.Features.Admin.Interfaces
{
    public interface IMockWebhookSender
    {
        Task<bool> SendAsync(string callbackUrl, string atomXml, CancellationToken cancellationToken = default);
    }
}
