using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YoutubeSummarizer.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
    {
        public static readonly Guid AdminRoleId = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001");
        public static readonly Guid UserRoleId = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002");

        public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
        {
            builder.HasData(
                new IdentityRole<Guid>
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "00000000-0000-0000-0000-000000000001"
                },
                new IdentityRole<Guid>
                {
                    Id = UserRoleId,
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "00000000-0000-0000-0000-000000000002"
                }
            );
        }
    }
}
