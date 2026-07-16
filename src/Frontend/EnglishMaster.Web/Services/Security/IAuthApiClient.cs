using EnglishMaster.Contracts.Security;

namespace EnglishMaster.Web.Services.Security;

public interface IAuthApiClient
{
    Task<(LoginResponse Response, string? ApiCookie)> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task LogoutAsync(string? apiCookie, CancellationToken cancellationToken);

    Task<CurrentUserDto?> GetCurrentUserAsync(CancellationToken cancellationToken);
}
