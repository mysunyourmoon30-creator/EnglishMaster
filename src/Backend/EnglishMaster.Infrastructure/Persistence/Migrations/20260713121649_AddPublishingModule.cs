using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublishJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OutputFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    OutputPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TemplateContent = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishedArtifacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublishJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PublicUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedArtifacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishedArtifacts_PublishJobs_PublishJobId",
                        column: x => x.PublishJobId,
                        principalTable: "PublishJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedArtifacts_Format",
                table: "PublishedArtifacts",
                column: "Format");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedArtifacts_PublishJobId",
                table: "PublishedArtifacts",
                column: "PublishJobId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedArtifacts_SourceId",
                table: "PublishedArtifacts",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedArtifacts_SourceType",
                table: "PublishedArtifacts",
                column: "SourceType");

            migrationBuilder.CreateIndex(
                name: "IX_PublishJobs_CreatedAt",
                table: "PublishJobs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PublishJobs_Format",
                table: "PublishJobs",
                column: "Format");

            migrationBuilder.CreateIndex(
                name: "IX_PublishJobs_SourceId",
                table: "PublishJobs",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishJobs_SourceType",
                table: "PublishJobs",
                column: "SourceType");

            migrationBuilder.CreateIndex(
                name: "IX_PublishJobs_Status",
                table: "PublishJobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PublishTemplates_Format",
                table: "PublishTemplates",
                column: "Format");

            migrationBuilder.CreateIndex(
                name: "IX_PublishTemplates_IsDefault",
                table: "PublishTemplates",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_PublishTemplates_Slug",
                table: "PublishTemplates",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedArtifacts");

            migrationBuilder.DropTable(
                name: "PublishTemplates");

            migrationBuilder.DropTable(
                name: "PublishJobs");
        }
    }
}
