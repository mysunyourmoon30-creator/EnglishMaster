using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AudioMediaId",
                table: "Words",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImageMediaId",
                table: "Words",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PublicUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_AudioMediaId",
                table: "Words",
                column: "AudioMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_ImageMediaId",
                table: "Words",
                column: "ImageMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_ContentType",
                table: "Media",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_Media_CreatedAt",
                table: "Media",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Media_FileName",
                table: "Media",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_Media_IsActive",
                table: "Media",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Media_MediaType",
                table: "Media",
                column: "MediaType");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Media_AudioMediaId",
                table: "Words",
                column: "AudioMediaId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Media_ImageMediaId",
                table: "Words",
                column: "ImageMediaId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Media_AudioMediaId",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_Media_ImageMediaId",
                table: "Words");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Words_AudioMediaId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_ImageMediaId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "AudioMediaId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ImageMediaId",
                table: "Words");
        }
    }
}
