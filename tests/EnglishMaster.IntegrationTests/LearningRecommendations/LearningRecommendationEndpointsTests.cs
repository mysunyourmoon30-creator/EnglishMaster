using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.LearningRecommendations;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.LearningRecommendations;

public sealed class LearningRecommendationEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task ContinueLearning_ReturnsInProgressPublishedLesson()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var title = Unique("Continue Lesson");
        await SeedAsync(dbContext =>
        {
            var lesson = Lesson.Create(title, "summary", "description", CefrLevel.A1, null, null, 10, 1, DateTimeOffset.UtcNow);
            lesson.Publish(DateTimeOffset.UtcNow);
            dbContext.Lessons.Add(lesson);
            dbContext.LessonProgress.Add(LessonProgress.Create(userId, lesson.Id, 40, LearningProgressStatus.InProgress, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var items = await client.GetFromJsonAsync<IReadOnlyCollection<ContinueLearningItemDto>>("/api/v1/me/learning/continue");

        Assert.Contains(items!, item => item.Title == title && item.ProgressPercent == 40);
    }

    [Fact]
    public async Task ContinueLearning_DoesNotReturnCompletedLesson()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var title = Unique("Completed Lesson");
        await SeedAsync(dbContext =>
        {
            var lesson = Lesson.Create(title, "summary", "description", CefrLevel.A1, null, null, 10, 1, DateTimeOffset.UtcNow);
            lesson.Publish(DateTimeOffset.UtcNow);
            dbContext.Lessons.Add(lesson);
            dbContext.LessonProgress.Add(LessonProgress.Create(userId, lesson.Id, 100, LearningProgressStatus.Completed, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var items = await client.GetFromJsonAsync<IReadOnlyCollection<ContinueLearningItemDto>>("/api/v1/me/learning/continue");

        Assert.DoesNotContain(items!, item => item.Title == title);
    }

    [Fact]
    public async Task ContinueLearning_DoesNotExposeOtherUsersProgress()
    {
        var otherUserId = Guid.NewGuid();
        var title = Unique("Other User Lesson");
        await SeedAsync(dbContext =>
        {
            var lesson = Lesson.Create(title, "summary", "description", CefrLevel.A1, null, null, 10, 1, DateTimeOffset.UtcNow);
            lesson.Publish(DateTimeOffset.UtcNow);
            dbContext.Lessons.Add(lesson);
            dbContext.LessonProgress.Add(LessonProgress.Create(otherUserId, lesson.Id, 50, LearningProgressStatus.InProgress, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var items = await client.GetFromJsonAsync<IReadOnlyCollection<ContinueLearningItemDto>>("/api/v1/me/learning/continue");

        Assert.DoesNotContain(items!, item => item.Title == title);
    }

    [Fact]
    public async Task RecommendedCourses_MatchesStudentCefrAndExcludesDraft()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var publishedTitle = Unique("Recommended Course");
        var draftTitle = Unique("Draft Course");
        await SeedAsync(dbContext =>
        {
            dbContext.StudentProfiles.Add(StudentProfile.Create(userId, CefrLevel.B1, DateTimeOffset.UtcNow));
            var published = Course.Create(publishedTitle, "summary", "description", CefrLevel.B1, null, null, 10, 1, DateTimeOffset.UtcNow);
            published.Publish(DateTimeOffset.UtcNow);
            var draft = Course.Create(draftTitle, "summary", "description", CefrLevel.B1, null, null, 10, 1, DateTimeOffset.UtcNow);
            dbContext.Courses.AddRange(published, draft);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var items = await client.GetFromJsonAsync<IReadOnlyCollection<LearningRecommendationDto>>("/api/v1/me/learning/recommended-courses");

        Assert.Contains(items!, item => item.Title == publishedTitle && item.CefrLevel == "B1");
        Assert.DoesNotContain(items!, item => item.Title == draftTitle);
    }

    [Fact]
    public async Task NextLesson_ReturnsFirstIncompleteLessonInCourse()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var nextTitle = Unique("Next Lesson");
        Guid courseId = Guid.Empty;
        await SeedAsync(dbContext =>
        {
            var course = Course.Create(Unique("Course"), "summary", "description", CefrLevel.A2, null, null, 20, 1, DateTimeOffset.UtcNow);
            course.Publish(DateTimeOffset.UtcNow);
            courseId = course.Id;
            var completed = Lesson.Create(Unique("Done"), "summary", "description", CefrLevel.A2, null, null, 10, 1, DateTimeOffset.UtcNow);
            completed.Publish(DateTimeOffset.UtcNow);
            var next = Lesson.Create(nextTitle, "summary", "description", CefrLevel.A2, null, null, 10, 2, DateTimeOffset.UtcNow);
            next.Publish(DateTimeOffset.UtcNow);
            dbContext.Courses.Add(course);
            dbContext.Lessons.AddRange(completed, next);
            dbContext.CourseLessons.AddRange(CourseLesson.Create(course.Id, completed.Id, 1, true, DateTimeOffset.UtcNow), CourseLesson.Create(course.Id, next.Id, 2, true, DateTimeOffset.UtcNow));
            dbContext.LessonProgress.Add(LessonProgress.Create(userId, completed.Id, 100, LearningProgressStatus.Completed, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var item = await client.GetFromJsonAsync<LearningRecommendationDto>($"/api/v1/me/learning/courses/{courseId}/next-lesson");

        Assert.Equal(nextTitle, item!.Title);
    }

    [Fact]
    public async Task NextLesson_ReturnsNotFoundWhenAllLessonsCompleted()
    {
        var userId = await GetSuperAdminUserIdAsync();
        Guid courseId = Guid.Empty;
        await SeedAsync(dbContext =>
        {
            var course = Course.Create(Unique("Complete Course"), "summary", "description", CefrLevel.A2, null, null, 20, 1, DateTimeOffset.UtcNow);
            course.Publish(DateTimeOffset.UtcNow);
            courseId = course.Id;
            var lesson = Lesson.Create(Unique("Only Lesson"), "summary", "description", CefrLevel.A2, null, null, 10, 1, DateTimeOffset.UtcNow);
            lesson.Publish(DateTimeOffset.UtcNow);
            dbContext.Courses.Add(course);
            dbContext.Lessons.Add(lesson);
            dbContext.CourseLessons.Add(CourseLesson.Create(course.Id, lesson.Id, 1, true, DateTimeOffset.UtcNow));
            dbContext.LessonProgress.Add(LessonProgress.Create(userId, lesson.Id, 100, LearningProgressStatus.Completed, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync($"/api/v1/me/learning/courses/{courseId}/next-lesson");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReviewRecommendations_DoNotExposeQuizAnswers()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var title = Unique("Weak Quiz");
        await SeedAsync(dbContext =>
        {
            var quiz = Quiz.Create(title, "summary", "description", CefrLevel.B2, null, null, null, null, 10, 80, 1, DateTimeOffset.UtcNow);
            quiz.Publish(DateTimeOffset.UtcNow);
            dbContext.Quizzes.Add(quiz);
            dbContext.QuizAttempts.Add(QuizAttempt.Create(userId, quiz.Id, 40, passed: false, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync("/api/v1/me/learning/review");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(title, body);
        Assert.DoesNotContain("IsCorrect", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Correct", body, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<Guid> GetSuperAdminUserIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        return await dbContext.AppUsers.Where(user => user.Email == "superadmin@englishmaster.test").Select(user => user.Id).SingleAsync();
    }

    private async Task SeedAsync(Func<EnglishMasterDbContext, Task> seed)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        await seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
