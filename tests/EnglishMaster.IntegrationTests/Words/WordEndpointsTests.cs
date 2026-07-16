using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Categories;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Contracts.Tags;
using EnglishMaster.Contracts.Words;

namespace EnglishMaster.IntegrationTests.Words;

public sealed class WordEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public WordEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task WordEndpointsSupportCreateGetSearchUpdateAndDelete()
    {
        var createResponse = await client.PostAsJsonAsync("/api/v1/words", CreateRequest("hello"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<WordDto>();
        Assert.NotNull(created);
        Assert.Equal("hello", created.Text);
        Assert.Equal("hello", created.Slug);
        Assert.True(created.IsActive);

        var getResponse = await client.GetAsync($"/api/v1/words/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var searchResponse = await client.GetAsync("/api/v1/words?search=hello");
        Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<WordSearchResponse>();
        Assert.Contains(searchResult!.Items, item => item.Id == created.Id);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/v1/words/{created.Id}",
            new UpdateWordRequest(
                "hello",
                "/he'lo/",
                "/he'lo/",
                "heh-lo",
                "Thai meaning",
                "an updated greeting",
                "Interjection",
                "A2",
                "Hello there.",
                "Thai example",
                true));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<WordDto>();
        Assert.Equal("an updated greeting", updated!.MeaningEn);
        Assert.Equal("A2", updated.CefrLevel);

        var deleteResponse = await client.DeleteAsync($"/api/v1/words/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var activeSearchResponse = await client.GetAsync("/api/v1/words?search=hello");
        var activeSearchResult = await activeSearchResponse.Content.ReadFromJsonAsync<WordSearchResponse>();
        Assert.DoesNotContain(activeSearchResult!.Items, item => item.Id == created.Id);

        var inactiveSearchResponse = await client.GetAsync("/api/v1/words?search=hello&isActive=false");
        var inactiveSearchResult = await inactiveSearchResponse.Content.ReadFromJsonAsync<WordSearchResponse>();
        Assert.Contains(inactiveSearchResult!.Items, item => item.Id == created.Id);
    }

    [Fact]
    public async Task CreateReturnsValidationProblemForInvalidInput()
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/words",
            new CreateWordRequest(
                string.Empty,
                null,
                null,
                null,
                string.Empty,
                "meaning",
                "Unknown",
                "A1",
                null,
                null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SearchReturnsValidationProblemWhenSearchIsTooLong()
    {
        var response = await client.GetAsync($"/api/v1/words?search={new string('a', 201)}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SearchSupportsPaginationAndSortByText()
    {
        var prefix = UniquePrefix("page");
        await client.PostAsJsonAsync("/api/v1/words", CreateRequest($"{prefix} charlie"));
        await client.PostAsJsonAsync("/api/v1/words", CreateRequest($"{prefix} alpha"));
        await client.PostAsJsonAsync("/api/v1/words", CreateRequest($"{prefix} bravo"));

        var response = await client.GetFromJsonAsync<WordSearchResponse>(
            $"/api/v1/words?search={prefix}&pageNumber=2&pageSize=2&sortBy=Text&sortDirection=Asc");

        Assert.NotNull(response);
        var item = Assert.Single(response!.Items);
        Assert.Equal($"{prefix} charlie", item.Text);
        Assert.Equal(2, response.PageNumber);
        Assert.Equal(2, response.PageSize);
        Assert.Equal(3, response.TotalCount);
        Assert.Equal(2, response.TotalPages);
        Assert.True(response.HasPreviousPage);
        Assert.False(response.HasNextPage);
    }

    [Fact]
    public async Task SearchSupportsCefrAndActiveFilters()
    {
        var prefix = UniquePrefix("filter");
        await client.PostAsJsonAsync("/api/v1/words", CreateRequest($"{prefix} a1", cefrLevel: "A1"));
        var b2Response = await client.PostAsJsonAsync("/api/v1/words", CreateRequest($"{prefix} b2", cefrLevel: "B2"));
        var inactiveResponse = await client.PostAsJsonAsync("/api/v1/words", CreateRequest($"{prefix} inactive", cefrLevel: "B2"));
        var inactive = await inactiveResponse.Content.ReadFromJsonAsync<WordDto>();
        await client.DeleteAsync($"/api/v1/words/{inactive!.Id}");

        var activeB2 = await client.GetFromJsonAsync<WordSearchResponse>(
            $"/api/v1/words?search={prefix}&cefrLevel=B2&isActive=true");
        var inactiveResult = await client.GetFromJsonAsync<WordSearchResponse>(
            $"/api/v1/words?search={prefix}&isActive=false");

        Assert.Equal(HttpStatusCode.Created, b2Response.StatusCode);
        var activeItem = Assert.Single(activeB2!.Items);
        Assert.Equal($"{prefix} b2", activeItem.Text);
        var inactiveItem = Assert.Single(inactiveResult!.Items);
        Assert.Equal($"{prefix} inactive", inactiveItem.Text);
    }

    [Fact]
    public async Task SearchSupportsMeaningFields()
    {
        var prefix = UniquePrefix("meaning");
        var meaningTh = $"{prefix} thai";
        var meaningEn = $"{prefix} english";
        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/words",
            CreateRequest($"{prefix} text", meaningTh: meaningTh, meaningEn: meaningEn));
        var created = await createResponse.Content.ReadFromJsonAsync<WordDto>();

        var thaiResult = await client.GetFromJsonAsync<WordSearchResponse>(
            $"/api/v1/words?search={Uri.EscapeDataString(meaningTh)}");
        var englishResult = await client.GetFromJsonAsync<WordSearchResponse>(
            $"/api/v1/words?search={Uri.EscapeDataString(meaningEn)}");

        Assert.Contains(thaiResult!.Items, item => item.Id == created!.Id);
        Assert.Contains(englishResult!.Items, item => item.Id == created!.Id);
    }

    [Fact]
    public async Task WordEndpointsSupportCategoryAndTags()
    {
        var prefix = UniquePrefix("relation");
        var categoryResponse = await client.PostAsJsonAsync(
            "/api/v1/categories",
            new CreateCategoryRequest($"{prefix} category", "Related category", 1));
        var tagResponse = await client.PostAsJsonAsync(
            "/api/v1/tags",
            new CreateTagRequest($"{prefix} tag", "Related tag"));
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();
        var tag = await tagResponse.Content.ReadFromJsonAsync<TagDto>();

        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/words",
            CreateRequest(
                $"{prefix} word",
                categoryId: category!.Id,
                tagIds: [tag!.Id]));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<WordDto>();
        Assert.NotNull(created);
        Assert.Equal(category.Id, created.CategoryId);
        Assert.Equal(category.Name, created.Category!.Name);
        var createdTag = Assert.Single(created.Tags);
        Assert.Equal(tag.Id, createdTag.Id);

        var getResult = await client.GetFromJsonAsync<WordDto>($"/api/v1/words/{created.Id}");
        Assert.Equal(category.Id, getResult!.CategoryId);
        Assert.Contains(getResult.Tags, item => item.Id == tag.Id);

        var byCategory = await client.GetFromJsonAsync<WordSearchResponse>(
            $"/api/v1/words?categoryId={category.Id}");
        Assert.Contains(byCategory!.Items, item => item.Id == created.Id);

        var byTag = await client.GetFromJsonAsync<WordSearchResponse>(
            $"/api/v1/words?tagId={tag.Id}");
        Assert.Contains(byTag!.Items, item => item.Id == created.Id);
    }

    [Fact]
    public async Task WordEndpointsSupportImageAndAudioMedia()
    {
        var prefix = UniquePrefix("media");
        var imageResponse = await client.PostAsJsonAsync(
            "/api/v1/media",
            CreateMediaRequest($"{prefix}-image.jpg", "Image", "image/jpeg"));
        var audioResponse = await client.PostAsJsonAsync(
            "/api/v1/media",
            CreateMediaRequest($"{prefix}-audio.mp3", "Audio", "audio/mpeg"));
        var image = await imageResponse.Content.ReadFromJsonAsync<MediaDto>();
        var audio = await audioResponse.Content.ReadFromJsonAsync<MediaDto>();

        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/words",
            CreateRequest(
                $"{prefix} word",
                imageMediaId: image!.Id,
                audioMediaId: audio!.Id));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<WordDto>();
        Assert.NotNull(created);
        Assert.Equal(image.Id, created.ImageMediaId);
        Assert.Equal(audio.Id, created.AudioMediaId);
        Assert.Equal("Image", created.ImageMedia!.MediaType);
        Assert.Equal("Audio", created.AudioMedia!.MediaType);

        var getResult = await client.GetFromJsonAsync<WordDto>($"/api/v1/words/{created.Id}");
        Assert.Equal(image.Id, getResult!.ImageMediaId);
        Assert.Equal(audio.Id, getResult.AudioMediaId);

        var searchResult = await client.GetFromJsonAsync<WordSearchResponse>(
            $"/api/v1/words?search={prefix}");
        var searchItem = Assert.Single(searchResult!.Items, item => item.Id == created.Id);
        Assert.Equal(image.Id, searchItem.ImageMediaId);
        Assert.Equal(audio.Id, searchItem.AudioMediaId);
    }

    [Fact]
    public async Task CreateReturnsValidationProblemWhenSlugAlreadyExists()
    {
        var firstResponse = await client.PostAsJsonAsync("/api/v1/words", CreateRequest("duplicate create"));
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await client.PostAsJsonAsync("/api/v1/words", CreateRequest("duplicate-create"));

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateReturnsValidationProblemWhenSlugAlreadyExists()
    {
        var firstResponse = await client.PostAsJsonAsync("/api/v1/words", CreateRequest("duplicate update"));
        var secondResponse = await client.PostAsJsonAsync("/api/v1/words", CreateRequest("rename target"));
        var second = await secondResponse.Content.ReadFromJsonAsync<WordDto>();

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/v1/words/{second!.Id}",
            new UpdateWordRequest(
                "duplicate-update",
                null,
                null,
                null,
                "Thai meaning",
                null,
                "Noun",
                "A1",
                null,
                null,
                true));

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    private static CreateWordRequest CreateRequest(
        string text,
        string cefrLevel = "A1",
        string partOfSpeech = "Interjection",
        string meaningTh = "Thai meaning",
        string meaningEn = "greeting",
        Guid? categoryId = null,
        IReadOnlyCollection<Guid>? tagIds = null,
        Guid? imageMediaId = null,
        Guid? audioMediaId = null)
    {
        return new CreateWordRequest(
            text,
            "/he'lo/",
            "/he'lo/",
            "heh-lo",
            meaningTh,
            meaningEn,
            partOfSpeech,
            cefrLevel,
            "Hello there.",
            "Thai example",
            categoryId,
            tagIds,
            imageMediaId,
            audioMediaId);
    }

    private static CreateMediaRequest CreateMediaRequest(
        string fileName,
        string mediaType,
        string contentType)
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

    private static string UniquePrefix(string purpose)
    {
        return $"{purpose}-{Guid.NewGuid():N}";
    }
}
