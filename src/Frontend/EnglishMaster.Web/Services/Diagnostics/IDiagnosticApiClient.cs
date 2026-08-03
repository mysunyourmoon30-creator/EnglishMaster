using EnglishMaster.Contracts.Diagnostics;

namespace EnglishMaster.Web.Services.Diagnostics;

public interface IDiagnosticApiClient
{
    Task<DiagnosticQuizDto> GetQuizAsync(Guid quizId, CancellationToken cancellationToken);
    Task<DiagnosticResultDto> SubmitAsync(Guid quizId, SubmitDiagnosticRequest request, CancellationToken cancellationToken);
}
