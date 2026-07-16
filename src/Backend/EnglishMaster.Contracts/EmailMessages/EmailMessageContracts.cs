namespace EnglishMaster.Contracts.EmailMessages;

public sealed record EmailMessageDto(
    Guid Id,
    string ToEmail,
    string ToName,
    string Subject,
    string Body,
    bool IsHtml,
    string Status,
    DateTimeOffset? SentAt,
    DateTimeOffset? FailedAt,
    string ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record EmailMessageSearchResponse(
    IReadOnlyCollection<EmailMessageDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record QueueEmailMessageRequest(
    string ToEmail,
    string ToName,
    string Subject,
    string Body,
    bool IsHtml);

public sealed record MarkEmailFailedRequest(string ErrorMessage);
