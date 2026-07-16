namespace EnglishMaster.Domain.Pronunciations;

public sealed class Pronunciation
{
    private readonly List<MinimalPair> minimalPairs = [];

    private Pronunciation()
    {
        IpaUk = string.Empty;
        IpaUs = string.Empty;
        ThaiReading = string.Empty;
        Syllables = string.Empty;
        StressPattern = string.Empty;
        MouthPosition = string.Empty;
        TonguePosition = string.Empty;
        CommonMistake = string.Empty;
        PracticeNote = string.Empty;
    }

    private Pronunciation(
        Guid id,
        Guid wordId,
        string? ipaUk,
        string? ipaUs,
        string? thaiReading,
        string? syllables,
        string? stressPattern,
        string? mouthPosition,
        string? tonguePosition,
        string? commonMistake,
        string? practiceNote,
        Guid? audioSlowMediaId,
        Guid? audioNormalMediaId,
        Guid? mouthImageMediaId,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            wordId,
            ipaUk,
            ipaUs,
            thaiReading,
            syllables,
            stressPattern,
            mouthPosition,
            tonguePosition,
            commonMistake,
            practiceNote,
            audioSlowMediaId,
            audioNormalMediaId,
            mouthImageMediaId,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public Guid WordId { get; private set; }

    public string IpaUk { get; private set; } = string.Empty;

    public string IpaUs { get; private set; } = string.Empty;

    public string ThaiReading { get; private set; } = string.Empty;

    public string Syllables { get; private set; } = string.Empty;

    public string StressPattern { get; private set; } = string.Empty;

    public string MouthPosition { get; private set; } = string.Empty;

    public string TonguePosition { get; private set; } = string.Empty;

    public string CommonMistake { get; private set; } = string.Empty;

    public string PracticeNote { get; private set; } = string.Empty;

    public Guid? AudioSlowMediaId { get; private set; }

    public Guid? AudioNormalMediaId { get; private set; }

    public Guid? MouthImageMediaId { get; private set; }

    public IReadOnlyCollection<MinimalPair> MinimalPairs => minimalPairs.AsReadOnly();

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Pronunciation Create(
        Guid wordId,
        string? ipaUk,
        string? ipaUs,
        string? thaiReading,
        string? syllables,
        string? stressPattern,
        string? mouthPosition,
        string? tonguePosition,
        string? commonMistake,
        string? practiceNote,
        Guid? audioSlowMediaId,
        Guid? audioNormalMediaId,
        Guid? mouthImageMediaId,
        DateTimeOffset now)
    {
        return new Pronunciation(
            Guid.NewGuid(),
            wordId,
            ipaUk,
            ipaUs,
            thaiReading,
            syllables,
            stressPattern,
            mouthPosition,
            tonguePosition,
            commonMistake,
            practiceNote,
            audioSlowMediaId,
            audioNormalMediaId,
            mouthImageMediaId,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        Guid wordId,
        string? ipaUk,
        string? ipaUs,
        string? thaiReading,
        string? syllables,
        string? stressPattern,
        string? mouthPosition,
        string? tonguePosition,
        string? commonMistake,
        string? practiceNote,
        Guid? audioSlowMediaId,
        Guid? audioNormalMediaId,
        Guid? mouthImageMediaId,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            wordId,
            ipaUk,
            ipaUs,
            thaiReading,
            syllables,
            stressPattern,
            mouthPosition,
            tonguePosition,
            commonMistake,
            practiceNote,
            audioSlowMediaId,
            audioNormalMediaId,
            mouthImageMediaId,
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
        Guid wordId,
        string? ipaUk,
        string? ipaUs,
        string? thaiReading,
        string? syllables,
        string? stressPattern,
        string? mouthPosition,
        string? tonguePosition,
        string? commonMistake,
        string? practiceNote,
        Guid? audioSlowMediaId,
        Guid? audioNormalMediaId,
        Guid? mouthImageMediaId,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        WordId = PronunciationDomainGuard.RequiredId(wordId, nameof(wordId));
        IpaUk = PronunciationDomainGuard.OptionalText(ipaUk, nameof(IpaUk), PronunciationFieldLimits.IpaUk);
        IpaUs = PronunciationDomainGuard.OptionalText(ipaUs, nameof(IpaUs), PronunciationFieldLimits.IpaUs);
        if (IpaUk.Length == 0 && IpaUs.Length == 0)
        {
            throw new ArgumentException("IPA UK or IPA US is required.", nameof(ipaUk));
        }

        ThaiReading = PronunciationDomainGuard.OptionalText(thaiReading, nameof(ThaiReading), PronunciationFieldLimits.ThaiReading);
        Syllables = PronunciationDomainGuard.OptionalText(syllables, nameof(Syllables), PronunciationFieldLimits.Syllables);
        StressPattern = PronunciationDomainGuard.OptionalText(stressPattern, nameof(StressPattern), PronunciationFieldLimits.StressPattern);
        MouthPosition = PronunciationDomainGuard.OptionalText(mouthPosition, nameof(MouthPosition), PronunciationFieldLimits.MouthPosition);
        TonguePosition = PronunciationDomainGuard.OptionalText(tonguePosition, nameof(TonguePosition), PronunciationFieldLimits.TonguePosition);
        CommonMistake = PronunciationDomainGuard.OptionalText(commonMistake, nameof(CommonMistake), PronunciationFieldLimits.CommonMistake);
        PracticeNote = PronunciationDomainGuard.OptionalText(practiceNote, nameof(PracticeNote), PronunciationFieldLimits.PracticeNote);
        AudioSlowMediaId = PronunciationDomainGuard.OptionalId(audioSlowMediaId, nameof(AudioSlowMediaId));
        AudioNormalMediaId = PronunciationDomainGuard.OptionalId(audioNormalMediaId, nameof(AudioNormalMediaId));
        MouthImageMediaId = PronunciationDomainGuard.OptionalId(mouthImageMediaId, nameof(MouthImageMediaId));

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
