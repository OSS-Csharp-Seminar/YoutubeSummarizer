using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces
{
    public interface IYoutubeChannelRepository
    {
        Task<YoutubeChannel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<YoutubeChannel>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
        Task<YoutubeChannel?> GetByYoutubeChannelIdAsync(string youtubeChannelId, CancellationToken cancellationToken = default);
        Task<List<YoutubeChannel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(YoutubeChannel channel, CancellationToken cancellationToken = default);
        Task UpdateAsync(YoutubeChannel channel, CancellationToken cancellationToken = default);
        Task DeleteAsync(YoutubeChannel channel, CancellationToken cancellationToken = default);
    }
}
