namespace EnglishMaster.Domain.Pronunciations;

public sealed class MinimalPair
{
    private MinimalPair()
    {
        PairWordText = string.Empty;
        PairIpa = string.Empty;
        PairThaiReading = string.Empty;
        DifferenceNote = string.Empty;
    }

    private MinimalPair(
        Guid id,
        Guid pronunciationId,
        string? pairWordText,
        string? pairIpa,
        string? pairThaiReading,
        string? differenceNote,
        Guid? audioMediaId,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            pronunciationId,
            pairWordText,
            pairIpa,
            pairThaiReading,
            differenceNote,
            audioMediaId,
            sortOrder,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public Guid PronunciationId { get; private set; }

    public string PairWordText { get; private set; } = string.Empty;

    public string PairIpa { get; private set; } = string.Empty;

    public string PairThaiReading { get; private set; } = string.Empty;

    public string DifferenceNote { get; private set; } = string.Empty;

    public Guid? AudioMediaId { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static MinimalPair Create(
        Guid pronunciationId,
        string? pairWordText,
        string? pairIpa,
        string? pairThaiReading,
        string? differenceNote,
        Guid? audioMediaId,
        int sortOrder,
        DateTimeOffset now)
    {
        return new MinimalPair(
            Guid.NewGuid(),
            pronunciationId,
            pairWordText,
            pairIpa,
            pairThaiReading,
            differenceNote,
            audioMediaId,
            sortOrder,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string? pairWordText,
        string? pairIpa,
        string? pairThaiReading,
        string? differenceNote,
        Guid? audioMediaId,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            PronunciationId,
            pairWordText,
            pairIpa,
            pairThaiReading,
            differenceNote,
            audioMediaId,
            sortOrder,
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

    private void Apply(
        Guid pronunciationId,
        string? pairWordText,
        string? pairIpa,
        string? pairThaiReading,
        string? differenceNote,
        Guid? audioMediaId,
        int sortOrder,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        PronunciationId = PronunciationDomainGuard.RequiredId(pronunciationId, nameof(pronunciationId));
        PairWordText = PronunciationDomainGuard.RequiredText(pairWordText, nameof(PairWordText), MinimalPairFieldLimits.PairWordText);
        PairIpa = PronunciationDomainGuard.OptionalText(pairIpa, nameof(PairIpa), MinimalPairFieldLimits.PairIpa);
        PairThaiReading = PronunciationDomainGuard.OptionalText(pairThaiReading, nameof(PairThaiReading), MinimalPairFieldLimits.PairThaiReading);
        DifferenceNote = PronunciationDomainGuard.OptionalText(differenceNote, nameof(DifferenceNote), MinimalPairFieldLimits.DifferenceNote);
        AudioMediaId = PronunciationDomainGuard.OptionalId(audioMediaId, nameof(AudioMediaId));
        SortOrder = sortOrder >= 0
            ? sortOrder
            : throw new ArgumentOutOfRangeException(nameof(sortOrder), "SortOrder must be greater than or equal to zero.");

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
