using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace YoutubeSummarizerWeb.Client.Services;

public class AuthRetryHandler : DelegatingHandler
{
    private readonly AuthStateService _authState;
    private readonly NavigationManager _navigation;

    public AuthRetryHandler(AuthStateService authState, NavigationManager navigation)
    {
        _authState = authState;
        _navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content != null)
            await request.Content.LoadIntoBufferAsync();

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        var baseUri = new Uri(request.RequestUri!.GetLeftPart(UriPartial.Authority));
        var refreshRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, "/api/auth/refresh-token"));
        refreshRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        var refreshResponse = await base.SendAsync(refreshRequest, cancellationToken);

        if (!refreshResponse.IsSuccessStatusCode)
        {
            _authState.ClearUser();
            _navigation.NavigateTo("/login", forceLoad: true);
            return response;
        }

        var retry = CloneRequest(request);
        retry.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return await base.SendAsync(retry, cancellationToken);
    }

    private static HttpRequestMessage CloneRequest(HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri);
        foreach (var header in req.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        if (req.Content != null)
            clone.Content = req.Content;
        return clone;
    }
}
