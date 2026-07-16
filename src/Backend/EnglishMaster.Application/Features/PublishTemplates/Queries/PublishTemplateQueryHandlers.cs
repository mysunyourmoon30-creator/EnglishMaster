using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Application.Features.PublishTemplates.Dtos;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublishTemplates.Queries;

public sealed class GetPublishTemplateByIdQueryHandler
{
    private readonly IPublishTemplateRepository templateRepository;

    public GetPublishTemplateByIdQueryHandler(IPublishTemplateRepository templateRepository)
    {
        this.templateRepository = templateRepository;
    }

    public async Task<Result<PublishTemplateDto>> HandleAsync(GetPublishTemplateByIdQuery query, CancellationToken cancellationToken)
    {
        var template = await templateRepository.GetByIdAsync(query.Id, cancellationToken);
        return template is null
            ? Result<PublishTemplateDto>.NotFound(nameof(query.Id), "Publish template was not found.")
            : Result<PublishTemplateDto>.Success(PublishTemplateMapper.ToDto(template));
    }
}

public sealed class SearchPublishTemplatesQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;

    private readonly IPublishTemplateRepository templateRepository;

    public SearchPublishTemplatesQueryHandler(IPublishTemplateRepository templateRepository)
    {
        this.templateRepository = templateRepository;
    }

    public async Task<Result<PublishTemplateSearchResponse>> HandleAsync(SearchPublishTemplatesQuery query, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var format = PublishInputValidator.ParseOptionalEnum<PublishFormat>(query.Format, nameof(query.Format), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);

        if (errors.Count > 0)
        {
            return Result<PublishTemplateSearchResponse>.Validation([.. errors]);
        }

        var result = await templateRepository.SearchAsync(new PublishTemplateSearchCriteria(format, query.IsDefault, query.IsActive, pageNumber, pageSize), cancellationToken);
        var totalPages = result.TotalCount == 0 ? 0 : (int)Math.Ceiling(result.TotalCount / (double)pageSize);
        var items = result.Items.Select(PublishTemplateMapper.ToDto).ToArray();

        return Result<PublishTemplateSearchResponse>.Success(new PublishTemplateSearchResponse(items, pageNumber, pageSize, result.TotalCount, totalPages, pageNumber > 1 && totalPages > 0, pageNumber < totalPages));
    }

    private static int NormalizePageNumber(int? value, ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageNumber;
        }

        if (value.Value < 1)
        {
            errors.Add(new ValidationError(nameof(SearchPublishTemplatesQuery.PageNumber), $"{nameof(SearchPublishTemplatesQuery.PageNumber)} must be greater than or equal to 1."));
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
            errors.Add(new ValidationError(nameof(SearchPublishTemplatesQuery.PageSize), $"{nameof(SearchPublishTemplatesQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
            return DefaultPageSize;
        }

        return value.Value;
    }
}
