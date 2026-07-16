using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.GrammarRules;

namespace EnglishMaster.Web.Services.Grammar;

public sealed class GrammarRuleFormModel
{
    [Required]
    public Guid? GrammarTopicId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string RuleText { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? ExplanationTh { get; set; }

    [StringLength(4000)]
    public string? ExplanationEn { get; set; }

    [StringLength(1000)]
    public string? StructurePattern { get; set; }

    [StringLength(2000)]
    public string? CommonMistake { get; set; }

    [StringLength(2000)]
    public string? CorrectUsageNote { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public static GrammarRuleFormModel FromDto(GrammarRuleDto rule)
    {
        return new GrammarRuleFormModel
        {
            GrammarTopicId = rule.GrammarTopicId,
            Title = rule.Title,
            RuleText = rule.RuleText,
            ExplanationTh = rule.ExplanationTh,
            ExplanationEn = rule.ExplanationEn,
            StructurePattern = rule.StructurePattern,
            CommonMistake = rule.CommonMistake,
            CorrectUsageNote = rule.CorrectUsageNote,
            SortOrder = rule.SortOrder,
            IsActive = rule.IsActive
        };
    }

    public CreateGrammarRuleRequest ToCreateRequest()
    {
        return new CreateGrammarRuleRequest(
            GrammarTopicId ?? Guid.Empty,
            Title,
            RuleText,
            ExplanationTh,
            ExplanationEn,
            StructurePattern,
            CommonMistake,
            CorrectUsageNote,
            SortOrder);
    }

    public UpdateGrammarRuleRequest ToUpdateRequest()
    {
        return new UpdateGrammarRuleRequest(
            GrammarTopicId ?? Guid.Empty,
            Title,
            RuleText,
            ExplanationTh,
            ExplanationEn,
            StructurePattern,
            CommonMistake,
            CorrectUsageNote,
            SortOrder,
            IsActive);
    }
}
