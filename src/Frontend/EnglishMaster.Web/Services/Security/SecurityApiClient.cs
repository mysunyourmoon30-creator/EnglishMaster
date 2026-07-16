using System.Net.Http.Json;
using EnglishMaster.Contracts.Security;

namespace EnglishMaster.Web.Services.Security;

public interface ISecurityApiClient
{
    Task<UserSearchResponse> SearchUsersAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<UserDto?> GetUserAsync(Guid id, CancellationToken cancellationToken);

    Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);

    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken);

    Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken);

    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);

    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);

    Task<RoleSearchResponse> SearchRolesAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<RoleDto?> GetRoleAsync(Guid id, CancellationToken cancellationToken);

    Task<RoleDto> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken);

    Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken);

    Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PermissionDto>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken);

    Task AssignPermissionAsync(Guid roleId, string permissionKey, CancellationToken cancellationToken);

    Task RemovePermissionAsync(Guid roleId, string permissionKey, CancellationToken cancellationToken);
}

internal sealed class SecurityApiClient(HttpClient httpClient) : ISecurityApiClient
{
    public async Task<UserSearchResponse> SearchUsersAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = QueryString($"api/v1/users?pageNumber={pageNumber}&pageSize={pageSize}", ("search", search), ("isActive", isActive?.ToString()));
        return await GetRequiredAsync<UserSearchResponse>(query, cancellationToken);
    }

    public async Task<UserDto?> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/users/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: cancellationToken);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/users", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<UserDto>(response, cancellationToken);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/users/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<UserDto>(response, cancellationToken);
    }

    public async Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/users/{id}/deactivate", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/users/{userId}/roles/{roleId}", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/users/{userId}/roles/{roleId}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<RoleSearchResponse> SearchRolesAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = QueryString($"api/v1/roles?pageNumber={pageNumber}&pageSize={pageSize}", ("search", search), ("isActive", isActive?.ToString()));
        return await GetRequiredAsync<RoleSearchResponse>(query, cancellationToken);
    }

    public async Task<RoleDto?> GetRoleAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/roles/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<RoleDto>(cancellationToken: cancellationToken);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/roles", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<RoleDto>(response, cancellationToken);
    }

    public async Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/roles/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<RoleDto>(response, cancellationToken);
    }

    public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/roles/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken) =>
        await GetRequiredAsync<IReadOnlyCollection<PermissionDto>>("api/v1/permissions", cancellationToken);

    public async Task<IReadOnlyCollection<PermissionDto>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken) =>
        await GetRequiredAsync<IReadOnlyCollection<PermissionDto>>($"api/v1/roles/{roleId}/permissions", cancellationToken);

    public async Task AssignPermissionAsync(Guid roleId, string permissionKey, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"api/v1/roles/{roleId}/permissions", new AssignPermissionRequest(permissionKey), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task RemovePermissionAsync(Guid roleId, string permissionKey, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/roles/{roleId}/permissions/{Uri.EscapeDataString(permissionKey)}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    private async Task<T> GetRequiredAsync<T>(string endpoint, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<T>(response, cancellationToken);
    }

    private static string QueryString(string endpoint, params (string Name, string? Value)[] parameters)
    {
        foreach (var parameter in parameters.Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value)))
        {
            endpoint += $"&{parameter.Name}={Uri.EscapeDataString(parameter.Value!)}";
        }

        return endpoint;
    }
}
