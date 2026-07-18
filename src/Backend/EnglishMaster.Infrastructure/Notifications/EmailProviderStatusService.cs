using EnglishMaster.Application.Features.EmailMessages;
using Microsoft.Extensions.Options;

namespace EnglishMaster.Infrastructure.Notifications;

public sealed class EmailProviderStatusService : IEmailProviderStatusService
{
    private readonly EmailOptions options;

    public EmailProviderStatusService(IOptions<EmailOptions> options)
    {
        this.options = options.Value;
    }

    public EmailProviderStatusDto GetStatus()
    {
        var provider = NormalizeProvider(options.Provider);
        return new EmailProviderStatusDto(
            provider,
            provider == "Development" || IsSmtpConfigured(options),
            options.FromEmail,
            options.FromName,
            SupportsTestSend: true);
    }

    internal static string NormalizeProvider(string? provider) =>
        string.Equals(provider, "Smtp", StringComparison.OrdinalIgnoreCase) ? "Smtp" : "Development";

    internal static bool IsSmtpConfigured(EmailOptions options) =>
        !string.IsNullOrWhiteSpace(options.FromEmail) &&
        !string.IsNullOrWhiteSpace(options.Smtp.Host) &&
        options.Smtp.Port > 0;
}
