using EnglishMaster.Application.Features.Categories.Dtos;
using EnglishMaster.Contracts.Categories;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Categories.Commands;

public sealed class UpdateCategoryCommandHandler
{
    private readonly ICategoryRepository categoryRepository;
    private readonly TimeProvider timeProvider;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        TimeProvider timeProvider)
    {
        this.categoryRepository = categoryRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<CategoryDto>> HandleAsync(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null)
        {
            return Result<CategoryDto>.NotFound(nameof(command.Id), "Category was not found.");
        }

        var validation = CategoryInputValidator.Validate(
            command.Name,
            command.Description,
            command.SortOrder,
            command.IsActive);

        if (!validation.IsSuccess)
        {
            return Result<CategoryDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await categoryRepository.SlugExistsAsync(input.Slug, command.Id, cancellationToken))
        {
            return Result<CategoryDto>.Validation(
                new ValidationError(nameof(command.Name), "A category with this name already exists."));
        }

        category.Update(
            input.Name,
            input.Description,
            input.SortOrder,
            input.IsActive,
            timeProvider.GetUtcNow());

        await categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Success(CategoryMapper.ToDto(category));
    }
}
