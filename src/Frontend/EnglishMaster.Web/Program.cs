using System.Security.Claims;
using EnglishMaster.Web.Components;
using EnglishMaster.Web.Services.Books;
using EnglishMaster.Web.Services.BulkOperations;
using EnglishMaster.Web.Services.Categories;
using EnglishMaster.Web.Services.ContentQuality;
using EnglishMaster.Web.Services.ContentRevisions;
using EnglishMaster.Web.Services.Courses;
using EnglishMaster.Web.Services.Grammar;
using EnglishMaster.Web.Services.ImportExport;
using EnglishMaster.Web.Services.ImportJobs;
using EnglishMaster.Web.Services.Lessons;
using EnglishMaster.Web.Services.LearningRecommendations;
using EnglishMaster.Web.Services.LearningGoals;
using EnglishMaster.Web.Services.LearningReports;
using EnglishMaster.Web.Services.DailyStudyPlans;
using EnglishMaster.Web.Services.Media;
using EnglishMaster.Web.Services.Motivation;
using EnglishMaster.Web.Services.EmailMessages;
using EnglishMaster.Web.Services.Notifications;
using EnglishMaster.Web.Services.Pronunciations;
using EnglishMaster.Web.Services.Practice;
using EnglishMaster.Web.Services.Publishing;
using EnglishMaster.Web.Services.PublicSearch;
using EnglishMaster.Web.Services.Quizzes;
using EnglishMaster.Web.Services.Reports;
using EnglishMaster.Web.Services;
using EnglishMaster.Web.Services.Security;
using EnglishMaster.Web.Services.Tags;
using EnglishMaster.Web.Services.Words;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("EnglishMaster.Web")
    .AddCookie("EnglishMaster.Web", options =>
    {
        options.Cookie.Name = ".EnglishMaster.Web";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddTransient<AuthCookieHandler>();
builder.Services.AddHttpClient("EnglishMaster.Api", client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001/";
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthCookieHandler>();
builder.Services.AddScoped(provider =>
    provider.GetRequiredService<IHttpClientFactory>().CreateClient("EnglishMaster.Api"));
builder.Services.AddScoped<IWordsApiClient, WordsApiClient>();
builder.Services.AddScoped<ICategoriesApiClient, CategoriesApiClient>();
builder.Services.AddScoped<IContentQualityApiClient, ContentQualityApiClient>();
builder.Services.AddScoped<IContentRevisionApiClient, ContentRevisionApiClient>();
builder.Services.AddScoped<ITagsApiClient, TagsApiClient>();
builder.Services.AddScoped<IMediaApiClient, MediaApiClient>();
builder.Services.AddScoped<IMotivationApiClient, MotivationApiClient>();
builder.Services.AddScoped<IPronunciationsApiClient, PronunciationsApiClient>();
builder.Services.AddScoped<IPracticeApiClient, PracticeApiClient>();
builder.Services.AddScoped<IGrammarApiClient, GrammarApiClient>();
builder.Services.AddScoped<IImportExportApiClient, ImportExportApiClient>();
builder.Services.AddScoped<IImportJobApiClient, ImportJobApiClient>();
builder.Services.AddScoped<ILessonApiClient, LessonApiClient>();
builder.Services.AddScoped<ILearningRecommendationApiClient, LearningRecommendationApiClient>();
builder.Services.AddScoped<ILearningGoalApiClient, LearningGoalApiClient>();
builder.Services.AddScoped<ILearningReportApiClient, LearningReportApiClient>();
builder.Services.AddScoped<IDailyStudyPlanApiClient, DailyStudyPlanApiClient>();
builder.Services.AddScoped<ICourseApiClient, CourseApiClient>();
builder.Services.AddScoped<IBookApiClient, BookApiClient>();
builder.Services.AddScoped<IBulkOperationApiClient, BulkOperationApiClient>();
builder.Services.AddScoped<IQuizApiClient, QuizApiClient>();
builder.Services.AddScoped<IReportsApiClient, ReportsApiClient>();
builder.Services.AddScoped<IPublishingApiClient, PublishingApiClient>();
builder.Services.AddScoped<IPublicSearchApiClient, PublicSearchApiClient>();
builder.Services.AddScoped<INotificationsApiClient, NotificationsApiClient>();
builder.Services.AddScoped<IEmailMessagesApiClient, EmailMessagesApiClient>();
builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddScoped<ISecurityApiClient, SecurityApiClient>();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"]);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase) &&
        context.User.Identity?.IsAuthenticated != true)
    {
        context.Response.Redirect($"/login?returnUrl={Uri.EscapeDataString(context.Request.Path + context.Request.QueryString)}");
        return;
    }

    await next(context);
});
app.UseAntiforgery();

app.MapPost("/login", async (
    HttpContext httpContext,
    IAuthApiClient authApiClient,
    CancellationToken cancellationToken) =>
{
    var form = await httpContext.Request.ReadFormAsync(cancellationToken);
    var email = form["email"].ToString();
    var password = form["password"].ToString();

    try
    {
        var (login, apiCookie) = await authApiClient.LoginAsync(new(email, password), cancellationToken);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, login.User.Id.ToString()),
            new(ClaimTypes.Name, login.User.DisplayName),
            new(ClaimTypes.Email, login.User.Email)
        };
        claims.AddRange(login.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(login.User.Permissions.Select(permission => new Claim("permission", permission)));
        if (!string.IsNullOrWhiteSpace(apiCookie))
        {
            claims.Add(new Claim("api_cookie", apiCookie));
        }

        await httpContext.SignInAsync(
            "EnglishMaster.Web",
            new ClaimsPrincipal(new ClaimsIdentity(claims, "EnglishMaster.Web")),
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        var returnUrl = httpContext.Request.Query["returnUrl"].ToString();
        return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/admin" : returnUrl);
    }
    catch (ApiRequestException)
    {
        return Results.Redirect("/login?error=Invalid%20email%20or%20password.");
    }
}).DisableAntiforgery();

app.MapPost("/logout", async (
    HttpContext httpContext,
    IAuthApiClient authApiClient,
    CancellationToken cancellationToken) =>
{
    var apiCookie = httpContext.User.FindFirstValue("api_cookie");
    await authApiClient.LogoutAsync(apiCookie, cancellationToken);
    await httpContext.SignOutAsync("EnglishMaster.Web");
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
