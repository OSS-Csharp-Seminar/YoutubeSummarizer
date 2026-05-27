namespace YoutubeSummarizer.Application.Features.Summarize
{
    public class AiSettings
    {
        public string BasePrompt { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string FallbackBaseUrl { get; set; } = string.Empty;
        public string FallbackModel { get; set; } = string.Empty;
    }
}
