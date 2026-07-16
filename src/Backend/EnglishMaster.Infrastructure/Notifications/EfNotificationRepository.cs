using EnglishMaster.Application.Features.Notifications;
using EnglishMaster.Application.Features.Notifications.Dtos;
using EnglishMaster.Domain.Notifications;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Notifications;

public sealed class EfNotificationRepository : INotificationRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfNotificationRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<NotificationDto> AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(notification);
    }

    public async Task<NotificationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var notification = await dbContext.Notifications.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return notification is null ? null : ToDto(notification);
    }

    public async Task<NotificationDto?> GetByIdForRecipientAsync(Guid id, Guid recipientUserId, CancellationToken cancellationToken)
    {
        var notification = await dbContext.Notifications.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id && item.RecipientUserId == recipientUserId, cancellationToken);
        return notification is null ? null : ToDto(notification);
    }

    public async Task<NotificationSearchResponse> SearchAsync(
        Guid? recipientUserId,
        string? status,
        string? eventType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Notifications.AsNoTracking();
        if (recipientUserId.HasValue)
        {
            query = query.Where(notification => notification.RecipientUserId == recipientUserId.Value);
        }

        if (Enum.TryParse<NotificationStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(notification => notification.Status == parsedStatus);
        }

        if (Enum.TryParse<NotificationEventType>(eventType, ignoreCase: true, out var parsedEventType))
        {
            query = query.Where(notification => notification.EventType == parsedEventType);
        }

        var total = await query.CountAsync(cancellationToken);
        var notifications = await query
            .OrderByDescending(notification => notification.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        var items = notifications.Select(ToDto).ToArray();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new NotificationSearchResponse(items, pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    public async Task<int> CountUnreadAsync(Guid recipientUserId, CancellationToken cancellationToken) =>
        await dbContext.Notifications.AsNoTracking()
            .CountAsync(notification => notification.RecipientUserId == recipientUserId && notification.Status != NotificationStatus.Read, cancellationToken);

    public async Task<NotificationDto?> MarkReadAsync(Guid id, Guid recipientUserId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var notification = await dbContext.Notifications
            .SingleOrDefaultAsync(item => item.Id == id && item.RecipientUserId == recipientUserId, cancellationToken);
        if (notification is null)
        {
            return null;
        }

        notification.MarkRead(now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(notification);
    }

    public async Task<NotificationDto?> MarkSentAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var notification = await dbContext.Notifications.FindAsync([id], cancellationToken);
        if (notification is null)
        {
            return null;
        }

        notification.MarkSent(now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(notification);
    }

    public async Task<NotificationDto?> MarkFailedAsync(Guid id, string errorMessage, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var notification = await dbContext.Notifications.FindAsync([id], cancellationToken);
        if (notification is null)
        {
            return null;
        }

        notification.MarkFailed(errorMessage, now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(notification);
    }

    private static NotificationDto ToDto(Notification notification) =>
        new(
            notification.Id,
            notification.RecipientUserId,
            notification.Type.ToString(),
            notification.EventType.ToString(),
            notification.Title,
            notification.Message,
            notification.Status.ToString(),
            notification.ReadAt,
            notification.SentAt,
            notification.FailedAt,
            notification.ErrorMessage,
            notification.CreatedAt,
            notification.UpdatedAt);
}
