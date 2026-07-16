using EnglishMaster.Application.Features.Notifications.Dtos;
using EnglishMaster.Domain.Notifications;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Notifications.Commands;

public sealed record CreateNotificationCommand(Guid RecipientUserId, string Type, string EventType, string Title, string Message);

public sealed record MarkNotificationAsReadCommand(Guid Id, Guid RecipientUserId);

public sealed record MarkNotificationAsSentCommand(Guid Id);

public sealed record MarkNotificationAsFailedCommand(Guid Id, string ErrorMessage);

public sealed class NotificationCommandHandler
{
    private readonly INotificationRepository repository;
    private readonly TimeProvider timeProvider;

    public NotificationCommandHandler(INotificationRepository repository, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<NotificationDto>> CreateAsync(CreateNotificationCommand command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<NotificationType>(command.Type, ignoreCase: true, out var type))
        {
            return Result<NotificationDto>.Validation(new ValidationError(nameof(command.Type), "Notification type is invalid."));
        }

        if (!Enum.TryParse<NotificationEventType>(command.EventType, ignoreCase: true, out var eventType))
        {
            return Result<NotificationDto>.Validation(new ValidationError(nameof(command.EventType), "Notification event type is invalid."));
        }

        try
        {
            var notification = Notification.Create(
                command.RecipientUserId,
                type,
                eventType,
                command.Title,
                command.Message,
                timeProvider.GetUtcNow());

            return Result<NotificationDto>.Success(await repository.AddAsync(notification, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<NotificationDto>.Validation(new ValidationError(exception.ParamName ?? "notification", exception.Message));
        }
    }

    public async Task<Result<NotificationDto>> MarkReadAsync(MarkNotificationAsReadCommand command, CancellationToken cancellationToken)
    {
        var notification = await repository.MarkReadAsync(command.Id, command.RecipientUserId, timeProvider.GetUtcNow(), cancellationToken);
        return notification is null
            ? Result<NotificationDto>.NotFound(nameof(command.Id), "Notification was not found.")
            : Result<NotificationDto>.Success(notification);
    }

    public async Task<Result<NotificationDto>> MarkSentAsync(MarkNotificationAsSentCommand command, CancellationToken cancellationToken)
    {
        var notification = await repository.MarkSentAsync(command.Id, timeProvider.GetUtcNow(), cancellationToken);
        return notification is null
            ? Result<NotificationDto>.NotFound(nameof(command.Id), "Notification was not found.")
            : Result<NotificationDto>.Success(notification);
    }

    public async Task<Result<NotificationDto>> MarkFailedAsync(MarkNotificationAsFailedCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var notification = await repository.MarkFailedAsync(command.Id, command.ErrorMessage, timeProvider.GetUtcNow(), cancellationToken);
            return notification is null
                ? Result<NotificationDto>.NotFound(nameof(command.Id), "Notification was not found.")
                : Result<NotificationDto>.Success(notification);
        }
        catch (ArgumentException exception)
        {
            return Result<NotificationDto>.Validation(new ValidationError(exception.ParamName ?? nameof(command.ErrorMessage), exception.Message));
        }
    }
}

public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository repository;
    private readonly TimeProvider timeProvider;

    public NotificationService(INotificationRepository repository, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<NotificationDto> CreateAsync(
        Guid recipientUserId,
        NotificationType type,
        NotificationEventType eventType,
        string title,
        string message,
        CancellationToken cancellationToken)
    {
        var notification = Notification.Create(recipientUserId, type, eventType, title, message, timeProvider.GetUtcNow());
        return await repository.AddAsync(notification, cancellationToken);
    }
}
