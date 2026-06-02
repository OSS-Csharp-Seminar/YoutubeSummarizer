using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Infrastructure.Ngrok
{
    public class NgrokTunnelInitializer : BackgroundService
    {
        private readonly IPublicBaseUrlState _publicBaseUrlState;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NgrokTunnelInitializer> _logger;

        public NgrokTunnelInitializer(
            IPublicBaseUrlState publicBaseUrlState,
            IHttpClientFactory httpClientFactory,
            IServiceProvider serviceProvider,
            ILogger<NgrokTunnelInitializer> logger)
        {
            _publicBaseUrlState = publicBaseUrlState;
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            string? lastKnownUrl = null;

            while (!stoppingToken.IsCancellationRequested)
            {
                var currentUrl = await PollNgrokTunnelAsync(httpClient, stoppingToken);

                if (currentUrl != lastKnownUrl)
                {
                    _publicBaseUrlState.PublicBaseUrl = currentUrl;

                    if (currentUrl is not null)
                    {
                        _logger.LogInformation("Ngrok public base URL set to {PublicBaseUrl}", currentUrl);
                        await RefreshWebhookSubscriptionsAsync(stoppingToken);
                    }
                    else
                    {
                        _logger.LogWarning("Ngrok tunnel is no longer available. Webhook callbacks will be unavailable until it comes back.");
                    }

                    lastKnownUrl = currentUrl;
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task<string?> PollNgrokTunnelAsync(HttpClient httpClient, CancellationToken cancellationToken)
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<NgrokApiResponse>(
                    "http://localhost:4040/api/tunnels", cancellationToken);

                return response?.Tunnels.FirstOrDefault(t =>
                    t.PublicUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))?.PublicUrl;
            }
            catch
            {
                return null;
            }
        }

        private async Task RefreshWebhookSubscriptionsAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IYoutubeWebhookSubscriptionService>();
                await svc.RefreshAllSubscriptionsAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to refresh webhook subscriptions after ngrok URL change.");
            }
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
