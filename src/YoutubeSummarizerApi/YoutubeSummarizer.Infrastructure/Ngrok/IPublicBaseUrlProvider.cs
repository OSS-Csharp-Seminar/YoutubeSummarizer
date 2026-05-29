namespace YoutubeSummarizer.Infrastructure.Ngrok
{
    public interface IPublicBaseUrlProvider
    {
        string? PublicBaseUrl { get; }
    }
}
