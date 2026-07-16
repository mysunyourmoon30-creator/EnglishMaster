namespace EnglishMaster.Contracts.GrammarTopics;

public sealed record GrammarTopicDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string CefrLevel,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
