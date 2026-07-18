using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Certificates;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Certificates;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.Certificates;

public sealed class CertificateGenerationEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task GenerateCourseCertificate_IssuesCertificateForCompletedCourse()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var courseTitle = Unique("Certificate Course");
        Guid courseId = Guid.Empty;
        Guid templateId = Guid.Empty;
        await SeedAsync(dbContext =>
        {
            var template = CertificateTemplate.Create(Unique("course-certificate").ToLowerInvariant(), "Course Completion", string.Empty, "{{student}} completed {{course}} on {{issuedAt}}.", DateTimeOffset.UtcNow);
            var course = Course.Create(courseTitle, "summary", "description", CefrLevel.B1, null, null, 30, 1, DateTimeOffset.UtcNow);
            course.Publish(DateTimeOffset.UtcNow);
            courseId = course.Id;
            templateId = template.Id;
            dbContext.CertificateTemplates.Add(template);
            dbContext.Courses.Add(course);
            dbContext.CourseProgress.Add(CourseProgress.Create(userId, course.Id, 100, LearningProgressStatus.Completed, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync($"/api/v1/me/certificates/courses/{courseId}/generate", new GenerateCourseCertificateRequest(templateId));
        var certificate = await response.Content.ReadFromJsonAsync<IssuedCertificateDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(courseId, certificate!.CourseId);
        Assert.Equal(courseTitle, certificate.CourseTitle);
        Assert.Contains("superadmin", certificate.RenderedBody);
        Assert.Contains(courseTitle, certificate.RenderedBody);
        Assert.StartsWith("cert-", certificate.VerificationCode);
    }

    [Fact]
    public async Task GenerateCourseCertificate_IsIdempotentForSameCourse()
    {
        var userId = await GetSuperAdminUserIdAsync();
        Guid courseId = Guid.Empty;
        Guid templateId = Guid.Empty;
        await SeedAsync(dbContext =>
        {
            var template = CertificateTemplate.Create(Unique("idempotent-template").ToLowerInvariant(), "Course Completion", string.Empty, "{{student}} completed {{course}}.", DateTimeOffset.UtcNow);
            var course = Course.Create(Unique("Idempotent Course"), "summary", "description", CefrLevel.A2, null, null, 30, 1, DateTimeOffset.UtcNow);
            course.Publish(DateTimeOffset.UtcNow);
            courseId = course.Id;
            templateId = template.Id;
            dbContext.CertificateTemplates.Add(template);
            dbContext.Courses.Add(course);
            dbContext.CourseProgress.Add(CourseProgress.Create(userId, course.Id, 100, LearningProgressStatus.Completed, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var first = await client.PostAsJsonAsync($"/api/v1/me/certificates/courses/{courseId}/generate", new GenerateCourseCertificateRequest(templateId));
        var second = await client.PostAsJsonAsync($"/api/v1/me/certificates/courses/{courseId}/generate", new GenerateCourseCertificateRequest(templateId));
        var firstCertificate = await first.Content.ReadFromJsonAsync<IssuedCertificateDto>();
        var secondCertificate = await second.Content.ReadFromJsonAsync<IssuedCertificateDto>();

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal(firstCertificate!.Id, secondCertificate!.Id);
        Assert.Equal(firstCertificate.VerificationCode, secondCertificate.VerificationCode);
    }

    [Fact]
    public async Task GenerateCourseCertificate_RejectsIncompleteCourse()
    {
        var userId = await GetSuperAdminUserIdAsync();
        Guid courseId = Guid.Empty;
        Guid templateId = Guid.Empty;
        await SeedAsync(dbContext =>
        {
            var template = CertificateTemplate.Create(Unique("incomplete-template").ToLowerInvariant(), "Course Completion", string.Empty, "{{student}} completed {{course}}.", DateTimeOffset.UtcNow);
            var course = Course.Create(Unique("Incomplete Course"), "summary", "description", CefrLevel.A2, null, null, 30, 1, DateTimeOffset.UtcNow);
            course.Publish(DateTimeOffset.UtcNow);
            courseId = course.Id;
            templateId = template.Id;
            dbContext.CertificateTemplates.Add(template);
            dbContext.Courses.Add(course);
            dbContext.CourseProgress.Add(CourseProgress.Create(userId, course.Id, 80, LearningProgressStatus.InProgress, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync($"/api/v1/me/certificates/courses/{courseId}/generate", new GenerateCourseCertificateRequest(templateId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task MyCertificates_ReturnsIssuedCertificatesForCurrentUser()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var otherUserId = Guid.NewGuid();
        var ownCourseTitle = Unique("Own Certificate");
        await SeedAsync(dbContext =>
        {
            var template = CertificateTemplate.Create(Unique("list-template").ToLowerInvariant(), "Course Completion", string.Empty, "Body", DateTimeOffset.UtcNow);
            var own = Course.Create(ownCourseTitle, "summary", "description", CefrLevel.A2, null, null, 30, 1, DateTimeOffset.UtcNow);
            var other = Course.Create(Unique("Other Certificate"), "summary", "description", CefrLevel.A2, null, null, 30, 2, DateTimeOffset.UtcNow);
            dbContext.CertificateTemplates.Add(template);
            dbContext.Courses.AddRange(own, other);
            dbContext.IssuedCertificates.Add(IssuedCertificate.Create(userId, own.Id, template.Id, Unique("cert").ToLowerInvariant(), "SuperAdmin", own.Title, template.Code, "Own body", DateTimeOffset.UtcNow));
            dbContext.IssuedCertificates.Add(IssuedCertificate.Create(otherUserId, other.Id, template.Id, Unique("cert").ToLowerInvariant(), "Other", other.Title, template.Code, "Other body", DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var certificates = await client.GetFromJsonAsync<IReadOnlyCollection<IssuedCertificateDto>>("/api/v1/me/certificates");

        Assert.Contains(certificates!, certificate => certificate.CourseTitle == ownCourseTitle);
        Assert.DoesNotContain(certificates!, certificate => certificate.RecipientName == "Other");
    }

    [Fact]
    public async Task PublicCertificateVerification_ReturnsIssuedCertificateWithoutLogin()
    {
        var verificationCode = Unique("cert").ToLowerInvariant();
        var courseTitle = Unique("Public Certificate");
        await SeedAsync(dbContext =>
        {
            var template = CertificateTemplate.Create(Unique("public-template").ToLowerInvariant(), "Course Completion", string.Empty, "Body", DateTimeOffset.UtcNow);
            var course = Course.Create(courseTitle, "summary", "description", CefrLevel.A2, null, null, 30, 1, DateTimeOffset.UtcNow);
            dbContext.CertificateTemplates.Add(template);
            dbContext.Courses.Add(course);
            dbContext.IssuedCertificates.Add(IssuedCertificate.Create(Guid.NewGuid(), course.Id, template.Id, verificationCode, "Public Learner", course.Title, template.Code, "Body", DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });

        var certificate = await client.GetFromJsonAsync<PublicCertificateVerificationDto>($"/api/v1/public/certificates/{verificationCode}");

        Assert.NotNull(certificate);
        Assert.Equal(courseTitle, certificate!.CourseTitle);
        Assert.True(certificate.IsValid);
    }

    [Fact]
    public async Task PublicCertificateVerification_ReturnsNotFoundForUnknownCode()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });

        var response = await client.GetAsync($"/api/v1/public/certificates/{Unique("missing").ToLowerInvariant()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PublicCertificateVerification_MarksRevokedCertificateInvalid()
    {
        var verificationCode = Unique("cert").ToLowerInvariant();
        await SeedAsync(dbContext =>
        {
            var template = CertificateTemplate.Create(Unique("revoked-template").ToLowerInvariant(), "Course Completion", string.Empty, "Body", DateTimeOffset.UtcNow);
            var course = Course.Create(Unique("Revoked Course"), "summary", "description", CefrLevel.A2, null, null, 30, 1, DateTimeOffset.UtcNow);
            var certificate = IssuedCertificate.Create(Guid.NewGuid(), course.Id, template.Id, verificationCode, "Revoked Learner", course.Title, template.Code, "Body", DateTimeOffset.UtcNow);
            certificate.Revoke(DateTimeOffset.UtcNow);
            dbContext.CertificateTemplates.Add(template);
            dbContext.Courses.Add(course);
            dbContext.IssuedCertificates.Add(certificate);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });

        var certificate = await client.GetFromJsonAsync<PublicCertificateVerificationDto>($"/api/v1/public/certificates/{verificationCode}");

        Assert.NotNull(certificate);
        Assert.True(certificate!.IsRevoked);
        Assert.False(certificate.IsValid);
    }

    private async Task<Guid> GetSuperAdminUserIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        return await dbContext.AppUsers.Where(user => user.Email == "superadmin@englishmaster.test").Select(user => user.Id).SingleAsync();
    }

    private async Task SeedAsync(Func<EnglishMasterDbContext, Task> seed)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        await seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
