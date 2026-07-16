namespace EnglishMaster.Application.Features.Tags.Queries;

public sealed record SearchTagsQuery(
    string? Search,
    bool? IsActive);
