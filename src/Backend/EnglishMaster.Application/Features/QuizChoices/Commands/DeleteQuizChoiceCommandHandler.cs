using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizChoices.Commands;

public sealed class DeleteQuizChoiceCommandHandler
{
    private readonly IQuizChoiceRepository choiceRepository;
    private readonly TimeProvider timeProvider;

    public DeleteQuizChoiceCommandHandler(
        IQuizChoiceRepository choiceRepository,
        TimeProvider timeProvider)
    {
        this.choiceRepository = choiceRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(DeleteQuizChoiceCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.Id), $"{nameof(command.Id)} cannot be empty."));
        }

        var choice = await choiceRepository.GetByIdAsync(command.Id, cancellationToken);
        if (choice is null)
        {
            return Result.NotFound(nameof(command.Id), "Quiz choice was not found.");
        }

        choice.Deactivate(timeProvider.GetUtcNow());
        await choiceRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
