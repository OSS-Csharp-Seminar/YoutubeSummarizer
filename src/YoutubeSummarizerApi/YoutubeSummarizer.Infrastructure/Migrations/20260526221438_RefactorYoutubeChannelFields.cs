using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeSummarizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorYoutubeChannelFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_YoutubeChannels_ChannelIdentifier",
                table: "YoutubeChannels");

            migrationBuilder.DropIndex(
                name: "IX_YoutubeChannels_YoutubeChannelId",
                table: "YoutubeChannels");

            migrationBuilder.RenameColumn(
                name: "ChannelIdentifier",
                table: "YoutubeChannels",
                newName: "ChannelName");

            migrationBuilder.AlterColumn<string>(
                name: "YoutubeChannelId",
                table: "YoutubeChannels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannels_YoutubeChannelId",
                table: "YoutubeChannels",
                column: "YoutubeChannelId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_YoutubeChannels_YoutubeChannelId",
                table: "YoutubeChannels");

            migrationBuilder.RenameColumn(
                name: "ChannelName",
                table: "YoutubeChannels",
                newName: "ChannelIdentifier");

            migrationBuilder.AlterColumn<string>(
                name: "YoutubeChannelId",
                table: "YoutubeChannels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannels_ChannelIdentifier",
                table: "YoutubeChannels",
                column: "ChannelIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannels_YoutubeChannelId",
                table: "YoutubeChannels",
                column: "YoutubeChannelId");
        }
    }
}
