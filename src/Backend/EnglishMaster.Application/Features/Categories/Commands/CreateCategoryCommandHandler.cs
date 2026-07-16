using EnglishMaster.Application.Features.Categories.Dtos;
using EnglishMaster.Contracts.Categories;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Categories.Commands;

public sealed class CreateCategoryCommandHandler
{
    private readonly ICategoryRepository categoryRepository;
    private readonly TimeProvider timeProvider;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        TimeProvider timeProvider)
    {
        this.categoryRepository = categoryRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<CategoryDto>> HandleAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var validation = CategoryInputValidator.Validate(
            command.Name,
            command.Description,
            command.SortOrder,
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<CategoryDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await categoryRepository.SlugExistsAsync(input.Slug, null, cancellationToken))
        {
            return Result<CategoryDto>.Validation(
                new ValidationError(nameof(command.Name), "A category with this name already exists."));
        }

        var category = Category.Create(
            input.Name,
            input.Description,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await categoryRepository.AddAsync(category, cancellationToken);
        await categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Success(CategoryMapper.ToDto(category));
    }
}
