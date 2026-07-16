namespace EnglishMaster.Domain.Practice;

public sealed class PracticeSessionItem
{
    private PracticeSessionItem()
    {
        ContentType = string.Empty;
        PracticeType = string.Empty;
        PromptText = string.Empty;
        AnswerText = string.Empty;
        UserAnswer = string.Empty;
    }

    private PracticeSessionItem(Guid practiceSessionId, Guid practiceItemId, string contentType, Guid contentId, string practiceType, string promptText, string answerText, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        PracticeSessionId = RequiredId(practiceSessionId, nameof(practiceSessionId));
        PracticeItemId = RequiredId(practiceItemId, nameof(practiceItemId));
        ContentType = RequiredText(contentType, nameof(ContentType), 64);
        ContentId = RequiredId(contentId, nameof(contentId));
        PracticeType = RequiredText(practiceType, nameof(PracticeType), 64);
        PromptText = RequiredText(promptText, nameof(PromptText), 1000);
        AnswerText = answerText?.Trim() ?? string.Empty;
        UserAnswer = string.Empty;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid PracticeSessionId { get; private set; }
    public Guid PracticeItemId { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public Guid ContentId { get; private set; }
    public string PracticeType { get; private set; } = string.Empty;
    public string PromptText { get; private set; } = string.Empty;
    public string AnswerText { get; private set; } = string.Empty;
    public string UserAnswer { get; private set; } = string.Empty;
    public PracticeResult? Result { get; private set; }
    public bool? IsCorrect { get; private set; }
    public DateTimeOffset? PracticedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static PracticeSessionItem Create(Guid practiceSessionId, Guid practiceItemId, string contentType, Guid contentId, string practiceType, string promptText, string answerText, DateTimeOffset now) =>
        new(practiceSessionId, practiceItemId, contentType, contentId, practiceType, promptText, answerText, now);

    public void Submit(string? userAnswer, PracticeResult result, DateTimeOffset now)
    {
        if (PracticedAt.HasValue)
        {
            throw new InvalidOperationException("Practice session item has already been submitted.");
        }

        UserAnswer = userAnswer?.Trim() ?? string.Empty;
        Result = result;
        IsCorrect = result != PracticeResult.Again;
        PracticedAt = now;
        UpdatedAt = now;
    }

    private static Guid RequiredId(Guid value, string fieldName) =>
        value == Guid.Empty ? throw new ArgumentException($"{fieldName} is required.", fieldName) : value;

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
