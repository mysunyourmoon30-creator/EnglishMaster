using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.Application.Features.PublishJobs.Dtos;

public enum PublishJobSortBy
{
    CreatedAt = 1,
    Title = 2,
    Status = 3
}

public enum PublishSortDirection
{
    Asc = 1,
    Desc = 2
}

public sealed record PublishJobSearchCriteria(
    PublishSourceType? SourceType,
    Guid? SourceId,
    PublishFormat? Format,
    PublishJobStatus? Status,
    int PageNumber,
    int PageSize,
    PublishJobSortBy SortBy,
    PublishSortDirection SortDirection);

public sealed record PublishJobSearchResult(
    IReadOnlyCollection<PublishJob> Items,
    int TotalCount);

public static class PublishJobMapper
{
    public static PublishJobDto ToDto(PublishJob publishJob)
    {
        return new PublishJobDto(
            publishJob.Id,
            publishJob.SourceType.ToString(),
            publishJob.SourceId,
            publishJob.Format.ToString(),
            publishJob.Status.ToString(),
            publishJob.Title,
            publishJob.OutputFileName,
            publishJob.OutputPath,
            publishJob.ErrorMessage,
            publishJob.RequestedBy,
            publishJob.StartedAt,
            publishJob.CompletedAt,
            publishJob.CreatedAt,
            publishJob.UpdatedAt);
    }
}
