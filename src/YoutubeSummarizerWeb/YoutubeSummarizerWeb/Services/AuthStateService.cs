namespace YoutubeSummarizerWeb.Services;

public class AuthStateService
{
    public AuthResponse? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;
    public string? DisplayName => CurrentUser?.Email.Split('@')[0];

    public event Action? OnChange;

    public void SetUser(AuthResponse user)
    {
        CurrentUser = user;
        OnChange?.Invoke();
    }

    public void ClearUser()
    {
        CurrentUser = null;
        OnChange?.Invoke();
    }
}
