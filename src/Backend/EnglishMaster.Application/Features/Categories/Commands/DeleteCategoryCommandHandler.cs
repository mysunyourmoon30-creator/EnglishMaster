using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Categories.Commands;

public sealed class DeleteCategoryCommandHandler
{
    private readonly ICategoryRepository categoryRepository;
    private readonly TimeProvider timeProvider;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        TimeProvider timeProvider)
    {
        this.categoryRepository = categoryRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null)
        {
            return Result.NotFound(nameof(command.Id), "Category was not found.");
        }

        category.Deactivate(timeProvider.GetUtcNow());
        await categoryRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
