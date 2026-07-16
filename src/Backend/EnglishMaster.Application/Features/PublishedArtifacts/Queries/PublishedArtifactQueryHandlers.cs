using EnglishMaster.Application.Features.PublishedArtifacts.Dtos;
using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublishedArtifacts.Queries;

public sealed class GetPublishedArtifactByIdQueryHandler
{
    private readonly IPublishedArtifactRepository artifactRepository;

    public GetPublishedArtifactByIdQueryHandler(IPublishedArtifactRepository artifactRepository)
    {
        this.artifactRepository = artifactRepository;
    }

    public async Task<Result<PublishedArtifactDto>> HandleAsync(GetPublishedArtifactByIdQuery query, CancellationToken cancellationToken)
    {
        var artifact = await artifactRepository.GetByIdAsync(query.Id, cancellationToken);
        return artifact is null
            ? Result<PublishedArtifactDto>.NotFound(nameof(query.Id), "Published artifact was not found.")
            : Result<PublishedArtifactDto>.Success(PublishedArtifactMapper.ToDto(artifact));
    }
}

public sealed class GetArtifactsByPublishJobIdQueryHandler
{
    private readonly IPublishedArtifactRepository artifactRepository;

    public GetArtifactsByPublishJobIdQueryHandler(IPublishedArtifactRepository artifactRepository)
    {
        this.artifactRepository = artifactRepository;
    }

    public async Task<Result<IReadOnlyCollection<PublishedArtifactDto>>> HandleAsync(GetArtifactsByPublishJobIdQuery query, CancellationToken cancellationToken)
    {
        if (query.PublishJobId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<PublishedArtifactDto>>.Validation(new ValidationError(nameof(query.PublishJobId), "PublishJobId is required."));
        }

        var artifacts = await artifactRepository.GetByPublishJobIdAsync(query.PublishJobId, cancellationToken);
        return Result<IReadOnlyCollection<PublishedArtifactDto>>.Success(artifacts.Select(PublishedArtifactMapper.ToDto).ToArray());
    }
}

public sealed class SearchPublishedArtifactsQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;

    private readonly IPublishedArtifactRepository artifactRepository;

    public SearchPublishedArtifactsQueryHandler(IPublishedArtifactRepository artifactRepository)
    {
        this.artifactRepository = artifactRepository;
    }

    public async Task<Result<PublishedArtifactSearchResponse>> HandleAsync(SearchPublishedArtifactsQuery query, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        PublishInputValidator.ValidateOptionalId(query.PublishJobId, nameof(query.PublishJobId), errors);
        PublishInputValidator.ValidateOptionalId(query.SourceId, nameof(query.SourceId), errors);
        var sourceType = PublishInputValidator.ParseOptionalEnum<PublishSourceType>(query.SourceType, nameof(query.SourceType), errors);
        var format = PublishInputValidator.ParseOptionalEnum<PublishFormat>(query.Format, nameof(query.Format), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);

        if (errors.Count > 0)
        {
            return Result<PublishedArtifactSearchResponse>.Validation([.. errors]);
        }

        var result = await artifactRepository.SearchAsync(new PublishedArtifactSearchCriteria(query.PublishJobId, sourceType, query.SourceId, format, pageNumber, pageSize), cancellationToken);
        var totalPages = result.TotalCount == 0 ? 0 : (int)Math.Ceiling(result.TotalCount / (double)pageSize);
        var items = result.Items.Select(PublishedArtifactMapper.ToDto).ToArray();

        return Result<PublishedArtifactSearchResponse>.Success(new PublishedArtifactSearchResponse(items, pageNumber, pageSize, result.TotalCount, totalPages, pageNumber > 1 && totalPages > 0, pageNumber < totalPages));
    }

    private static int NormalizePageNumber(int? value, ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageNumber;
        }

        if (value.Value < 1)
        {
            errors.Add(new ValidationError(nameof(SearchPublishedArtifactsQuery.PageNumber), $"{nameof(SearchPublishedArtifactsQuery.PageNumber)} must be greater than or equal to 1."));
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
            errors.Add(new ValidationError(nameof(SearchPublishedArtifactsQuery.PageSize), $"{nameof(SearchPublishedArtifactsQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
            return DefaultPageSize;
        }

        return value.Value;
    }
}
