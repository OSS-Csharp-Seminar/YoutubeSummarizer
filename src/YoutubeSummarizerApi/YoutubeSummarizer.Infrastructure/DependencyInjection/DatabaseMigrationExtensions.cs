using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Domain.Models;
using YoutubeSummarizer.Infrastructure.Persistence;
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

            SeedAdminUser(scope.ServiceProvider).GetAwaiter().GetResult();
        }

        private static async Task SeedAdminUser(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var db = services.GetRequiredService<ApplicationDbContext>();

            const string email = "admin@youtubesummarizer.com";

            if (await userManager.FindByEmailAsync(email) is not null)
                return;

            var domainUser = new User
            {
                FirstName = "Admin",
                LastName = "Adminovich",
                Email = email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            db.DomainUsers.Add(domainUser);
            await db.SaveChangesAsync();

            var applicationUser = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = "Admin",
                LastName = "Adminovich",
                DomainUserId = domainUser.Id,
                DomainUser = domainUser,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(applicationUser, "Abcd1$");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(applicationUser, "Admin");
        }
    }
}
