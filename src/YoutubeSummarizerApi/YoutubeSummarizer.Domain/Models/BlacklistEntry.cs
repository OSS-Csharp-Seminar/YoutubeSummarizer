using System;

namespace YoutubeSummarizer.Domain.Models
{
    public class BlacklistEntry
    {
        public Guid UserId { get; set; }
        public string Keyword { get; set; } = string.Empty;

        public User User { get; set; } = null!;
    }
}
