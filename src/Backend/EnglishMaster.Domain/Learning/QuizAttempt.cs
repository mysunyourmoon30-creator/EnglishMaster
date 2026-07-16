namespace EnglishMaster.Domain.Learning;

public sealed class QuizAttempt
{
    private QuizAttempt()
    {
    }

    private QuizAttempt(Guid userId, Guid quizId, decimal score, bool passed, DateTimeOffset attemptedAt, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        UserId = userId == Guid.Empty ? throw new ArgumentException("UserId is required.", nameof(userId)) : userId;
        QuizId = quizId == Guid.Empty ? throw new ArgumentException("QuizId is required.", nameof(quizId)) : quizId;
        Score = Math.Clamp(score, 0, 100);
        Passed = passed;
        AttemptedAt = attemptedAt;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid QuizId { get; private set; }
    public decimal Score { get; private set; }
    public bool Passed { get; private set; }
    public DateTimeOffset AttemptedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static QuizAttempt Create(Guid userId, Guid quizId, decimal score, bool passed, DateTimeOffset attemptedAt, DateTimeOffset now) =>
        new(userId, quizId, score, passed, attemptedAt, now);
}

