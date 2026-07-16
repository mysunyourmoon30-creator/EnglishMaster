namespace EnglishMaster.Domain.Grammar;

public sealed class GrammarExample
{
    private GrammarExample()
    {
        ExampleEn = string.Empty;
        TranslationTh = string.Empty;
        ExplanationTh = string.Empty;
    }

    private GrammarExample(
        Guid id,
        Guid grammarRuleId,
        string? exampleEn,
        string? translationTh,
        string? explanationTh,
        bool isCorrectExample,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            grammarRuleId,
            exampleEn,
            translationTh,
            explanationTh,
            isCorrectExample,
            sortOrder,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public Guid GrammarRuleId { get; private set; }

    public string ExampleEn { get; private set; } = string.Empty;

    public string TranslationTh { get; private set; } = string.Empty;

    public string ExplanationTh { get; private set; } = string.Empty;

    public bool IsCorrectExample { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static GrammarExample Create(
        Guid grammarRuleId,
        string? exampleEn,
        string? translationTh,
        string? explanationTh,
        bool isCorrectExample,
        int sortOrder,
        DateTimeOffset now)
    {
        return new GrammarExample(
            Guid.NewGuid(),
            grammarRuleId,
            exampleEn,
            translationTh,
            explanationTh,
            isCorrectExample,
            sortOrder,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string? exampleEn,
        string? translationTh,
        string? explanationTh,
        bool isCorrectExample,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            GrammarRuleId,
            exampleEn,
            translationTh,
            explanationTh,
            isCorrectExample,
            sortOrder,
            isActive,
            now);
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

    private void Apply(
        Guid grammarRuleId,
        string? exampleEn,
        string? translationTh,
        string? explanationTh,
        bool isCorrectExample,
        int sortOrder,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        GrammarRuleId = GrammarDomainGuard.RequiredId(grammarRuleId, nameof(grammarRuleId));
        ExampleEn = GrammarDomainGuard.RequiredText(exampleEn, nameof(ExampleEn), GrammarExampleFieldLimits.ExampleEn);
        TranslationTh = GrammarDomainGuard.OptionalText(translationTh, nameof(TranslationTh), GrammarExampleFieldLimits.TranslationTh);
        ExplanationTh = GrammarDomainGuard.OptionalText(explanationTh, nameof(ExplanationTh), GrammarExampleFieldLimits.ExplanationTh);
        IsCorrectExample = isCorrectExample;
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
