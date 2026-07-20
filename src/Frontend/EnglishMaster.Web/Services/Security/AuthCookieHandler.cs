using System.Security.Claims;

namespace EnglishMaster.Web.Services.Security;

internal sealed class AuthCookieHandler(
    IHttpContextAccessor httpContextAccessor,
    IApiSessionStore apiSessionStore)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var apiSessionId = httpContextAccessor.HttpContext?.User.FindFirstValue("api_session_id");
        var apiCookie = string.IsNullOrWhiteSpace(apiSessionId)
            ? null
            : apiSessionStore.Get(apiSessionId);
        if (!string.IsNullOrWhiteSpace(apiCookie) && !request.Headers.Contains("Cookie"))
        {
            request.Headers.Add("Cookie", apiCookie);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
