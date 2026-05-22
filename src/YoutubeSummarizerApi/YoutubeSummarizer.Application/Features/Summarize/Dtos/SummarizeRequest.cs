namespace YoutubeSummarizer.Application.Features.Summarize.Dtos
{
    public class SummarizeRequest
    {
        public string VideoUrl { get; set; } = string.Empty;
        public string? AdditionalInstructions { get; set; }
    }
}
