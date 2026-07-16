using EnglishMaster.Contracts.Notifications;

namespace EnglishMaster.Web.Services.Notifications;

public interface INotificationsApiClient
{
    Task<NotificationSearchResponse> GetMyNotificationsAsync(string? status, string? eventType, CancellationToken cancellationToken);

    Task<int> GetUnreadCountAsync(CancellationToken cancellationToken);

    Task<NotificationDto> MarkReadAsync(Guid id, CancellationToken cancellationToken);

    Task<NotificationSearchResponse> SearchAdminAsync(string? status, string? eventType, CancellationToken cancellationToken);
}
