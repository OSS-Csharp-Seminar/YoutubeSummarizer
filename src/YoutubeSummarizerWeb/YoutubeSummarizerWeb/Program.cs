using Microsoft.AspNetCore.Components;
using YoutubeSummarizerWeb.Components;
using YoutubeSummarizerWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Auth state & token storage (circuit-scoped)
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddScoped<IAuthRefreshService, AuthRefreshService>();

// AuthService — unauthenticated endpoints (login, register, logout)
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5017");
});

// RefreshClient — used by AuthRefreshService, no auth handler (avoids circular refresh)
builder.Services.AddHttpClient("RefreshClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5017");
});

// ApiClient — bare inner handler used by AuthenticationDelegatingHandler
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5017");
});

// SubscriptionService — Scoped so AuthenticationDelegatingHandler gets circuit-scoped services.
// Cannot use AddHttpMessageHandler<T>() here because HttpClientFactory resolves handlers
// in its own separate scope, causing ITokenStorageService to always be empty.
// Instead, the handler is instantiated inline with services from the current circuit scope.
builder.Services.AddScoped<SubscriptionService>(sp =>
{
    var handlerFactory = sp.GetRequiredService<IHttpMessageHandlerFactory>();
    var tokenStorage = sp.GetRequiredService<ITokenStorageService>();
    var authRefresh = sp.GetRequiredService<IAuthRefreshService>();
    var authState = sp.GetRequiredService<AuthStateService>();
    var navigation = sp.GetRequiredService<NavigationManager>();

    var handler = new AuthenticationDelegatingHandler(tokenStorage, authRefresh, authState, navigation)
    {
        InnerHandler = handlerFactory.CreateHandler("ApiClient")
    };

    var httpClient = new HttpClient(handler, disposeHandler: true)
    {
        BaseAddress = new Uri("http://localhost:5017")
    };

    return new SubscriptionService(httpClient);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(YoutubeSummarizerWeb.Client._Imports).Assembly);

app.Run();
