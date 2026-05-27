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
            builder.HasIndex(x => new { x.UserId, x.ChannelId }).IsUnique();
            builder.Property(x => x.SummarizationStyle).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.YoutubeChannel)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(x => x.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
