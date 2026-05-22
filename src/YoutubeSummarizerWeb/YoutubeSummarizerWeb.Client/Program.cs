using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using YoutubeSummarizerWeb.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<AuthStateService>();
builder.Services.AddTransient<CookieHandler>();
builder.Services.AddTransient<AuthRetryHandler>();

builder.Services.AddHttpClient("Auth", client =>
    client.BaseAddress = new Uri("http://localhost:5017"))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient("Api", client =>
    client.BaseAddress = new Uri("http://localhost:5017"))
    .AddHttpMessageHandler<CookieHandler>()
    .AddHttpMessageHandler<AuthRetryHandler>();

builder.Services.AddScoped(sp =>
    new AuthService(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("Auth"),
        sp.GetRequiredService<AuthStateService>()));

builder.Services.AddScoped(sp =>
    new SubscriptionService(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api")));

await builder.Build().RunAsync();
