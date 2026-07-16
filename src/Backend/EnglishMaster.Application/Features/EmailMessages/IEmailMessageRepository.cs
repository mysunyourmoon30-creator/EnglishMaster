using EnglishMaster.Application.Features.EmailMessages.Dtos;
using EnglishMaster.Domain.Notifications;

namespace EnglishMaster.Application.Features.EmailMessages;

public interface IEmailMessageRepository
{
    Task<EmailMessageDto> AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<EmailMessageDto>> GetPendingAsync(int maxItems, CancellationToken cancellationToken);

    Task<EmailMessageSearchResponse> SearchAsync(string? status, string? toEmail, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<EmailMessageDto?> MarkSentAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken);

    Task<EmailMessageDto?> MarkFailedAsync(Guid id, string errorMessage, DateTimeOffset now, CancellationToken cancellationToken);
}

public interface IEmailSender
{
    Task SendAsync(EmailSendRequest request, CancellationToken cancellationToken);
}

public sealed record EmailSendRequest(
    string ToEmail,
    string? ToName,
    string Subject,
    string Body,
    bool IsHtml);

public sealed record EmailProviderStatusDto(
    string Provider,
    bool IsConfigured,
    string FromEmail,
    string FromName,
    bool SupportsTestSend);

public sealed record EmailDeliveryQueueProcessResult(
    int Requested,
    int Processed,
    int Sent,
    int Failed);

public interface IEmailProviderStatusService
{
    EmailProviderStatusDto GetStatus();
}
