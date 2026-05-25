using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using YoutubeSummarizer.Application.Features.AI.PromptTemplates;
using YoutubeSummarizer.Application.Features.Summarize.Interfaces;
using YoutubeSummarizer.Application.Features.Summarize.Services;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeChannels.Services;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Services;
using YoutubeSummarizer.Application.Features.Admin.Interfaces;
using YoutubeSummarizer.Application.Features.Admin.Services;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;
using YoutubeSummarizer.Application.Features.Notifications.Services;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Services;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Application.Mappings;
using YoutubeSummarizer.Application.Services;
using YoutubeSummarizer.Application.Validators;

namespace YoutubeSummarizer.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<UserMapper>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IYoutubeTranscriptService, YoutubeTranscriptService>();
            services.AddScoped<ISummarizeService, SummarizeService>();
            services.AddScoped<ITranscriptPromptTemplateProvider, TranscriptPromptTemplateProvider>();
            services.AddScoped<IYoutubeChannelSubscriptionService, YoutubeChannelSubscriptionService>();
            services.AddScoped<IYoutubeWebhookSubscriptionService, YoutubeWebhookSubscriptionService>();
            services.AddScoped<IYoutubeWebhookVerificationService, YoutubeWebhookVerificationService>();
            services.AddScoped<IYoutubeWebhookNotificationService, YoutubeWebhookNotificationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>();

            return services;
        }
    }
}
