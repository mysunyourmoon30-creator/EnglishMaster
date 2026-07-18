namespace EnglishMaster.Infrastructure.Monitoring;

public sealed class SystemHealthWorkerOptions
{
    public bool Enabled { get; set; } = true;

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(5);

    public int ConsecutiveFailuresBeforeAlert { get; set; } = 3;

    public int FailedEmailCountThreshold { get; set; } = 10;

    public int FailedPublishJobCountThreshold { get; set; } = 5;

    public int FailedImportJobCountThreshold { get; set; } = 5;

    public string AlertRecipientEmail { get; set; } = string.Empty;

    public TimeSpan AlertCooldown { get; set; } = TimeSpan.FromMinutes(60);
}
