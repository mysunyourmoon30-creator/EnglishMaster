namespace EnglishMaster.Domain.LearningReports;

public sealed class WeeklyLearningReportInsight
{
    private WeeklyLearningReportInsight()
    {
        InsightType = string.Empty;
        Severity = string.Empty;
        Message = string.Empty;
        Recommendation = string.Empty;
    }

    private WeeklyLearningReportInsight(Guid weeklyLearningReportId, string insightType, string severity, string message, string? recommendation, int sortOrder, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        WeeklyLearningReportId = weeklyLearningReportId == Guid.Empty ? throw new ArgumentException("WeeklyLearningReportId is required.", nameof(weeklyLearningReportId)) : weeklyLearningReportId;
        InsightType = RequiredText(insightType, nameof(InsightType), 64);
        Severity = RequiredText(severity, nameof(Severity), 32);
        Message = RequiredText(message, nameof(Message), 300);
        Recommendation = recommendation?.Trim() ?? string.Empty;
        SortOrder = sortOrder < 0 ? throw new ArgumentException("SortOrder must not be negative.", nameof(sortOrder)) : sortOrder;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid WeeklyLearningReportId { get; private set; }
    public string InsightType { get; private set; } = string.Empty;
    public string Severity { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string Recommendation { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static WeeklyLearningReportInsight Create(Guid reportId, string type, string severity, string message, string? recommendation, int sortOrder, DateTimeOffset now) =>
        new(reportId, type, severity, message, recommendation, sortOrder, now);

    private static string RequiredText(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized.Length > maxLength ? throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName) : normalized;
    }
}
