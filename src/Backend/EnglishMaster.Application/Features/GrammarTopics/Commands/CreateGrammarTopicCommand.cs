namespace EnglishMaster.Application.Features.GrammarTopics.Commands;

public sealed record CreateGrammarTopicCommand(
    string Title,
    string? Summary,
    string CefrLevel,
    int SortOrder);
