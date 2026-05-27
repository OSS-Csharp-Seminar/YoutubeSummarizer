using YoutubeSummarizer.Application.Features.Admin.Dtos;

namespace YoutubeSummarizer.Application.Features.Admin.Interfaces
{
    public interface IYoutubeMetadataClient
    {
        Task<YoutubeVideoMetadata> GetVideoMetadataAsync(string videoUrl, CancellationToken cancellationToken = default);
        Task<YoutubeChannelMetadata> GetChannelMetadataAsync(string channelUrl, CancellationToken cancellationToken = default);
    }
}
