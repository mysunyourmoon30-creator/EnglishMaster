using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Categories;

namespace EnglishMaster.IntegrationTests.Categories;

public sealed class CategoryEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public CategoryEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CategoryEndpointsSupportCreateGetSearchUpdateAndDelete()
    {
        var name = UniqueName("category");
        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/categories",
            new CreateCategoryRequest(name, "Core vocabulary", 10));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(created);
        Assert.Equal(name, created.Name);
        Assert.Equal(name, created.Slug);
        Assert.True(created.IsActive);

        var getResponse = await client.GetAsync($"/api/v1/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var searchResult = await client.GetFromJsonAsync<CategorySearchResponse>(
            $"/api/v1/categories?search={Uri.EscapeDataString(name)}");
        Assert.Contains(searchResult!.Items, item => item.Id == created.Id);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/v1/categories/{created.Id}",
            new UpdateCategoryRequest(name, "Updated description", 20, true));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.Equal(20, updated!.SortOrder);
        Assert.Equal("Updated description", updated.Description);

        var deleteResponse = await client.DeleteAsync($"/api/v1/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var activeSearchResult = await client.GetFromJsonAsync<CategorySearchResponse>(
            $"/api/v1/categories?search={Uri.EscapeDataString(name)}");
        Assert.DoesNotContain(activeSearchResult!.Items, item => item.Id == created.Id);

        var inactiveSearchResult = await client.GetFromJsonAsync<CategorySearchResponse>(
            $"/api/v1/categories?search={Uri.EscapeDataString(name)}&isActive=false");
        Assert.Contains(inactiveSearchResult!.Items, item => item.Id == created.Id);
    }

    private static string UniqueName(string purpose)
    {
        return $"{purpose}-{Guid.NewGuid():N}";
    }
}
