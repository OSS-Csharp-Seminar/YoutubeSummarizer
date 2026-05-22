using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YoutubeSummarizer.Infrastructure.Persistence.Entities;

namespace YoutubeSummarizer.Infrastructure.Persistence.Configurations
{
    public class ApiSettingConfiguration : IEntityTypeConfiguration<ApiSetting>
    {
        public void Configure(EntityTypeBuilder<ApiSetting> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ApiKey)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(x => x.ProviderName);
        }
    }
}
