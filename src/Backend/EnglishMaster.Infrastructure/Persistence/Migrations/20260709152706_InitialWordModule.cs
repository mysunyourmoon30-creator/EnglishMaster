using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialWordModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    IpaUk = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IpaUs = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThaiReading = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeaningTh = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MeaningEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PartOfSpeech = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CefrLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExampleEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExampleTh = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_CefrLevel",
                table: "Words",
                column: "CefrLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Words_IsActive",
                table: "Words",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Words_Slug",
                table: "Words",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Words_Text",
                table: "Words",
                column: "Text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Words");
        }
    }
}
