using System.Reflection;
using EnglishMaster.Application.Features.Diagnostics;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Diagnostics;

public sealed class DiagnosticHandlerTests
{
    [Fact]
    public async Task GetQuizAsync_ReturnsOnlyActiveAnswerableContentWithoutAnswerMetadata()
    {
        var (repository, quiz, questions) = CreateQuiz(CefrLevel.A1);
        var inactiveQuestion = CreateQuestion(quiz, 9);
        inactiveQuestion.Deactivate(DateTimeOffset.UtcNow);
        AttachQuestion(quiz, inactiveQuestion);
        questions[0].Choices.Last().Deactivate(DateTimeOffset.UtcNow);
        var handler = new DiagnosticHandler(repository);

        var result = await handler.GetQuizAsync(new GetDiagnosticQuizQuery(quiz.Id), CancellationToken.None);

        Assert.Equal(ResultStatus.Success, result.Status);
        Assert.Single(result.Value!.Questions);
        Assert.Equal(questions[1].Id, result.Value.Questions.Single().Id);
        Assert.Equal("A1", result.Value.CefrLevel);
        Assert.All(result.Value.Questions.SelectMany(question => question.Choices), choice =>
        {
            Assert.NotEqual(Guid.Empty, choice.Id);
            Assert.NotEmpty(choice.Text);
        });
    }

    [Fact]
    public async Task SubmitAsync_ScoresOnServerAndUsesInclusivePassingBoundary()
    {
        var (repository, quiz, questions) = CreateQuiz(CefrLevel.B1, passingScore: 50);
        var handler = new DiagnosticHandler(repository);
        var answers = new[]
        {
            new DiagnosticAnswer(questions[0].Id, questions[0].Choices.Single(choice => choice.IsCorrect).Id),
            new DiagnosticAnswer(questions[1].Id, questions[1].Choices.Single(choice => !choice.IsCorrect).Id)
        };

        var result = await handler.SubmitAsync(new SubmitDiagnosticCommand(quiz.Id, answers), CancellationToken.None);

        Assert.Equal(ResultStatus.Success, result.Status);
        Assert.Equal(1, result.Value!.CorrectCount);
        Assert.Equal(2, result.Value.TotalQuestions);
        Assert.Equal(50, result.Value.PercentageScore);
        Assert.True(result.Value.Passed);
        Assert.Equal("B1", result.Value.AssessedCefrLevel);
        Assert.Contains("not a calibrated placement result", result.Value.Recommendation);
    }

    [Fact]
    public async Task SubmitAsync_WithNullCefr_ReturnsBoundedRecommendationWithoutLevel()
    {
        var (repository, quiz, questions) = CreateQuiz(null, passingScore: 70, questionCount: 1);
        var handler = new DiagnosticHandler(repository);
        var correct = questions[0].Choices.Single(choice => choice.IsCorrect);

        var result = await handler.SubmitAsync(
            new SubmitDiagnosticCommand(quiz.Id, [new DiagnosticAnswer(questions[0].Id, correct.Id)]),
            CancellationToken.None);

        Assert.Equal(ResultStatus.Success, result.Status);
        Assert.Null(result.Value!.AssessedCefrLevel);
        Assert.Contains("this quiz", result.Value.Recommendation);
    }

