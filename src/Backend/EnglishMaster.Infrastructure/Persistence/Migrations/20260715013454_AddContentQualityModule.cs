using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContentQualityModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentQualityChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CheckedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CheckedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentQualityChecks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentQualityRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentQualityRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentQualityFindings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentQualityCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentQualityFindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentQualityFindings_ContentQualityChecks_ContentQualityCheckId",
                        column: x => x.ContentQualityCheckId,
                        principalTable: "ContentQualityChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityChecks_CheckedAt",
                table: "ContentQualityChecks",
                column: "CheckedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityChecks_ContentId",
                table: "ContentQualityChecks",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityChecks_ContentType",
                table: "ContentQualityChecks",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityChecks_Status",
                table: "ContentQualityChecks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityFindings_ContentQualityCheckId",
                table: "ContentQualityFindings",
                column: "ContentQualityCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityFindings_IsResolved",
                table: "ContentQualityFindings",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityFindings_RuleCode",
                table: "ContentQualityFindings",
                column: "RuleCode");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityFindings_Severity",
                table: "ContentQualityFindings",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityRules_Code",
                table: "ContentQualityRules",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityRules_ContentType",
                table: "ContentQualityRules",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualityRules_Severity",
                table: "ContentQualityRules",
                column: "Severity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentQualityFindings");

            migrationBuilder.DropTable(
                name: "ContentQualityRules");

            migrationBuilder.DropTable(
                name: "ContentQualityChecks");
        }
    }
}
