using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.SignalR.Client;

namespace YoutubeSummarizerWeb.Client.Services;

public class NotificationHubConnection : IAsyncDisposable
{
    private HubConnection? _hubConnection;

    public event Action? OnNotificationReceived;

    public async Task StartAsync()
    {
        if (_hubConnection is not null) return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5017/hubs/notifications", options =>
            {
                options.HttpMessageHandlerFactory = innerHandler =>
                    new HubCookieHandler(innerHandler);
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On("NotificationReceived", () =>
        {
            OnNotificationReceived?.Invoke();
        });

        try
        {
            await _hubConnection.StartAsync();
        }
        catch
        {
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }

    private class HubCookieHandler : DelegatingHandler
    {
        public HubCookieHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
