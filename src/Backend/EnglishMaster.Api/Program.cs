using System.Security.Claims;
using EnglishMaster.Api.Endpoints;
using EnglishMaster.Api.Health;
using EnglishMaster.Application.Features.BookChapters.Commands;
using EnglishMaster.Application.Features.BookChapters.Queries;
using EnglishMaster.Application.Features.Books.Commands;
using EnglishMaster.Application.Features.Books.Queries;
using EnglishMaster.Application.Features.BulkOperations.Commands;
using EnglishMaster.Application.Features.BulkOperations.Queries;
using EnglishMaster.Application.Features.Categories.Commands;
using EnglishMaster.Application.Features.Categories.Queries;
using EnglishMaster.Application.Features.ContentQuality.Commands;
using EnglishMaster.Application.Features.ContentQuality.Queries;
using EnglishMaster.Application.Features.ContentRevisionRestores.Commands;
using EnglishMaster.Application.Features.ContentRevisionRestores.Queries;
using EnglishMaster.Application.Features.ContentRevisions.Commands;
using EnglishMaster.Application.Features.ContentRevisions.Queries;
using EnglishMaster.Application.Features.Courses.Commands;
using EnglishMaster.Application.Features.Courses.Queries;
using EnglishMaster.Application.Features.EmailMessages.Commands;
using EnglishMaster.Application.Features.EmailMessages.Queries;
using EnglishMaster.Application.Features.GrammarExamples.Commands;
using EnglishMaster.Application.Features.GrammarExamples.Queries;
using EnglishMaster.Application.Features.GrammarRules.Commands;
using EnglishMaster.Application.Features.GrammarRules.Queries;
using EnglishMaster.Application.Features.GrammarTopics.Commands;
using EnglishMaster.Application.Features.GrammarTopics.Queries;
using EnglishMaster.Application.Features.ImportJobs.Commands;
using EnglishMaster.Application.Features.ImportJobs.Queries;
using EnglishMaster.Application.Features.LessonSections.Commands;
using EnglishMaster.Application.Features.LessonSections.Queries;
using EnglishMaster.Application.Features.Lessons.Commands;
using EnglishMaster.Application.Features.Lessons.Queries;
using EnglishMaster.Application.Features.LearningRecommendations.Queries;
using EnglishMaster.Application.Features.LearningGoals.Commands;
using EnglishMaster.Application.Features.LearningGoals.Queries;
using EnglishMaster.Application.Features.DailyStudyPlans.Commands;
using EnglishMaster.Application.Features.DailyStudyPlans.Queries;
using EnglishMaster.Application.Features.LearningReports.Commands;
using EnglishMaster.Application.Features.LearningReports.Queries;
using EnglishMaster.Application.Features.Media.Commands;
using EnglishMaster.Application.Features.Media.Queries;
using EnglishMaster.Application.Features.Motivation.Commands;
using EnglishMaster.Application.Features.Motivation.Queries;
using EnglishMaster.Application.Features.Achievements.Commands;
using EnglishMaster.Application.Features.Achievements.Queries;
using EnglishMaster.Application.Features.MinimalPairs.Commands;
using EnglishMaster.Application.Features.MinimalPairs.Queries;
using EnglishMaster.Application.Features.Notifications.Commands;
using EnglishMaster.Application.Features.Notifications.Queries;
using EnglishMaster.Application.Features.Pronunciations.Commands;
using EnglishMaster.Application.Features.Pronunciations.Queries;
using EnglishMaster.Application.Features.PublishedArtifacts.Queries;
using EnglishMaster.Application.Features.PublishJobs.Commands;
using EnglishMaster.Application.Features.PublishJobs.Queries;
using EnglishMaster.Application.Features.PublishTemplates.Commands;
using EnglishMaster.Application.Features.PublishTemplates.Queries;
using EnglishMaster.Application.Features.PublicSearch.Queries;
using EnglishMaster.Application.Features.Practice.Commands;
using EnglishMaster.Application.Features.Practice.Queries;
using EnglishMaster.Application.Features.QuizChoices.Commands;
using EnglishMaster.Application.Features.QuizChoices.Queries;
using EnglishMaster.Application.Features.QuizQuestions.Commands;
using EnglishMaster.Application.Features.QuizQuestions.Queries;
using EnglishMaster.Application.Features.Quizzes.Commands;
using EnglishMaster.Application.Features.Quizzes.Queries;
using EnglishMaster.Application.Features.Reports.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Application.Features.Tags.Commands;
using EnglishMaster.Application.Features.Tags.Queries;
using EnglishMaster.Application.Features.Words.Commands;
using EnglishMaster.Application.Features.Words.Queries;
using EnglishMaster.Infrastructure;
using EnglishMaster.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddAuthentication(SecurityEndpoints.CookieScheme)
    .AddCookie(SecurityEndpoints.CookieScheme, options =>
    {
        options.Cookie.Name = ".EnglishMaster.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing")
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization(options =>
{
    foreach (var permission in Permissions.All)
    {
        options.AddPolicy(permission.Key, policy =>
            policy.RequireClaim(SecurityPermissionClaimTypes.Permission, permission.Key));
    }
});
builder.Services.AddScoped<CreateCategoryCommandHandler>();
builder.Services.AddScoped<UpdateCategoryCommandHandler>();
builder.Services.AddScoped<DeleteCategoryCommandHandler>();
builder.Services.AddScoped<GetCategoryByIdQueryHandler>();
builder.Services.AddScoped<SearchCategoriesQueryHandler>();
builder.Services.AddScoped<ContentQualityCommandHandler>();
builder.Services.AddScoped<ContentQualityQueryHandler>();
builder.Services.AddScoped<ContentRevisionCommandHandler>();
builder.Services.AddScoped<ContentRevisionQueryHandler>();
builder.Services.AddScoped<ContentRevisionRestoreCommandHandler>();
builder.Services.AddScoped<ContentRevisionRestoreQueryHandler>();
builder.Services.AddScoped<BulkOperationCommandHandler>();
builder.Services.AddScoped<BulkOperationQueryHandler>();
builder.Services.AddScoped<ImportJobCommandHandler>();
builder.Services.AddScoped<ImportJobQueryHandler>();
builder.Services.AddScoped<CreateTagCommandHandler>();
builder.Services.AddScoped<UpdateTagCommandHandler>();
builder.Services.AddScoped<DeleteTagCommandHandler>();
builder.Services.AddScoped<GetTagByIdQueryHandler>();
builder.Services.AddScoped<SearchTagsQueryHandler>();
builder.Services.AddScoped<CreateGrammarTopicCommandHandler>();
builder.Services.AddScoped<UpdateGrammarTopicCommandHandler>();
builder.Services.AddScoped<DeleteGrammarTopicCommandHandler>();
builder.Services.AddScoped<ActivateGrammarTopicCommandHandler>();
builder.Services.AddScoped<DeactivateGrammarTopicCommandHandler>();
builder.Services.AddScoped<GetGrammarTopicByIdQueryHandler>();
builder.Services.AddScoped<SearchGrammarTopicsQueryHandler>();
builder.Services.AddScoped<CreateGrammarRuleCommandHandler>();
builder.Services.AddScoped<UpdateGrammarRuleCommandHandler>();
builder.Services.AddScoped<DeleteGrammarRuleCommandHandler>();
builder.Services.AddScoped<ActivateGrammarRuleCommandHandler>();
builder.Services.AddScoped<DeactivateGrammarRuleCommandHandler>();
builder.Services.AddScoped<AddRelatedWordToGrammarRuleCommandHandler>();
builder.Services.AddScoped<RemoveRelatedWordFromGrammarRuleCommandHandler>();
builder.Services.AddScoped<GetGrammarRuleByIdQueryHandler>();
builder.Services.AddScoped<GetGrammarRulesByTopicIdQueryHandler>();
builder.Services.AddScoped<SearchGrammarRulesQueryHandler>();
builder.Services.AddScoped<AddGrammarExampleCommandHandler>();
builder.Services.AddScoped<UpdateGrammarExampleCommandHandler>();
builder.Services.AddScoped<DeleteGrammarExampleCommandHandler>();
builder.Services.AddScoped<GetGrammarExampleByIdQueryHandler>();
builder.Services.AddScoped<GetGrammarExamplesByRuleIdQueryHandler>();
builder.Services.AddScoped<CreateMediaCommandHandler>();
builder.Services.AddScoped<UpdateMediaCommandHandler>();
builder.Services.AddScoped<DeleteMediaCommandHandler>();
builder.Services.AddScoped<ActivateMediaCommandHandler>();
builder.Services.AddScoped<DeactivateMediaCommandHandler>();
builder.Services.AddScoped<UploadMediaCommandHandler>();
builder.Services.AddScoped<GetMediaByIdQueryHandler>();
builder.Services.AddScoped<SearchMediaQueryHandler>();
builder.Services.AddScoped<CreatePronunciationCommandHandler>();
builder.Services.AddScoped<UpdatePronunciationCommandHandler>();
builder.Services.AddScoped<DeletePronunciationCommandHandler>();
builder.Services.AddScoped<ActivatePronunciationCommandHandler>();
builder.Services.AddScoped<DeactivatePronunciationCommandHandler>();
builder.Services.AddScoped<GetPronunciationByIdQueryHandler>();
builder.Services.AddScoped<GetPronunciationByWordIdQueryHandler>();
builder.Services.AddScoped<SearchPronunciationsQueryHandler>();
builder.Services.AddScoped<AddMinimalPairCommandHandler>();
builder.Services.AddScoped<UpdateMinimalPairCommandHandler>();
builder.Services.AddScoped<DeleteMinimalPairCommandHandler>();
builder.Services.AddScoped<GetMinimalPairByIdQueryHandler>();
builder.Services.AddScoped<GetMinimalPairsByPronunciationIdQueryHandler>();
builder.Services.AddScoped<CreateWordCommandHandler>();
builder.Services.AddScoped<UpdateWordCommandHandler>();
builder.Services.AddScoped<DeleteWordCommandHandler>();
builder.Services.AddScoped<GetWordByIdQueryHandler>();
builder.Services.AddScoped<SearchWordsQueryHandler>();
builder.Services.AddScoped<CreateLessonCommandHandler>();
builder.Services.AddScoped<UpdateLessonCommandHandler>();
builder.Services.AddScoped<DeleteLessonCommandHandler>();
builder.Services.AddScoped<PublishLessonCommandHandler>();
builder.Services.AddScoped<UnpublishLessonCommandHandler>();
builder.Services.AddScoped<ActivateLessonCommandHandler>();
builder.Services.AddScoped<DeactivateLessonCommandHandler>();
builder.Services.AddScoped<AddWordToLessonCommandHandler>();
builder.Services.AddScoped<RemoveWordFromLessonCommandHandler>();
builder.Services.AddScoped<AddGrammarRuleToLessonCommandHandler>();
builder.Services.AddScoped<RemoveGrammarRuleFromLessonCommandHandler>();
builder.Services.AddScoped<GetLessonByIdQueryHandler>();
builder.Services.AddScoped<SearchLessonsQueryHandler>();
builder.Services.AddScoped<LearningRecommendationQueryHandler>();
builder.Services.AddScoped<LearningGoalCommandHandler>();
builder.Services.AddScoped<LearningGoalQueryHandler>();
builder.Services.AddScoped<DailyStudyPlanCommandHandler>();
builder.Services.AddScoped<DailyStudyPlanQueryHandler>();
builder.Services.AddScoped<AddLessonSectionCommandHandler>();
builder.Services.AddScoped<UpdateLessonSectionCommandHandler>();
builder.Services.AddScoped<DeleteLessonSectionCommandHandler>();
builder.Services.AddScoped<ReorderLessonSectionsCommandHandler>();
builder.Services.AddScoped<GetLessonSectionByIdQueryHandler>();
builder.Services.AddScoped<GetLessonSectionsByLessonIdQueryHandler>();
builder.Services.AddScoped<CreateCourseCommandHandler>();
builder.Services.AddScoped<UpdateCourseCommandHandler>();
builder.Services.AddScoped<DeleteCourseCommandHandler>();
builder.Services.AddScoped<PublishCourseCommandHandler>();
builder.Services.AddScoped<UnpublishCourseCommandHandler>();
builder.Services.AddScoped<ActivateCourseCommandHandler>();
builder.Services.AddScoped<DeactivateCourseCommandHandler>();
builder.Services.AddScoped<AddLessonToCourseCommandHandler>();
builder.Services.AddScoped<RemoveLessonFromCourseCommandHandler>();
builder.Services.AddScoped<ReorderCourseLessonsCommandHandler>();
builder.Services.AddScoped<GetCourseByIdQueryHandler>();
builder.Services.AddScoped<SearchCoursesQueryHandler>();
builder.Services.AddScoped<GetCourseLessonsByCourseIdQueryHandler>();
builder.Services.AddScoped<CreateBookCommandHandler>();
builder.Services.AddScoped<UpdateBookCommandHandler>();
builder.Services.AddScoped<DeleteBookCommandHandler>();
builder.Services.AddScoped<PublishBookCommandHandler>();
builder.Services.AddScoped<UnpublishBookCommandHandler>();
builder.Services.AddScoped<ActivateBookCommandHandler>();
builder.Services.AddScoped<DeactivateBookCommandHandler>();
builder.Services.AddScoped<GetBookByIdQueryHandler>();
builder.Services.AddScoped<SearchBooksQueryHandler>();
builder.Services.AddScoped<AddBookChapterCommandHandler>();
builder.Services.AddScoped<UpdateBookChapterCommandHandler>();
builder.Services.AddScoped<DeleteBookChapterCommandHandler>();
builder.Services.AddScoped<ReorderBookChaptersCommandHandler>();
builder.Services.AddScoped<AddLessonToBookChapterCommandHandler>();
builder.Services.AddScoped<RemoveLessonFromBookChapterCommandHandler>();
builder.Services.AddScoped<ReorderBookChapterLessonsCommandHandler>();
builder.Services.AddScoped<GetBookChapterByIdQueryHandler>();
builder.Services.AddScoped<GetBookChaptersByBookIdQueryHandler>();
builder.Services.AddScoped<GetBookChapterLessonsByBookChapterIdQueryHandler>();
builder.Services.AddScoped<CreateQuizCommandHandler>();
builder.Services.AddScoped<UpdateQuizCommandHandler>();
builder.Services.AddScoped<DeleteQuizCommandHandler>();
builder.Services.AddScoped<PublishQuizCommandHandler>();
builder.Services.AddScoped<UnpublishQuizCommandHandler>();
builder.Services.AddScoped<ActivateQuizCommandHandler>();
builder.Services.AddScoped<DeactivateQuizCommandHandler>();
builder.Services.AddScoped<GetQuizByIdQueryHandler>();
builder.Services.AddScoped<SearchQuizzesQueryHandler>();
builder.Services.AddScoped<AddQuizQuestionCommandHandler>();
builder.Services.AddScoped<UpdateQuizQuestionCommandHandler>();
builder.Services.AddScoped<DeleteQuizQuestionCommandHandler>();
builder.Services.AddScoped<ActivateQuizQuestionCommandHandler>();
builder.Services.AddScoped<DeactivateQuizQuestionCommandHandler>();
builder.Services.AddScoped<ReorderQuizQuestionsCommandHandler>();
builder.Services.AddScoped<GetQuizQuestionByIdQueryHandler>();
builder.Services.AddScoped<GetQuizQuestionsByQuizIdQueryHandler>();
builder.Services.AddScoped<AddQuizChoiceCommandHandler>();
builder.Services.AddScoped<UpdateQuizChoiceCommandHandler>();
builder.Services.AddScoped<DeleteQuizChoiceCommandHandler>();
builder.Services.AddScoped<ActivateQuizChoiceCommandHandler>();
builder.Services.AddScoped<DeactivateQuizChoiceCommandHandler>();
builder.Services.AddScoped<ReorderQuizChoicesCommandHandler>();
builder.Services.AddScoped<GetQuizChoiceByIdQueryHandler>();
builder.Services.AddScoped<GetQuizChoicesByQuestionIdQueryHandler>();
builder.Services.AddScoped<GetAdminDashboardSummaryQueryHandler>();
builder.Services.AddScoped<GetContentStatusSummaryQueryHandler>();
builder.Services.AddScoped<GetLearningProgressSummaryQueryHandler>();
builder.Services.AddScoped<GetQuizAnalyticsSummaryQueryHandler>();
builder.Services.AddScoped<GetRecentActivitySummaryQueryHandler>();
builder.Services.AddScoped<NotificationCommandHandler>();
builder.Services.AddScoped<NotificationQueryHandler>();
builder.Services.AddScoped<EmailMessageCommandHandler>();
builder.Services.AddScoped<EmailMessageQueryHandler>();
builder.Services.AddScoped<EmailProviderQueryHandler>();
builder.Services.AddScoped<CreatePublishJobCommandHandler>();
builder.Services.AddScoped<StartPublishJobCommandHandler>();
builder.Services.AddScoped<CompletePublishJobCommandHandler>();
builder.Services.AddScoped<FailPublishJobCommandHandler>();
builder.Services.AddScoped<CancelPublishJobCommandHandler>();
builder.Services.AddScoped<RunPublishJobCommandHandler>();
builder.Services.AddScoped<GetPublishJobByIdQueryHandler>();
builder.Services.AddScoped<SearchPublishJobsQueryHandler>();
builder.Services.AddScoped<CreatePublishTemplateCommandHandler>();
builder.Services.AddScoped<UpdatePublishTemplateCommandHandler>();
builder.Services.AddScoped<DeletePublishTemplateCommandHandler>();
builder.Services.AddScoped<ActivatePublishTemplateCommandHandler>();
builder.Services.AddScoped<DeactivatePublishTemplateCommandHandler>();
builder.Services.AddScoped<GetPublishTemplateByIdQueryHandler>();
builder.Services.AddScoped<SearchPublishTemplatesQueryHandler>();
builder.Services.AddScoped<GetPublishedArtifactByIdQueryHandler>();
builder.Services.AddScoped<GetArtifactsByPublishJobIdQueryHandler>();
builder.Services.AddScoped<SearchPublishedArtifactsQueryHandler>();
builder.Services.AddScoped<LoginCommandHandler>();
builder.Services.AddScoped<GetCurrentUserQueryHandler>();
builder.Services.AddScoped<ChangePasswordCommandHandler>();
builder.Services.AddScoped<UserCommandQueryHandlers>();
builder.Services.AddScoped<RoleCommandQueryHandlers>();
builder.Services.AddScoped<PermissionQueryHandler>();
builder.Services.AddScoped<PublicSearchQueryHandler>();
builder.Services.AddScoped<PracticeCommandHandler>();
builder.Services.AddScoped<PracticeQueryHandler>();
builder.Services.AddScoped<MotivationCommandHandler>();
builder.Services.AddScoped<MotivationQueryHandler>();
builder.Services.AddScoped<AchievementCommandHandler>();
builder.Services.AddScoped<AchievementQueryHandler>();
builder.Services.AddScoped<LearningReportCommandHandler>();
builder.Services.AddScoped<LearningReportQueryHandler>();
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if ((builder.Environment.IsProduction() || builder.Environment.IsStaging()) &&
    string.IsNullOrWhiteSpace(defaultConnectionString))
{
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured for staging and production.");
}

builder.Services.AddInfrastructure(defaultConnectionString, builder.Configuration);
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);

