using EnglishMaster.Application.Features.QuizChoices.Commands;
using EnglishMaster.Application.Features.QuizQuestions.Commands;
using EnglishMaster.Application.Features.Quizzes.Commands;
using EnglishMaster.Application.Features.Quizzes.Queries;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Quizzes;

public sealed class QuizUseCaseTests
{
    [Fact]
    public async Task CreateQuizCreatesQuizWhenInputIsValid()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quizzes = new FakeQuizRepository();
        var handler = CreateQuizHandler(quizzes, now);

        var result = await handler.HandleAsync(
            new CreateQuizCommand("Starter Quiz", "Check basics", null, "A1", null, null, null, null, 15, 70, 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(quizzes.Quizzes);
        Assert.Equal("starter-quiz", result.Value!.Slug);
    }

    [Fact]
    public async Task CreateQuizReturnsValidationErrorWhenTitleSlugAlreadyExists()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quizzes = new FakeQuizRepository();
        quizzes.Quizzes.Add(CreateQuiz("Starter Quiz", now));
        var handler = CreateQuizHandler(quizzes, now);

        var result = await handler.HandleAsync(
            new CreateQuizCommand("Starter Quiz", null, null, null, null, null, null, null, 0, 70, 1),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "Title");
        Assert.Single(quizzes.Quizzes);
    }

    [Fact]
    public async Task PublishAndUnpublishQuizUpdateStatus()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quizzes = new FakeQuizRepository();
        var quiz = CreateQuiz("Starter Quiz", now);
        quizzes.Quizzes.Add(quiz);

