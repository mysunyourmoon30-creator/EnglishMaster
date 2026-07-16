using EnglishMaster.Application.Features.EmailMessages.Dtos;
using EnglishMaster.Domain.Notifications;
using EnglishMaster.Shared.Results;
using System.Net.Mail;

namespace EnglishMaster.Application.Features.EmailMessages.Commands;

public sealed record QueueEmailMessageCommand(string ToEmail, string? ToName, string Subject, string Body, bool IsHtml);

public sealed record MarkEmailAsSentCommand(Guid Id);

public sealed record MarkEmailAsFailedCommand(Guid Id, string ErrorMessage);

public sealed record SendTestEmailCommand(string ToEmail, string? ToName, string Subject, string Body, bool IsHtml);

public sealed record ProcessPendingEmailQueueCommand(int? MaxItems);

public sealed record RetryFailedEmailCommand(Guid Id);

public sealed class EmailMessageCommandHandler
{
    private readonly IEmailMessageRepository repository;
    private readonly IEmailSender emailSender;
    private readonly TimeProvider timeProvider;

    public EmailMessageCommandHandler(IEmailMessageRepository repository, IEmailSender emailSender, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.emailSender = emailSender;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<EmailMessageDto>> QueueAsync(QueueEmailMessageCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var email = EmailMessage.Queue(command.ToEmail, command.ToName, command.Subject, command.Body, command.IsHtml, timeProvider.GetUtcNow());
            return Result<EmailMessageDto>.Success(await repository.AddAsync(email, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<EmailMessageDto>.Validation(new ValidationError(exception.ParamName ?? "emailMessage", exception.Message));
        }
    }

    public async Task<Result<EmailMessageDto>> MarkSentAsync(MarkEmailAsSentCommand command, CancellationToken cancellationToken)
    {
        var email = await repository.MarkSentAsync(command.Id, timeProvider.GetUtcNow(), cancellationToken);
        return email is null
            ? Result<EmailMessageDto>.NotFound(nameof(command.Id), "Email message was not found.")
            : Result<EmailMessageDto>.Success(email);
    }

    public async Task<Result<EmailMessageDto>> MarkFailedAsync(MarkEmailAsFailedCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var email = await repository.MarkFailedAsync(command.Id, command.ErrorMessage, timeProvider.GetUtcNow(), cancellationToken);
            return email is null
                ? Result<EmailMessageDto>.NotFound(nameof(command.Id), "Email message was not found.")
                : Result<EmailMessageDto>.Success(email);
        }
        catch (ArgumentException exception)
        {
            return Result<EmailMessageDto>.Validation(new ValidationError(exception.ParamName ?? nameof(command.ErrorMessage), exception.Message));
        }
    }

    public async Task<Result> SendTestAsync(SendTestEmailCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var request = new EmailSendRequest(
                EmailAddress(command.ToEmail, nameof(command.ToEmail)),
                Optional(command.ToName, nameof(command.ToName), 256),
                Required(command.Subject, nameof(command.Subject), 256),
                Required(command.Body, nameof(command.Body), 8000),
                command.IsHtml);

            await emailSender.SendAsync(request, cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException exception)
        {
            return Result.Validation(new ValidationError(exception.ParamName ?? "email", exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result.Validation(new ValidationError("emailProvider", exception.Message));
        }
    }

    public async Task<Result<EmailDeliveryQueueProcessResult>> ProcessPendingQueueAsync(ProcessPendingEmailQueueCommand command, CancellationToken cancellationToken)
    {
        var maxItems = Math.Clamp(command.MaxItems ?? 10, 1, 50);
        var pending = await repository.GetPendingAsync(maxItems, cancellationToken);
        var sent = 0;
        var failed = 0;

        foreach (var email in pending)
        {
            try
            {
                await emailSender.SendAsync(
                    new EmailSendRequest(email.ToEmail, email.ToName, email.Subject, email.Body, email.IsHtml),
                    cancellationToken);
                await repository.MarkSentAsync(email.Id, timeProvider.GetUtcNow(), cancellationToken);
                sent++;
            }
            catch (Exception exception) when (exception is InvalidOperationException or TimeoutException)
            {
                await repository.MarkFailedAsync(email.Id, SanitizeError(exception.Message), timeProvider.GetUtcNow(), cancellationToken);
                failed++;
            }
        }

        return Result<EmailDeliveryQueueProcessResult>.Success(new EmailDeliveryQueueProcessResult(maxItems, pending.Count, sent, failed));
    }

    public async Task<Result<EmailMessageDto>> RetryFailedAsync(RetryFailedEmailCommand command, CancellationToken cancellationToken)
    {
        var email = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (email is null)
        {
            return Result<EmailMessageDto>.NotFound(nameof(command.Id), "Email message was not found.");
        }

        if (!string.Equals(email.Status, EmailMessageStatus.Failed.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return Result<EmailMessageDto>.Validation(new ValidationError(nameof(command.Id), "Only failed email messages can be retried."));
        }

        try
        {
            await emailSender.SendAsync(
                new EmailSendRequest(email.ToEmail, email.ToName, email.Subject, email.Body, email.IsHtml),
                cancellationToken);
            var sent = await repository.MarkSentAsync(email.Id, timeProvider.GetUtcNow(), cancellationToken);
            return Result<EmailMessageDto>.Success(sent!);
        }
        catch (Exception exception) when (exception is InvalidOperationException or TimeoutException)
        {
            var failed = await repository.MarkFailedAsync(email.Id, SanitizeError(exception.Message), timeProvider.GetUtcNow(), cancellationToken);
            return Result<EmailMessageDto>.Success(failed!);
        }
    }

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = Optional(value, fieldName, maxLength);
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string EmailAddress(string? value, string fieldName)
    {
        var normalized = Required(value, fieldName, 256);
        try
        {
            _ = new MailAddress(normalized);
            return normalized;
        }
        catch (FormatException)
        {
            throw new ArgumentException($"{fieldName} must be a valid email address.", fieldName);
        }
    }

    private static string Optional(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }

    private static string SanitizeError(string message)
    {
        var normalized = string.IsNullOrWhiteSpace(message) ? "Email delivery failed." : message.Trim();
        return normalized.Length > 300 ? normalized[..300] : normalized;
    }
}
