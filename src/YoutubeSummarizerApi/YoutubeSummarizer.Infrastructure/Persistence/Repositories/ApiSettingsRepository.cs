using Microsoft.EntityFrameworkCore;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.Persistence.Repositories
{
    public class ApiSettingsRepository : IApiSettingsRepository
    {
        private readonly ApplicationDbContext _db;

        public ApiSettingsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> GetApiKeyAsync(string providerName, CancellationToken cancellationToken = default)
        {
            var setting = await _db.ApiSettings
                .Where(x => x.ProviderName == providerName && x.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (setting is null)
                throw new InvalidOperationException($"No active API key found for provider '{providerName}'");

            return setting.ApiKey;
        }
    }
}
