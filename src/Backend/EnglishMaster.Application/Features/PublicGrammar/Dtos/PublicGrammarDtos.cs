namespace EnglishMaster.Application.Features.PublicGrammar.Dtos;

public sealed record PublicGrammarTopicDetailDto(
    string Title,
    string Slug,
    string Summary,
    string CefrLevel,
    IReadOnlyCollection<PublicGrammarRuleSummaryDto> Rules);

public sealed record PublicGrammarRuleSummaryDto(
    string Title,
    string Slug,
    string RuleText,
    string StructurePattern);

public sealed record PublicGrammarRuleDetailDto(
    string Title,
    string Slug,
    string RuleText,
    string ExplanationTh,
    string ExplanationEn,
    string StructurePattern,
    string CommonMistake,
    string CorrectUsageNote,
    PublicGrammarTopicSummaryDto Topic,
    IReadOnlyCollection<PublicGrammarExampleDto> Examples,
    IReadOnlyCollection<PublicGrammarRelatedWordDto> RelatedWords);

public sealed record PublicGrammarTopicSummaryDto(
    string Title,
    string Slug,
    string CefrLevel);

public sealed record PublicGrammarExampleDto(
    string ExampleEn,
    string TranslationTh,
    string ExplanationTh,
    bool IsCorrectExample);

public sealed record PublicGrammarRelatedWordDto(
    string Text,
    string Slug,
    string MeaningTh);