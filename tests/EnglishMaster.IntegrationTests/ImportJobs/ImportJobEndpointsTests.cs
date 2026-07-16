using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.ImportJobs;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Contracts.Words;

namespace EnglishMaster.IntegrationTests.ImportJobs;

public sealed class ImportJobEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task ImportPermissions_AreSeeded()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var permissions = await client.GetFromJsonAsync<IReadOnlyCollection<PermissionDto>>("/api/v1/permissions");

        Assert.Contains(permissions!, permission => permission.Key == "import.read");
        Assert.Contains(permissions!, permission => permission.Key == "import.upload");
        Assert.Contains(permissions!, permission => permission.Key == "import.validate");
        Assert.Contains(permissions!, permission => permission.Key == "import.run");
        Assert.Contains(permissions!, permission => permission.Key == "import.rollback");
    }

    [Fact]
    public async Task UploadImportJob_CreatesUploadedJobAndRows()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var job = await UploadAsync(client, "Text,MeaningTh,CefrLevel\nhello,สวัสดี,A1");
        var rows = await client.GetFromJsonAsync<IReadOnlyCollection<ImportJobRowDto>>($"/api/v1/import-jobs/{job.Id}/rows");

        Assert.Equal("Uploaded", job.Status);
        Assert.Equal(1, job.TotalRows);
        Assert.Single(rows!);
    }

    [Fact]
    public async Task UploadImportJob_WithInvalidJson_ReturnsValidationProblem()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/import-jobs/upload", new UploadImportJobRequest("Words", "JSON", "words.json", "{ invalid json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadImportJob_WithMismatchedExtension_ReturnsValidationProblem()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/import-jobs/upload", new UploadImportJobRequest("Words", "JSON", "words.csv", "[]"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadImportJob_WithPathLikeFileName_ReturnsValidationProblem()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/import-jobs/upload", new UploadImportJobRequest("Words", "CSV", "../words.csv", "Text,MeaningTh,CefrLevel\nhello,meaning,A1"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidateWordImport_WithMissingText_ReturnsRowError()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var job = await UploadAsync(client, "Text,MeaningTh,CefrLevel\n,ความหมาย,A1");

        var validated = await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");
        var errors = await client.GetFromJsonAsync<IReadOnlyCollection<ImportValidationErrorDto>>($"/api/v1/import-jobs/{job.Id}/errors");

        Assert.Equal("ValidationFailed", validated.Status);
        Assert.Contains(errors!, error => error.ErrorCode == "WORD_TEXT_REQUIRED");
    }

    [Fact]
    public async Task ValidateImportJob_CannotRunTwice()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var job = await UploadAsync(client, "Text,MeaningTh,CefrLevel\nsinglevalidate,meaning,A1");
        await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");

        var secondValidate = await client.PostAsync($"/api/v1/import-jobs/{job.Id}/validate", null);

        Assert.Equal(HttpStatusCode.BadRequest, secondValidate.StatusCode);
    }

    [Fact]
    public async Task ValidateCategoryImport_WithMissingName_ReturnsRowError()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var job = await UploadAsync(client, "Name,Slug\n,missing-name", "Categories");

        var validated = await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");
        var errors = await client.GetFromJsonAsync<IReadOnlyCollection<ImportValidationErrorDto>>($"/api/v1/import-jobs/{job.Id}/errors");

        Assert.Equal("ValidationFailed", validated.Status);
        Assert.Contains(errors!, error => error.ErrorCode == "CATEGORY_NAME_REQUIRED");
    }

    [Fact]
    public async Task ValidateLessonImport_WithNegativeEstimatedMinutes_ReturnsRowError()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var job = await UploadAsync(client, "Title,Slug,EstimatedMinutes\nLesson One,lesson-one,-1", "Lessons");

        var validated = await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");
        var errors = await client.GetFromJsonAsync<IReadOnlyCollection<ImportValidationErrorDto>>($"/api/v1/import-jobs/{job.Id}/errors");

        Assert.Equal("ValidationFailed", validated.Status);
        Assert.Contains(errors!, error => error.ErrorCode == "LESSON_ESTIMATED_MINUTES_NEGATIVE");
    }

    [Fact]
    public async Task ValidateQuizImport_WithInvalidPassingScore_ReturnsRowError()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var job = await UploadAsync(client, "Title,PassingScore\nQuiz One,101", "Quizzes");

        var validated = await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");
        var errors = await client.GetFromJsonAsync<IReadOnlyCollection<ImportValidationErrorDto>>($"/api/v1/import-jobs/{job.Id}/errors");

        Assert.Equal("ValidationFailed", validated.Status);
        Assert.Contains(errors!, error => error.ErrorCode == "QUIZ_PASSING_SCORE_INVALID");
    }

    [Fact]
    public async Task ValidateWordImport_DetectsDuplicateText()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await CreateWordAsync(client, "duplicateword");
        var job = await UploadAsync(client, "Text,MeaningTh,CefrLevel\nduplicateword,ซ้ำ,A1");

        await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");
        var errors = await client.GetFromJsonAsync<IReadOnlyCollection<ImportValidationErrorDto>>($"/api/v1/import-jobs/{job.Id}/errors");

        Assert.Contains(errors!, error => error.ErrorCode == "WORD_TEXT_DUPLICATE");
    }

    [Fact]
    public async Task ConfirmImportJob_OnlyWorksAfterPreviewReady()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var job = await UploadAsync(client, "Text,MeaningTh,CefrLevel\nconfirmword,ยืนยัน,A1");

        var earlyConfirm = await client.PostAsync($"/api/v1/import-jobs/{job.Id}/confirm", null);
        Assert.Equal(HttpStatusCode.BadRequest, earlyConfirm.StatusCode);

        var validated = await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");
        Assert.Equal("PreviewReady", validated.Status);

        var confirmed = await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/confirm");
        Assert.Equal("Confirmed", confirmed.Status);
    }

    [Fact]
    public async Task RunImportJob_WithValidWordRows_CreatesWords()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var wordText = $"importword{Guid.NewGuid():N}";
        var job = await UploadAsync(client, $"Text,MeaningTh,CefrLevel\n{wordText},นำเข้า,A1");
        await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");
        await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/confirm");

        var completed = await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/run");
        var search = await client.GetFromJsonAsync<WordSearchResponse>($"/api/v1/words?searchTerm={wordText}");

        Assert.Equal("Completed", completed.Status);
        Assert.Equal(1, completed.ImportedRows);
        Assert.Contains(search!.Items, word => word.Text == wordText);
    }

    [Fact]
    public async Task RollbackImportJob_RemovesCreatedWord()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var wordText = $"rollbackword{Guid.NewGuid():N}";
        var job = await UploadAsync(client, $"Text,MeaningTh,CefrLevel\n{wordText},ย้อนกลับ,A1");
        await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/validate");
        await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/confirm");
        await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/run");

        var rolledBack = await PostAsync(client, $"/api/v1/import-jobs/{job.Id}/rollback");
        var search = await client.GetFromJsonAsync<WordSearchResponse>($"/api/v1/words?searchTerm={wordText}");

        Assert.Equal("RolledBack", rolledBack.Status);
        Assert.DoesNotContain(search!.Items, word => word.Text == wordText);
    }

    private static async Task<ImportJobDto> UploadAsync(HttpClient client, string content, string importType = "Words")
    {
        var response = await client.PostAsJsonAsync("/api/v1/import-jobs/upload", new UploadImportJobRequest(importType, "CSV", "words.csv", content));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ImportJobDto>())!;
    }

    private static async Task<ImportJobDto> PostAsync(HttpClient client, string path)
    {
        var response = await client.PostAsync(path, null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ImportJobDto>())!;
    }

    private static async Task<WordDto> CreateWordAsync(HttpClient client, string text)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/words",
            new CreateWordRequest(text, null, null, null, "Thai meaning", "English meaning", "Noun", "A1", null, null));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<WordDto>())!;
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));
}
