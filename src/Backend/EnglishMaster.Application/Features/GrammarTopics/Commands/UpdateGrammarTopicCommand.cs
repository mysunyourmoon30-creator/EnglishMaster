namespace EnglishMaster.Application.Features.GrammarTopics.Commands;

public sealed record UpdateGrammarTopicCommand(
    Guid Id,
    string Title,
    string? Summary,
    string CefrLevel,
    int SortOrder,
    bool IsActive);
