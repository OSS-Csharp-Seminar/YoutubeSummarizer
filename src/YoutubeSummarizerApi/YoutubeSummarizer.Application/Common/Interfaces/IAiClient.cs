namespace YoutubeSummarizer.Application.Common.Interfaces
{
    public interface IAiClient
    {
        Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
