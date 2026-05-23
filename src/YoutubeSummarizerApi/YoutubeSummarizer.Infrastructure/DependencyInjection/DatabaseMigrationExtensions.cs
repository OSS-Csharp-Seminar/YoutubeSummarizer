using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;

namespace YoutubeSummarizer.Infrastructure.DependencyInjection
{
    public static class DatabaseMigrationExtensions
    {
        public static void MigrateDatabase(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            var retries = 0;
            const int maxRetries = 10;

            while (true)
            {
                try
                {
                    db.Database.CreateExecutionStrategy().Execute(() => db.Database.Migrate());
                    break;
                }
                catch (Exception ex) when (retries < maxRetries)
                {
                    retries++;
                    logger.LogWarning("Database not ready (attempt {Attempt}/{Max}): {Message}", retries, maxRetries, ex.Message);
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
