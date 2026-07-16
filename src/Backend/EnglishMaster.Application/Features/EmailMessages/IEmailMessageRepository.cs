using EnglishMaster.Application.Features.EmailMessages.Dtos;
using EnglishMaster.Domain.Notifications;

namespace EnglishMaster.Application.Features.EmailMessages;

public interface IEmailMessageRepository
{
    Task<EmailMessageDto> AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken);

    Task<EmailMessageSearchResponse> SearchAsync(string? status, string? toEmail, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<EmailMessageDto?> MarkSentAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken);

    Task<EmailMessageDto?> MarkFailedAsync(Guid id, string errorMessage, DateTimeOffset now, CancellationToken cancellationToken);
}

public interface IEmailSender
{
    Task SendAsync(EmailMessageDto message, CancellationToken cancellationToken);
}
