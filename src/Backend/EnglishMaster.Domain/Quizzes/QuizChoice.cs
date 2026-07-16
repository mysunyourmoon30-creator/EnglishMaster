namespace EnglishMaster.Domain.Quizzes;

public sealed class QuizChoice
{
    private QuizChoice()
    {
        ChoiceText = string.Empty;
        ExplanationTh = string.Empty;
        ExplanationEn = string.Empty;
    }

    private QuizChoice(
        Guid id,
        Guid quizQuestionId,
        string? choiceText,
        bool isCorrect,
        string? explanationTh,
        string? explanationEn,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = QuizDomainGuard.RequiredId(id, nameof(id));
        QuizQuestionId = QuizDomainGuard.RequiredId(quizQuestionId, nameof(quizQuestionId));
        CreatedAt = createdAt;
        Apply(choiceText, isCorrect, explanationTh, explanationEn, sortOrder, isActive, updatedAt);
    }

    public Guid Id { get; private set; }

    public Guid QuizQuestionId { get; private set; }

    public string ChoiceText { get; private set; } = string.Empty;

    public bool IsCorrect { get; private set; }

    public string ExplanationTh { get; private set; } = string.Empty;

    public string ExplanationEn { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static QuizChoice Create(
        Guid quizQuestionId,
        string? choiceText,
        bool isCorrect,
        string? explanationTh,
        string? explanationEn,
        int sortOrder,
        DateTimeOffset now)
    {
        return new QuizChoice(
            Guid.NewGuid(),
            quizQuestionId,
            choiceText,
            isCorrect,
            explanationTh,
            explanationEn,
            sortOrder,
            isActive: true,
            now,
            now);
    }

    public void Update(
        string? choiceText,
        bool isCorrect,
        string? explanationTh,
        string? explanationEn,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(choiceText, isCorrect, explanationTh, explanationEn, sortOrder, isActive, now);
    }

    public void Activate(DateTimeOffset now)
    {
        IsActive = true;
        UpdatedAt = now;
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        UpdatedAt = now;
    }

    public void Reorder(int sortOrder, DateTimeOffset now)
    {
        SortOrder = QuizDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
        UpdatedAt = now;
    }

    private void Apply(
        string? choiceText,
        bool isCorrect,
        string? explanationTh,
        string? explanationEn,
        int sortOrder,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        ChoiceText = QuizDomainGuard.RequiredText(
            choiceText,
            nameof(ChoiceText),
            nameof(choiceText),
            QuizChoiceFieldLimits.ChoiceText);
        IsCorrect = isCorrect;
        ExplanationTh = QuizDomainGuard.OptionalText(
            explanationTh,
            nameof(ExplanationTh),
            nameof(explanationTh),
            QuizChoiceFieldLimits.ExplanationTh);
        ExplanationEn = QuizDomainGuard.OptionalText(
            explanationEn,
            nameof(ExplanationEn),
            nameof(explanationEn),
            QuizChoiceFieldLimits.ExplanationEn);
        SortOrder = QuizDomainGuard.NonNegative(sortOrder, nameof(sortOrder));

        if (isActive)
        {
            Activate(updatedAt);
        }
        else
        {
            Deactivate(updatedAt);
        }
    }
}
