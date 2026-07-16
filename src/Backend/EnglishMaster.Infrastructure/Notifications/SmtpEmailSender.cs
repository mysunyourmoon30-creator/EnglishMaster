using System.Net;
using System.Net.Mail;
using EnglishMaster.Application.Features.EmailMessages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnglishMaster.Infrastructure.Notifications;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions options;
    private readonly ILogger<SmtpEmailSender> logger;

    public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task SendAsync(EmailSendRequest request, CancellationToken cancellationToken)
    {
        if (!EmailProviderStatusService.IsSmtpConfigured(options))
        {
            throw new InvalidOperationException("SMTP email provider is not configured.");
        }

        try
        {
            await SendCoreAsync(request, cancellationToken);
            logger.LogInformation("SMTP email sent to {ToEmail} with subject {Subject}.", request.ToEmail, request.Subject);
        }
        catch (FormatException exception)
        {
            logger.LogWarning(exception, "SMTP email delivery failed because an address was invalid.");
            throw new InvalidOperationException("Email provider configuration or recipient address is invalid.");
        }
        catch (SmtpException exception)
        {
            logger.LogWarning(exception, "SMTP email delivery failed for {ToEmail}.", request.ToEmail);
            throw new InvalidOperationException("Email provider failed to send the message.");
        }
    }

    private async Task SendCoreAsync(EmailSendRequest request, CancellationToken cancellationToken)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(options.FromEmail, options.FromName),
            Subject = request.Subject,
            Body = request.Body,
            IsBodyHtml = request.IsHtml
        };
        message.To.Add(new MailAddress(request.ToEmail, request.ToName));

#pragma warning disable SYSLIB0014
        using var client = new SmtpClient(options.Smtp.Host, options.Smtp.Port)
        {
            EnableSsl = options.Smtp.UseSsl
        };
#pragma warning restore SYSLIB0014

        if (!string.IsNullOrWhiteSpace(options.Smtp.UserName))
        {
            client.Credentials = new NetworkCredential(options.Smtp.UserName, options.Smtp.Password);
        }

        await client.SendMailAsync(message, cancellationToken);
    }
}