    [Fact]
    public async Task SubmitAsync_RejectsDuplicateIncompleteMismatchedAndOversizedAnswers()
    {
        var (repository, quiz, questions) = CreateQuiz(CefrLevel.A2);
        var handler = new DiagnosticHandler(repository);
        var firstCorrect = questions[0].Choices.Single(choice => choice.IsCorrect);
        var secondCorrect = questions[1].Choices.Single(choice => choice.IsCorrect);

        var duplicate = await handler.SubmitAsync(new SubmitDiagnosticCommand(quiz.Id,
        [
            new DiagnosticAnswer(questions[0].Id, firstCorrect.Id),
            new DiagnosticAnswer(questions[0].Id, firstCorrect.Id)
        ]), CancellationToken.None);
        var incomplete = await handler.SubmitAsync(new SubmitDiagnosticCommand(quiz.Id,
            [new DiagnosticAnswer(questions[0].Id, firstCorrect.Id)]), CancellationToken.None);
        var mismatched = await handler.SubmitAsync(new SubmitDiagnosticCommand(quiz.Id,
        [
            new DiagnosticAnswer(questions[0].Id, secondCorrect.Id),
            new DiagnosticAnswer(questions[1].Id, firstCorrect.Id)
        ]), CancellationToken.None);
        var oversized = await handler.SubmitAsync(new SubmitDiagnosticCommand(quiz.Id,
            Enumerable.Range(0, 101).Select(_ => new DiagnosticAnswer(Guid.NewGuid(), Guid.NewGuid())).ToArray()), CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, duplicate.Status);
        Assert.Equal(ResultStatus.ValidationError, incomplete.Status);
        Assert.Equal(ResultStatus.ValidationError, mismatched.Status);
        Assert.Equal(ResultStatus.ValidationError, oversized.Status);
    }

    [Fact]
    public async Task GetQuizAsync_HidesMissingUnpublishedInactiveAndUnanswerableQuizzes()
    {
        var repository = new FakeQuizRepository();
        var unpublished = CreateBareQuiz("Unpublished");
        var inactive = CreateBareQuiz("Inactive");
        inactive.Publish(DateTimeOffset.UtcNow);
        inactive.Deactivate(DateTimeOffset.UtcNow);
        var empty = CreateBareQuiz("Empty");
        empty.Publish(DateTimeOffset.UtcNow);
        repository.Quizzes.AddRange([unpublished, inactive, empty]);
        var handler = new DiagnosticHandler(repository);

        var missingResult = await handler.GetQuizAsync(new GetDiagnosticQuizQuery(Guid.NewGuid()), CancellationToken.None);
        var unpublishedResult = await handler.GetQuizAsync(new GetDiagnosticQuizQuery(unpublished.Id), CancellationToken.None);
        var inactiveResult = await handler.GetQuizAsync(new GetDiagnosticQuizQuery(inactive.Id), CancellationToken.None);
        var emptyResult = await handler.GetQuizAsync(new GetDiagnosticQuizQuery(empty.Id), CancellationToken.None);

        Assert.All([missingResult, unpublishedResult, inactiveResult, emptyResult], result =>
            Assert.Equal(ResultStatus.NotFound, result.Status));
    }

    private static (FakeQuizRepository Repository, Quiz Quiz, QuizQuestion[] Questions) CreateQuiz(
        CefrLevel? cefrLevel,
        int passingScore = 70,
        int questionCount = 2)
    {
        var repository = new FakeQuizRepository();
        var quiz = Quiz.Create("Diagnostic", "Summary", "Description", cefrLevel, null, null, null, null, 10, passingScore, 0, DateTimeOffset.UtcNow);
        quiz.Publish(DateTimeOffset.UtcNow);
        var questions = Enumerable.Range(0, questionCount).Select(index => CreateQuestion(quiz, index)).ToArray();
        foreach (var question in questions)
        {
            AttachQuestion(quiz, question);
        }
        repository.Quizzes.Add(quiz);
        return (repository, quiz, questions);
    }

    private static Quiz CreateBareQuiz(string title) =>
        Quiz.Create(title, null, null, null, null, null, null, null, 0, 70, 0, DateTimeOffset.UtcNow);

    private static QuizQuestion CreateQuestion(Quiz quiz, int sortOrder)
    {
        var question = QuizQuestion.Create(quiz.Id, $"Question {sortOrder}", QuizQuestionType.SingleChoice, "secret th", "secret en", 1, sortOrder, null, null, null, DateTimeOffset.UtcNow);
        question.AddChoice("Correct", true, "secret", "secret", 0, DateTimeOffset.UtcNow);
        question.AddChoice("Wrong", false, "secret", "secret", 1, DateTimeOffset.UtcNow);
        return question;
    }

    private static void AttachQuestion(Quiz quiz, QuizQuestion question)
    {
        var field = typeof(Quiz).GetField("questions", BindingFlags.Instance | BindingFlags.NonPublic)!;
        ((List<QuizQuestion>)field.GetValue(quiz)!).Add(question);
    }
}
