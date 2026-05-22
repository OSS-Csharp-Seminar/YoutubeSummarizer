using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeSummarizer.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration: IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(x => x.DomainUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(x => x.DomainUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
