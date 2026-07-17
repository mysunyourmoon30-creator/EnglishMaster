using System.Net.Http.Json;
using EnglishMaster.Contracts.Security;

namespace EnglishMaster.Web.Services.Security;

internal sealed class AuthApiClient(HttpClient httpClient) : IAuthApiClient
{
    public async Task<(LoginResponse Response, string? ApiCookie)> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/auth/login", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        var value = await ApiClientResponseHandler.ReadRequiredAsync<LoginResponse>(response, cancellationToken);

        // A large claim set (e.g. SuperAdmin's full permission list) can exceed the single-cookie
        // size limit, so ASP.NET Core's cookie auth splits it into ".EnglishMaster.Admin=chunks-N"
        // plus ".EnglishMaster.AdminC1", "...AdminC2", etc. All parts must be forwarded together;
        // forwarding only the base cookie leaves the ticket incomplete and every request 401s.
        var cookie = response.Headers.TryGetValues("Set-Cookie", out var cookies)
            ? string.Join(
                "; ",
                cookies
                    .Select(item => item.Split(';', 2)[0])
                    .Where(nameValue => nameValue.Split('=', 2)[0].StartsWith(".EnglishMaster.Admin", StringComparison.Ordinal)))
            : null;
        return (value, string.IsNullOrEmpty(cookie) ? null : cookie);
    }

    public async Task LogoutAsync(string? apiCookie, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/logout");
        if (!string.IsNullOrWhiteSpace(apiCookie))
        {
            request.Headers.Add("Cookie", apiCookie);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
        {
            await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        }
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/auth/me", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<CurrentUserDto>(cancellationToken: cancellationToken);
    }
}
