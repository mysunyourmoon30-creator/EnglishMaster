using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPronunciationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pronunciations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpaUk = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IpaUs = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThaiReading = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Syllables = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StressPattern = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MouthPosition = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TonguePosition = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CommonMistake = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PracticeNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AudioSlowMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AudioNormalMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MouthImageMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pronunciations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pronunciations_Media_AudioNormalMediaId",
                        column: x => x.AudioNormalMediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pronunciations_Media_AudioSlowMediaId",
                        column: x => x.AudioSlowMediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pronunciations_Media_MouthImageMediaId",
                        column: x => x.MouthImageMediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pronunciations_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinimalPairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PronunciationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PairWordText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PairIpa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PairThaiReading = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DifferenceNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AudioMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinimalPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinimalPairs_Media_AudioMediaId",
                        column: x => x.AudioMediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MinimalPairs_Pronunciations_PronunciationId",
                        column: x => x.PronunciationId,
                        principalTable: "Pronunciations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MinimalPairs_AudioMediaId",
                table: "MinimalPairs",
                column: "AudioMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_MinimalPairs_IsActive",
                table: "MinimalPairs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MinimalPairs_PairWordText",
                table: "MinimalPairs",
                column: "PairWordText");

            migrationBuilder.CreateIndex(
                name: "IX_MinimalPairs_PronunciationId",
                table: "MinimalPairs",
                column: "PronunciationId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_AudioNormalMediaId",
                table: "Pronunciations",
                column: "AudioNormalMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_AudioSlowMediaId",
                table: "Pronunciations",
                column: "AudioSlowMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_IpaUk",
                table: "Pronunciations",
                column: "IpaUk");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_IpaUs",
                table: "Pronunciations",
                column: "IpaUs");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_IsActive",
                table: "Pronunciations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_MouthImageMediaId",
                table: "Pronunciations",
                column: "MouthImageMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_WordId",
                table: "Pronunciations",
                column: "WordId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinimalPairs");

            migrationBuilder.DropTable(
                name: "Pronunciations");
        }
    }
}
