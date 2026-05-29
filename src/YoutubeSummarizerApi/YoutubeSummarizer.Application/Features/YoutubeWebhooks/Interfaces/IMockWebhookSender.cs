using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Dtos;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IMockWebhookSender
    {
        Task<MockWebhookResult> SendAsync(string atomXml, CancellationToken cancellationToken = default);
    }
}
