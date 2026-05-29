using System.Threading;
using System.Threading.Tasks;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IWebhookPayloadQueue
    {
        ValueTask EnqueueAsync(string payload, CancellationToken cancellationToken = default);
        ValueTask<string> DequeueAsync(CancellationToken cancellationToken = default);
    }
}
