namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed record AddRelatedWordToGrammarRuleCommand(
    Guid GrammarRuleId,
    Guid WordId);
