namespace EnglishMaster.Domain.Quizzes;

public sealed class QuizQuestion
{
    private readonly List<QuizChoice> choices = [];

    private QuizQuestion()
    {
        QuestionText = string.Empty;
        ExplanationTh = string.Empty;
        ExplanationEn = string.Empty;
    }

    private QuizQuestion(
        Guid id,
        Guid quizId,
        string? questionText,
        QuizQuestionType questionType,
        string? explanationTh,
        string? explanationEn,
        int points,
        int sortOrder,
        Guid? wordId,
        Guid? grammarRuleId,
        Guid? pronunciationId,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = QuizDomainGuard.RequiredId(id, nameof(id));
        QuizId = QuizDomainGuard.RequiredId(quizId, nameof(quizId));
        CreatedAt = createdAt;
        Apply(
            questionText,
            questionType,
            explanationTh,
            explanationEn,
            points,
            sortOrder,
            wordId,
            grammarRuleId,
            pronunciationId,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public Guid QuizId { get; private set; }

    public string QuestionText { get; private set; } = string.Empty;

    public QuizQuestionType QuestionType { get; private set; }

    public string ExplanationTh { get; private set; } = string.Empty;

    public string ExplanationEn { get; private set; } = string.Empty;

    public int Points { get; private set; }

    public int SortOrder { get; private set; }

    public Guid? WordId { get; private set; }

    public Guid? GrammarRuleId { get; private set; }

    public Guid? PronunciationId { get; private set; }

    public IReadOnlyCollection<QuizChoice> Choices => choices.AsReadOnly();

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static QuizQuestion Create(
        Guid quizId,
        string? questionText,
        QuizQuestionType questionType,
        string? explanationTh,
        string? explanationEn,
        int points,
        int sortOrder,
        Guid? wordId,
        Guid? grammarRuleId,
        Guid? pronunciationId,
        DateTimeOffset now)
    {
        return new QuizQuestion(
            Guid.NewGuid(),
            quizId,
            questionText,
            questionType,
            explanationTh,
            explanationEn,
            points,
            sortOrder,
            wordId,
            grammarRuleId,
            pronunciationId,
            isActive: true,
            now,
            now);
    }

    public void Update(
        string? questionText,
        QuizQuestionType questionType,
        string? explanationTh,
        string? explanationEn,
        int points,
        int sortOrder,
        Guid? wordId,
        Guid? grammarRuleId,
        Guid? pronunciationId,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            questionText,
            questionType,
            explanationTh,
            explanationEn,
            points,
            sortOrder,
            wordId,
            grammarRuleId,
            pronunciationId,
            isActive,
            now);
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

    public bool CanAddCorrectChoice(Guid? excludedChoiceId = null)
    {
        return QuestionType == QuizQuestionType.MultipleChoice ||
            Choices.Count(choice => choice.IsActive && choice.IsCorrect && (!excludedChoiceId.HasValue || choice.Id != excludedChoiceId.Value)) == 0;
    }

    public QuizChoice AddChoice(
        string? choiceText,
        bool isCorrect,
        string? explanationTh,
        string? explanationEn,
        int sortOrder,
        DateTimeOffset now)
    {
        if (isCorrect && !CanAddCorrectChoice())
        {
            throw new InvalidOperationException($"{QuestionType} questions can have only one correct choice.");
        }

        var choice = QuizChoice.Create(
            Id,
            choiceText,
            isCorrect,
            explanationTh,
            explanationEn,
            sortOrder,
            now);
        choices.Add(choice);
        UpdatedAt = now;
        return choice;
    }

    public bool RemoveChoice(Guid choiceId, DateTimeOffset now)
    {
        QuizDomainGuard.RequiredId(choiceId, nameof(choiceId));
        var choice = choices.SingleOrDefault(item => item.Id == choiceId);
        if (choice is null)
        {
            return false;
        }

        choices.Remove(choice);
        UpdatedAt = now;
        return true;
    }

    private void Apply(
        string? questionText,
        QuizQuestionType questionType,
        string? explanationTh,
        string? explanationEn,
        int points,
        int sortOrder,
        Guid? wordId,
        Guid? grammarRuleId,
        Guid? pronunciationId,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        QuestionText = QuizDomainGuard.RequiredText(
            questionText,
            nameof(QuestionText),
            nameof(questionText),
            QuizQuestionFieldLimits.QuestionText);
        QuestionType = QuizDomainGuard.DefinedEnum(questionType, nameof(questionType));
        ExplanationTh = QuizDomainGuard.OptionalText(
            explanationTh,
            nameof(ExplanationTh),
            nameof(explanationTh),
            QuizQuestionFieldLimits.ExplanationTh);
        ExplanationEn = QuizDomainGuard.OptionalText(
            explanationEn,
            nameof(ExplanationEn),
            nameof(explanationEn),
            QuizQuestionFieldLimits.ExplanationEn);
        Points = QuizDomainGuard.Positive(points, nameof(points));
        SortOrder = QuizDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
        WordId = QuizDomainGuard.OptionalId(wordId, nameof(wordId));
        GrammarRuleId = QuizDomainGuard.OptionalId(grammarRuleId, nameof(grammarRuleId));
        PronunciationId = QuizDomainGuard.OptionalId(pronunciationId, nameof(pronunciationId));

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
