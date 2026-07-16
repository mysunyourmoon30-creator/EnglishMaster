namespace EnglishMaster.Contracts.GrammarTopics;

public sealed record CreateGrammarTopicRequest(
    string Title,
    string? Summary,
    string CefrLevel,
    int SortOrder);
