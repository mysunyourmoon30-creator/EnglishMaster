using EnglishMaster.Infrastructure.Monitoring;

namespace EnglishMaster.UnitTests.Monitoring;

public sealed class SystemHealthWorkerOptionsTests
{
    [Fact]
    public void Defaults_AreSensible()
    {
        var options = new SystemHealthWorkerOptions();

        Assert.True(options.Enabled);
        Assert.Equal(TimeSpan.FromMinutes(5), options.PollingInterval);
        Assert.Equal(3, options.ConsecutiveFailuresBeforeAlert);
        Assert.Equal(10, options.FailedEmailCountThreshold);
        Assert.Equal(5, options.FailedPublishJobCountThreshold);
        Assert.Equal(5, options.FailedImportJobCountThreshold);
        Assert.Equal(string.Empty, options.AlertRecipientEmail);
        Assert.Equal(TimeSpan.FromMinutes(60), options.AlertCooldown);
    }
}

public sealed class SystemHealthWorkerShouldAlertTests
{
    private static readonly DateTimeOffset Now = new(2026, 7, 18, 12, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(60);

    [Fact]
    public void BelowThreshold_DoesNotAlert()
    {
        Assert.False(SystemHealthWorker.ShouldAlert(currentValue: 2, threshold: 3, lastAlertAt: null, Cooldown, Now));
    }

    [Fact]
    public void AtThreshold_WithNoPriorAlert_Alerts()
    {
        Assert.True(SystemHealthWorker.ShouldAlert(currentValue: 3, threshold: 3, lastAlertAt: null, Cooldown, Now));
    }

    [Fact]
    public void AboveThreshold_WithNoPriorAlert_Alerts()
    {
        Assert.True(SystemHealthWorker.ShouldAlert(currentValue: 10, threshold: 3, lastAlertAt: null, Cooldown, Now));
    }

    [Fact]
    public void AboveThreshold_WithinCooldown_DoesNotAlertAgain()
    {
        var lastAlertAt = Now - TimeSpan.FromMinutes(30);

        Assert.False(SystemHealthWorker.ShouldAlert(currentValue: 10, threshold: 3, lastAlertAt, Cooldown, Now));
    }

    [Fact]
    public void AboveThreshold_AfterCooldownElapsed_AlertsAgain()
    {
        var lastAlertAt = Now - TimeSpan.FromMinutes(61);

        Assert.True(SystemHealthWorker.ShouldAlert(currentValue: 10, threshold: 3, lastAlertAt, Cooldown, Now));
    }

    [Fact]
    public void AboveThreshold_ExactlyAtCooldownBoundary_Alerts()
    {
        var lastAlertAt = Now - Cooldown;

        Assert.True(SystemHealthWorker.ShouldAlert(currentValue: 10, threshold: 3, lastAlertAt, Cooldown, Now));
    }
}
