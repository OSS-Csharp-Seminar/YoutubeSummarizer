using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using YoutubeSummarizer.Application.Features.Summarize.Interfaces;
using YoutubeSummarizer.Application.Features.Summarize.Services;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Services;
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
            services.AddValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>();

            return services;
        }
    }
}
