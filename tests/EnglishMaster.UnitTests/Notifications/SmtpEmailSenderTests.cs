using EnglishMaster.Application.Features.EmailMessages;
using EnglishMaster.Infrastructure.Notifications;

namespace EnglishMaster.UnitTests.Notifications;

public sealed class SmtpEmailSenderTests
{
    [Fact]
    public void BuildMessage_WithHtmlBody_SetsHtmlBodyAndHeaders()
    {
        var options = new EmailOptions { FromEmail = "noreply@englishmaster.test", FromName = "EnglishMaster" };
        var request = new EmailSendRequest("learner@example.test", "Learner", "Welcome", "<p>Hi</p>", IsHtml: true);

        var message = SmtpEmailSender.BuildMessage(options, request);

        Assert.Equal("EnglishMaster", message.From[0].Name);
        Assert.Contains("noreply@englishmaster.test", message.From.ToString());
        Assert.Contains("learner@example.test", message.To.ToString());
        Assert.Equal("Welcome", message.Subject);
        Assert.Equal("<p>Hi</p>", message.HtmlBody);
    }

    [Fact]
    public void BuildMessage_WithPlainTextBody_SetsTextBody()
    {
        var options = new EmailOptions { FromEmail = "noreply@englishmaster.test", FromName = "EnglishMaster" };
        var request = new EmailSendRequest("learner@example.test", null, "Reminder", "Plain text body", IsHtml: false);

        var message = SmtpEmailSender.BuildMessage(options, request);

        Assert.Equal("Plain text body", message.TextBody);
        Assert.Null(message.HtmlBody);
    }
}

public sealed class EmailProviderStatusServiceTests
{
    [Theory]
    [InlineData("Smtp", "Smtp")]
    [InlineData("smtp", "Smtp")]
    [InlineData("Development", "Development")]
    [InlineData("", "Development")]
    [InlineData(null, "Development")]
    [InlineData("Unknown", "Development")]
    public void NormalizeProvider_ReturnsExpectedCanonicalValue(string? input, string expected)
    {
        Assert.Equal(expected, EmailProviderStatusService.NormalizeProvider(input));
    }

    [Fact]
    public void IsSmtpConfigured_TrueWhenHostFromEmailAndPortSet()
    {
        var options = new EmailOptions
        {
            FromEmail = "noreply@englishmaster.test",
            Smtp = new SmtpEmailOptions { Host = "smtp.example.test", Port = 587 }
        };

        Assert.True(EmailProviderStatusService.IsSmtpConfigured(options));
    }

    [Theory]
    [InlineData("", "smtp.example.test", 587)]
    [InlineData("noreply@englishmaster.test", "", 587)]
    [InlineData("noreply@englishmaster.test", "smtp.example.test", 0)]
    public void IsSmtpConfigured_FalseWhenAnyRequiredFieldMissing(string fromEmail, string host, int port)
    {
        var options = new EmailOptions
        {
            FromEmail = fromEmail,
            Smtp = new SmtpEmailOptions { Host = host, Port = port }
        };

        Assert.False(EmailProviderStatusService.IsSmtpConfigured(options));
    }
}
