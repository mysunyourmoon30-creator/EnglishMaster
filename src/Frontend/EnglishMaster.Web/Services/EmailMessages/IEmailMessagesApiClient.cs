using EnglishMaster.Contracts.EmailMessages;

namespace EnglishMaster.Web.Services.EmailMessages;

public interface IEmailMessagesApiClient
{
    Task<EmailMessageSearchResponse> SearchAsync(string? status, string? toEmail, CancellationToken cancellationToken);

    Task<EmailMessageDto> QueueAsync(QueueEmailMessageRequest request, CancellationToken cancellationToken);
}
