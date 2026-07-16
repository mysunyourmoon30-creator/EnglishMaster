namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed record RemoveRelatedWordFromGrammarRuleCommand(
    Guid GrammarRuleId,
    Guid WordId);
