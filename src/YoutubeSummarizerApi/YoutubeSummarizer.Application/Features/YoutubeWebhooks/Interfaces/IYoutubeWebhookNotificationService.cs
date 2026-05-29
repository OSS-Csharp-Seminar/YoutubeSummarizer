using System.Threading;
using System.Threading.Tasks;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IYoutubeWebhookNotificationService
    {
        Task ProcessNotificationAsync(string payload, CancellationToken cancellationToken = default);
    }
}
