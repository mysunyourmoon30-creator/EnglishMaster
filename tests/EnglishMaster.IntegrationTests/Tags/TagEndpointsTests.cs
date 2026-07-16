using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Tags;

namespace EnglishMaster.IntegrationTests.Tags;

public sealed class TagEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public TagEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task TagEndpointsSupportCreateGetSearchUpdateAndDelete()
    {
        var name = UniqueName("tag");
        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/tags",
            new CreateTagRequest(name, "Travel vocabulary"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<TagDto>();
        Assert.NotNull(created);
        Assert.Equal(name, created.Name);
        Assert.Equal(name, created.Slug);
        Assert.True(created.IsActive);

        var getResponse = await client.GetAsync($"/api/v1/tags/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var searchResult = await client.GetFromJsonAsync<TagSearchResponse>(
            $"/api/v1/tags?search={Uri.EscapeDataString(name)}");
        Assert.Contains(searchResult!.Items, item => item.Id == created.Id);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/v1/tags/{created.Id}",
            new UpdateTagRequest(name, "Updated description", true));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<TagDto>();
        Assert.Equal("Updated description", updated!.Description);

        var deleteResponse = await client.DeleteAsync($"/api/v1/tags/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var activeSearchResult = await client.GetFromJsonAsync<TagSearchResponse>(
            $"/api/v1/tags?search={Uri.EscapeDataString(name)}");
        Assert.DoesNotContain(activeSearchResult!.Items, item => item.Id == created.Id);

        var inactiveSearchResult = await client.GetFromJsonAsync<TagSearchResponse>(
            $"/api/v1/tags?search={Uri.EscapeDataString(name)}&isActive=false");
        Assert.Contains(inactiveSearchResult!.Items, item => item.Id == created.Id);
    }

    private static string UniqueName(string purpose)
    {
        return $"{purpose}-{Guid.NewGuid():N}";
    }
}
