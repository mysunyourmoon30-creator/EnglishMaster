using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressMotivationPracticeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AchievementType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TargetValue = table.Column<int>(type: "int", nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgressPercent = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    LastAccessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookProgress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgressPercent = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    LastAccessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseProgress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyStudyPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TargetMinutes = table.Column<int>(type: "int", nullable: false),
                    CompletedMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    CompletedItems = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyStudyPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TargetValue = table.Column<int>(type: "int", nullable: false),
                    CurrentValue = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TargetDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LessonProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgressPercent = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    LastAccessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonProgress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PracticeItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PracticeType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DueAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastPracticedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NextReviewAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    CorrectCount = table.Column<int>(type: "int", nullable: false),
                    IncorrectCount = table.Column<int>(type: "int", nullable: false),
                    CurrentIntervalDays = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PracticeSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    CompletedItems = table.Column<int>(type: "int", nullable: false),
                    CorrectItems = table.Column<int>(type: "int", nullable: false),
                    IncorrectItems = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false),
                    AttemptedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentAchievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AchievementDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ProgressValue = table.Column<int>(type: "int", nullable: false),
                    EarnedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAchievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentCefrLevel = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentStreaks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStreakDays = table.Column<int>(type: "int", nullable: false),
                    LongestStreakDays = table.Column<int>(type: "int", nullable: false),
                    LastActivityDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StreakStartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentStreaks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyLearningReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeekStartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    WeekEndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GeneratedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalStudyMinutes = table.Column<int>(type: "int", nullable: false),
                    ActiveStudyDays = table.Column<int>(type: "int", nullable: false),
                    CompletedDailyPlans = table.Column<int>(type: "int", nullable: false),
                    LessonsStarted = table.Column<int>(type: "int", nullable: false),
                    LessonsCompleted = table.Column<int>(type: "int", nullable: false),
                    CoursesStarted = table.Column<int>(type: "int", nullable: false),
                    CoursesCompleted = table.Column<int>(type: "int", nullable: false),
                    BooksStarted = table.Column<int>(type: "int", nullable: false),
                    BooksCompleted = table.Column<int>(type: "int", nullable: false),
                    PracticeSessionsCompleted = table.Column<int>(type: "int", nullable: false),
                    PracticeItemsCompleted = table.Column<int>(type: "int", nullable: false),
                    QuizAttempts = table.Column<int>(type: "int", nullable: false),
                    QuizzesPassed = table.Column<int>(type: "int", nullable: false),
                    AverageQuizScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GoalsCompleted = table.Column<int>(type: "int", nullable: false),
                    AchievementsEarned = table.Column<int>(type: "int", nullable: false),
                    CurrentStreakDays = table.Column<int>(type: "int", nullable: false),
                    LongestStreakDays = table.Column<int>(type: "int", nullable: false),
                    SummaryText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyLearningReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyStudyPlanItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyStudyPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstimatedMinutes = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyStudyPlanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyStudyPlanItems_DailyStudyPlans_DailyStudyPlanId",
                        column: x => x.DailyStudyPlanId,
                        principalTable: "DailyStudyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticeSessionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PracticeSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PracticeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PracticeType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PromptText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UserAnswer = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    PracticedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeSessionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PracticeSessionItems_PracticeSessions_PracticeSessionId",
                        column: x => x.PracticeSessionId,
                        principalTable: "PracticeSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyLearningReportInsights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeeklyLearningReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsightType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyLearningReportInsights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyLearningReportInsights_WeeklyLearningReports_WeeklyLearningReportId",
                        column: x => x.WeeklyLearningReportId,
                        principalTable: "WeeklyLearningReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementDefinitions_AchievementType",
                table: "AchievementDefinitions",
                column: "AchievementType");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementDefinitions_Code",
                table: "AchievementDefinitions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AchievementDefinitions_IsActive",
                table: "AchievementDefinitions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BookProgress_UserId_BookId",
                table: "BookProgress",
                columns: new[] { "UserId", "BookId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookProgress_UserId_Status_LastAccessedAt",
                table: "BookProgress",
                columns: new[] { "UserId", "Status", "LastAccessedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_UserId_CourseId",
                table: "CourseProgress",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_UserId_Status_LastAccessedAt",
                table: "CourseProgress",
                columns: new[] { "UserId", "Status", "LastAccessedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyStudyPlanItems_DailyStudyPlanId_SortOrder",
                table: "DailyStudyPlanItems",
                columns: new[] { "DailyStudyPlanId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyStudyPlans_StudentProfileId_PlanDate",
                table: "DailyStudyPlans",
                columns: new[] { "StudentProfileId", "PlanDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyStudyPlans_StudentProfileId_Status",
                table: "DailyStudyPlans",
                columns: new[] { "StudentProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningGoals_StudentProfileId_GoalType",
                table: "LearningGoals",
                columns: new[] { "StudentProfileId", "GoalType" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningGoals_StudentProfileId_Status",
                table: "LearningGoals",
                columns: new[] { "StudentProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_UserId_LessonId",
                table: "LessonProgress",
                columns: new[] { "UserId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_UserId_Status_LastAccessedAt",
                table: "LessonProgress",
                columns: new[] { "UserId", "Status", "LastAccessedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeItems_StudentProfileId_ContentType_ContentId_PracticeType",
                table: "PracticeItems",
                columns: new[] { "StudentProfileId", "ContentType", "ContentId", "PracticeType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PracticeItems_StudentProfileId_Status_DueAt",
                table: "PracticeItems",
                columns: new[] { "StudentProfileId", "Status", "DueAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessionItems_PracticeItemId",
                table: "PracticeSessionItems",
                column: "PracticeItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessionItems_PracticeSessionId",
                table: "PracticeSessionItems",
                column: "PracticeSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessions_StudentProfileId_StartedAt",
                table: "PracticeSessions",
                columns: new[] { "StudentProfileId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSessions_StudentProfileId_Status",
                table: "PracticeSessions",
                columns: new[] { "StudentProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId_Passed",
                table: "QuizAttempts",
                columns: new[] { "UserId", "Passed" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId_QuizId_AttemptedAt",
                table: "QuizAttempts",
                columns: new[] { "UserId", "QuizId", "AttemptedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAchievements_EarnedAt",
                table: "StudentAchievements",
                column: "EarnedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAchievements_Status",
                table: "StudentAchievements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAchievements_StudentProfileId",
                table: "StudentAchievements",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAchievements_StudentProfileId_AchievementDefinitionId",
                table: "StudentAchievements",
                columns: new[] { "StudentProfileId", "AchievementDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_UserId",
                table: "StudentProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentStreaks_StudentProfileId",
                table: "StudentStreaks",
                column: "StudentProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyLearningReportInsights_WeeklyLearningReportId_SortOrder",
                table: "WeeklyLearningReportInsights",
                columns: new[] { "WeeklyLearningReportId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyLearningReports_StudentProfileId_GeneratedAt",
                table: "WeeklyLearningReports",
                columns: new[] { "StudentProfileId", "GeneratedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyLearningReports_StudentProfileId_WeekStartDate",
                table: "WeeklyLearningReports",
                columns: new[] { "StudentProfileId", "WeekStartDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementDefinitions");

            migrationBuilder.DropTable(
                name: "BookProgress");

            migrationBuilder.DropTable(
                name: "CourseProgress");

            migrationBuilder.DropTable(
                name: "DailyStudyPlanItems");

            migrationBuilder.DropTable(
                name: "LearningGoals");

            migrationBuilder.DropTable(
                name: "LessonProgress");

            migrationBuilder.DropTable(
                name: "PracticeItems");

            migrationBuilder.DropTable(
                name: "PracticeSessionItems");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropTable(
                name: "StudentAchievements");

            migrationBuilder.DropTable(
                name: "StudentProfiles");

            migrationBuilder.DropTable(
                name: "StudentStreaks");

            migrationBuilder.DropTable(
                name: "WeeklyLearningReportInsights");

            migrationBuilder.DropTable(
                name: "DailyStudyPlans");

            migrationBuilder.DropTable(
                name: "PracticeSessions");

            migrationBuilder.DropTable(
                name: "WeeklyLearningReports");
        }
    }
}
