using EnglishMaster.Contracts.StudentProgress;

namespace EnglishMaster.Web.Services.StudentProgress;

public interface IStudentProgressApiClient
{
    Task<StudentProgressSummaryDto> GetSummaryAsync(int limit, CancellationToken cancellationToken);
}
