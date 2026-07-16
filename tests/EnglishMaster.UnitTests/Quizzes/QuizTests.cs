using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.UnitTests.Quizzes;

public sealed class QuizTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var categoryId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        var quiz = Quiz.Create(
            "  Starter Quiz  ",
            "  Summary  ",
            "  Description  ",
            CefrLevel.A1,
            categoryId,
            lessonId,
            courseId,
            bookId,
            15,
            80,
            2,
            now);

        Assert.NotEqual(Guid.Empty, quiz.Id);
        Assert.Equal("Starter Quiz", quiz.Title);
        Assert.Equal("starter-quiz", quiz.Slug);
        Assert.Equal("Summary", quiz.Summary);
        Assert.Equal("Description", quiz.Description);
        Assert.Equal(CefrLevel.A1, quiz.CefrLevel);
        Assert.Equal(categoryId, quiz.CategoryId);
        Assert.Equal(lessonId, quiz.LessonId);
        Assert.Equal(courseId, quiz.CourseId);
        Assert.Equal(bookId, quiz.BookId);
        Assert.Equal(15, quiz.TimeLimitMinutes);
        Assert.Equal(80, quiz.PassingScore);
        Assert.Equal(2, quiz.SortOrder);
        Assert.False(quiz.IsPublished);
        Assert.True(quiz.IsActive);
        Assert.Equal(now, quiz.CreatedAt);
        Assert.Equal(now, quiz.UpdatedAt);
    }

    [Theory]
    [InlineData("Starter Quiz", "starter-quiz")]
    [InlineData(" A1 / First Exercise ", "a1-first-exercise")]
    [InlineData("teacher's check", "teachers-check")]
    public void GenerateSlugCreatesUrlFriendlyValue(string title, string expectedSlug)
    {
        Assert.Equal(expectedSlug, Quiz.GenerateSlug(title));
    }

    [Fact]
    public void CreateRequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() => Quiz.Create(
            " ",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            0,
            70,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("title", exception.ParamName);
    }

    [Fact]
    public void TimeLimitMustNotBeNegative()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Quiz.Create(
            "Starter Quiz",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            -1,
            70,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("timeLimitMinutes", exception.ParamName);
    }

    [Fact]
    public void PassingScoreMustBePercentage()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Quiz.Create(
            "Starter Quiz",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            0,
            101,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("passingScore", exception.ParamName);
    }

    [Fact]
    public void PublishUnpublishActivateAndDeactivateUpdateStatus()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var quiz = CreateQuiz(createdAt);

        quiz.Publish(createdAt.AddMinutes(1));
        Assert.True(quiz.IsPublished);

        quiz.Unpublish(createdAt.AddMinutes(2));
        Assert.False(quiz.IsPublished);

        quiz.Deactivate(createdAt.AddMinutes(3));
        Assert.False(quiz.IsActive);

        quiz.Activate(createdAt.AddMinutes(4));
        Assert.True(quiz.IsActive);
    }

    [Fact]
    public void QuizQuestionRequiresTextAndDefaultsActive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var question = QuizQuestion.Create(
            Guid.NewGuid(),
            " Choose the correct answer ",
            QuizQuestionType.SingleChoice,
            null,
            null,
            2,
            1,
            null,
            null,
            null,
            now);

        Assert.Equal("Choose the correct answer", question.QuestionText);
        Assert.Equal(QuizQuestionType.SingleChoice, question.QuestionType);
        Assert.Equal(2, question.Points);
        Assert.True(question.IsActive);
    }

    [Fact]
    public void QuizQuestionRequiresDefinedQuestionType()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => QuizQuestion.Create(
            Guid.NewGuid(),
            "Choose the correct answer",
            (QuizQuestionType)999,
            null,
            null,
            1,
            0,
            null,
            null,
            null,
            DateTimeOffset.UtcNow));

        Assert.Equal("questionType", exception.ParamName);
    }

    [Fact]
    public void QuizChoiceRequiresTextAndTracksCorrectAnswer()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var choice = QuizChoice.Create(Guid.NewGuid(), " Correct answer ", true, null, null, 0, now);

        Assert.Equal("Correct answer", choice.ChoiceText);
        Assert.True(choice.IsCorrect);
        Assert.True(choice.IsActive);
    }

    [Theory]
    [InlineData(QuizQuestionType.SingleChoice)]
    [InlineData(QuizQuestionType.TrueFalse)]
    public void NonMultipleChoiceQuestionsAllowOnlyOneCorrectChoice(QuizQuestionType questionType)
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var question = QuizQuestion.Create(Guid.NewGuid(), "Question", questionType, null, null, 1, 0, null, null, null, now);

        question.AddChoice("First", true, null, null, 0, now);

        Assert.Throws<InvalidOperationException>(() => question.AddChoice("Second", true, null, null, 1, now));
    }

    [Fact]
    public void MultipleChoiceQuestionsAllowMoreThanOneCorrectChoice()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var question = QuizQuestion.Create(Guid.NewGuid(), "Question", QuizQuestionType.MultipleChoice, null, null, 1, 0, null, null, null, now);

        question.AddChoice("First", true, null, null, 0, now);
        question.AddChoice("Second", true, null, null, 1, now);

        Assert.Equal(2, question.Choices.Count(choice => choice.IsCorrect));
    }

    private static Quiz CreateQuiz(DateTimeOffset now)
    {
        return Quiz.Create("Starter Quiz", null, null, null, null, null, null, null, 0, 70, 0, now);
    }
}
