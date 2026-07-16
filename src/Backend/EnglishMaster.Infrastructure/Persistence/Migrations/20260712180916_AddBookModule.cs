using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Subtitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CefrLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CoverMediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuthorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Edition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstimatedPages = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Books_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Books_Media_CoverMediaId",
                        column: x => x.CoverMediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BookChapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ContentMarkdown = table.Column<string>(type: "nvarchar(max)", maxLength: 12000, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookChapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookChapters_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookChapterLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookChapterLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookChapterLessons_BookChapters_BookChapterId",
                        column: x => x.BookChapterId,
                        principalTable: "BookChapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookChapterLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookChapterLessons_BookChapterId",
                table: "BookChapterLessons",
                column: "BookChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_BookChapterLessons_BookChapterId_LessonId",
                table: "BookChapterLessons",
                columns: new[] { "BookChapterId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookChapterLessons_LessonId",
                table: "BookChapterLessons",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_BookChapterLessons_SortOrder",
                table: "BookChapterLessons",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_BookChapters_BookId",
                table: "BookChapters",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookChapters_IsActive",
                table: "BookChapters",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BookChapters_SortOrder",
                table: "BookChapters",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CefrLevel",
                table: "Books",
                column: "CefrLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CourseId",
                table: "Books",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CoverMediaId",
                table: "Books",
                column: "CoverMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_IsActive",
                table: "Books",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Books_IsPublished",
                table: "Books",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Slug",
                table: "Books",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookChapterLessons");

            migrationBuilder.DropTable(
                name: "BookChapters");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
