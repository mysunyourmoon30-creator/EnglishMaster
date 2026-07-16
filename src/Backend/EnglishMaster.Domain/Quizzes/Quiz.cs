using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Domain.Quizzes;

public sealed class Quiz
{
    private readonly List<QuizQuestion> questions = [];

    private Quiz()
    {
        Title = string.Empty;
        Slug = string.Empty;
        Summary = string.Empty;
        Description = string.Empty;
    }

    private Quiz(
        Guid id,
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? lessonId,
        Guid? courseId,
        Guid? bookId,
        int timeLimitMinutes,
        int passingScore,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = QuizDomainGuard.RequiredId(id, nameof(id));
        CreatedAt = createdAt;
        Apply(
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            lessonId,
            courseId,
            bookId,
            timeLimitMinutes,
            passingScore,
            sortOrder,
            isPublished,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Summary { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public CefrLevel? CefrLevel { get; private set; }

    public Guid? CategoryId { get; private set; }

    public Guid? LessonId { get; private set; }

    public Guid? CourseId { get; private set; }

    public Guid? BookId { get; private set; }

    public int TimeLimitMinutes { get; private set; }

    public int PassingScore { get; private set; }

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<QuizQuestion> Questions => questions.AsReadOnly();

    public bool IsPublished { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Quiz Create(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? lessonId,
        Guid? courseId,
        Guid? bookId,
        int timeLimitMinutes,
        int passingScore,
        int sortOrder,
        DateTimeOffset now)
    {
        return new Quiz(
            Guid.NewGuid(),
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            lessonId,
            courseId,
            bookId,
            timeLimitMinutes,
            passingScore,
            sortOrder,
            isPublished: false,
            isActive: true,
            now,
            now);
    }

    public void Update(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? lessonId,
        Guid? courseId,
        Guid? bookId,
        int timeLimitMinutes,
        int passingScore,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            lessonId,
            courseId,
            bookId,
            timeLimitMinutes,
            passingScore,
            sortOrder,
            isPublished,
            isActive,
            now);
    }

    public void Publish(DateTimeOffset now)
    {
        IsPublished = true;
        UpdatedAt = now;
    }

    public void Unpublish(DateTimeOffset now)
    {
        IsPublished = false;
        UpdatedAt = now;
    }

    public void Activate(DateTimeOffset now)
    {
        IsActive = true;
        UpdatedAt = now;
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        UpdatedAt = now;
    }

    public static string GenerateSlug(string? title)
    {
        return SlugGenerator.Generate(title, nameof(Title), nameof(title), QuizFieldLimits.Title);
    }

    private void Apply(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? lessonId,
        Guid? courseId,
        Guid? bookId,
        int timeLimitMinutes,
        int passingScore,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Title = QuizDomainGuard.RequiredText(title, nameof(Title), nameof(title), QuizFieldLimits.Title);
        Slug = GenerateSlug(Title);
        Summary = QuizDomainGuard.OptionalText(summary, nameof(Summary), nameof(summary), QuizFieldLimits.Summary);
        Description = QuizDomainGuard.OptionalText(description, nameof(Description), nameof(description), QuizFieldLimits.Description);
        CefrLevel = cefrLevel;
        CategoryId = QuizDomainGuard.OptionalId(categoryId, nameof(categoryId));
        LessonId = QuizDomainGuard.OptionalId(lessonId, nameof(lessonId));
        CourseId = QuizDomainGuard.OptionalId(courseId, nameof(courseId));
        BookId = QuizDomainGuard.OptionalId(bookId, nameof(bookId));
        TimeLimitMinutes = QuizDomainGuard.NonNegative(timeLimitMinutes, nameof(timeLimitMinutes));
        PassingScore = QuizDomainGuard.Percentage(passingScore, nameof(passingScore));
        SortOrder = QuizDomainGuard.NonNegative(sortOrder, nameof(sortOrder));

        if (isPublished)
        {
            Publish(updatedAt);
        }
        else
        {
            Unpublish(updatedAt);
        }

        if (isActive)
        {
            Activate(updatedAt);
        }
        else
        {
            Deactivate(updatedAt);
        }
    }
}
