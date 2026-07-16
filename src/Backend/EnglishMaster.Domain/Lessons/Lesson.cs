using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Domain.Lessons;

public sealed class Lesson
{
    private readonly List<LessonSection> sections = [];
    private readonly List<LessonWord> words = [];
    private readonly List<LessonGrammarRule> grammarRules = [];

    private Lesson()
    {
        Title = string.Empty;
        Slug = string.Empty;
        Summary = string.Empty;
        Description = string.Empty;
    }

    private Lesson(
        Guid id,
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            thumbnailMediaId,
            estimatedMinutes,
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

    public Guid? ThumbnailMediaId { get; private set; }

    public int EstimatedMinutes { get; private set; }

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<LessonSection> Sections => sections.AsReadOnly();

    public IReadOnlyCollection<LessonWord> Words => words.AsReadOnly();

    public IReadOnlyCollection<LessonGrammarRule> GrammarRules => grammarRules.AsReadOnly();

    public bool IsPublished { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Lesson Create(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
        int sortOrder,
        DateTimeOffset now)
    {
        return new Lesson(
            Guid.NewGuid(),
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            thumbnailMediaId,
            estimatedMinutes,
            sortOrder,
            isPublished: false,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
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
            thumbnailMediaId,
            estimatedMinutes,
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

    public void AddWord(Guid wordId, int sortOrder, DateTimeOffset now)
    {
        LessonDomainGuard.RequiredId(wordId, nameof(wordId));
        if (words.Any(item => item.WordId == wordId))
        {
            return;
        }

        words.Add(new LessonWord(Id, wordId, sortOrder));
        UpdatedAt = now;
    }

    public bool RemoveWord(Guid wordId, DateTimeOffset now)
    {
        LessonDomainGuard.RequiredId(wordId, nameof(wordId));
        var relation = words.SingleOrDefault(item => item.WordId == wordId);
        if (relation is null)
        {
            return false;
        }

        words.Remove(relation);
        UpdatedAt = now;
        return true;
    }

    public void AddGrammarRule(Guid grammarRuleId, int sortOrder, DateTimeOffset now)
    {
        LessonDomainGuard.RequiredId(grammarRuleId, nameof(grammarRuleId));
        if (grammarRules.Any(item => item.GrammarRuleId == grammarRuleId))
        {
            return;
        }

        grammarRules.Add(new LessonGrammarRule(Id, grammarRuleId, sortOrder));
        UpdatedAt = now;
    }

    public bool RemoveGrammarRule(Guid grammarRuleId, DateTimeOffset now)
    {
        LessonDomainGuard.RequiredId(grammarRuleId, nameof(grammarRuleId));
        var relation = grammarRules.SingleOrDefault(item => item.GrammarRuleId == grammarRuleId);
        if (relation is null)
        {
            return false;
        }

        grammarRules.Remove(relation);
        UpdatedAt = now;
        return true;
    }

    public static string GenerateSlug(string? title)
    {
        return SlugGenerator.Generate(title, nameof(Title), nameof(title), LessonFieldLimits.Title);
    }

    private void Apply(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Title = LessonDomainGuard.RequiredText(title, nameof(Title), LessonFieldLimits.Title);
        Slug = GenerateSlug(Title);
        Summary = LessonDomainGuard.OptionalText(summary, nameof(Summary), LessonFieldLimits.Summary);
        Description = LessonDomainGuard.OptionalText(description, nameof(Description), LessonFieldLimits.Description);
        CefrLevel = cefrLevel;
        CategoryId = LessonDomainGuard.OptionalId(categoryId, nameof(categoryId));
        ThumbnailMediaId = LessonDomainGuard.OptionalId(thumbnailMediaId, nameof(thumbnailMediaId));
        EstimatedMinutes = LessonDomainGuard.NonNegative(estimatedMinutes, nameof(estimatedMinutes));
        SortOrder = LessonDomainGuard.NonNegative(sortOrder, nameof(sortOrder));

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
