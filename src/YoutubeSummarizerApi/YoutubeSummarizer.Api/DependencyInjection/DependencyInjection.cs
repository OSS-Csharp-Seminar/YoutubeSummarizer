using YoutubeSummarizer.Api.Hubs;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;

namespace YoutubeSummarizer.Api.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<INotificationHubService, NotificationHubService>();

            return services;
        }
    }
}
