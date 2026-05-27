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
            builder.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(x => x.YoutubeChannelId).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.YoutubeChannelId).IsUnique();
            builder.Property(x => x.ChannelName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ChannelUrl).IsRequired();
        }
    }
}
