using System.Threading;
using System.Threading.Tasks;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IYoutubeWebhookVerificationService
    {
        Task ConfirmSubscriptionAsync(string topicUrl, int leaseSeconds, CancellationToken cancellationToken = default);
    }
}
