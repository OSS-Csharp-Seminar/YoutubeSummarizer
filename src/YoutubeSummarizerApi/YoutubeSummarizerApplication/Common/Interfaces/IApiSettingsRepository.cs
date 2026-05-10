namespace YoutubeSummarizer.Application.Common.Interfaces
{
    public interface IApiSettingsRepository
    {
        Task<string> GetApiKeyAsync(string providerName, CancellationToken cancellationToken = default);
    }
}
