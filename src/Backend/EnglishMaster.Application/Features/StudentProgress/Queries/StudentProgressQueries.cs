using EnglishMaster.Application.Features.StudentProgress.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.StudentProgress.Queries;

public sealed record GetMyStudentProgressQuery(Guid UserId, int? Limit);

public sealed class StudentProgressQueryHandler
{
    private const int DefaultLimit = 20;
    private const int MaximumLimit = 50;
    private readonly IStudentProgressRepository repository;

    public StudentProgressQueryHandler(IStudentProgressRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<StudentProgressSummaryDto>> HandleAsync(
        GetMyStudentProgressQuery query,
        CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(query.Limit ?? DefaultLimit, 1, MaximumLimit);
        var summary = await repository.GetSummaryAsync(query.UserId, limit, cancellationToken);
        return Result<StudentProgressSummaryDto>.Success(summary);
    }
}
