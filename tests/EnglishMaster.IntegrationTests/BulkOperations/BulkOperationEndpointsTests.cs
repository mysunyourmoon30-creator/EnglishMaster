using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.BulkOperations;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Contracts.Words;

namespace EnglishMaster.IntegrationTests.BulkOperations;

public sealed class BulkOperationEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task BulkOperationPermissions_AreSeeded()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var permissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>("/api/v1/permissions");

        Assert.Contains(permissions!, permission => permission.Key == "bulk-operations.read");
        Assert.Contains(permissions!, permission => permission.Key == "bulk-operations.run");
        Assert.Contains(permissions!, permission => permission.Key == "bulk-operations.cancel");
    }

    [Fact]
    public async Task CreateBulkOperation_CreatesPendingOperationWithItems()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var word = await CreateWordAsync(client);

        var operation = await CreateBulkOperationAsync(client, "RunQualityCheck", "Word", [word.Id]);
        var items = await client.GetFromJsonAsync<IReadOnlyCollection<BulkOperationItemDto>>($"/api/v1/bulk-operations/{operation.Id}/items");

        Assert.Equal("Pending", operation.Status);
        Assert.Equal(1, operation.TotalItems);
        Assert.Single(items!);
        Assert.Equal(word.Id, items!.Single().ContentId);
    }

    [Fact]
    public async Task CreateBulkOperation_DeduplicatesContentIdsForTotalItems()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var word = await CreateWordAsync(client);

        var operation = await CreateBulkOperationAsync(client, "Export", "Word", [word.Id, word.Id]);
        var items = await client.GetFromJsonAsync<IReadOnlyCollection<BulkOperationItemDto>>($"/api/v1/bulk-operations/{operation.Id}/items");

        Assert.Equal(1, operation.TotalItems);
        Assert.Single(items!);
    }

    [Fact]
    public async Task RunBulkQualityCheck_RecordsItemSuccess()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var word = await CreateWordAsync(client);
        var operation = await CreateBulkOperationAsync(client, "RunQualityCheck", "Word", [word.Id]);

        var runResponse = await client.PostAsync($"/api/v1/bulk-operations/{operation.Id}/run", null);

        Assert.Equal(HttpStatusCode.OK, runResponse.StatusCode);
        var completed = await runResponse.Content.ReadFromJsonAsync<BulkOperationDto>();
        Assert.Equal("Completed", completed!.Status);
        Assert.Equal(1, completed.SucceededItems);
        var items = await client.GetFromJsonAsync<IReadOnlyCollection<BulkOperationItemDto>>($"/api/v1/bulk-operations/{operation.Id}/items");
        Assert.Equal("Success", items!.Single().Status);
    }

    [Fact]
    public async Task BulkPublish_ForUnsupportedContentType_RecordsFailure()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var word = await CreateWordAsync(client);
        var operation = await CreateBulkOperationAsync(client, "Publish", "Word", [word.Id]);

        var runResponse = await client.PostAsync($"/api/v1/bulk-operations/{operation.Id}/run", null);

        Assert.Equal(HttpStatusCode.OK, runResponse.StatusCode);
        var completed = await runResponse.Content.ReadFromJsonAsync<BulkOperationDto>();
        Assert.Equal("Failed", completed!.Status);
        Assert.Equal(1, completed.FailedItems);
    }

    [Fact]
    public async Task BulkOperation_WithMixedValidAndMissingItems_IsPartiallyCompleted()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var lesson = await CreateLessonAsync(client);
        var operation = await CreateBulkOperationAsync(client, "Publish", "Lesson", [lesson.Id, Guid.NewGuid()]);

        var runResponse = await client.PostAsync($"/api/v1/bulk-operations/{operation.Id}/run", null);

        Assert.Equal(HttpStatusCode.OK, runResponse.StatusCode);
        var completed = await runResponse.Content.ReadFromJsonAsync<BulkOperationDto>();
        Assert.Equal("PartiallyCompleted", completed!.Status);
        Assert.Equal(1, completed.SucceededItems);
        Assert.Equal(1, completed.FailedItems);
    }

    [Fact]
    public async Task PendingBulkOperation_CanBeCancelled()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var word = await CreateWordAsync(client);
        var operation = await CreateBulkOperationAsync(client, "Export", "Word", [word.Id]);

        var cancelResponse = await client.PostAsync($"/api/v1/bulk-operations/{operation.Id}/cancel", null);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        var cancelled = await cancelResponse.Content.ReadFromJsonAsync<BulkOperationDto>();
        Assert.Equal("Cancelled", cancelled!.Status);
    }

    [Fact]
    public async Task BulkOperationPermissions_AreMappedToEditorAndReviewerButNotViewer()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var roles = await client.GetFromJsonAsync<RoleSearchResponse>("/api/v1/roles?pageSize=100");
        var editor = roles!.Items.Single(role => role.Name == "ContentEditor");
        var reviewer = roles.Items.Single(role => role.Name == "Reviewer");
        var viewer = roles.Items.Single(role => role.Name == "Viewer");
        var editorPermissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>($"/api/v1/roles/{editor.Id}/permissions");
        var reviewerPermissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>($"/api/v1/roles/{reviewer.Id}/permissions");
        var viewerPermissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>($"/api/v1/roles/{viewer.Id}/permissions");

        Assert.Contains(editorPermissions!, permission => permission.Key == "bulk-operations.read");
        Assert.Contains(editorPermissions!, permission => permission.Key == "bulk-operations.run");
        Assert.Contains(reviewerPermissions!, permission => permission.Key == "bulk-operations.read");
        Assert.Contains(reviewerPermissions!, permission => permission.Key == "bulk-operations.run");
        Assert.DoesNotContain(viewerPermissions!, permission => permission.Key.StartsWith("bulk-operations.", StringComparison.Ordinal));
    }

    [Fact]
    public async Task CreateBulkPublish_RequiresUnderlyingPublishPermission()
    {
        using var adminClient = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(adminClient);
        var word = await CreateWordAsync(adminClient);
        await CreateUserWithRoleAsync(adminClient, "bulk-editor@englishmaster.test", "ContentEditor");

        using var editorClient = factory.CreateClient(new() { HandleCookies = true });
        await editorClient.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("bulk-editor@englishmaster.test", "EditorPass1"));

        var response = await editorClient.PostAsJsonAsync(
            "/api/v1/bulk-operations",
            new CreateBulkOperationRequest("Publish", "Word", [word.Id], "Try publish", null, null, null));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static async Task<BulkOperationDto> CreateBulkOperationAsync(HttpClient client, string operationType, string contentType, IReadOnlyCollection<Guid> contentIds)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/bulk-operations",
            new CreateBulkOperationRequest(operationType, contentType, contentIds, "Bulk test", null, null, "json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<BulkOperationDto>())!;
    }

    private static async Task<WordDto> CreateWordAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/words",
            new CreateWordRequest(
                $"bulk word {Guid.NewGuid():N}",
                null,
                null,
                null,
                "Thai meaning",
                "English meaning",
                "Noun",
                "A1",
                "Example sentence.",
                "Thai example."));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<WordDto>())!;
    }

    private static async Task<LessonDto> CreateLessonAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest($"Bulk Lesson {Guid.NewGuid():N}", null, null, null, null, null, 0, 0));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<LessonDto>())!;
    }

    private static async Task CreateUserWithRoleAsync(HttpClient client, string email, string roleName)
    {
        var roles = await client.GetFromJsonAsync<RoleSearchResponse>("/api/v1/roles?pageSize=100");
        var role = roles!.Items.Single(item => item.Name == roleName);
        var createUserResponse = await client.PostAsJsonAsync(
            "/api/v1/users",
            new CreateUserRequest(email, email.Split('@')[0], "Bulk Editor", "EditorPass1", []));
        createUserResponse.EnsureSuccessStatusCode();
        var user = await createUserResponse.Content.ReadFromJsonAsync<UserDto>();
        var assignResponse = await client.PostAsync($"/api/v1/users/{user!.Id}/roles/{role.Id}", null);
        assignResponse.EnsureSuccessStatusCode();
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.test",
            "TestPassword1"));
}
