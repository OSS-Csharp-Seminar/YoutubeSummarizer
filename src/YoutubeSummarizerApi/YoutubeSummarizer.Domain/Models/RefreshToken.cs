using System;

namespace YoutubeSummarizer.Domain.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public DateTime? UsedAtUtc { get; set; }
        public string? ReplacedByToken { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public string? RevokedByIp { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsRevoked => RevokedAtUtc.HasValue;
        public bool IsUsed => UsedAtUtc.HasValue;
        public bool IsActive => !IsExpired && !IsRevoked && !IsUsed;
    }
}
