using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Infrastructure.BackgroundServices
{
    public class YoutubeWebhookRenewalBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<YoutubeWebhookRenewalBackgroundService> _logger;

        public YoutubeWebhookRenewalBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<YoutubeWebhookRenewalBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var svc = scope.ServiceProvider.GetRequiredService<IYoutubeWebhookSubscriptionService>();
                    await svc.RenewExpiringSubscriptionsAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error during webhook subscription renewal.");
                }
            }
        }
    }
}
