namespace EnglishMaster.Contracts.GrammarTopics;

public sealed record UpdateGrammarTopicRequest(
    string Title,
    string? Summary,
    string CefrLevel,
    int SortOrder,
    bool IsActive);
