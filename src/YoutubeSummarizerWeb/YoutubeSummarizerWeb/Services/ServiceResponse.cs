namespace YoutubeSummarizerWeb.Services;

public class ServiceResponse<T>
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}
