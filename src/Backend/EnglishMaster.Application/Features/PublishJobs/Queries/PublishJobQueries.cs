namespace EnglishMaster.Application.Features.PublishJobs.Queries;

public sealed record GetPublishJobByIdQuery(Guid Id);

public sealed record SearchPublishJobsQuery(
    string? SourceType,
    Guid? SourceId,
    string? Format,
    string? Status,
    int? PageNumber,
    int? PageSize,
    string? SortBy,
    string? SortDirection);
