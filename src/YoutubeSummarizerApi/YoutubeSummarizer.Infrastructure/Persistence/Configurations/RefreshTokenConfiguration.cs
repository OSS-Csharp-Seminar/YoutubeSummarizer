using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(x => x.Token).IsUnique();

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.CreatedByIp).HasMaxLength(50);
            builder.Property(x => x.RevokedByIp).HasMaxLength(50);
            builder.Property(x => x.ReplacedByToken).HasMaxLength(200);

            builder.Ignore(x => x.IsExpired);
            builder.Ignore(x => x.IsRevoked);
            builder.Ignore(x => x.IsUsed);
            builder.Ignore(x => x.IsActive);
        }
    }
}
