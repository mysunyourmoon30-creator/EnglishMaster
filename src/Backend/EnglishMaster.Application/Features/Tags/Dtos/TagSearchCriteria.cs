using EnglishMaster.Domain.Tags;

namespace EnglishMaster.Application.Features.Tags.Dtos;

public sealed record TagSearchCriteria(
    string? SearchTerm,
    bool? IsActive);

public sealed record TagSearchResult(
    IReadOnlyCollection<Tag> Items);
