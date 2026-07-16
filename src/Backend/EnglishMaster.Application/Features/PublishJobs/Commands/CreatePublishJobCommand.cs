namespace EnglishMaster.Application.Features.PublishJobs.Commands;

public sealed record CreatePublishJobCommand(
    string? SourceType,
    Guid SourceId,
    string? Format,
    string? Title,
    string? RequestedBy);
