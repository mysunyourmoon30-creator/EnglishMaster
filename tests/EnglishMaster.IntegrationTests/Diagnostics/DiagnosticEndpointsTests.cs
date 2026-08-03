using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Diagnostics;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.Diagnostics;

public sealed class DiagnosticEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task GetDiagnostic_ReturnsLearnerSafeActiveContent()
    {
        var seeded = await SeedDiagnosticAsync(CefrLevel.A1, passingScore: 50, includeInactiveQuestion: true);
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync($"/api/v1/me/diagnostics/quizzes/{seeded.Quiz.Id}");
        var json = await response.Content.ReadAsStringAsync();
        var quiz = await response.Content.ReadFromJsonAsync<DiagnosticQuizDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(quiz);
        Assert.Equal(2, quiz.Questions.Count);
        Assert.DoesNotContain(quiz.Questions, question => question.Id == seeded.InactiveQuestionId);
        Assert.All(quiz.Questions, question => Assert.Equal(2, question.Choices.Count));
        Assert.DoesNotContain("isCorrect", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("explanation", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SubmitDiagnostic_ScoresServerSideAtPassingBoundary()
    {
        var seeded = await SeedDiagnosticAsync(CefrLevel.B1, passingScore: 50);
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var answers = new[]
        {
            new DiagnosticAnswerRequest(seeded.Questions[0].QuestionId, seeded.Questions[0].CorrectChoiceId),
            new DiagnosticAnswerRequest(seeded.Questions[1].QuestionId, seeded.Questions[1].WrongChoiceId)
        };

        var response = await client.PostAsJsonAsync(
            $"/api/v1/me/diagnostics/quizzes/{seeded.Quiz.Id}/submit",
            new SubmitDiagnosticRequest(answers));
        var result = await response.Content.ReadFromJsonAsync<DiagnosticResultDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(1, result.CorrectCount);
        Assert.Equal(2, result.TotalQuestions);
        Assert.Equal(50, result.PercentageScore);
        Assert.True(result.Passed);
        Assert.Equal("B1", result.AssessedCefrLevel);
        Assert.Contains("not a calibrated placement result", result.Recommendation);
    }

    [Fact]
    public async Task SubmitDiagnostic_RejectsDuplicateIncompleteMismatchedAndOversizedAnswers()
    {
        var seeded = await SeedDiagnosticAsync(CefrLevel.A2, passingScore: 70);
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var first = seeded.Questions[0];
        var second = seeded.Questions[1];

        var duplicate = await client.PostAsJsonAsync($"/api/v1/me/diagnostics/quizzes/{seeded.Quiz.Id}/submit",
            new SubmitDiagnosticRequest([
                new(first.QuestionId, first.CorrectChoiceId),
                new(first.QuestionId, first.CorrectChoiceId)]));
        var incomplete = await client.PostAsJsonAsync($"/api/v1/me/diagnostics/quizzes/{seeded.Quiz.Id}/submit",
            new SubmitDiagnosticRequest([new(first.QuestionId, first.CorrectChoiceId)]));
        var mismatched = await client.PostAsJsonAsync($"/api/v1/me/diagnostics/quizzes/{seeded.Quiz.Id}/submit",
            new SubmitDiagnosticRequest([
                new(first.QuestionId, second.CorrectChoiceId),
                new(second.QuestionId, first.CorrectChoiceId)]));
        var oversized = await client.PostAsJsonAsync($"/api/v1/me/diagnostics/quizzes/{seeded.Quiz.Id}/submit",
            new SubmitDiagnosticRequest(Enumerable.Range(0, 101)
                .Select(_ => new DiagnosticAnswerRequest(Guid.NewGuid(), Guid.NewGuid())).ToArray()));

        Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, incomplete.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, mismatched.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, oversized.StatusCode);
    }

    [Fact]
    public async Task Diagnostic_HidesMissingUnpublishedInactiveAndUnanswerableQuizzes()
    {
        var unpublished = await SeedDiagnosticAsync(null, 70, publish: false);
        var inactive = await SeedDiagnosticAsync(null, 70, active: false);
        var empty = await SeedEmptyQuizAsync();
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var missingResponse = await client.GetAsync($"/api/v1/me/diagnostics/quizzes/{Guid.NewGuid()}");
        var unpublishedResponse = await client.GetAsync($"/api/v1/me/diagnostics/quizzes/{unpublished.Quiz.Id}");
        var inactiveResponse = await client.GetAsync($"/api/v1/me/diagnostics/quizzes/{inactive.Quiz.Id}");
        var emptyResponse = await client.GetAsync($"/api/v1/me/diagnostics/quizzes/{empty.Id}");

        Assert.All([missingResponse, unpublishedResponse, inactiveResponse, emptyResponse], response =>
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode));
    }

    [Fact]
    public async Task SubmitDiagnostic_WithNoCefr_DoesNotInventPlacementLevel()
    {
        var seeded = await SeedDiagnosticAsync(null, passingScore: 70);
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync($"/api/v1/me/diagnostics/quizzes/{seeded.Quiz.Id}/submit",
            new SubmitDiagnosticRequest(seeded.Questions
                .Select(question => new DiagnosticAnswerRequest(question.QuestionId, question.CorrectChoiceId)).ToArray()));
        var result = await response.Content.ReadFromJsonAsync<DiagnosticResultDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.AssessedCefrLevel);
        Assert.Contains("this quiz", result.Recommendation);
    }

    private async Task<SeededDiagnostic> SeedDiagnosticAsync(
        CefrLevel? cefrLevel,
        int passingScore,
        bool publish = true,
        bool active = true,
        bool includeInactiveQuestion = false)
    {
        var quiz = Quiz.Create(
            $"Diagnostic {Guid.NewGuid():N}",
            "A bounded diagnostic.",
            "Learner diagnostic content.",
            cefrLevel,
            null,
            null,
            null,
            null,
            10,
            passingScore,
            0,
            DateTimeOffset.UtcNow);
        if (publish)
        {
            quiz.Publish(DateTimeOffset.UtcNow);
        }
        if (!active)
        {
            quiz.Deactivate(DateTimeOffset.UtcNow);
        }

        var questions = Enumerable.Range(0, 2).Select(index => CreateQuestion(quiz.Id, index)).ToArray();
        QuizQuestion? inactiveQuestion = null;
        if (includeInactiveQuestion)
        {
            inactiveQuestion = CreateQuestion(quiz.Id, 9).Question;
            inactiveQuestion.Deactivate(DateTimeOffset.UtcNow);
        }

        await SeedAsync(dbContext =>
        {
            dbContext.Quizzes.Add(quiz);
            dbContext.QuizQuestions.AddRange(questions.Select(item => item.Question));
            if (inactiveQuestion is not null)
            {
                dbContext.QuizQuestions.Add(inactiveQuestion);
            }
            return Task.CompletedTask;
        });

        return new SeededDiagnostic(
            quiz,
            questions.Select(item => new SeededQuestion(item.Question.Id, item.CorrectChoiceId, item.WrongChoiceId)).ToArray(),
            inactiveQuestion?.Id);
    }

    private async Task<Quiz> SeedEmptyQuizAsync()
    {
        var quiz = Quiz.Create($"Empty {Guid.NewGuid():N}", null, null, null, null, null, null, null, 0, 70, 0, DateTimeOffset.UtcNow);
        quiz.Publish(DateTimeOffset.UtcNow);
        await SeedAsync(dbContext =>
        {
            dbContext.Quizzes.Add(quiz);
            return Task.CompletedTask;
        });
        return quiz;
    }

    private static (QuizQuestion Question, Guid CorrectChoiceId, Guid WrongChoiceId) CreateQuestion(Guid quizId, int sortOrder)
    {
        var question = QuizQuestion.Create(quizId, $"Question {sortOrder}", QuizQuestionType.SingleChoice, "secret", "secret", 1, sortOrder, null, null, null, DateTimeOffset.UtcNow);
        var correct = question.AddChoice("Correct", true, "secret", "secret", 0, DateTimeOffset.UtcNow);
        var wrong = question.AddChoice("Wrong", false, "secret", "secret", 1, DateTimeOffset.UtcNow);
        return (question, correct.Id, wrong.Id);
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

    private sealed record SeededDiagnostic(Quiz Quiz, IReadOnlyList<SeededQuestion> Questions, Guid? InactiveQuestionId);
    private sealed record SeededQuestion(Guid QuestionId, Guid CorrectChoiceId, Guid WrongChoiceId);
}
