using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.ContentQuality;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Contracts.Quizzes;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Contracts.Words;

namespace EnglishMaster.IntegrationTests.ContentQuality;

public sealed class ContentQualityEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task ContentQualityPermissions_AreSeeded()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var permissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>("/api/v1/permissions");
        Assert.NotNull(permissions);

        Assert.Contains(permissions, permission => permission.Key == "content-quality.read");
        Assert.Contains(permissions, permission => permission.Key == "content-quality.run");
        Assert.Contains(permissions, permission => permission.Key == "content-quality.manage");
    }

    [Fact]
    public async Task QualityRule_CanBeCreatedAndSearched()
    {
        using var client = factory.CreateClient();
        var code = $"TEST.RULE.{Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("/api/v1/content-quality/rules", new CreateContentQualityRuleRequest(
            code,
            "Test Rule",
            "Checks test content.",
            "Word",
            "Warning"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var search = await client.GetFromJsonAsync<ContentQualityRuleSearchResponse>($"/api/v1/content-quality/rules?contentType=Word");
        Assert.Contains(search!.Items, rule => rule.Code == code);
    }

    [Fact]
    public async Task RunQualityCheck_ForWordWithMissingIpa_ReturnsWarningFinding()
    {
        using var client = factory.CreateClient();
        var word = await CreateWordAsync(client, $"quality word {Guid.NewGuid():N}", ipaUk: null, ipaUs: null);

        var check = await RunAsync(client, "Word", word.Id);

        Assert.True(check.Score < 100);
        Assert.Contains(check.Findings, finding => finding.RuleCode == "WORD.IPA");
    }

    [Fact]
    public async Task RunQualityCheck_ForLessonWithNoSections_ReturnsFailedCheck()
    {
        using var client = factory.CreateClient();
        var lessonResponse = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest($"Quality Lesson {Guid.NewGuid():N}", null, null, null, null, null, 0, 0));
        var lesson = await lessonResponse.Content.ReadFromJsonAsync<LessonDto>();

        var check = await RunAsync(client, "Lesson", lesson!.Id);

        Assert.Equal("Failed", check.Status);
        Assert.Contains(check.Findings, finding => finding.RuleCode == "LESSON.SECTIONS");
    }

    [Fact]
    public async Task RunQualityCheck_ForQuizWithNoCorrectChoice_ReturnsFailedCheck()
    {
        using var client = factory.CreateClient();
        var quizResponse = await client.PostAsJsonAsync(
            "/api/v1/quizzes",
            new CreateQuizRequest($"Quality Quiz {Guid.NewGuid():N}", null, null, null, null, null, null, null, 0, 70, 0));
        var quiz = await quizResponse.Content.ReadFromJsonAsync<QuizDto>();
        var questionResponse = await client.PostAsJsonAsync(
            $"/api/v1/quizzes/{quiz!.Id}/questions",
            new CreateQuizQuestionRequest("Pick one.", "SingleChoice", null, null, 1, 0, null, null, null));
        var question = await questionResponse.Content.ReadFromJsonAsync<QuizQuestionDto>();
        await client.PostAsJsonAsync(
            $"/api/v1/quiz-questions/{question!.Id}/choices",
            new CreateQuizChoiceRequest("Wrong", false, null, null, 0));

        var check = await RunAsync(client, "Quiz", quiz.Id);

        Assert.Equal("Failed", check.Status);
        Assert.Contains(check.Findings, finding => finding.RuleCode == "QUIZ.CORRECT_CHOICE");
    }

    [Fact]
    public async Task RunQualityCheck_ForCourseWithNoLessons_ReturnsFailedCheck()
    {
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/v1/courses",
            new CreateCourseRequest($"Quality Course {Guid.NewGuid():N}", null, null, null, null, null, 0, 0));
        var course = await response.Content.ReadFromJsonAsync<CourseDto>();

        var check = await RunAsync(client, "Course", course!.Id);

        Assert.Equal("Failed", check.Status);
        Assert.Contains(check.Findings, finding => finding.RuleCode == "COURSE.LESSONS");
    }

    [Fact]
    public async Task RunQualityCheck_ForBookWithNoChapters_ReturnsFailedCheck()
    {
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/v1/books",
            new CreateBookRequest($"Quality Book {Guid.NewGuid():N}", null, null, null, null, null, null, null, null, null, null, 0, 0));
        var book = await response.Content.ReadFromJsonAsync<BookDto>();

        var check = await RunAsync(client, "Book", book!.Id);

        Assert.Equal("Failed", check.Status);
        Assert.Contains(check.Findings, finding => finding.RuleCode == "BOOK.CHAPTERS");
    }

    [Fact]
    public async Task LatestQualityCheck_ReturnsNewestCheckForContent()
    {
        using var client = factory.CreateClient();
        var word = await CreateWordAsync(client, $"latest quality word {Guid.NewGuid():N}", ipaUk: null, ipaUs: null);
        var first = await RunAsync(client, "Word", word.Id);
        var second = await RunAsync(client, "Word", word.Id);

        var latest = await client.GetFromJsonAsync<ContentQualityCheckDto>($"/api/v1/content-quality/Word/{word.Id}/latest");

        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal(second.Id, latest!.Id);
    }

    [Fact]
    public async Task MarkFindingResolved_UpdatesFinding()
    {
        using var client = factory.CreateClient();
        var word = await CreateWordAsync(client, $"resolve quality word {Guid.NewGuid():N}", ipaUk: null, ipaUs: null);
        var check = await RunAsync(client, "Word", word.Id);
        var finding = check.Findings.First();

        var resolveResponse = await client.PostAsync($"/api/v1/content-quality/findings/{finding.Id}/resolve", null);

        Assert.Equal(HttpStatusCode.OK, resolveResponse.StatusCode);
        var resolved = await resolveResponse.Content.ReadFromJsonAsync<ContentQualityFindingDto>();
        Assert.True(resolved!.IsResolved);
    }

    private static async Task<ContentQualityCheckDto> RunAsync(HttpClient client, string contentType, Guid contentId)
    {
        var response = await client.PostAsync($"/api/v1/content-quality/checks/{contentType}/{contentId}/run", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ContentQualityCheckDto>())!;
    }

    private static async Task<WordDto> CreateWordAsync(HttpClient client, string text, string? ipaUk, string? ipaUs)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/words",
            new CreateWordRequest(
                text,
                ipaUk,
                ipaUs,
                null,
                "Thai meaning",
                "English meaning",
                "Noun",
                "A1",
                "Example sentence.",
                "Thai example."));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<WordDto>())!;
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.test",
            "TestPassword1"));
}
