using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Features.Summarize;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Infrastructure.ExternalServices.Ai;
using YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeTranscript;
using YoutubeSummarizer.Infrastructure.Persistence;
using YoutubeSummarizer.Infrastructure.Persistence.DbContext;
using YoutubeSummarizer.Infrastructure.Persistence.Repositorys;
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
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Add Identity
           services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IApiSettingsRepository, ApiSettingsRepository>();
            services.AddScoped<IAiClient, AiClient>();
            services.Configure<AiSettings>(configuration.GetSection("AiSettings"));

            services.AddHttpClient<IYoutubeTranscriptClient, YoutubeTranscriptClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ExternalApis:YoutubeTranscript:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}
