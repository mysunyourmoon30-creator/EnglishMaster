using EnglishMaster.Domain.Grammar;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Dtos;

internal static class GrammarRuleInputValidator
{
    public static Result<GrammarRuleInput> Validate(
        Guid grammarTopicId,
        string? title,
        string? ruleText,
        string? explanationTh,
        string? explanationEn,
        string? structurePattern,
        string? commonMistake,
        string? correctUsageNote,
        int sortOrder,
        bool isActive)
    {
        var errors = new List<ValidationError>();
        ValidateRequiredId(grammarTopicId, nameof(grammarTopicId), errors);
        var normalizedTitle = NormalizeRequired(title, nameof(title), GrammarRuleFieldLimits.Title, errors);
        var normalizedSlug = NormalizeSlug(normalizedTitle, nameof(title), errors);
        var normalizedRuleText = NormalizeRequired(ruleText, nameof(ruleText), GrammarRuleFieldLimits.RuleText, errors);
        var normalizedExplanationTh = NormalizeOptional(explanationTh, nameof(explanationTh), GrammarRuleFieldLimits.ExplanationTh, errors);
        var normalizedExplanationEn = NormalizeOptional(explanationEn, nameof(explanationEn), GrammarRuleFieldLimits.ExplanationEn, errors);
        var normalizedStructurePattern = NormalizeOptional(structurePattern, nameof(structurePattern), GrammarRuleFieldLimits.StructurePattern, errors);
        var normalizedCommonMistake = NormalizeOptional(commonMistake, nameof(commonMistake), GrammarRuleFieldLimits.CommonMistake, errors);
        var normalizedCorrectUsageNote = NormalizeOptional(correctUsageNote, nameof(correctUsageNote), GrammarRuleFieldLimits.CorrectUsageNote, errors);

        if (sortOrder < 0)
        {
            errors.Add(new ValidationError(nameof(sortOrder), $"{nameof(sortOrder)} must be greater than or equal to zero."));
        }

        if (errors.Count > 0)
        {
            return Result<GrammarRuleInput>.Validation([.. errors]);
        }

        return Result<GrammarRuleInput>.Success(new GrammarRuleInput(
            grammarTopicId,
            normalizedTitle,
            normalizedSlug,
            normalizedRuleText,
            normalizedExplanationTh,
            normalizedExplanationEn,
            normalizedStructurePattern,
            normalizedCommonMistake,
            normalizedCorrectUsageNote,
            sortOrder,
            isActive));
    }

    private static void ValidateRequiredId(
        Guid value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
        }
    }

    private static string NormalizeSlug(
        string normalizedTitle,
        string field,
        ICollection<ValidationError> errors)
    {
        if (normalizedTitle.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            return GrammarRule.GenerateSlug(normalizedTitle);
        }
        catch (ArgumentException)
        {
            errors.Add(new ValidationError(field, $"{field} must contain at least one letter or digit."));
            return string.Empty;
        }
    }

    private static string NormalizeRequired(
        string? value,
        string field,
        int maxLength,
        ICollection<ValidationError> errors)
    {
        var normalized = NormalizeOptional(value, field, maxLength, errors);
        if (normalized.Length == 0)
        {
            errors.Add(new ValidationError(field, $"{field} is required."));
        }

        return normalized;
    }

    private static string NormalizeOptional(
        string? value,
        string field,
        int maxLength,
        ICollection<ValidationError> errors)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            errors.Add(new ValidationError(field, $"{field} must be {maxLength} characters or fewer."));
        }

        return normalized;
    }
}
