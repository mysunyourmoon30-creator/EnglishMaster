using EnglishMaster.Application.Features.StudentProgress.Dtos;

namespace EnglishMaster.Application.Features.StudentProgress;

public interface IStudentProgressRepository
{
    Task<StudentProgressSummaryDto> GetSummaryAsync(
        Guid userId,
        int limit,
        CancellationToken cancellationToken);
}
