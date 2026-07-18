namespace EnglishMaster.Infrastructure.Notifications;

public sealed class EmailDeliveryWorkerOptions
{
    public bool Enabled { get; set; } = true;

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(60);

    public int MaxItemsPerRun { get; set; } = 10;
}
