using EnglishMaster.Domain.Words;

namespace EnglishMaster.Domain.Learning;

public sealed class StudentProfile
{
    private StudentProfile()
    {
    }

    private StudentProfile(Guid userId, CefrLevel? currentCefrLevel, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        UserId = userId == Guid.Empty ? throw new ArgumentException("UserId is required.", nameof(userId)) : userId;
        CurrentCefrLevel = currentCefrLevel;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public CefrLevel? CurrentCefrLevel { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static StudentProfile Create(Guid userId, CefrLevel? currentCefrLevel, DateTimeOffset now) =>
        new(userId, currentCefrLevel, now);
}

