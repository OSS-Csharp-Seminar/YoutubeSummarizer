using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeSummarizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserYoutubeChannelSubscriptions_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions",
                column: "YoutubeChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlacklistEntries_DomainUsers_UserId",
                table: "BlacklistEntries",
                column: "UserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_DomainUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_DomainUsers_UserId",
                table: "UserNotifications",
                column: "UserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserYoutubeChannelSubscriptions_DomainUsers_UserId",
                table: "UserYoutubeChannelSubscriptions",
                column: "UserId",
                principalTable: "DomainUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserYoutubeChannelSubscriptions_YoutubeChannels_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions",
                column: "YoutubeChannelId",
                principalTable: "YoutubeChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlacklistEntries_DomainUsers_UserId",
                table: "BlacklistEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_DomainUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_DomainUsers_UserId",
                table: "UserNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_UserYoutubeChannelSubscriptions_DomainUsers_UserId",
                table: "UserYoutubeChannelSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserYoutubeChannelSubscriptions_YoutubeChannels_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserYoutubeChannelSubscriptions_YoutubeChannelId",
                table: "UserYoutubeChannelSubscriptions");
        }
    }
}
