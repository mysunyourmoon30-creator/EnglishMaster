using EnglishMaster.Application.Features.Notifications.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Notifications.Queries;

public sealed record GetMyNotificationsQuery(Guid RecipientUserId, string? Status, string? EventType, int? PageNumber, int? PageSize);

public sealed record GetUnreadNotificationCountQuery(Guid RecipientUserId);

public sealed record SearchNotificationsQuery(string? Status, string? EventType, int? PageNumber, int? PageSize);

public sealed class NotificationQueryHandler
{
    private readonly INotificationRepository repository;

    public NotificationQueryHandler(INotificationRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<NotificationSearchResponse>> GetMyNotificationsAsync(GetMyNotificationsQuery query, CancellationToken cancellationToken) =>
        Result<NotificationSearchResponse>.Success(await repository.SearchAsync(
            query.RecipientUserId,
            query.Status,
            query.EventType,
            Math.Max(query.PageNumber ?? 1, 1),
            Math.Clamp(query.PageSize ?? 20, 1, 100),
            cancellationToken));

    public async Task<Result<int>> GetUnreadCountAsync(GetUnreadNotificationCountQuery query, CancellationToken cancellationToken) =>
        Result<int>.Success(await repository.CountUnreadAsync(query.RecipientUserId, cancellationToken));

    public async Task<Result<NotificationSearchResponse>> SearchAsync(SearchNotificationsQuery query, CancellationToken cancellationToken) =>
        Result<NotificationSearchResponse>.Success(await repository.SearchAsync(
            recipientUserId: null,
            query.Status,
            query.EventType,
            Math.Max(query.PageNumber ?? 1, 1),
            Math.Clamp(query.PageSize ?? 20, 1, 100),
            cancellationToken));
}
