using EnglishMaster.Application.Features.Notifications.Dtos;
using EnglishMaster.Domain.Notifications;

namespace EnglishMaster.Application.Features.Notifications;

public interface INotificationRepository
{
    Task<NotificationDto> AddAsync(Notification notification, CancellationToken cancellationToken);

    Task<NotificationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<NotificationDto?> GetByIdForRecipientAsync(Guid id, Guid recipientUserId, CancellationToken cancellationToken);

    Task<NotificationSearchResponse> SearchAsync(Guid? recipientUserId, string? status, string? eventType, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<int> CountUnreadAsync(Guid recipientUserId, CancellationToken cancellationToken);

    Task<NotificationDto?> MarkReadAsync(Guid id, Guid recipientUserId, DateTimeOffset now, CancellationToken cancellationToken);

    Task<NotificationDto?> MarkSentAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken);

    Task<NotificationDto?> MarkFailedAsync(Guid id, string errorMessage, DateTimeOffset now, CancellationToken cancellationToken);
}

public interface INotificationService
{
    Task<NotificationDto> CreateAsync(Guid recipientUserId, NotificationType type, NotificationEventType eventType, string title, string message, CancellationToken cancellationToken);
}
