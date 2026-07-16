using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EnglishMaster.Contracts.Media;

namespace EnglishMaster.IntegrationTests.Media;

public sealed class MediaEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public MediaEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task MediaEndpointsSupportCreateGetSearchUpdateAndDelete()
    {
        var createResponse = await client.PostAsJsonAsync("/api/v1/media", CreateRequest("hero.jpg", "Image"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<MediaDto>();
        Assert.NotNull(created);
        Assert.Equal("hero.jpg", created.FileName);
        Assert.Equal("Image", created.MediaType);
        Assert.True(created.IsActive);

        var getResponse = await client.GetAsync($"/api/v1/media/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getJson = await getResponse.Content.ReadAsStringAsync();
        using (var document = JsonDocument.Parse(getJson))
        {
            Assert.False(document.RootElement.TryGetProperty("storagePath", out _));
        }

        var searchResponse = await client.GetAsync("/api/v1/media?mediaType=Image&search=hero");
        Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<MediaSearchResponse>();
        Assert.Contains(searchResult!.Items, item => item.Id == created.Id);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/v1/media/{created.Id}",
            new UpdateMediaRequest(
                "hero-updated.jpg",
                "hero.jpg",
                ".jpg",
                "image/jpeg",
                256,
                "Image",
                "/media/hero-updated.jpg",
                "Updated hero",
                "Updated description",
                true));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<MediaDto>();
        Assert.Equal("hero-updated.jpg", updated!.FileName);
        Assert.Equal(256, updated.FileSize);

        var deleteResponse = await client.DeleteAsync($"/api/v1/media/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var activeSearch = await client.GetFromJsonAsync<MediaSearchResponse>("/api/v1/media?search=hero");
        Assert.DoesNotContain(activeSearch!.Items, item => item.Id == created.Id);

        var inactiveSearch = await client.GetFromJsonAsync<MediaSearchResponse>("/api/v1/media?search=hero&isActive=false");
        Assert.Contains(inactiveSearch!.Items, item => item.Id == created.Id);
    }

    [Fact]
    public async Task CreateReturnsValidationProblemForInvalidInput()
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/media",
            new CreateMediaRequest(
                string.Empty,
                string.Empty,
                null,
                string.Empty,
                0,
                "Unknown",
                null,
                null,
                null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateReturnsValidationProblemForUnsafePublicUrl()
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/media",
            new CreateMediaRequest(
                "hero.jpg",
                "hero.jpg",
                ".jpg",
                "image/jpeg",
                128,
                "Image",
                "https://example.test/hero.jpg",
                null,
                null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static CreateMediaRequest CreateRequest(
        string fileName,
        string mediaType,
        string contentType = "image/jpeg")
    {
        var extension = Path.GetExtension(fileName);
        return new CreateMediaRequest(
            fileName,
            fileName,
            extension,
            contentType,
            128,
            mediaType,
            $"/media/{fileName}",
            $"{fileName} alt",
            $"{fileName} description");
    }
}
