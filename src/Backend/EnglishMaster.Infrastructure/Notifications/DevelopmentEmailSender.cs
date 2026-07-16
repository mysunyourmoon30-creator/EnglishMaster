using EnglishMaster.Application.Features.EmailMessages;
using EnglishMaster.Application.Features.EmailMessages.Dtos;
using Microsoft.Extensions.Logging;

namespace EnglishMaster.Infrastructure.Notifications;

public sealed class DevelopmentEmailSender : IEmailSender
{
    private readonly ILogger<DevelopmentEmailSender> logger;

    public DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger)
    {
        this.logger = logger;
    }

    public Task SendAsync(EmailMessageDto message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Development email queued for {ToEmail} with subject {Subject}.",
            message.ToEmail,
            message.Subject);
        return Task.CompletedTask;
    }
}
