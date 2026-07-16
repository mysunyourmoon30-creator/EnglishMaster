using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.GrammarTopics.Dtos;

public sealed record GrammarTopicSearchCriteria(
    string? SearchTerm,
    CefrLevel? CefrLevel,
    bool? IsActive,
    int PageNumber,
    int PageSize);

public sealed record GrammarTopicSearchResult(
    IReadOnlyCollection<GrammarTopic> Items,
    int TotalCount);
