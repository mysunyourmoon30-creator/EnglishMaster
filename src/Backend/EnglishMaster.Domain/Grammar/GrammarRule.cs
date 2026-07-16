using EnglishMaster.Domain.Common;

namespace EnglishMaster.Domain.Grammar;

public sealed class GrammarRule
{
    private readonly List<GrammarExample> examples = [];
    private readonly List<GrammarRuleWord> relatedWords = [];

    private GrammarRule()
    {
        Title = string.Empty;
        Slug = string.Empty;
        RuleText = string.Empty;
        ExplanationTh = string.Empty;
        ExplanationEn = string.Empty;
        StructurePattern = string.Empty;
        CommonMistake = string.Empty;
        CorrectUsageNote = string.Empty;
    }

    private GrammarRule(
        Guid id,
        Guid grammarTopicId,
        string? title,
        string? ruleText,
        string? explanationTh,
        string? explanationEn,
        string? structurePattern,
        string? commonMistake,
        string? correctUsageNote,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            grammarTopicId,
            title,
            ruleText,
            explanationTh,
            explanationEn,
            structurePattern,
            commonMistake,
            correctUsageNote,
            sortOrder,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public Guid GrammarTopicId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string RuleText { get; private set; } = string.Empty;

    public string ExplanationTh { get; private set; } = string.Empty;

    public string ExplanationEn { get; private set; } = string.Empty;

    public string StructurePattern { get; private set; } = string.Empty;

    public string CommonMistake { get; private set; } = string.Empty;

    public string CorrectUsageNote { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<GrammarExample> Examples => examples.AsReadOnly();

    public IReadOnlyCollection<GrammarRuleWord> RelatedWords => relatedWords.AsReadOnly();

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static GrammarRule Create(
        Guid grammarTopicId,
        string? title,
        string? ruleText,
        string? explanationTh,
        string? explanationEn,
        string? structurePattern,
        string? commonMistake,
        string? correctUsageNote,
        int sortOrder,
        DateTimeOffset now)
    {
        return new GrammarRule(
            Guid.NewGuid(),
            grammarTopicId,
            title,
            ruleText,
            explanationTh,
            explanationEn,
            structurePattern,
            commonMistake,
            correctUsageNote,
            sortOrder,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        Guid grammarTopicId,
        string? title,
        string? ruleText,
        string? explanationTh,
        string? explanationEn,
        string? structurePattern,
        string? commonMistake,
        string? correctUsageNote,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            grammarTopicId,
            title,
            ruleText,
            explanationTh,
            explanationEn,
            structurePattern,
            commonMistake,
            correctUsageNote,
            sortOrder,
            isActive,
            now);
    }

    public void AddRelatedWord(Guid wordId, DateTimeOffset now)
    {
        GrammarDomainGuard.RequiredId(wordId, nameof(wordId));
        if (relatedWords.Any(item => item.WordId == wordId))
        {
            return;
        }

        relatedWords.Add(new GrammarRuleWord(Id, wordId));
        UpdatedAt = now;
    }

    public bool RemoveRelatedWord(Guid wordId, DateTimeOffset now)
    {
        GrammarDomainGuard.RequiredId(wordId, nameof(wordId));
        var relation = relatedWords.SingleOrDefault(item => item.WordId == wordId);
        if (relation is null)
        {
            return false;
        }

        relatedWords.Remove(relation);
        UpdatedAt = now;
        return true;
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
        return SlugGenerator.Generate(title, nameof(Title), nameof(title), GrammarRuleFieldLimits.Title);
    }

    private void Apply(
        Guid grammarTopicId,
        string? title,
        string? ruleText,
        string? explanationTh,
        string? explanationEn,
        string? structurePattern,
        string? commonMistake,
        string? correctUsageNote,
        int sortOrder,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        GrammarTopicId = GrammarDomainGuard.RequiredId(grammarTopicId, nameof(grammarTopicId));
        Title = GrammarDomainGuard.RequiredText(title, nameof(Title), GrammarRuleFieldLimits.Title);
        Slug = GenerateSlug(Title);
        RuleText = GrammarDomainGuard.RequiredText(ruleText, nameof(RuleText), GrammarRuleFieldLimits.RuleText);
        ExplanationTh = GrammarDomainGuard.OptionalText(explanationTh, nameof(ExplanationTh), GrammarRuleFieldLimits.ExplanationTh);
        ExplanationEn = GrammarDomainGuard.OptionalText(explanationEn, nameof(ExplanationEn), GrammarRuleFieldLimits.ExplanationEn);
        StructurePattern = GrammarDomainGuard.OptionalText(structurePattern, nameof(StructurePattern), GrammarRuleFieldLimits.StructurePattern);
        CommonMistake = GrammarDomainGuard.OptionalText(commonMistake, nameof(CommonMistake), GrammarRuleFieldLimits.CommonMistake);
        CorrectUsageNote = GrammarDomainGuard.OptionalText(correctUsageNote, nameof(CorrectUsageNote), GrammarRuleFieldLimits.CorrectUsageNote);
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
