namespace EnglishMaster.Domain.Media;

public static class MediaFieldLimits
{
    public const int FileName = 255;
    public const int OriginalFileName = 255;
    public const int FileExtension = 20;
    public const int ContentType = 120;
    public const int StoragePath = 500;
    public const int PublicUrl = 500;
    public const int AltText = 300;
    public const int Description = 1000;
}
