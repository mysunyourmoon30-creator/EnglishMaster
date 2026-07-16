using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublishJobs.Queries;

public sealed class GetPublishJobByIdQueryHandler
{
    private readonly IPublishJobRepository publishJobRepository;

    public GetPublishJobByIdQueryHandler(IPublishJobRepository publishJobRepository)
    {
        this.publishJobRepository = publishJobRepository;
    }

    public async Task<Result<PublishJobDto>> HandleAsync(GetPublishJobByIdQuery query, CancellationToken cancellationToken)
    {
        var publishJob = await publishJobRepository.GetByIdAsync(query.Id, cancellationToken);
        return publishJob is null
            ? Result<PublishJobDto>.NotFound(nameof(query.Id), "Publish job was not found.")
            : Result<PublishJobDto>.Success(PublishJobMapper.ToDto(publishJob));
    }
}

public sealed class SearchPublishJobsQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;

    private readonly IPublishJobRepository publishJobRepository;

    public SearchPublishJobsQueryHandler(IPublishJobRepository publishJobRepository)
    {
        this.publishJobRepository = publishJobRepository;
    }

    public async Task<Result<PublishJobSearchResponse>> HandleAsync(SearchPublishJobsQuery query, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var sourceType = PublishInputValidator.ParseOptionalEnum<PublishSourceType>(query.SourceType, nameof(query.SourceType), errors);
        var format = PublishInputValidator.ParseOptionalEnum<PublishFormat>(query.Format, nameof(query.Format), errors);
        var status = PublishInputValidator.ParseOptionalEnum<PublishJobStatus>(query.Status, nameof(query.Status), errors);
        PublishInputValidator.ValidateOptionalId(query.SourceId, nameof(query.SourceId), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);
        var sortBy = PublishInputValidator.ParseOptionalEnum(query.SortBy, nameof(query.SortBy), PublishJobSortBy.CreatedAt, errors);
        var sortDirection = PublishInputValidator.ParseOptionalEnum(query.SortDirection, nameof(query.SortDirection), PublishSortDirection.Desc, errors);

        if (errors.Count > 0)
        {
            return Result<PublishJobSearchResponse>.Validation([.. errors]);
        }

        var result = await publishJobRepository.SearchAsync(new PublishJobSearchCriteria(sourceType, query.SourceId, format, status, pageNumber, pageSize, sortBy, sortDirection), cancellationToken);
        var totalPages = result.TotalCount == 0 ? 0 : (int)Math.Ceiling(result.TotalCount / (double)pageSize);
        var items = result.Items.Select(PublishJobMapper.ToDto).ToArray();

        return Result<PublishJobSearchResponse>.Success(new PublishJobSearchResponse(items, pageNumber, pageSize, result.TotalCount, totalPages, pageNumber > 1 && totalPages > 0, pageNumber < totalPages));
    }

    private static int NormalizePageNumber(int? value, ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageNumber;
        }

        if (value.Value < 1)
        {
            errors.Add(new ValidationError(nameof(SearchPublishJobsQuery.PageNumber), $"{nameof(SearchPublishJobsQuery.PageNumber)} must be greater than or equal to 1."));
            return DefaultPageNumber;
        }

        return value.Value;
    }

    private static int NormalizePageSize(int? value, ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageSize;
        }

        if (value.Value < 1 || value.Value > MaximumPageSize)
        {
            errors.Add(new ValidationError(nameof(SearchPublishJobsQuery.PageSize), $"{nameof(SearchPublishJobsQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
            return DefaultPageSize;
        }

        return value.Value;
    }
}
