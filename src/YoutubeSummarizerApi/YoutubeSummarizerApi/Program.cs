using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YoutubeSummarizer.Api.DependencyInjection;
using YoutubeSummarizer.Application.DependencyInjection;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;
using YoutubeSummarizer.Infrastructure.DependencyInjection;
using YoutubeSummarizer.Infrastructure.Security;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi();

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// Add JWT Authentication
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Extract JWT from HttpOnly cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("access_token", out var token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();
//using var scope = app.Services.CreateScope();
//var transcriptService = scope.ServiceProvider.GetRequiredService<IYoutubeTranscriptService>();
//var result = await transcriptService.GetTranscriptAsync(new GetYoutubeTranscriptRequest
//{
//    VideoUrl = "https://www.youtube.com/watch?v=rng_yUSwrgU"
//});
//Console.WriteLine($"VideoId: {result.VideoId}");
//for (int i = 0; i < 20; i++)
//{
//    Console.WriteLine($"Segments: {result.Transcript[i]}");
//}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
