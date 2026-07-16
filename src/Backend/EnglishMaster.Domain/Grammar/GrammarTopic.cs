using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Domain.Grammar;

public sealed class GrammarTopic
{
    private readonly List<GrammarRule> rules = [];

    private GrammarTopic()
    {
        Title = string.Empty;
        Slug = string.Empty;
        Summary = string.Empty;
    }

    private GrammarTopic(
        Guid id,
        string? title,
        string? summary,
        CefrLevel cefrLevel,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(title, summary, cefrLevel, sortOrder, isActive, updatedAt);
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Summary { get; private set; } = string.Empty;

    public CefrLevel CefrLevel { get; private set; }

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<GrammarRule> Rules => rules.AsReadOnly();

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static GrammarTopic Create(
        string? title,
        string? summary,
        CefrLevel cefrLevel,
        int sortOrder,
        DateTimeOffset now)
    {
        return new GrammarTopic(
            Guid.NewGuid(),
            title,
            summary,
            cefrLevel,
            sortOrder,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string? title,
        string? summary,
        CefrLevel cefrLevel,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(title, summary, cefrLevel, sortOrder, isActive, now);
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
        return SlugGenerator.Generate(title, nameof(Title), nameof(title), GrammarTopicFieldLimits.Title);
    }

    private void Apply(
        string? title,
        string? summary,
        CefrLevel cefrLevel,
        int sortOrder,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Title = GrammarDomainGuard.RequiredText(title, nameof(Title), GrammarTopicFieldLimits.Title);
        Slug = GenerateSlug(Title);
        Summary = GrammarDomainGuard.OptionalText(summary, nameof(Summary), GrammarTopicFieldLimits.Summary);
        CefrLevel = cefrLevel;
        SortOrder = GrammarDomainGuard.SortOrder(sortOrder, nameof(sortOrder));

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
