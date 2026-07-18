using System.Net.Sockets;
using EnglishMaster.Application.Features.EmailMessages;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

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
            var message = BuildMessage(options, request);
            await SendCoreAsync(message, cancellationToken);
            logger.LogInformation("SMTP email sent to {ToEmail} with subject {Subject}.", request.ToEmail, request.Subject);
        }
        catch (FormatException exception)
        {
            logger.LogWarning(exception, "SMTP email delivery failed because an address was invalid.");
            throw new InvalidOperationException("Email provider configuration or recipient address is invalid.");
        }
        catch (Exception exception) when (
            exception is AuthenticationException
            or SmtpCommandException
            or SmtpProtocolException
            or SocketException
            or IOException)
        {
            logger.LogWarning(exception, "SMTP email delivery failed for {ToEmail}.", request.ToEmail);
            throw new InvalidOperationException("Email provider failed to send the message.");
        }
    }

    internal static MimeMessage BuildMessage(EmailOptions options, EmailSendRequest request)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.FromName, options.FromEmail));
        message.To.Add(new MailboxAddress(request.ToName, request.ToEmail));
        message.Subject = request.Subject;

        var builder = new BodyBuilder();
        if (request.IsHtml)
        {
            builder.HtmlBody = request.Body;
        }
        else
        {
            builder.TextBody = request.Body;
        }

        message.Body = builder.ToMessageBody();
        return message;
    }

    private async Task SendCoreAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(
            options.Smtp.Host,
            options.Smtp.Port,
            options.Smtp.UseSsl ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.None,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(options.Smtp.UserName))
        {
            await client.AuthenticateAsync(options.Smtp.UserName, options.Smtp.Password, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
