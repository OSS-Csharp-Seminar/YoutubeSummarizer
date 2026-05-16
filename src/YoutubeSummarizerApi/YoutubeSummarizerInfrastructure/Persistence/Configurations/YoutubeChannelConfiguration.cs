using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Infrastructure.Persistence.Configurations
{
    public class YoutubeChannelConfiguration : IEntityTypeConfiguration<YoutubeChannel>
    {
        public void Configure(EntityTypeBuilder<YoutubeChannel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ChannelIdentifier).IsRequired().HasMaxLength(200);
            builder.HasIndex(x => x.ChannelIdentifier).IsUnique();
            builder.Property(x => x.ChannelUrl).IsRequired();
            builder.Property(x => x.YoutubeChannelId).HasMaxLength(100);
            builder.HasIndex(x => x.YoutubeChannelId);
        }
    }
}
