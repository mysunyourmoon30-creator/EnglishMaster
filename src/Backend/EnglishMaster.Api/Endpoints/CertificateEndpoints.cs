using System.Security.Claims;
using EnglishMaster.Application.Features.Certificates;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Certificates;
using EnglishMaster.Shared.Results;
using AppIssuedCertificateDto = EnglishMaster.Application.Features.Certificates.IssuedCertificateDto;
using AppTemplateDto = EnglishMaster.Application.Features.Certificates.CertificateTemplateDto;
using AppTemplateSearchResponse = EnglishMaster.Application.Features.Certificates.CertificateTemplateSearchResponse;
using ContractIssuedCertificateDto = EnglishMaster.Contracts.Certificates.IssuedCertificateDto;
using ContractTemplateDto = EnglishMaster.Contracts.Certificates.CertificateTemplateDto;
using ContractTemplateSearchResponse = EnglishMaster.Contracts.Certificates.CertificateTemplateSearchResponse;

namespace EnglishMaster.Api.Endpoints;

public static class CertificateEndpoints
{
    public static IEndpointRouteBuilder MapCertificateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var templates = endpoints.MapGroup("/api/v1/admin/certificate-templates")
            .WithTags("Admin Certificate Templates");

        templates.MapGet("", SearchTemplatesAsync).RequireAuthorization(Permissions.CertificatesRead);
        templates.MapGet("/{id:guid}", GetTemplateAsync).RequireAuthorization(Permissions.CertificatesRead);
        templates.MapPost("", CreateTemplateAsync).RequireAuthorization(Permissions.CertificatesManage);
        templates.MapPut("/{id:guid}", UpdateTemplateAsync).RequireAuthorization(Permissions.CertificatesManage);
        templates.MapPost("/{id:guid}/activate", ActivateTemplateAsync).RequireAuthorization(Permissions.CertificatesManage);
        templates.MapPost("/{id:guid}/deactivate", DeactivateTemplateAsync).RequireAuthorization(Permissions.CertificatesManage);

        var mine = endpoints.MapGroup("/api/v1/me/certificates")
            .WithTags("Certificates")
            .RequireAuthorization();
        mine.MapGet("", GetMyCertificatesAsync);
        mine.MapPost("/courses/{courseId:guid}/generate", GenerateCourseCertificateAsync);

        return endpoints;
    }

    private static async Task<IResult> SearchTemplatesAsync(CertificateTemplateQueryHandler handler, string? search, bool? isActive, int? pageNumber, int? pageSize, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchCertificateTemplatesQuery(search, isActive, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> GetTemplateAsync(Guid id, CertificateTemplateQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetByIdAsync(new GetCertificateTemplateByIdQuery(id), cancellationToken));

    private static async Task<IResult> CreateTemplateAsync(CreateCertificateTemplateRequest request, CertificateTemplateCommandHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.CreateAsync(new CreateCertificateTemplateCommand(request.Code, request.Name, request.Description, request.BodyTemplate), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/admin/certificate-templates/{result.Value!.Id}", ToContract(result.Value))
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateTemplateAsync(Guid id, UpdateCertificateTemplateRequest request, CertificateTemplateCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.UpdateAsync(new UpdateCertificateTemplateCommand(id, request.Name, request.Description, request.BodyTemplate, request.IsActive), cancellationToken));

    private static async Task<IResult> ActivateTemplateAsync(Guid id, CertificateTemplateCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SetActiveAsync(new SetCertificateTemplateActiveCommand(id, IsActive: true), cancellationToken));

    private static async Task<IResult> DeactivateTemplateAsync(Guid id, CertificateTemplateCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SetActiveAsync(new SetCertificateTemplateActiveCommand(id, IsActive: false), cancellationToken));

    private static async Task<IResult> GetMyCertificatesAsync(ClaimsPrincipal user, CertificateGenerationQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId)
            ? ToHttpResult(await handler.GetMineAsync(new GetMyCertificatesQuery(userId, limit), cancellationToken))
            : Results.Unauthorized();

    private static async Task<IResult> GenerateCourseCertificateAsync(ClaimsPrincipal user, Guid courseId, GenerateCourseCertificateRequest request, CertificateGenerationCommandHandler handler, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        return ToHttpResult(await handler.GenerateAsync(new GenerateCourseCertificateCommand(userId, courseId, request.TemplateId), cancellationToken));
    }

    private static IResult ToHttpResult(Result<AppTemplateDto> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };

    private static IResult ToHttpResult(Result<AppTemplateSearchResponse> result) =>
        result.Status == ResultStatus.Success
            ? Results.Ok(new ContractTemplateSearchResponse(result.Value!.Items.Select(ToContract).ToArray(), result.Value.PageNumber, result.Value.PageSize, result.Value.TotalCount, result.Value.TotalPages, result.Value.HasPreviousPage, result.Value.HasNextPage))
            : Results.Problem();

    private static IResult ToHttpResult(Result<AppIssuedCertificateDto> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppIssuedCertificateDto>> result) =>
        result.Status == ResultStatus.Success
            ? Results.Ok(result.Value!.Select(ToContract).ToArray())
            : Results.Problem();

    private static ContractTemplateDto ToContract(AppTemplateDto template) =>
        new(template.Id, template.Code, template.Name, template.Description, template.BodyTemplate, template.IsActive, template.CreatedAt, template.UpdatedAt);

    private static ContractIssuedCertificateDto ToContract(AppIssuedCertificateDto certificate) =>
        new(certificate.Id, certificate.CourseId, certificate.VerificationCode, certificate.RecipientName, certificate.CourseTitle, certificate.TemplateCode, certificate.RenderedBody, certificate.IssuedAt, certificate.IsRevoked);

    private static Dictionary<string, string[]> ToValidationDictionary(IEnumerable<ValidationError> errors) =>
        errors.GroupBy(error => error.Field).ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
