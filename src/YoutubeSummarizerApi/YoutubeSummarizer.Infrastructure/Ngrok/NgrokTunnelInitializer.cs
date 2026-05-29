using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace YoutubeSummarizer.Infrastructure.Ngrok
{
    public class NgrokTunnelInitializer : BackgroundService
    {
        private readonly IPublicBaseUrlWriter _publicBaseUrlWriter;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NgrokTunnelInitializer> _logger;

        public NgrokTunnelInitializer(
            IPublicBaseUrlWriter publicBaseUrlWriter,
            IHttpClientFactory httpClientFactory,
            ILogger<NgrokTunnelInitializer> logger)
        {
            _publicBaseUrlWriter = publicBaseUrlWriter;
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
                        _publicBaseUrlWriter.SetPublicBaseUrl(tunnel.PublicUrl);
                        _logger.LogInformation("Ngrok public base URL set to {PublicBaseUrl}", tunnel.PublicUrl);
                        return;
                    }
                }
                catch
                {
                }

                _logger.LogInformation("Waiting for ngrok tunnel... (attempt {Attempt}/15)", i + 1);
                await Task.Delay(2000, stoppingToken);
            }

            _logger.LogWarning("Could not resolve ngrok tunnel URL. Webhook callbacks will be unavailable until configured.");
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
