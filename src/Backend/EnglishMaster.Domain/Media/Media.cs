namespace EnglishMaster.Domain.Media;

public sealed class Media
{
    private Media()
    {
        FileName = string.Empty;
        OriginalFileName = string.Empty;
        FileExtension = string.Empty;
        ContentType = string.Empty;
        StoragePath = string.Empty;
        PublicUrl = string.Empty;
        AltText = string.Empty;
        Description = string.Empty;
    }

    private Media(
        Guid id,
        string fileName,
        string originalFileName,
        string fileExtension,
        string contentType,
        long fileSize,
        MediaType mediaType,
        string storagePath,
        string publicUrl,
        string altText,
        string description,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            fileName,
            originalFileName,
            fileExtension,
            contentType,
            fileSize,
            mediaType,
            storagePath,
            publicUrl,
            altText,
            description,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string OriginalFileName { get; private set; } = string.Empty;

    public string FileExtension { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long FileSize { get; private set; }

    public MediaType MediaType { get; private set; }

    public string StoragePath { get; private set; } = string.Empty;

    public string PublicUrl { get; private set; } = string.Empty;

    public string AltText { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Media Create(
        string fileName,
        string originalFileName,
        string fileExtension,
        string contentType,
        long fileSize,
        MediaType mediaType,
        string storagePath,
        string publicUrl,
        string altText,
        string description,
        DateTimeOffset now)
    {
        return new Media(
            Guid.NewGuid(),
            fileName,
            originalFileName,
            fileExtension,
            contentType,
            fileSize,
            mediaType,
            storagePath,
            publicUrl,
            altText,
            description,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string fileName,
        string originalFileName,
        string fileExtension,
        string contentType,
        long fileSize,
        MediaType mediaType,
        string storagePath,
        string publicUrl,
        string altText,
        string description,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            fileName,
            originalFileName,
            fileExtension,
            contentType,
            fileSize,
            mediaType,
            storagePath,
            publicUrl,
            altText,
            description,
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
        string fileName,
        string originalFileName,
        string fileExtension,
        string contentType,
        long fileSize,
        MediaType mediaType,
        string storagePath,
        string publicUrl,
        string altText,
        string description,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        FileName = NormalizeRequired(fileName, nameof(FileName), MediaFieldLimits.FileName);
        OriginalFileName = NormalizeRequired(originalFileName, nameof(OriginalFileName), MediaFieldLimits.OriginalFileName);
        FileExtension = NormalizeOptional(fileExtension, nameof(FileExtension), MediaFieldLimits.FileExtension);
        ContentType = NormalizeRequired(contentType, nameof(ContentType), MediaFieldLimits.ContentType);
        FileSize = fileSize > 0
            ? fileSize
            : throw new ArgumentOutOfRangeException(nameof(fileSize), "FileSize must be greater than zero.");
        if (!Enum.IsDefined(mediaType))
        {
            throw new ArgumentOutOfRangeException(nameof(mediaType), "MediaType is invalid.");
        }

        MediaType = mediaType;
        StoragePath = NormalizeRequired(storagePath, nameof(StoragePath), MediaFieldLimits.StoragePath);
        PublicUrl = NormalizeOptional(publicUrl, nameof(PublicUrl), MediaFieldLimits.PublicUrl);
        AltText = NormalizeOptional(altText, nameof(AltText), MediaFieldLimits.AltText);
        Description = NormalizeOptional(description, nameof(Description), MediaFieldLimits.Description);

        if (isActive)
        {
            Activate(updatedAt);
        }
        else
        {
            Deactivate(updatedAt);
        }
    }

    private static string NormalizeRequired(string? value, string fieldName, int maxLength)
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