var app = builder.Build();

if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(httpContext =>
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Results.Problem(
                title: "An unexpected error occurred.",
                statusCode: StatusCodes.Status500InternalServerError)
                .ExecuteAsync(httpContext);
        });
    });
    app.UseHsts();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.Use(async (context, next) =>
{
    if (app.Environment.IsEnvironment("Testing") &&
        context.User.Identity?.IsAuthenticated != true &&
        IsNonSecurityApiRoute(context.Request.Path))
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, "Integration Test Admin")
        };
        claims.AddRange(Permissions.All.Select(permission =>
            new Claim(SecurityPermissionClaimTypes.Permission, permission.Key)));
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, SecurityEndpoints.CookieScheme));
    }

    await next(context);
});
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var isApiRoute = path.StartsWithSegments("/api/v1", StringComparison.OrdinalIgnoreCase);
    var isLoginRoute = path.StartsWithSegments("/api/v1/auth/login", StringComparison.OrdinalIgnoreCase);
    var isPublicApiRoute = path.StartsWithSegments("/api/v1/public", StringComparison.OrdinalIgnoreCase);
    var shouldProtectApiRoute = isApiRoute && !isLoginRoute && !isPublicApiRoute && !app.Environment.IsEnvironment("Testing");
    if (shouldProtectApiRoute && context.User.Identity?.IsAuthenticated != true)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    await next(context);
});
app.UseAuthorization();

