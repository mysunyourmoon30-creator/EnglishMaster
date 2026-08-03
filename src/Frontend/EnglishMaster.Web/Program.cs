using System.Net;
using System.Security.Claims;

using EnglishMaster.Web.Components;
using EnglishMaster.Web.Services;
using EnglishMaster.Web.Services.Analytics;
using EnglishMaster.Web.Services.Books;
using EnglishMaster.Web.Services.BulkOperations;
using EnglishMaster.Web.Services.Categories;
using EnglishMaster.Web.Services.Certificates;
using EnglishMaster.Web.Services.ContentQuality;
using EnglishMaster.Web.Services.ContentRevisions;
using EnglishMaster.Web.Services.Courses;
using EnglishMaster.Web.Services.DailyStudyPlans;
using EnglishMaster.Web.Services.EmailMessages;
using EnglishMaster.Web.Services.Grammar;
using EnglishMaster.Web.Services.ImportExport;
using EnglishMaster.Web.Services.ImportJobs;
using EnglishMaster.Web.Services.LearningGoals;
using EnglishMaster.Web.Services.LearningRecommendations;
using EnglishMaster.Web.Services.LearningReports;
using EnglishMaster.Web.Services.Lessons;
using EnglishMaster.Web.Services.Media;
using EnglishMaster.Web.Services.Motivation;
using EnglishMaster.Web.Services.Navigation;
using EnglishMaster.Web.Services.Notifications;
using EnglishMaster.Web.Services.Practice;
using EnglishMaster.Web.Services.Pronunciations;
using EnglishMaster.Web.Services.PublicSearch;
using EnglishMaster.Web.Services.Publishing;
using EnglishMaster.Web.Services.Quizzes;
using EnglishMaster.Web.Services.Reports;
using EnglishMaster.Web.Services.Security;
using EnglishMaster.Web.Services.SystemHealth;
using EnglishMaster.Web.Services.Tags;
using EnglishMaster.Web.Services.Words;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
var allowInsecureLoopbackCookies =
    builder.Configuration.GetValue<bool>("Auth:AllowInsecureLoopbackCookies");
if (allowInsecureLoopbackCookies &&
    !builder.Environment.IsStaging() &&
    !builder.Environment.IsEnvironment("Testing"))
{
    throw new InvalidOperationException(
        "Auth:AllowInsecureLoopbackCookies may only be enabled in Staging or Testing.");
}

var forwardedHeadersEnabled =
    builder.Configuration.GetValue<bool>("ForwardedHeaders:Enabled");
if (forwardedHeadersEnabled)
{
    var knownProxyValue = builder.Configuration["ForwardedHeaders:KnownProxy"];
    if (!IPAddress.TryParse(knownProxyValue, out var knownProxy))
    {
        throw new InvalidOperationException(
            "ForwardedHeaders:KnownProxy must be a valid IP address when forwarded headers are enabled.");
    }

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto;
        options.ForwardLimit = 1;
        options.KnownProxies.Add(knownProxy);
    });
}

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(context.Configuration["Logging:FilePath"] is { Length: > 0 } configuredLogPath
            ? configuredLogPath
            : Path.Combine(AppContext.BaseDirectory, "logs"), "englishmaster-web-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
if (builder.Configuration["DataProtection:KeysPath"] is { Length: > 0 } dataProtectionKeysPath)
{
    Directory.CreateDirectory(dataProtectionKeysPath);
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IApiSessionStore, InMemoryApiSessionStore>();
builder.Services.AddAuthentication("EnglishMaster.Web")
    .AddCookie("EnglishMaster.Web", options =>
    {
        options.Cookie.Name = ".EnglishMaster.Web";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Events.OnSigningIn = context =>
        {
            if (allowInsecureLoopbackCookies &&
                IsLoopbackHost(context.Request.Host.Host))
            {
                context.CookieOptions.Secure = false;
            }

            return Task.CompletedTask;
        };
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
})
    // UseCookies = false: the default handler silently consumes Set-Cookie into its own
    // CookieContainer, so AuthApiClient.LoginAsync would never see the API's session cookie
    // to forward it via AuthCookieHandler, and every subsequent request would come back 401.
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false })
    .AddHttpMessageHandler<AuthCookieHandler>();
