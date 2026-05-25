namespace YoutubeSummarizer.Domain.Models
{
    public class BlacklistedKeyword
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Keyword { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}