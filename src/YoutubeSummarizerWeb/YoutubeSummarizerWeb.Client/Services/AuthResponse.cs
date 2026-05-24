namespace YoutubeSummarizerWeb.Client.Services;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public List<string> Roles { get; set; } = new();
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public AuthResponse? User { get; set; }
}
