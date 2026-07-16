using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContentRevisionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentRevisionRestoreRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentRevisionRestoreRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    DiffJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentRevisions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisionRestoreRequests_ContentRevisionId",
                table: "ContentRevisionRestoreRequests",
                column: "ContentRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisionRestoreRequests_RequestedAt",
                table: "ContentRevisionRestoreRequests",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisionRestoreRequests_RequestedBy",
                table: "ContentRevisionRestoreRequests",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisionRestoreRequests_Status",
                table: "ContentRevisionRestoreRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_ChangedAt",
                table: "ContentRevisions",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_ChangedBy",
                table: "ContentRevisions",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_ContentType_ContentId",
                table: "ContentRevisions",
                columns: new[] { "ContentType", "ContentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_EventType",
                table: "ContentRevisions",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_ContentRevisions_RevisionNumber",
                table: "ContentRevisions",
                column: "RevisionNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentRevisionRestoreRequests");

            migrationBuilder.DropTable(
                name: "ContentRevisions");
        }
    }
}
