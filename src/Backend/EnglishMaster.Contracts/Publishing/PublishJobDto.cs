namespace EnglishMaster.Contracts.Publishing;

public sealed record PublishJobDto(
    Guid Id,
    string SourceType,
    Guid SourceId,
    string Format,
    string Status,
    string Title,
    string OutputFileName,
    string OutputPath,
    string ErrorMessage,
    string RequestedBy,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
