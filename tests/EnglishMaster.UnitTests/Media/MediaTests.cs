using EnglishMaster.Domain.Media;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.UnitTests.Media;

public sealed class MediaTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var media = MediaEntity.Create(
            " image.jpg ",
            " upload.jpg ",
            " .jpg ",
            " image/jpeg ",
            128,
            MediaType.Image,
            " media/image.jpg ",
            " /media/image.jpg ",
            " Image alt ",
            " Description ",
            now);

        Assert.NotEqual(Guid.Empty, media.Id);
        Assert.Equal("image.jpg", media.FileName);
        Assert.Equal("upload.jpg", media.OriginalFileName);
        Assert.Equal(".jpg", media.FileExtension);
        Assert.Equal("image/jpeg", media.ContentType);
        Assert.Equal(128, media.FileSize);
        Assert.Equal(MediaType.Image, media.MediaType);
        Assert.Equal("media/image.jpg", media.StoragePath);
        Assert.True(media.IsActive);
        Assert.Equal(now, media.CreatedAt);
        Assert.Equal(now, media.UpdatedAt);
    }

    [Fact]
    public void CreateRequiresFileName()
    {
        var exception = Assert.Throws<ArgumentException>(() => MediaEntity.Create(
            string.Empty,
            "upload.jpg",
            ".jpg",
            "image/jpeg",
            128,
            MediaType.Image,
            "media/image.jpg",
            "/media/image.jpg",
            string.Empty,
            string.Empty,
            DateTimeOffset.UtcNow));

        Assert.Equal("FileName", exception.ParamName);
    }

    [Fact]
    public void CreateRequiresPositiveFileSize()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => MediaEntity.Create(
            "image.jpg",
            "upload.jpg",
            ".jpg",
            "image/jpeg",
            0,
            MediaType.Image,
            "media/image.jpg",
            "/media/image.jpg",
            string.Empty,
            string.Empty,
            DateTimeOffset.UtcNow));

        Assert.Equal("fileSize", exception.ParamName);
    }

    [Fact]
    public void CreateRejectsUndefinedMediaType()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => MediaEntity.Create(
            "image.jpg",
            "upload.jpg",
            ".jpg",
            "image/jpeg",
            128,
            (MediaType)999,
            "media/image.jpg",
            "/media/image.jpg",
            string.Empty,
            string.Empty,
            DateTimeOffset.UtcNow));

        Assert.Equal("mediaType", exception.ParamName);
    }

    [Fact]
    public void ActivateAndDeactivateUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var media = MediaEntity.Create(
            "image.jpg",
            "upload.jpg",
            ".jpg",
            "image/jpeg",
            128,
            MediaType.Image,
            "media/image.jpg",
            "/media/image.jpg",
            string.Empty,
            string.Empty,
            createdAt);

        var deactivatedAt = createdAt.AddMinutes(5);
        media.Deactivate(deactivatedAt);
        Assert.False(media.IsActive);
        Assert.Equal(deactivatedAt, media.UpdatedAt);

        var activatedAt = createdAt.AddMinutes(10);
        media.Activate(activatedAt);
        Assert.True(media.IsActive);
        Assert.Equal(activatedAt, media.UpdatedAt);
    }
}
