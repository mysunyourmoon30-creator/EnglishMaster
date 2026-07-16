using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGrammarModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GrammarTopics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CefrLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrammarRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrammarTopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    RuleText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ExplanationTh = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ExplanationEn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    StructurePattern = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CommonMistake = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CorrectUsageNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrammarRules_GrammarTopics_GrammarTopicId",
                        column: x => x.GrammarTopicId,
                        principalTable: "GrammarTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrammarExamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrammarRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExampleEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TranslationTh = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExplanationTh = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsCorrectExample = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarExamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrammarExamples_GrammarRules_GrammarRuleId",
                        column: x => x.GrammarRuleId,
                        principalTable: "GrammarRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrammarRuleWords",
                columns: table => new
                {
                    GrammarRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarRuleWords", x => new { x.GrammarRuleId, x.WordId });
                    table.ForeignKey(
                        name: "FK_GrammarRuleWords_GrammarRules_GrammarRuleId",
                        column: x => x.GrammarRuleId,
                        principalTable: "GrammarRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrammarRuleWords_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrammarExamples_GrammarRuleId",
                table: "GrammarExamples",
                column: "GrammarRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarExamples_IsActive",
                table: "GrammarExamples",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_GrammarTopicId",
                table: "GrammarRules",
                column: "GrammarTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_IsActive",
                table: "GrammarRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_Slug",
                table: "GrammarRules",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_Title",
                table: "GrammarRules",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRuleWords_WordId",
                table: "GrammarRuleWords",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTopics_CefrLevel",
                table: "GrammarTopics",
                column: "CefrLevel");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTopics_IsActive",
                table: "GrammarTopics",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTopics_Slug",
                table: "GrammarTopics",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTopics_Title",
                table: "GrammarTopics",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrammarExamples");

            migrationBuilder.DropTable(
                name: "GrammarRuleWords");

            migrationBuilder.DropTable(
                name: "GrammarRules");

            migrationBuilder.DropTable(
                name: "GrammarTopics");
        }
    }
}
