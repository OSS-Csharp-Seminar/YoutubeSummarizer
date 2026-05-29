using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Infrastructure.Ngrok;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeWebhooks
{
    public class YoutubeWebSubClient : IYoutubeWebSubClient
    {
        private readonly HttpClient _httpClient;
        private readonly IPublicBaseUrlProvider _publicBaseUrlProvider;

        public YoutubeWebSubClient(HttpClient httpClient, IPublicBaseUrlProvider publicBaseUrlProvider)
        {
            _httpClient = httpClient;
            _publicBaseUrlProvider = publicBaseUrlProvider;
        }

        public async Task SubscribeAsync(string youtubeChannelId, CancellationToken cancellationToken = default)
        {
            var callbackUrl = ResolveCallbackUrl();
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

        public async Task UnsubscribeAsync(string youtubeChannelId, CancellationToken cancellationToken = default)
        {
            var callbackUrl = ResolveCallbackUrl();
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

        private string ResolveCallbackUrl()
        {
            var publicBaseUrl = _publicBaseUrlProvider.PublicBaseUrl
                ?? throw new InvalidOperationException("Public base URL is not yet configured. Is ngrok running?");
            return $"{publicBaseUrl}/{YoutubeWebhookCallbackPath.Value}";
        }
    }
}
