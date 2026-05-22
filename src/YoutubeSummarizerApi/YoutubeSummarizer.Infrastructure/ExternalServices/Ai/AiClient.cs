using YoutubeSummarizer.Application.Common.Interfaces;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.Ai
{
    public class AiClient : IAiClient
    {
        public Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("AI client endpoint not yet configured.");
        }
    }
}
