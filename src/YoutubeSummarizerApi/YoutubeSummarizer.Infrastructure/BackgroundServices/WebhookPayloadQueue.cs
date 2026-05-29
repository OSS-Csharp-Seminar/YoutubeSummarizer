using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;

namespace YoutubeSummarizer.Infrastructure.BackgroundServices
{
    public class WebhookPayloadQueue : IWebhookPayloadQueue
    {
        private readonly Channel<string> _channel = Channel.CreateBounded<string>(100);

        public ValueTask EnqueueAsync(string payload, CancellationToken cancellationToken = default)
            => _channel.Writer.WriteAsync(payload, cancellationToken);

        public ValueTask<string> DequeueAsync(CancellationToken cancellationToken = default)
            => _channel.Reader.ReadAsync(cancellationToken);
    }
}
