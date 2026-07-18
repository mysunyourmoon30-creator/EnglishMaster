namespace EnglishMaster.Contracts.SystemHealth;

public sealed record SystemHealthResponse(
    bool DatabaseHealthy,
    int FailedEmailCount,
    int FailedPublishJobCount,
    int FailedImportJobCount,
    DateTimeOffset CheckedAt);
