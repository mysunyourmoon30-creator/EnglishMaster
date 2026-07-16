using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIssuedCertificateModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssuedCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TemplateCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RenderedBody = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    IssuedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedCertificates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssuedCertificates_CourseId",
                table: "IssuedCertificates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedCertificates_UserId_CourseId",
                table: "IssuedCertificates",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssuedCertificates_UserId_IssuedAt",
                table: "IssuedCertificates",
                columns: new[] { "UserId", "IssuedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_IssuedCertificates_VerificationCode",
                table: "IssuedCertificates",
                column: "VerificationCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssuedCertificates");
        }
    }
}
