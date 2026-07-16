using EnglishMaster.Application.Features.LearningReports.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LearningReports.Commands;

public sealed record GenerateWeeklyLearningReportCommand(Guid UserId, DateTimeOffset? WeekStartDate, bool Regenerate);
public sealed record RegenerateWeeklyLearningReportCommand(Guid UserId, Guid ReportId);
public sealed record ArchiveWeeklyLearningReportCommand(Guid UserId, Guid ReportId);

public sealed class LearningReportCommandHandler
{
    private readonly ILearningReportRepository repository;

    public LearningReportCommandHandler(ILearningReportRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<WeeklyLearningReportDto>> GenerateAsync(GenerateWeeklyLearningReportCommand command, CancellationToken cancellationToken) =>
        Result<WeeklyLearningReportDto>.Success(await repository.GenerateWeeklyReportAsync(command.UserId, command.WeekStartDate ?? DateTimeOffset.UtcNow, command.Regenerate, cancellationToken));

    public async Task<Result<WeeklyLearningReportDto>> RegenerateAsync(RegenerateWeeklyLearningReportCommand command, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(command.UserId, command.ReportId, cancellationToken);
        return existing is null
            ? Result<WeeklyLearningReportDto>.NotFound(nameof(command.ReportId), "Learning report was not found.")
            : Result<WeeklyLearningReportDto>.Success(await repository.GenerateWeeklyReportAsync(command.UserId, existing.WeekStartDate, regenerate: true, cancellationToken));
    }

    public async Task<Result<WeeklyLearningReportDto>> ArchiveAsync(ArchiveWeeklyLearningReportCommand command, CancellationToken cancellationToken)
    {
        var report = await repository.ArchiveAsync(command.UserId, command.ReportId, cancellationToken);
        return report is null ? Result<WeeklyLearningReportDto>.NotFound(nameof(command.ReportId), "Learning report was not found.") : Result<WeeklyLearningReportDto>.Success(report);
    }
}
