using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Infrastructure.Persistence.Configurations
{
    public class BlacklistedKeywordConfiguration : IEntityTypeConfiguration<BlacklistedKeyword>
    {
        public void Configure(EntityTypeBuilder<BlacklistedKeyword> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Keyword)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(x => new { x.UserId, x.Keyword }).IsUnique();
        }
    }
}