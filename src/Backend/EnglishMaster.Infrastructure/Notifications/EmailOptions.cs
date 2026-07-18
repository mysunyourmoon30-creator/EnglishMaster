namespace EnglishMaster.Infrastructure.Notifications;

public sealed class EmailOptions
{
    public string Provider { get; set; } = "Development";

    public string FromEmail { get; set; } = "noreply@englishmaster.local";

    public string FromName { get; set; } = "EnglishMaster";

    public SmtpEmailOptions Smtp { get; set; } = new();
}

public sealed class SmtpEmailOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public bool UseSsl { get; set; } = true;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
