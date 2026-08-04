using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Contracts.StudentProgress;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.StudentProgress;

public sealed class StudentProgressSummaryEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task Progress_ReturnsOwnedPublishedContentAndCorrectSummary()
    {
        var userId = await StudentProgressTestSupport.GetSuperAdminUserIdAsync(factory);
        var lessonTitle = StudentProgressTestSupport.Unique("Progress Lesson");
        var courseTitle = StudentProgressTestSupport.Unique("Progress Course");
        var bookTitle = StudentProgressTestSupport.Unique("Progress Book");
        var now = DateTimeOffset.UtcNow;

        await StudentProgressTestSupport.SeedAsync(factory, dbContext =>
        {
            var lesson = Lesson.Create(lessonTitle, "Lesson summary", "description", CefrLevel.A1, null, null, 10, 1, now);
            lesson.Publish(now);
            var course = Course.Create(courseTitle, "Course summary", "description", CefrLevel.A2, null, null, 30, 1, now);
            course.Publish(now);
            var book = Book.Create(bookTitle, "subtitle", "Book summary", "description", CefrLevel.B1, null, null, null, "Author", "1", "1.0", 100, 1, now);
            book.Publish(now);

            dbContext.Lessons.Add(lesson);
            dbContext.Courses.Add(course);
            dbContext.Books.Add(book);
            dbContext.LessonProgress.Add(LessonProgress.Create(userId, lesson.Id, 35, LearningProgressStatus.InProgress, now.AddMinutes(-3), now));
            dbContext.CourseProgress.Add(CourseProgress.Create(userId, course.Id, 100, LearningProgressStatus.Completed, now.AddMinutes(-2), now));
            dbContext.BookProgress.Add(BookProgress.Create(userId, book.Id, 0, LearningProgressStatus.NotStarted, now.AddMinutes(-1), now));
            return Task.CompletedTask;
        });

        using var client = factory.CreateClient(new() { HandleCookies = true });
        await StudentProgressTestSupport.LoginAsync(client);

        var summary = await client.GetFromJsonAsync<StudentProgressSummaryDto>("/api/v1/me/progress");

        Assert.NotNull(summary);
        Assert.Equal(3, summary.TotalTrackedItems);
        Assert.Equal(1, summary.InProgressCount);
        Assert.Equal(1, summary.CompletedCount);
        Assert.Contains(summary.Lessons, item => item.Title == lessonTitle && item.ProgressPercent == 35 && item.Url.StartsWith("/lessons/", StringComparison.Ordinal));
        Assert.Contains(summary.Courses, item => item.Title == courseTitle && item.Status == "Completed" && item.Url.StartsWith("/courses/", StringComparison.Ordinal));
        Assert.Contains(summary.Books, item => item.Title == bookTitle && item.Summary == "Book summary" && item.Url.StartsWith("/books/", StringComparison.Ordinal));
    }
}
public sealed class StudentProgressPrivacyEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task Progress_ExcludesOtherUserDraftAndInactiveContent()
    {
        var userId = await StudentProgressTestSupport.GetSuperAdminUserIdAsync(factory);
        var otherUserTitle = StudentProgressTestSupport.Unique("Other User");
        var draftTitle = StudentProgressTestSupport.Unique("Draft");
        var inactiveTitle = StudentProgressTestSupport.Unique("Inactive");
        var now = DateTimeOffset.UtcNow;

        await StudentProgressTestSupport.SeedAsync(factory, dbContext =>
        {
            var otherUserLesson = Lesson.Create(otherUserTitle, "summary", "description", CefrLevel.A1, null, null, 10, 1, now);
            otherUserLesson.Publish(now);
            var draftLesson = Lesson.Create(draftTitle, "summary", "description", CefrLevel.A1, null, null, 10, 2, now);
            var inactiveLesson = Lesson.Create(inactiveTitle, "summary", "description", CefrLevel.A1, null, null, 10, 3, now);
            inactiveLesson.Publish(now);
            inactiveLesson.Deactivate(now);

            dbContext.Lessons.AddRange(otherUserLesson, draftLesson, inactiveLesson);
            dbContext.LessonProgress.Add(LessonProgress.Create(Guid.NewGuid(), otherUserLesson.Id, 40, LearningProgressStatus.InProgress, now, now));
            dbContext.LessonProgress.Add(LessonProgress.Create(userId, draftLesson.Id, 50, LearningProgressStatus.InProgress, now, now));
            dbContext.LessonProgress.Add(LessonProgress.Create(userId, inactiveLesson.Id, 60, LearningProgressStatus.InProgress, now, now));
            return Task.CompletedTask;
        });

        using var client = factory.CreateClient(new() { HandleCookies = true });
        await StudentProgressTestSupport.LoginAsync(client);

        var summary = await client.GetFromJsonAsync<StudentProgressSummaryDto>("/api/v1/me/progress");

        Assert.NotNull(summary);
        Assert.Empty(summary.Lessons);
        Assert.Equal(0, summary.TotalTrackedItems);
        Assert.DoesNotContain(summary.Lessons, item => item.Title == otherUserTitle || item.Title == draftTitle || item.Title == inactiveTitle);
    }
}

