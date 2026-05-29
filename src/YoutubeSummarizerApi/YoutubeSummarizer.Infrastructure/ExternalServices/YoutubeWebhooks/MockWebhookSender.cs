using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Dtos;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Infrastructure.Ngrok;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeWebhooks
{
    public class MockWebhookSender : IMockWebhookSender
    {
        private readonly HttpClient _httpClient;
        private readonly IPublicBaseUrlProvider _publicBaseUrlProvider;

        public MockWebhookSender(HttpClient httpClient, IPublicBaseUrlProvider publicBaseUrlProvider)
        {
            _httpClient = httpClient;
            _publicBaseUrlProvider = publicBaseUrlProvider;
        }

        public async Task<MockWebhookResult> SendAsync(string atomXml, CancellationToken cancellationToken = default)
        {
            var publicBaseUrl = _publicBaseUrlProvider.PublicBaseUrl;
            if (string.IsNullOrEmpty(publicBaseUrl))
                return MockWebhookResult.NotConfigured;

            var callbackUrl = $"{publicBaseUrl}/{YoutubeWebhookCallbackPath.Value}";
            var content = new StringContent(atomXml, Encoding.UTF8, "application/atom+xml");
            var response = await _httpClient.PostAsync(callbackUrl, content, cancellationToken);

            return response.IsSuccessStatusCode
                ? MockWebhookResult.Success
                : MockWebhookResult.EndpointError;
        }
    }
}
