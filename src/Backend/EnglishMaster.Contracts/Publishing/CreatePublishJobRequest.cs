namespace EnglishMaster.Contracts.Publishing;

public sealed record CreatePublishJobRequest(
    string SourceType,
    Guid SourceId,
    string Format,
    string Title,
    string? RequestedBy);
