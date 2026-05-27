using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Infrastructure.Persistence.Configurations
{
    public class BlacklistEntryConfiguration : IEntityTypeConfiguration<BlacklistEntry>
    {
        public void Configure(EntityTypeBuilder<BlacklistEntry> builder)
        {
            builder.HasKey(x => new { x.UserId, x.Keyword });

            builder.Property(x => x.Keyword)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(x => x.User)
                .WithMany(u => u.BlacklistEntries)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
