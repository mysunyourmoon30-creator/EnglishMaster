using EnglishMaster.Application.Features.EmailMessages;
using Microsoft.Extensions.Options;

namespace EnglishMaster.Infrastructure.Notifications;

public sealed class ConfiguredEmailSender : IEmailSender
{
    private readonly EmailOptions options;
    private readonly DevelopmentEmailSender developmentSender;
    private readonly SmtpEmailSender smtpSender;

    public ConfiguredEmailSender(
        IOptions<EmailOptions> options,
        DevelopmentEmailSender developmentSender,
        SmtpEmailSender smtpSender)
    {
        this.options = options.Value;
        this.developmentSender = developmentSender;
        this.smtpSender = smtpSender;
    }

    public Task SendAsync(EmailSendRequest request, CancellationToken cancellationToken) =>
        EmailProviderStatusService.NormalizeProvider(options.Provider) == "Smtp"
            ? smtpSender.SendAsync(request, cancellationToken)
            : developmentSender.SendAsync(request, cancellationToken);
}