public sealed class StudentProgressLimitEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task Progress_ClampsLimitAndOrdersMostRecentFirst()
    {
        var userId = await StudentProgressTestSupport.GetSuperAdminUserIdAsync(factory);
        var titlePrefix = StudentProgressTestSupport.Unique("Bounded Lesson");
        var now = DateTimeOffset.UtcNow.AddYears(1);

        await StudentProgressTestSupport.SeedAsync(factory, dbContext =>
        {
            for (var index = 0; index < 55; index++)
            {
                var lesson = Lesson.Create($"{titlePrefix}-{index:D2}", "summary", "description", CefrLevel.A1, null, null, 10, index, now);
                lesson.Publish(now);
                dbContext.Lessons.Add(lesson);
                dbContext.LessonProgress.Add(LessonProgress.Create(userId, lesson.Id, 10, LearningProgressStatus.InProgress, now.AddMinutes(-index), now));
            }

            return Task.CompletedTask;
        });

        using var client = factory.CreateClient(new() { HandleCookies = true });
        await StudentProgressTestSupport.LoginAsync(client);

        var summary = await client.GetFromJsonAsync<StudentProgressSummaryDto>("/api/v1/me/progress?limit=100");

        Assert.NotNull(summary);
        Assert.Equal(55, summary.TotalTrackedItems);
        Assert.Equal(50, summary.Lessons.Count);
        Assert.Equal($"{titlePrefix}-00", summary.Lessons.First().Title);
        Assert.Equal($"{titlePrefix}-49", summary.Lessons.Last().Title);
    }
}

public sealed class StudentProgressEmptyEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task Progress_WhenEmpty_ReturnsOkWithEmptyCollections()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await StudentProgressTestSupport.LoginAsync(client);

        var response = await client.GetAsync("/api/v1/me/progress");
        var summary = await response.Content.ReadFromJsonAsync<StudentProgressSummaryDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(summary);
        Assert.Equal(0, summary.TotalTrackedItems);
        Assert.Empty(summary.Lessons);
        Assert.Empty(summary.Courses);
        Assert.Empty(summary.Books);
    }
}

public sealed class StudentProgressAuthorizationMetadataTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public void ProgressEndpoint_RequiresAuthorization()
    {
        var dataSource = factory.Services.GetRequiredService<EndpointDataSource>();
        var endpoint = dataSource.Endpoints
            .OfType<RouteEndpoint>()
            .Single(route => route.RoutePattern.RawText == "/api/v1/me/progress");

        Assert.NotEmpty(endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>());
    }
}

internal static class StudentProgressTestSupport
{
    public static async Task<Guid> GetSuperAdminUserIdAsync(EnglishMasterApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        return await dbContext.AppUsers
            .Where(user => user.Email == "superadmin@englishmaster.test")
            .Select(user => user.Id)
            .SingleAsync();
    }

    public static async Task SeedAsync(
        EnglishMasterApiFactory factory,
        Func<EnglishMasterDbContext, Task> seed)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        await seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    public static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    public static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";
}
