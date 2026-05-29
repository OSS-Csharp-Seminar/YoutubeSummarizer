using System.Threading;
using System.Threading.Tasks;

namespace YoutubeSummarizer.Application.Common.Interfaces
{
    public interface IAiClient
    {
        Task<string> CompletePrimaryAsync(string prompt, CancellationToken cancellationToken = default);
        Task<string> CompleteFallbackAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
