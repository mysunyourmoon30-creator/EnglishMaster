using EnglishMaster.Application.Features.Media.Queries;
using EnglishMaster.Domain.Media;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.UnitTests.Media;

public sealed class SearchMediaQueryHandlerTests
{
    [Fact]
    public async Task HandleAsyncFiltersByMediaType()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var repository = new FakeMediaRepository();
        var image = CreateMedia("image.jpg", MediaType.Image, "image/jpeg", now);
        var audio = CreateMedia("audio.mp3", MediaType.Audio, "audio/mpeg", now);
        repository.Media.AddRange([image, audio]);
        var handler = new SearchMediaQueryHandler(repository);

        var result = await handler.HandleAsync(
            new SearchMediaQuery(null, "Image", null, true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!.Items);
        Assert.Equal(image.Id, item.Id);
        Assert.Equal("Image", item.MediaType);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorForInvalidMediaType()
    {
        var handler = new SearchMediaQueryHandler(new FakeMediaRepository());

        var result = await handler.HandleAsync(
            new SearchMediaQuery(null, "NotAType", null, true),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == nameof(SearchMediaQuery.MediaType));
    }

    private static MediaEntity CreateMedia(
        string fileName,
        MediaType mediaType,
        string contentType,
        DateTimeOffset now)
    {
        var extension = Path.GetExtension(fileName);
        return MediaEntity.Create(
            fileName,
            fileName,
            extension,
            contentType,
            128,
            mediaType,
            $"media/{fileName}",
            $"/media/{fileName}",
            string.Empty,
            string.Empty,
            now);
    }
}
