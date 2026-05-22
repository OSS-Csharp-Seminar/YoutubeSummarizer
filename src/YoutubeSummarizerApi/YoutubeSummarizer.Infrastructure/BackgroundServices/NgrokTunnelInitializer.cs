using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks;

namespace YoutubeSummarizer.Infrastructure.BackgroundServices
{
    public class NgrokTunnelInitializer : BackgroundService
    {
        private readonly IOptionsMonitor<YoutubeWebhookSettings> _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NgrokTunnelInitializer> _logger;

        public NgrokTunnelInitializer(
            IOptionsMonitor<YoutubeWebhookSettings> settings,
            IHttpClientFactory httpClientFactory,
            ILogger<NgrokTunnelInitializer> logger)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            for (var i = 0; i < 15; i++)
            {
                try
                {
                    var response = await httpClient.GetFromJsonAsync<NgrokApiResponse>(
                        "http://localhost:4040/api/tunnels", stoppingToken);

                    var tunnel = response?.Tunnels.FirstOrDefault(t =>
                        t.PublicUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase));

                    if (tunnel is not null)
                    {
                        var callbackUrl = $"{tunnel.PublicUrl}/api/webhooks/youtube";
                        _settings.CurrentValue.CallbackUrl = callbackUrl;
                        _logger.LogInformation("Ngrok tunnel URL set to {CallbackUrl}", callbackUrl);
                        return;
                    }
                }
                catch
                {
                }

                _logger.LogInformation("Waiting for ngrok tunnel... (attempt {Attempt}/15)", i + 1);
                await Task.Delay(2000, stoppingToken);
            }

            _logger.LogWarning("Could not resolve ngrok tunnel URL. Webhook callbacks will use the static config value.");
        }

        private class NgrokApiResponse
        {
            [JsonPropertyName("tunnels")]
            public List<NgrokTunnel> Tunnels { get; set; } = new();
        }

        private class NgrokTunnel
        {
            [JsonPropertyName("public_url")]
            public string PublicUrl { get; set; } = string.Empty;
        }
    }
}
