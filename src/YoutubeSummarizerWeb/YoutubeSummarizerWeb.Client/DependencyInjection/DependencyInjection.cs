using YoutubeSummarizerWeb.Client.Services;

namespace YoutubeSummarizerWeb.Client.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClient(this IServiceCollection services, Uri apiBaseAddress)
        {
            services.AddScoped<AuthStateService>();
            services.AddTransient<CookieHandler>();
            services.AddTransient<AuthRetryHandler>();

            services.AddHttpClient("Auth", client =>
                client.BaseAddress = apiBaseAddress)
                .AddHttpMessageHandler<CookieHandler>();

            services.AddHttpClient("Api", client =>
                client.BaseAddress = apiBaseAddress)
                .AddHttpMessageHandler<CookieHandler>()
                .AddHttpMessageHandler<AuthRetryHandler>();

            services.AddScoped(sp =>
                new AuthService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Auth"),
                    sp.GetRequiredService<AuthStateService>()));

            services.AddScoped(sp =>
                new SubscriptionService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api")));

            services.AddScoped(sp =>
                new NotificationService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api")));

            services.AddScoped(sp =>
                new AdminService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api")));

            services.AddScoped<NotificationHubConnection>();

            return services;
        }
    }
}
