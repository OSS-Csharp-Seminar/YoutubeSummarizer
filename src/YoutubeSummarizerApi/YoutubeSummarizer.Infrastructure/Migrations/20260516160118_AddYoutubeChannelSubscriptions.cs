using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeSummarizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYoutubeChannelSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserYoutubeChannelSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    YoutubeChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SummarizationStyle = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserYoutubeChannelSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YoutubeChannels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelIdentifier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChannelUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubeChannels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserYoutubeChannelSubscriptions_UserId_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions",
                columns: new[] { "UserId", "YoutubeChannelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannels_ChannelIdentifier",
                table: "YoutubeChannels",
                column: "ChannelIdentifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserYoutubeChannelSubscriptions");

            migrationBuilder.DropTable(
                name: "YoutubeChannels");
        }
    }
}
