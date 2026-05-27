using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeSummarizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSubscriptionChannelFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserYoutubeChannelSubscriptions_YoutubeChannels_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions");

            migrationBuilder.RenameColumn(
                name: "YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions",
                newName: "ChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_UserYoutubeChannelSubscriptions_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions",
                newName: "IX_UserYoutubeChannelSubscriptions_ChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_UserYoutubeChannelSubscriptions_UserId_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions",
                newName: "IX_UserYoutubeChannelSubscriptions_UserId_ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserYoutubeChannelSubscriptions_YoutubeChannels_ChannelId",
                table: "UserYoutubeChannelSubscriptions",
                column: "ChannelId",
                principalTable: "YoutubeChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserYoutubeChannelSubscriptions_YoutubeChannels_ChannelId",
                table: "UserYoutubeChannelSubscriptions");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "UserYoutubeChannelSubscriptions",
                newName: "YoutubeChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_UserYoutubeChannelSubscriptions_UserId_ChannelId",
                table: "UserYoutubeChannelSubscriptions",
                newName: "IX_UserYoutubeChannelSubscriptions_UserId_YoutubeChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_UserYoutubeChannelSubscriptions_ChannelId",
                table: "UserYoutubeChannelSubscriptions",
                newName: "IX_UserYoutubeChannelSubscriptions_YoutubeChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserYoutubeChannelSubscriptions_YoutubeChannels_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions",
                column: "YoutubeChannelId",
                principalTable: "YoutubeChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
