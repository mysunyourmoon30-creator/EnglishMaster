namespace EnglishMaster.Domain.Certificates;

public sealed class IssuedCertificate
{
    private IssuedCertificate()
    {
        VerificationCode = string.Empty;
        RecipientName = string.Empty;
        CourseTitle = string.Empty;
        TemplateCode = string.Empty;
        RenderedBody = string.Empty;
    }

    private IssuedCertificate(Guid userId, Guid courseId, Guid templateId, string verificationCode, string recipientName, string courseTitle, string templateCode, string renderedBody, DateTimeOffset issuedAt)
    {
        Id = Guid.NewGuid();
        UserId = RequiredId(userId, nameof(UserId));
        CourseId = RequiredId(courseId, nameof(CourseId));
        TemplateId = RequiredId(templateId, nameof(TemplateId));
        VerificationCode = Required(verificationCode, nameof(VerificationCode), 80);
        RecipientName = Required(recipientName, nameof(RecipientName), 200);
        CourseTitle = Required(courseTitle, nameof(CourseTitle), 200);
        TemplateCode = Required(templateCode, nameof(TemplateCode), 64);
        RenderedBody = Required(renderedBody, nameof(RenderedBody), 8000);
        IssuedAt = issuedAt;
        RevokedAt = null;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid TemplateId { get; private set; }
    public string VerificationCode { get; private set; } = string.Empty;
    public string RecipientName { get; private set; } = string.Empty;
    public string CourseTitle { get; private set; } = string.Empty;
    public string TemplateCode { get; private set; } = string.Empty;
    public string RenderedBody { get; private set; } = string.Empty;
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public bool IsRevoked => RevokedAt.HasValue;

    public static IssuedCertificate Create(Guid userId, Guid courseId, Guid templateId, string verificationCode, string recipientName, string courseTitle, string templateCode, string renderedBody, DateTimeOffset issuedAt) =>
        new(userId, courseId, templateId, verificationCode, recipientName, courseTitle, templateCode, renderedBody, issuedAt);

    public void Revoke(DateTimeOffset now) =>
        RevokedAt = now;

    private static Guid RequiredId(Guid value, string fieldName) =>
        value == Guid.Empty ? throw new ArgumentException($"{fieldName} is required.", fieldName) : value;

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }
}
