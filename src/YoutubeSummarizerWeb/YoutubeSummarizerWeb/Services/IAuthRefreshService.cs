namespace YoutubeSummarizerWeb.Services;

public interface IAuthRefreshService
{
    Task<bool> TryRefreshTokenAsync();
}
