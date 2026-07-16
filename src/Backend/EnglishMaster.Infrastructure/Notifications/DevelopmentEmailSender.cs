using EnglishMaster.Application.Features.EmailMessages;
using Microsoft.Extensions.Logging;

namespace EnglishMaster.Infrastructure.Notifications;

public sealed class DevelopmentEmailSender : IEmailSender
{
    private readonly ILogger<DevelopmentEmailSender> logger;

    public DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger)
    {
        this.logger = logger;
    }

    public Task SendAsync(EmailSendRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Development email queued for {ToEmail} with subject {Subject}.",
            request.ToEmail,
            request.Subject);
        return Task.CompletedTask;
    }
}
