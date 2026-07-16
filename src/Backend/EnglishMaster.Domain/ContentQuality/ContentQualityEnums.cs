namespace EnglishMaster.Domain.ContentQuality;

public enum ContentQualitySeverity
{
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}

public enum ContentQualityCheckStatus
{
    Passed = 1,
    Failed = 2,
    Warning = 3,
    NotChecked = 4
}
