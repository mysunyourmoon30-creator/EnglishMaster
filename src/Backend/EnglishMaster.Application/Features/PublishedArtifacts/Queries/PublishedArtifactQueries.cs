namespace EnglishMaster.Application.Features.PublishedArtifacts.Queries;

public sealed record GetPublishedArtifactByIdQuery(Guid Id);

public sealed record GetArtifactsByPublishJobIdQuery(Guid PublishJobId);

public sealed record SearchPublishedArtifactsQuery(
    Guid? PublishJobId,
    string? SourceType,
    Guid? SourceId,
    string? Format,
    int? PageNumber,
    int? PageSize);
