using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeWebhooks
{
    public class YoutubeWebSubClient : IYoutubeWebSubClient
    {
        private readonly HttpClient _httpClient;

        public YoutubeWebSubClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SubscribeAsync(string callbackUrl, string youtubeChannelId, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["hub.callback"] = callbackUrl,
                ["hub.mode"] = "subscribe",
                ["hub.topic"] = $"https://www.youtube.com/xml/feeds/videos.xml?channel_id={youtubeChannelId}",
                ["hub.verify"] = "async"
            });

            var response = await _httpClient.PostAsync("", content, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        
        public async Task UnsubscribeAsync(string callbackUrl, string youtubeChannelId, CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["hub.callback"] = callbackUrl,
                ["hub.mode"] = "unsubscribe",
                ["hub.topic"] = $"https://www.youtube.com/xml/feeds/videos.xml?channel_id={youtubeChannelId}",
                ["hub.verify"] = "async"
            });

            var response = await _httpClient.PostAsync("", content, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
