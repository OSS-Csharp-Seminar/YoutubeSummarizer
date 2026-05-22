namespace YoutubeSummarizer.Application.Common.Models
{
    public class ServiceResponse<T>
    {
        public bool Status { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; } = default;

        public static ServiceResponse<T> Success(T data, string message) => new()
        {
            Status = true,
            Message = message,
            Data = data
        };

        public static ServiceResponse<T> Failure(string message) => new()
        {
            Status = false,
            Message = message,
            Data = default
        };
    }
}
