using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Infrastructure.BackgroundServices
{
    public class WebhookNotificationProcessor : BackgroundService
    {
        private readonly IWebhookPayloadQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WebhookNotificationProcessor> _logger;

        public WebhookNotificationProcessor(
            IWebhookPayloadQueue queue,
            IServiceProvider serviceProvider,
            ILogger<WebhookNotificationProcessor> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var payload = await _queue.DequeueAsync(stoppingToken);

                    using var scope = _serviceProvider.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IYoutubeWebhookNotificationService>();
                    await service.ProcessNotificationAsync(payload, CancellationToken.None);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing webhook notification.");
                }
            }
        }
    }
}
