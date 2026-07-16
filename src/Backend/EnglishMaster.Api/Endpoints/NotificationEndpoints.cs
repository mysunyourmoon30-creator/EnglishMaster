using System.Security.Claims;
using EnglishMaster.Application.Features.EmailMessages.Commands;
using EnglishMaster.Application.Features.EmailMessages.Queries;
using EnglishMaster.Application.Features.Notifications.Commands;
using EnglishMaster.Application.Features.Notifications.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.EmailMessages;
using EnglishMaster.Contracts.Notifications;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var me = endpoints.MapGroup("/api/v1/me/notifications")
            .WithTags("My Notifications")
            .RequireAuthorization(Permissions.NotificationsRead);
        me.MapGet("", GetMyNotificationsAsync);
        me.MapGet("/unread-count", GetUnreadCountAsync);
        me.MapPost("/{id:guid}/read", MarkMyNotificationReadAsync);

        endpoints.MapGet("/api/v1/admin/notifications", SearchNotificationsAsync)
            .WithTags("Admin Notifications")
            .RequireAuthorization(Permissions.NotificationsManage);

        endpoints.MapGet("/api/v1/admin/email-messages", SearchEmailMessagesAsync)
            .WithTags("Admin Email Messages")
            .RequireAuthorization(Permissions.EmailRead);

        endpoints.MapGet("/api/v1/admin/email-provider/status", GetEmailProviderStatusAsync)
            .WithTags("Admin Email Provider")
            .RequireAuthorization(Permissions.EmailRead);

        endpoints.MapPost("/api/v1/admin/email-provider/test-send", SendTestEmailAsync)
            .WithTags("Admin Email Provider")
            .RequireAuthorization(Permissions.EmailManage);

        endpoints.MapPost("/api/v1/admin/email-delivery/process", ProcessEmailDeliveryQueueAsync)
            .WithTags("Admin Email Delivery")
            .RequireAuthorization(Permissions.EmailManage);

        endpoints.MapPost("/api/v1/admin/email-messages", QueueEmailMessageAsync)
            .WithTags("Admin Email Messages")
            .RequireAuthorization(Permissions.EmailManage);

        return endpoints;
    }

    private static async Task<IResult> GetMyNotificationsAsync(
        ClaimsPrincipal user,
        NotificationQueryHandler handler,
        string? status,
        string? eventType,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(user);
        return userId is null
            ? Results.Unauthorized()
            : ToHttpResult(await handler.GetMyNotificationsAsync(
                new GetMyNotificationsQuery(userId.Value, status, eventType, pageNumber, pageSize),
                cancellationToken));
    }

    private static async Task<IResult> GetUnreadCountAsync(
        ClaimsPrincipal user,
        NotificationQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(user);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetUnreadCountAsync(new GetUnreadNotificationCountQuery(userId.Value), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Ok(new UnreadNotificationCountResponse(result.Value))
            : ToHttpResult(result);
    }

    private static async Task<IResult> MarkMyNotificationReadAsync(
        Guid id,
        ClaimsPrincipal user,
        NotificationCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(user);
        return userId is null
            ? Results.Unauthorized()
            : ToHttpResult(await handler.MarkReadAsync(new MarkNotificationAsReadCommand(id, userId.Value), cancellationToken));
    }

    private static async Task<IResult> SearchNotificationsAsync(
        NotificationQueryHandler handler,
        string? status,
        string? eventType,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchNotificationsQuery(status, eventType, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> SearchEmailMessagesAsync(
        EmailMessageQueryHandler handler,
        string? status,
        string? toEmail,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchEmailMessagesQuery(status, toEmail, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> GetEmailProviderStatusAsync(
        EmailProviderQueryHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetStatusAsync(new GetEmailProviderStatusQuery(), cancellationToken));

    private static async Task<IResult> SendTestEmailAsync(
        SendTestEmailRequest request,
        EmailMessageCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.SendTestAsync(
            new SendTestEmailCommand(request.ToEmail, request.ToName, request.Subject, request.Body, request.IsHtml),
            cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> ProcessEmailDeliveryQueueAsync(
        ProcessEmailQueueRequest request,
        EmailMessageCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.ProcessPendingQueueAsync(new ProcessPendingEmailQueueCommand(request.MaxItems), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Ok(new EmailDeliveryQueueProcessResponse(
                result.Value!.Requested,
                result.Value.Processed,
                result.Value.Sent,
                result.Value.Failed))
            : ToHttpResult(result);
    }

    private static async Task<IResult> QueueEmailMessageAsync(
        QueueEmailMessageRequest request,
        EmailMessageCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.QueueAsync(
            new QueueEmailMessageCommand(request.ToEmail, request.ToName, request.Subject, request.Body, request.IsHtml),
            cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/admin/email-messages/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static Guid? GetUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    private static IResult ToHttpResult(Result result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.NoContent(),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.Ok(result.Value),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static Dictionary<string, string[]> ToValidationDictionary(IEnumerable<ValidationError> errors)
    {
        return errors
            .GroupBy(error => error.Field)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());
    }
}
