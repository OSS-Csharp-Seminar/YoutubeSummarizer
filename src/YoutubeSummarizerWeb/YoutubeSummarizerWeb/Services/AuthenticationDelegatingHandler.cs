using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace YoutubeSummarizerWeb.Services;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorage;
    private readonly IAuthRefreshService _authRefresh;
    private readonly AuthStateService _authState;
    private readonly NavigationManager _navigation;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AuthenticationDelegatingHandler(
        ITokenStorageService tokenStorage,
        IAuthRefreshService authRefresh,
        AuthStateService authState,
        NavigationManager navigation)
    {
        _tokenStorage = tokenStorage;
        _authRefresh = authRefresh;
        _authState = authState;
        _navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _tokenStorage.GetAccessTokenAsync();
        if (accessToken != null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (request.Content != null)
            await request.Content.LoadIntoBufferAsync();

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (!await _authRefresh.TryRefreshTokenAsync())
            {
                _authState.ClearUser();
                _navigation.NavigateTo("/login");
                return response;
            }
        }
        finally
        {
            _semaphore.Release();
        }

        var retry = CloneRequest(request);
        var newToken = await _tokenStorage.GetAccessTokenAsync();
        if (newToken != null)
            retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

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
