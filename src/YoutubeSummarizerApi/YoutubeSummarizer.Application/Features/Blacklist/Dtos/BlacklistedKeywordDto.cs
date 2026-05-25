namespace YoutubeSummarizer.Application.Features.Blacklist.Dtos
{
    public class BlacklistedKeywordDto
    {
        public Guid Id { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}