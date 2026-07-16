using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.ContentRevisions;
using EnglishMaster.Contracts.Security;

namespace EnglishMaster.IntegrationTests.ContentRevisions;

public sealed class ContentRevisionEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task ContentRevisionPermissions_AreSeeded()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var permissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>("/api/v1/permissions");
        Assert.NotNull(permissions);

        Assert.Contains(permissions, permission => permission.Key == "content-revisions.read");
        Assert.Contains(permissions, permission => permission.Key == "content-revisions.restore.request");
        Assert.Contains(permissions, permission => permission.Key == "content-revisions.restore.approve");
        Assert.Contains(permissions, permission => permission.Key == "content-revisions.manage");
    }

    [Fact]
    public async Task CreateRevision_CreatesRevision()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var contentId = Guid.NewGuid();

        var revision = await CreateRevisionAsync(client, "Word", contentId, "Created", "Initial word");

        Assert.Equal("word", revision.ContentType);
        Assert.Equal(contentId, revision.ContentId);
        Assert.Equal(1, revision.RevisionNumber);
        Assert.Equal("Created", revision.EventType);
    }

    [Fact]
    public async Task CreateRevision_IncrementsRevisionNumberForContent()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var contentId = Guid.NewGuid();

        var first = await CreateRevisionAsync(client, "Lesson", contentId, "Created", "First lesson");
        var second = await CreateRevisionAsync(client, "Lesson", contentId, "Updated", "Updated lesson");

        Assert.Equal(1, first.RevisionNumber);
        Assert.Equal(2, second.RevisionNumber);
    }

    [Fact]
    public async Task GetRevisionsByContent_ReturnsOnlyMatchingContent()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var contentId = Guid.NewGuid();
        var otherContentId = Guid.NewGuid();
        var revision = await CreateRevisionAsync(client, "Course", contentId, "Created", "Course revision");
        await CreateRevisionAsync(client, "Course", otherContentId, "Created", "Other course");

        var search = await client.GetFromJsonAsync<ContentRevisionSearchResponse>($"/api/v1/content-revisions/Course/{contentId}");

        Assert.NotNull(search);
        Assert.Single(search!.Items);
        Assert.Equal(revision.Id, search.Items.Single().Id);
    }

    [Fact]
    public async Task GetLatestRevisionForContent_ReturnsNewestRevision()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var contentId = Guid.NewGuid();
        await CreateRevisionAsync(client, "Book", contentId, "Created", "Book created");
        var latest = await CreateRevisionAsync(client, "Book", contentId, "Updated", "Book updated");

        var response = await client.GetFromJsonAsync<ContentRevisionDto>($"/api/v1/content-revisions/Book/{contentId}/latest");

        Assert.NotNull(response);
        Assert.Equal(latest.Id, response!.Id);
        Assert.Equal(2, response.RevisionNumber);
    }

    [Fact]
    public async Task CreateRevision_SanitizesSensitiveSnapshotFields()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var revision = await CreateRevisionAsync(
            client,
            "Quiz",
            Guid.NewGuid(),
            "Updated",
            "Sensitive quiz",
            """{"Title":"Safe quiz","Password":"secret","AccessToken":"abc","Nested":{"ApiKey":"key","SecurityStamp":"stamp","ClientSecret":"client-secret","Value":"visible"}}""");

        Assert.Contains("Safe quiz", revision.SnapshotJson, StringComparison.Ordinal);
        Assert.Contains("visible", revision.SnapshotJson, StringComparison.Ordinal);
        Assert.DoesNotContain("secret", revision.SnapshotJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("abc", revision.SnapshotJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("key", revision.SnapshotJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stamp", revision.SnapshotJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("client-secret", revision.SnapshotJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RestoreRequest_CanBeCreatedAndApproved()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var revision = await CreateRevisionAsync(client, "PublishTemplate", Guid.NewGuid(), "Updated", "Template updated");

        var request = await CreateRestoreRequestAsync(client, revision.Id);
        var approveResponse = await client.PostAsJsonAsync(
            $"/api/v1/content-revision-restore-requests/{request.Id}/approve",
            new ReviewContentRevisionRestoreRequestRequest("Looks good"));

        Assert.Equal(HttpStatusCode.OK, approveResponse.StatusCode);
        var approved = await approveResponse.Content.ReadFromJsonAsync<ContentRevisionRestoreRequestDto>();
        Assert.Equal("Approved", approved!.Status);
        Assert.Equal("Looks good", approved.ReviewNote);
    }

    [Fact]
    public async Task RestoreRequest_CanBeRejected()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var revision = await CreateRevisionAsync(client, "Word", Guid.NewGuid(), "Updated", "Word updated");
        var request = await CreateRestoreRequestAsync(client, revision.Id);

        var rejectResponse = await client.PostAsJsonAsync(
            $"/api/v1/content-revision-restore-requests/{request.Id}/reject",
            new ReviewContentRevisionRestoreRequestRequest("Needs a cleaner reason"));

        Assert.Equal(HttpStatusCode.OK, rejectResponse.StatusCode);
        var rejected = await rejectResponse.Content.ReadFromJsonAsync<ContentRevisionRestoreRequestDto>();
        Assert.Equal("Rejected", rejected!.Status);
        Assert.Equal("Needs a cleaner reason", rejected.ReviewNote);
    }

    [Fact]
    public async Task RestoreRequest_CanBeCompletedOnlyAfterApproval()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var revision = await CreateRevisionAsync(client, "Course", Guid.NewGuid(), "Updated", "Course updated");
        var request = await CreateRestoreRequestAsync(client, revision.Id);
        await client.PostAsJsonAsync(
            $"/api/v1/content-revision-restore-requests/{request.Id}/approve",
            new ReviewContentRevisionRestoreRequestRequest("Approved for restore"));

        var completeResponse = await client.PostAsync($"/api/v1/content-revision-restore-requests/{request.Id}/complete", null);

        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);
        var completed = await completeResponse.Content.ReadFromJsonAsync<ContentRevisionRestoreRequestDto>();
        Assert.Equal("Completed", completed!.Status);
    }

    [Fact]
    public async Task RestoreRequest_RejectedRequestCannotBeCompleted()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var revision = await CreateRevisionAsync(client, "Book", Guid.NewGuid(), "Updated", "Book updated");
        var request = await CreateRestoreRequestAsync(client, revision.Id);
        await client.PostAsJsonAsync(
            $"/api/v1/content-revision-restore-requests/{request.Id}/reject",
            new ReviewContentRevisionRestoreRequestRequest("Do not restore"));

        var completeResponse = await client.PostAsync($"/api/v1/content-revision-restore-requests/{request.Id}/complete", null);

        Assert.Equal(HttpStatusCode.BadRequest, completeResponse.StatusCode);
    }

    [Fact]
    public async Task RestoreRequest_RequiresExistingRevision()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync(
            "/api/v1/content-revision-restore-requests",
            new CreateContentRevisionRestoreRequestRequest(Guid.NewGuid(), "Missing revision."));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ContentRevisionPermissions_AreMappedToEditorAndReviewerRoles()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var roles = await client.GetFromJsonAsync<RoleSearchResponse>("/api/v1/roles?pageSize=100");
        var editor = roles!.Items.Single(role => role.Name == "ContentEditor");
        var reviewer = roles.Items.Single(role => role.Name == "Reviewer");
        var editorPermissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>($"/api/v1/roles/{editor.Id}/permissions");
        var reviewerPermissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>($"/api/v1/roles/{reviewer.Id}/permissions");

        Assert.Contains(editorPermissions!, permission => permission.Key == "content-revisions.read");
        Assert.Contains(editorPermissions!, permission => permission.Key == "content-revisions.restore.request");
        Assert.DoesNotContain(editorPermissions!, permission => permission.Key == "content-revisions.restore.approve");
        Assert.Contains(reviewerPermissions!, permission => permission.Key == "content-revisions.read");
        Assert.Contains(reviewerPermissions!, permission => permission.Key == "content-revisions.restore.approve");
    }

    private static async Task<ContentRevisionDto> CreateRevisionAsync(HttpClient client, string contentType, Guid contentId, string eventType, string title, string? snapshotJson = null)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/content-revisions",
            new CreateContentRevisionRequest(
                contentType,
                contentId,
                eventType,
                title,
                null,
                "Test change",
                snapshotJson ?? $$"""{"Title":"{{title}}"}""",
                null));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ContentRevisionDto>())!;
    }

    private static async Task<ContentRevisionRestoreRequestDto> CreateRestoreRequestAsync(HttpClient client, Guid revisionId)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/content-revision-restore-requests",
            new CreateContentRevisionRestoreRequestRequest(revisionId, "Restore this known-good revision."));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ContentRevisionRestoreRequestDto>())!;
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "superadmin@englishmaster.test",
            "TestPassword1"));
}
