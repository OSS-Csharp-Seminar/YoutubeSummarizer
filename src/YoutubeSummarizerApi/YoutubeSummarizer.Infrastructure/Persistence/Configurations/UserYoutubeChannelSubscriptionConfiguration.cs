using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeSummarizer.Domain.Models;

namespace YoutubeSummarizer.Infrastructure.Persistence.Configurations
{
    public class UserYoutubeChannelSubscriptionConfiguration : IEntityTypeConfiguration<UserYoutubeChannelSubscription>
    {
        public void Configure(EntityTypeBuilder<UserYoutubeChannelSubscription> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => new { x.UserId, x.YoutubeChannelId }).IsUnique();
            builder.Property(x => x.SummarizationStyle).IsRequired();
        }
    }
}
