using Microsoft.AspNetCore.Identity;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Infrastructure.Persistence
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid DomainUserId { get; set; }
        public virtual User? DomainUser { get; set; }
    }
}

