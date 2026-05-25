using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeSummarizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBlacklistedKeywords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlacklistedKeywords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlacklistedKeywords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedKeywords_UserId_Keyword",
                table: "BlacklistedKeywords",
                columns: new[] { "UserId", "Keyword" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlacklistedKeywords");
        }
    }
}
