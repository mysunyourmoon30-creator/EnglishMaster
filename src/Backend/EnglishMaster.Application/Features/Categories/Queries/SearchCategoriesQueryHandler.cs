using EnglishMaster.Application.Features.Categories.Dtos;
using EnglishMaster.Contracts.Categories;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Categories.Queries;

public sealed class SearchCategoriesQueryHandler
{
    private const int MaximumSearchLength = CategoryFieldLimits.Name;

    private readonly ICategoryRepository categoryRepository;

    public SearchCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<Result<CategorySearchResponse>> HandleAsync(
        SearchCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);

        if (errors.Count > 0)
        {
            return Result<CategorySearchResponse>.Validation([.. errors]);
        }

        var result = await categoryRepository.SearchAsync(
            new CategorySearchCriteria(search, query.IsActive ?? true),
            cancellationToken);

        return Result<CategorySearchResponse>.Success(new CategorySearchResponse(
            result.Items.Select(CategoryMapper.ToDto).ToArray()));
    }

    private static string? NormalizeSearch(
        string? value,
        ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > MaximumSearchLength)
        {
            errors.Add(new ValidationError(
                nameof(SearchCategoriesQuery.Search),
                $"{nameof(SearchCategoriesQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
        }

        return normalized;
    }
}