var mediaRoot = builder.Configuration["Media:LocalStoragePath"] is { Length: > 0 } configuredMediaRoot
    ? configuredMediaRoot
    : Path.Combine(AppContext.BaseDirectory, "media");
Directory.CreateDirectory(mediaRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(mediaRoot),
    RequestPath = "/media"
});

var publishingRoot = builder.Configuration["Publishing:LocalStoragePath"] is { Length: > 0 } configuredPublishingRoot
    ? configuredPublishingRoot
    : Path.Combine(AppContext.BaseDirectory, "publishing");
Directory.CreateDirectory(publishingRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(publishingRoot),
    RequestPath = "/publishing"
});

app.MapGet("/", () => Results.Ok(new { Name = "EnglishMaster.Api", Status = "OK" }));
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});
app.MapSecurityEndpoints();
app.MapCategoryEndpoints();
app.MapTagEndpoints();
app.MapMediaEndpoints();
app.MapPronunciationEndpoints();
app.MapGrammarEndpoints();
app.MapWordEndpoints();
app.MapLessonEndpoints();
app.MapLearningRecommendationEndpoints();
app.MapLearningGoalEndpoints();
app.MapDailyStudyPlanEndpoints();
app.MapCourseEndpoints();
app.MapBookEndpoints();
app.MapQuizEndpoints();
app.MapReportEndpoints();
app.MapNotificationEndpoints();
app.MapContentQualityEndpoints();
app.MapContentRevisionEndpoints();
app.MapBulkOperationEndpoints();
app.MapImportJobEndpoints();
app.MapPublishingEndpoints();
app.MapContentImportExportEndpoints();
app.MapPublicSearchEndpoints();
app.MapPracticeEndpoints();
app.MapMotivationEndpoints();
app.MapAchievementEndpoints();
app.MapLearningReportEndpoints();

if (!app.Environment.IsEnvironment("Testing"))
{
    await SecuritySeeder.SeedSecurityAsync(app.Services, app.Configuration);
}

app.Run();

static bool IsNonSecurityApiRoute(PathString path)
{
    if (!path.StartsWithSegments("/api/v1", out var remaining))
    {
        return false;
    }

    var value = remaining.Value ?? string.Empty;
    return !value.StartsWith("/auth", StringComparison.OrdinalIgnoreCase) &&
        !value.StartsWith("/users", StringComparison.OrdinalIgnoreCase) &&
        !value.StartsWith("/roles", StringComparison.OrdinalIgnoreCase) &&
        !value.StartsWith("/permissions", StringComparison.OrdinalIgnoreCase);
}

public partial class Program;