        var publishHandler = new PublishQuizCommandHandler(
            quizzes,
            new FakeCategoryRepository(),
            new FakeLessonRepository(),
            new FakeCourseRepository(),
            new FakeBookRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));
        var publishResult = await publishHandler.HandleAsync(new PublishQuizCommand(quiz.Id), CancellationToken.None);

        Assert.True(publishResult.IsSuccess);
        Assert.True(quiz.IsPublished);

        var unpublishHandler = new UnpublishQuizCommandHandler(
            quizzes,
            new FakeCategoryRepository(),
            new FakeLessonRepository(),
            new FakeCourseRepository(),
            new FakeBookRepository(),
            new FixedTimeProvider(now.AddMinutes(2)));
        var unpublishResult = await unpublishHandler.HandleAsync(new UnpublishQuizCommand(quiz.Id), CancellationToken.None);

        Assert.True(unpublishResult.IsSuccess);
        Assert.False(quiz.IsPublished);
    }

    [Fact]
    public async Task ActivateAndDeactivateQuizUpdateStatus()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quizzes = new FakeQuizRepository();
        var quiz = CreateQuiz("Starter Quiz", now);
        quizzes.Quizzes.Add(quiz);

        var deactivateHandler = new DeactivateQuizCommandHandler(
            quizzes,
            new FakeCategoryRepository(),
            new FakeLessonRepository(),
            new FakeCourseRepository(),
            new FakeBookRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));
        var deactivateResult = await deactivateHandler.HandleAsync(new DeactivateQuizCommand(quiz.Id), CancellationToken.None);

        Assert.True(deactivateResult.IsSuccess);
        Assert.False(quiz.IsActive);

        var activateHandler = new ActivateQuizCommandHandler(
            quizzes,
            new FakeCategoryRepository(),
            new FakeLessonRepository(),
            new FakeCourseRepository(),
            new FakeBookRepository(),
            new FixedTimeProvider(now.AddMinutes(2)));
        var activateResult = await activateHandler.HandleAsync(new ActivateQuizCommand(quiz.Id), CancellationToken.None);

        Assert.True(activateResult.IsSuccess);
        Assert.True(quiz.IsActive);
    }

    [Fact]
    public async Task AddQuizQuestionAddsQuestionWhenQuizIsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quizzes = new FakeQuizRepository();
        var quiz = CreateQuiz("Starter Quiz", now);
        quizzes.Quizzes.Add(quiz);
        var questions = new FakeQuizQuestionRepository();
        var handler = new AddQuizQuestionCommandHandler(
            questions,
            quizzes,
            new FakeWordRepository(),
            new FakeGrammarRuleRepository(),
            new FakePronunciationRepository(),
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new AddQuizQuestionCommand(quiz.Id, "Choose one", "SingleChoice", null, null, 1, 0, null, null, null),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(questions.Questions);
        Assert.Equal("Choose one", result.Value!.QuestionText);
    }

    [Fact]
    public async Task ReorderQuizQuestionsUpdatesSortOrder()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quizzes = new FakeQuizRepository();
        var quiz = CreateQuiz("Starter Quiz", now);
        quizzes.Quizzes.Add(quiz);
        var questions = new FakeQuizQuestionRepository();
        var first = CreateQuestion(quiz.Id, "First", 0, now);
        var second = CreateQuestion(quiz.Id, "Second", 1, now);
        questions.Questions.Add(first);
        questions.Questions.Add(second);
        var handler = new ReorderQuizQuestionsCommandHandler(
            questions,
            quizzes,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new ReorderQuizQuestionsCommand(quiz.Id, [second.Id, first.Id]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(second.Id, result.Value!.First().Id);
        Assert.Equal(0, second.SortOrder);
        Assert.Equal(1, first.SortOrder);
    }

    [Fact]
    public async Task AddQuizChoicePreventsDuplicateCorrectSingleChoiceAnswer()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var questions = new FakeQuizQuestionRepository();
        var question = CreateQuestion(Guid.NewGuid(), "Choose one", 0, now);
        questions.Questions.Add(question);
        var choices = new FakeQuizChoiceRepository();
        var handler = new AddQuizChoiceCommandHandler(
            choices,
            questions,
            new FixedTimeProvider(now.AddMinutes(1)));

        var first = await handler.HandleAsync(
            new AddQuizChoiceCommand(question.Id, "A", true, null, null, 0),
            CancellationToken.None);
        var duplicate = await handler.HandleAsync(
            new AddQuizChoiceCommand(question.Id, "B", true, null, null, 1),
            CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.Equal(1, choices.SaveChangesCount);
        Assert.Equal(ResultStatus.ValidationError, duplicate.Status);
    }

    [Fact]
    public async Task ReorderQuizChoicesUpdatesSortOrder()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var questions = new FakeQuizQuestionRepository();
        var question = CreateQuestion(Guid.NewGuid(), "Choose one", 0, now);
        questions.Questions.Add(question);
        var choices = new FakeQuizChoiceRepository();
        var first = question.AddChoice("A", true, null, null, 0, now);
        var second = question.AddChoice("B", false, null, null, 1, now);
        choices.Choices.Add(first);
        choices.Choices.Add(second);
        var handler = new ReorderQuizChoicesCommandHandler(
            choices,
            questions,
            new FixedTimeProvider(now.AddMinutes(1)));

        var result = await handler.HandleAsync(
            new ReorderQuizChoicesCommand(question.Id, [second.Id, first.Id]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(second.Id, result.Value!.First().Id);
        Assert.Equal(0, second.SortOrder);
        Assert.Equal(1, first.SortOrder);
    }

    [Fact]
    public async Task SearchQuizzesFiltersByCefr()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quizzes = new FakeQuizRepository();
        quizzes.Quizzes.Add(CreateQuiz("Starter Quiz", now, CefrLevel.A1));
        quizzes.Quizzes.Add(CreateQuiz("Intermediate Quiz", now, CefrLevel.B1));
        var handler = SearchHandler(quizzes);

        var result = await handler.HandleAsync(
            new SearchQuizzesQuery(null, "B1", null, null, null, null, null, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Intermediate Quiz", result.Value.Items.Single().Title);
    }

    [Fact]
    public async Task SearchQuizzesFiltersByPublishedStatus()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quizzes = new FakeQuizRepository();
        var draft = CreateQuiz("Draft Quiz", now);
        var published = CreateQuiz("Published Quiz", now);
        published.Publish(now);
        quizzes.Quizzes.Add(draft);
        quizzes.Quizzes.Add(published);
        var handler = SearchHandler(quizzes);

        var result = await handler.HandleAsync(
            new SearchQuizzesQuery(null, null, null, null, null, null, true, true, 1, 20),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Published Quiz", result.Value.Items.Single().Title);
    }

    private static CreateQuizCommandHandler CreateQuizHandler(
        FakeQuizRepository quizzes,
        DateTimeOffset now)
    {
        return new CreateQuizCommandHandler(
            quizzes,
            new FakeCategoryRepository(),
            new FakeLessonRepository(),
            new FakeCourseRepository(),
            new FakeBookRepository(),
            new FixedTimeProvider(now));
    }

    private static SearchQuizzesQueryHandler SearchHandler(FakeQuizRepository quizzes)
    {
        return new SearchQuizzesQueryHandler(
            quizzes,
            new FakeCategoryRepository(),
            new FakeLessonRepository(),
            new FakeCourseRepository(),
            new FakeBookRepository());
    }

    private static Quiz CreateQuiz(string title, DateTimeOffset now, CefrLevel? cefrLevel = null)
    {
        return Quiz.Create(title, null, null, cefrLevel, null, null, null, null, 0, 70, 0, now);
    }

    private static QuizQuestion CreateQuestion(Guid quizId, string text, int sortOrder, DateTimeOffset now)
    {
        return QuizQuestion.Create(
            quizId,
            text,
            QuizQuestionType.SingleChoice,
            null,
            null,
            1,
            sortOrder,
            null,
            null,
            null,
            now);
    }
}
