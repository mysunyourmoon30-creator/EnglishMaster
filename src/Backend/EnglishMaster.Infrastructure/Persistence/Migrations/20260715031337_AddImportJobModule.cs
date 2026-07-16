using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddImportJobModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ValidatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    FailedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RolledBackAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    ValidRows = table.Column<int>(type: "int", nullable: false),
                    InvalidRows = table.Column<int>(type: "int", nullable: false),
                    ImportedRows = table.Column<int>(type: "int", nullable: false),
                    FailedRows = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportJobRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    RawDataJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    ParsedDataJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedEntityType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedEntityType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UpdatedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportJobRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportJobRows_ImportJobs_ImportJobId",
                        column: x => x.ImportJobId,
                        principalTable: "ImportJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportValidationErrors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportJobRowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportValidationErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportValidationErrors_ImportJobRows_ImportJobRowId",
                        column: x => x.ImportJobRowId,
                        principalTable: "ImportJobRows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobRows_ImportJobId",
                table: "ImportJobRows",
                column: "ImportJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobRows_RowNumber",
                table: "ImportJobRows",
                column: "RowNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobRows_Status",
                table: "ImportJobRows",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_Format",
                table: "ImportJobs",
                column: "Format");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_ImportType",
                table: "ImportJobs",
                column: "ImportType");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_RequestedAt",
                table: "ImportJobs",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_RequestedBy",
                table: "ImportJobs",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_Status",
                table: "ImportJobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ImportValidationErrors_ImportJobRowId",
                table: "ImportValidationErrors",
                column: "ImportJobRowId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportValidationErrors_Severity",
                table: "ImportValidationErrors",
                column: "Severity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportValidationErrors");

            migrationBuilder.DropTable(
                name: "ImportJobRows");

            migrationBuilder.DropTable(
                name: "ImportJobs");
        }
    }
}
