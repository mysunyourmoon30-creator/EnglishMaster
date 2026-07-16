using EnglishMaster.Domain.Common;

namespace EnglishMaster.Domain.Words;

public sealed class Word
{
    private readonly List<WordTag> tags = [];

    private Word()
    {
        Text = string.Empty;
        Slug = string.Empty;
        IpaUk = string.Empty;
        IpaUs = string.Empty;
        ThaiReading = string.Empty;
        MeaningTh = string.Empty;
        MeaningEn = string.Empty;
        ExampleEn = string.Empty;
        ExampleTh = string.Empty;
    }

    private Word(
        Guid id,
        string text,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string meaningTh,
        string meaningEn,
        PartOfSpeech partOfSpeech,
        CefrLevel cefrLevel,
        string exampleEn,
        string exampleTh,
        Guid? categoryId,
        IEnumerable<Guid>? tagIds,
        Guid? imageMediaId,
        Guid? audioMediaId,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            text,
            ipaUk,
            ipaUs,
            thaiReading,
            meaningTh,
            meaningEn,
            partOfSpeech,
            cefrLevel,
            exampleEn,
            exampleTh,
            categoryId,
            tagIds,
            imageMediaId,
            audioMediaId,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string IpaUk { get; private set; } = string.Empty;

    public string IpaUs { get; private set; } = string.Empty;

    public string ThaiReading { get; private set; } = string.Empty;

    public string MeaningTh { get; private set; } = string.Empty;

    public string MeaningEn { get; private set; } = string.Empty;

    public PartOfSpeech PartOfSpeech { get; private set; }

    public CefrLevel CefrLevel { get; private set; }

    public string ExampleEn { get; private set; } = string.Empty;

    public string ExampleTh { get; private set; } = string.Empty;

    public Guid? CategoryId { get; private set; }

    public IReadOnlyCollection<WordTag> Tags => tags.AsReadOnly();

    public Guid? ImageMediaId { get; private set; }

    public Guid? AudioMediaId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Word Create(
        string text,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string meaningTh,
        string meaningEn,
        PartOfSpeech partOfSpeech,
        CefrLevel cefrLevel,
        string exampleEn,
        string exampleTh,
        Guid? categoryId,
        IEnumerable<Guid>? tagIds,
        Guid? imageMediaId,
        Guid? audioMediaId,
        DateTimeOffset now)
    {
        return new Word(
            Guid.NewGuid(),
            text,
            ipaUk,
            ipaUs,
            thaiReading,
            meaningTh,
            meaningEn,
            partOfSpeech,
            cefrLevel,
            exampleEn,
            exampleTh,
            categoryId,
            tagIds,
            imageMediaId,
            audioMediaId,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public static Word Create(
        string text,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string meaningTh,
        string meaningEn,
        PartOfSpeech partOfSpeech,
        CefrLevel cefrLevel,
        string exampleEn,
        string exampleTh,
        DateTimeOffset now)
    {
        return Create(
            text,
            ipaUk,
            ipaUs,
            thaiReading,
            meaningTh,
            meaningEn,
            partOfSpeech,
            cefrLevel,
            exampleEn,
            exampleTh,
            categoryId: null,
            tagIds: [],
            imageMediaId: null,
            audioMediaId: null,
            now);
    }

    public static Word Create(
        string text,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string meaningTh,
        string meaningEn,
        PartOfSpeech partOfSpeech,
        CefrLevel cefrLevel,
        string exampleEn,
        string exampleTh,
        Guid? categoryId,
        IEnumerable<Guid>? tagIds,
        DateTimeOffset now)
    {
        return Create(
            text,
            ipaUk,
            ipaUs,
            thaiReading,
            meaningTh,
            meaningEn,
            partOfSpeech,
            cefrLevel,
            exampleEn,
            exampleTh,
            categoryId,
            tagIds,
            imageMediaId: null,
            audioMediaId: null,
            now);
    }

    public void Update(
        string text,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string meaningTh,
        string meaningEn,
        PartOfSpeech partOfSpeech,
        CefrLevel cefrLevel,
        string exampleEn,
        string exampleTh,
        Guid? categoryId,
        IEnumerable<Guid>? tagIds,
        Guid? imageMediaId,
        Guid? audioMediaId,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            text,
            ipaUk,
            ipaUs,
            thaiReading,
            meaningTh,
            meaningEn,
            partOfSpeech,
            cefrLevel,
            exampleEn,
            exampleTh,
            categoryId,
            tagIds,
            imageMediaId,
            audioMediaId,
            isActive,
            now);
    }

    public void Update(
        string text,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string meaningTh,
        string meaningEn,
        PartOfSpeech partOfSpeech,
        CefrLevel cefrLevel,
        string exampleEn,
        string exampleTh,
        bool isActive,
        DateTimeOffset now)
    {
        Update(
            text,
            ipaUk,
            ipaUs,
            thaiReading,
            meaningTh,
            meaningEn,
            partOfSpeech,
            cefrLevel,
            exampleEn,
            exampleTh,
            categoryId: null,
            tagIds: [],
            imageMediaId: null,
            audioMediaId: null,
            isActive,
            now);
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        UpdatedAt = now;
    }

    private void Apply(
        string text,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string meaningTh,
        string meaningEn,
        PartOfSpeech partOfSpeech,
        CefrLevel cefrLevel,
        string exampleEn,
        string exampleTh,
        Guid? categoryId,
        IEnumerable<Guid>? tagIds,
        Guid? imageMediaId,
        Guid? audioMediaId,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Text = NormalizeRequired(text, nameof(Text), WordFieldLimits.Text);
        Slug = GenerateSlug(Text);
        IpaUk = NormalizeOptional(ipaUk, nameof(IpaUk), WordFieldLimits.IpaUk);
        IpaUs = NormalizeOptional(ipaUs, nameof(IpaUs), WordFieldLimits.IpaUs);
        ThaiReading = NormalizeOptional(thaiReading, nameof(ThaiReading), WordFieldLimits.ThaiReading);
        MeaningTh = NormalizeRequired(meaningTh, nameof(MeaningTh), WordFieldLimits.MeaningTh);
        MeaningEn = NormalizeOptional(meaningEn, nameof(MeaningEn), WordFieldLimits.MeaningEn);
        PartOfSpeech = partOfSpeech;
        CefrLevel = cefrLevel;
        ExampleEn = NormalizeOptional(exampleEn, nameof(ExampleEn), WordFieldLimits.ExampleEn);
        ExampleTh = NormalizeOptional(exampleTh, nameof(ExampleTh), WordFieldLimits.ExampleTh);
        SetCategory(categoryId, updatedAt);
        SetTags(tagIds ?? [], updatedAt);
        SetImageMedia(imageMediaId, updatedAt);
        SetAudioMedia(audioMediaId, updatedAt);

        if (isActive)
        {
            Activate(updatedAt);
        }
        else
        {
            Deactivate(updatedAt);
        }
    }

    public void Activate(DateTimeOffset now)
    {
        IsActive = true;
        UpdatedAt = now;
    }

    public void SetCategory(Guid? categoryId, DateTimeOffset now)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId));
        }

        CategoryId = categoryId;
        UpdatedAt = now;
    }

    public void SetImageMedia(Guid? imageMediaId, DateTimeOffset now)
    {
        if (imageMediaId == Guid.Empty)
        {
            throw new ArgumentException("ImageMediaId cannot be empty.", nameof(imageMediaId));
        }

        ImageMediaId = imageMediaId;
        UpdatedAt = now;
    }

    public void SetAudioMedia(Guid? audioMediaId, DateTimeOffset now)
    {
        if (audioMediaId == Guid.Empty)
        {
            throw new ArgumentException("AudioMediaId cannot be empty.", nameof(audioMediaId));
        }

        AudioMediaId = audioMediaId;
        UpdatedAt = now;
    }

    public void SetTags(IEnumerable<Guid> tagIds, DateTimeOffset now)
    {
        var normalizedTagIds = tagIds
            .Distinct()
            .ToArray();

        if (normalizedTagIds.Any(tagId => tagId == Guid.Empty))
        {
            throw new ArgumentException("TagIds cannot contain empty values.", nameof(tagIds));
        }

        tags.Clear();
        foreach (var tagId in normalizedTagIds)
        {
            tags.Add(new WordTag(Id, tagId));
        }

        UpdatedAt = now;
    }

    public static string GenerateSlug(string text)
    {
        return SlugGenerator.Generate(text, nameof(Text), nameof(text), WordFieldLimits.Text);
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        var normalized = NormalizeOptional(value, fieldName, maxLength);
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string NormalizeOptional(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }
}
