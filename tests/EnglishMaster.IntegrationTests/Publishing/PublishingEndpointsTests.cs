using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Publishing;

namespace EnglishMaster.IntegrationTests.Publishing;

public sealed class PublishingEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public PublishingEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task PublishingEndpointsSupportJobsTemplatesAndArtifacts()
    {
        var sourceId = Guid.NewGuid();
        var createJobResponse = await client.PostAsJsonAsync(
            "/api/v1/publish-jobs",
            new CreatePublishJobRequest("Lesson", sourceId, "Markdown", "Integration Publish Job", "integration-test"));
        Assert.Equal(HttpStatusCode.Created, createJobResponse.StatusCode);
        var job = await createJobResponse.Content.ReadFromJsonAsync<PublishJobDto>();
        Assert.NotNull(job);
        Assert.Equal("Pending", job.Status);

        var searchJobs = await client.GetFromJsonAsync<PublishJobSearchResponse>(
            "/api/v1/publish-jobs?status=Pending&pageNumber=1&pageSize=20");
        Assert.Contains(searchJobs!.Items, item => item.Id == job.Id);

        var runResponse = await client.PostAsync($"/api/v1/publish-jobs/{job.Id}/run", content: null);
        Assert.Equal(HttpStatusCode.OK, runResponse.StatusCode);
        var completedJob = await runResponse.Content.ReadFromJsonAsync<PublishJobDto>();
        Assert.Equal("Completed", completedJob!.Status);
        Assert.False(string.IsNullOrWhiteSpace(completedJob.OutputFileName));

        var artifacts = await client.GetFromJsonAsync<IReadOnlyCollection<PublishedArtifactDto>>(
            $"/api/v1/publish-jobs/{job.Id}/artifacts");
        Assert.NotNull(artifacts);
        var artifact = Assert.Single(artifacts);
        Assert.Equal("Markdown", artifact.Format);

        var createTemplateResponse = await client.PostAsJsonAsync(
            "/api/v1/publish-templates",
            new CreatePublishTemplateRequest("Integration Markdown Template", "Test template", "Markdown", "# {{title}}", true));
        Assert.Equal(HttpStatusCode.Created, createTemplateResponse.StatusCode);
        var template = await createTemplateResponse.Content.ReadFromJsonAsync<PublishTemplateDto>();
        Assert.Equal("integration-markdown-template", template!.Slug);

        var updateTemplateResponse = await client.PutAsJsonAsync(
            $"/api/v1/publish-templates/{template.Id}",
            new UpdatePublishTemplateRequest("Integration Markdown Template Updated", "Updated", "Markdown", "# {{title}}", false, true));
        Assert.Equal(HttpStatusCode.OK, updateTemplateResponse.StatusCode);
        var updatedTemplate = await updateTemplateResponse.Content.ReadFromJsonAsync<PublishTemplateDto>();
        Assert.Equal("integration-markdown-template-updated", updatedTemplate!.Slug);

        var deleteTemplateResponse = await client.DeleteAsync($"/api/v1/publish-templates/{template.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteTemplateResponse.StatusCode);
    }
}
