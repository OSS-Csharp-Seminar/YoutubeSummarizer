using System.Threading;
using System.Threading.Tasks;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IYoutubeWebSubClient
    {
        Task SubscribeAsync(string youtubeChannelId, CancellationToken cancellationToken = default);
        Task UnsubscribeAsync(string youtubeChannelId, CancellationToken cancellationToken = default);
    }
}
