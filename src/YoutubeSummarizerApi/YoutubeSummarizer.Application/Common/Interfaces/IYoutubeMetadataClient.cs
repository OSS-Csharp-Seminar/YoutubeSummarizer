using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Application.Common.Models;

namespace YoutubeSummarizer.Application.Common.Interfaces
{
    public interface IYoutubeMetadataClient
    {
        Task<YoutubeVideoMetadata> GetVideoMetadataAsync(string videoUrl, CancellationToken cancellationToken = default);
        Task<YoutubeChannelMetadata> GetChannelMetadataAsync(string channelUrl, CancellationToken cancellationToken = default);
    }
}
