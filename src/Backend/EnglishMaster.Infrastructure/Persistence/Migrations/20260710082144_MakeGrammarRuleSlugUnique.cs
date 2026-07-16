using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeGrammarRuleSlugUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GrammarRules_Slug",
                table: "GrammarRules");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_Slug",
                table: "GrammarRules",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GrammarRules_Slug",
                table: "GrammarRules");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_Slug",
                table: "GrammarRules",
                column: "Slug");
        }
    }
}
