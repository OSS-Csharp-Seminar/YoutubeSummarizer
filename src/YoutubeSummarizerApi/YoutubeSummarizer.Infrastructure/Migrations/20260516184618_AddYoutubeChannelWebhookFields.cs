using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeSummarizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYoutubeChannelWebhookFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWebhookSubscribed",
                table: "YoutubeChannels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWebhookSubscriptionAttemptUtc",
                table: "YoutubeChannels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WebhookExpiresAtUtc",
                table: "YoutubeChannels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeChannelId",
                table: "YoutubeChannels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannels_YoutubeChannelId",
                table: "YoutubeChannels",
                column: "YoutubeChannelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_YoutubeChannels_YoutubeChannelId",
                table: "YoutubeChannels");

            migrationBuilder.DropColumn(
                name: "IsWebhookSubscribed",
                table: "YoutubeChannels");

            migrationBuilder.DropColumn(
                name: "LastWebhookSubscriptionAttemptUtc",
                table: "YoutubeChannels");

            migrationBuilder.DropColumn(
                name: "WebhookExpiresAtUtc",
                table: "YoutubeChannels");

            migrationBuilder.DropColumn(
                name: "YoutubeChannelId",
                table: "YoutubeChannels");
        }
    }
}
