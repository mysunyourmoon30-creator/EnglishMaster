using EnglishMaster.Application.Features.EmailMessages.Dtos;
using EnglishMaster.Domain.Notifications;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.EmailMessages.Commands;

public sealed record QueueEmailMessageCommand(string ToEmail, string? ToName, string Subject, string Body, bool IsHtml);

public sealed record MarkEmailAsSentCommand(Guid Id);

public sealed record MarkEmailAsFailedCommand(Guid Id, string ErrorMessage);

public sealed record SendTestEmailCommand(string ToEmail, string? ToName, string Subject, string Body, bool IsHtml);

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
                Required(command.ToEmail, nameof(command.ToEmail), 256),
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

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = Optional(value, fieldName, maxLength);
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
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
}
