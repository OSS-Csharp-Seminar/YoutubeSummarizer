using System.Text;
using YoutubeSummarizer.Application.Features.Admin.Interfaces;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeWebhooks
{
    public class MockWebhookSender : IMockWebhookSender
    {
        private readonly HttpClient _httpClient;

        public MockWebhookSender(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SendAsync(string callbackUrl, string atomXml, CancellationToken cancellationToken = default)
        {
            var content = new StringContent(atomXml, Encoding.UTF8, "application/atom+xml");
            var response = await _httpClient.PostAsync(callbackUrl, content, cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
