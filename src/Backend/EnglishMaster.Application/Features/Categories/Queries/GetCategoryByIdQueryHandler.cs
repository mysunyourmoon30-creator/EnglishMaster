using EnglishMaster.Application.Features.Categories.Dtos;
using EnglishMaster.Contracts.Categories;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Categories.Queries;

public sealed class GetCategoryByIdQueryHandler
{
    private readonly ICategoryRepository categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryDto>> HandleAsync(
        GetCategoryByIdQuery query,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(query.Id, cancellationToken);
        return category is null
            ? Result<CategoryDto>.NotFound(nameof(query.Id), "Category was not found.")
            : Result<CategoryDto>.Success(CategoryMapper.ToDto(category));
    }
}
