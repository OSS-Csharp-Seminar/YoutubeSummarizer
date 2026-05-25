using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Features.Summarize;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Infrastructure.BackgroundServices;
using YoutubeSummarizer.Infrastructure.ExternalServices.Ai;
using YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeTranscript;
using YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeWebhooks;
using YoutubeSummarizer.Infrastructure.Persistence;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;
using YoutubeSummarizer.Infrastructure.Persistence.Repositories;
using YoutubeSummarizer.Infrastructure.Security;

namespace YoutubeSummarizer.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
                }));

            // Add Identity
           services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IApiSettingsRepository, ApiSettingsRepository>();
            services.AddScoped<IAiClient, AiClient>();
            services.Configure<AiSettings>(configuration.GetSection("AiSettings"));
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IYoutubeChannelRepository, YoutubeChannelRepository>();
            services.AddScoped<IUserYoutubeChannelSubscriptionRepository, UserYoutubeChannelSubscriptionRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IYoutubeWebhookNotificationParser, YoutubeAtomXmlParser>();

            services.AddHttpClient<IYoutubeTranscriptClient, YoutubeTranscriptClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ExternalApis:YoutubeTranscript:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.Configure<YoutubeWebhookSettings>(configuration.GetSection("Webhooks:Youtube"));
            services.AddHttpClient<IYoutubeWebSubClient, YoutubeWebSubClient>(client =>
            {
                client.BaseAddress = new Uri("https://pubsubhubbub.appspot.com/subscribe");
            });
            services.AddHostedService<YoutubeWebhookRenewalBackgroundService>();
            services.AddHostedService<NgrokTunnelInitializer>();

            return services;
        }
    }
}

