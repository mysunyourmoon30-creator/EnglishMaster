using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.GrammarTopics.Dtos;

internal sealed record GrammarTopicInput(
    string Title,
    string Slug,
    string Summary,
    CefrLevel CefrLevel,
    int SortOrder,
    bool IsActive);
