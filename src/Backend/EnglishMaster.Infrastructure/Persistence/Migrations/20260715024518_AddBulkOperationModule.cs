using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBulkOperationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BulkOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    SucceededItems = table.Column<int>(type: "int", nullable: false),
                    FailedItems = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TagIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ExportFormat = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulkOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BulkOperationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BulkOperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulkOperationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BulkOperationItems_BulkOperations_BulkOperationId",
                        column: x => x.BulkOperationId,
                        principalTable: "BulkOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BulkOperationItems_BulkOperationId",
                table: "BulkOperationItems",
                column: "BulkOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOperationItems_ContentId",
                table: "BulkOperationItems",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOperationItems_Status",
                table: "BulkOperationItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOperations_ContentType",
                table: "BulkOperations",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOperations_OperationType",
                table: "BulkOperations",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOperations_RequestedAt",
                table: "BulkOperations",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOperations_RequestedBy",
                table: "BulkOperations",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BulkOperations_Status",
                table: "BulkOperations",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BulkOperationItems");

            migrationBuilder.DropTable(
                name: "BulkOperations");
        }
    }
}
