namespace YoutubeSummarizer.Infrastructure.Ngrok
{
    public class PublicBaseUrlState : IPublicBaseUrlProvider, IPublicBaseUrlWriter
    {
        private volatile string? _publicBaseUrl;

        public string? PublicBaseUrl => _publicBaseUrl;

        public void SetPublicBaseUrl(string publicBaseUrl) => _publicBaseUrl = publicBaseUrl;
    }
}
