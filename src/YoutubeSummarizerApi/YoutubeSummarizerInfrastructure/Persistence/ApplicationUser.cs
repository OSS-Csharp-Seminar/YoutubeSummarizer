using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Infrastructure.Persistence
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public Guid DomainUserId { get; set; }
        public virtual User DomainUser { get; set; } 
    }
}