builder.Services.AddScoped(provider =>
    provider.GetRequiredService<IHttpClientFactory>().CreateClient("EnglishMaster.Api"));
builder.Services.AddScoped<IWordsApiClient, WordsApiClient>();
builder.Services.AddScoped<IAnalyticsApiClient, AnalyticsApiClient>();
builder.Services.AddScoped<ICategoriesApiClient, CategoriesApiClient>();
builder.Services.AddScoped<IContentQualityApiClient, ContentQualityApiClient>();
builder.Services.AddScoped<IContentRevisionApiClient, ContentRevisionApiClient>();
builder.Services.AddScoped<ITagsApiClient, TagsApiClient>();
builder.Services.AddScoped<IMediaApiClient, MediaApiClient>();
builder.Services.AddScoped<IMotivationApiClient, MotivationApiClient>();
builder.Services.AddScoped<IPronunciationsApiClient, PronunciationsApiClient>();
builder.Services.AddScoped<IPracticeApiClient, PracticeApiClient>();
builder.Services.AddScoped<IGrammarApiClient, GrammarApiClient>();
builder.Services.AddScoped<IPublicGrammarApiClient, PublicGrammarApiClient>();
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
builder.Services.AddScoped<ICertificateVerificationApiClient, CertificateVerificationApiClient>();
builder.Services.AddScoped<ICertificateTemplateApiClient, CertificateTemplateApiClient>();
builder.Services.AddScoped<ISystemHealthApiClient, SystemHealthApiClient>();
builder.Services.AddScoped<INotificationsApiClient, NotificationsApiClient>();
builder.Services.AddScoped<IEmailMessagesApiClient, EmailMessagesApiClient>();
builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddScoped<ISecurityApiClient, SecurityApiClient>();
builder.Services.AddScoped<BreadcrumbState>();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"]);

var app = builder.Build();

if (forwardedHeadersEnabled)
{
    app.UseForwardedHeaders();
}

app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
    context.Response.Headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    context.Response.Headers.TryAdd("Content-Security-Policy", "frame-ancestors 'self'");
    context.Response.Headers.TryAdd("X-Frame-Options", "SAMEORIGIN");
    await next(context);
});

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

// Mapped away from "/login" itself: Login.razor's own @page "/login" route also matches
// POST, so mapping the form handler at the same path causes an AmbiguousMatchException.
app.MapPost("/account/login", async (
    HttpContext httpContext,
    IAuthApiClient authApiClient,
    IApiSessionStore apiSessionStore,
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
        var apiSessionId = string.IsNullOrWhiteSpace(apiCookie)
            ? null
            : apiSessionStore.Store(apiCookie);
        if (!string.IsNullOrWhiteSpace(apiSessionId))
        {
            claims.Add(new Claim("api_session_id", apiSessionId));
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
});

app.MapPost("/logout", async (
    HttpContext httpContext,
    IAuthApiClient authApiClient,
    IApiSessionStore apiSessionStore,
    CancellationToken cancellationToken) =>
{
    var apiSessionId = httpContext.User.FindFirstValue("api_session_id");
    var apiCookie = string.IsNullOrWhiteSpace(apiSessionId)
        ? null
        : apiSessionStore.Get(apiSessionId);
    await authApiClient.LogoutAsync(apiCookie, cancellationToken);
    if (!string.IsNullOrWhiteSpace(apiSessionId))
    {
        apiSessionStore.Remove(apiSessionId);
    }

    await httpContext.SignOutAsync("EnglishMaster.Web");
    return Results.Redirect("/login");
});

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

static bool IsLoopbackHost(string host) =>
    string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) ||
    IPAddress.TryParse(host, out var address) && IPAddress.IsLoopback(address);
