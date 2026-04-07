using YoutubeSummarizer.Application.DependencyInjection;
using YoutubeSummarizer.Infrastructure.DependencyInjection;
using YoutubeSummarizer.Api.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
