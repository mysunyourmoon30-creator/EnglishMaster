using System.Security.Claims;

namespace EnglishMaster.Web.Services.Security;

internal sealed class AuthCookieHandler(
    IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var apiCookie = httpContextAccessor.HttpContext?.User.FindFirstValue("api_cookie");
        if (!string.IsNullOrWhiteSpace(apiCookie) && !request.Headers.Contains("Cookie"))
        {
            request.Headers.Add("Cookie", apiCookie);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
