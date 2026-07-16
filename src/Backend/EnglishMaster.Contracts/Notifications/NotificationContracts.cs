namespace EnglishMaster.Contracts.Notifications;

public sealed record NotificationDto(
    Guid Id,
    Guid RecipientUserId,
    string Type,
    string EventType,
    string Title,
    string Message,
    string Status,
    DateTimeOffset? ReadAt,
    DateTimeOffset? SentAt,
    DateTimeOffset? FailedAt,
    string ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record NotificationSearchResponse(
    IReadOnlyCollection<NotificationDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record CreateNotificationRequest(
    Guid RecipientUserId,
    string Type,
    string EventType,
    string Title,
    string Message);

public sealed record MarkNotificationFailedRequest(string ErrorMessage);

public sealed record UnreadNotificationCountResponse(int Count);
