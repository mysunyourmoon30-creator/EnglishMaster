using EnglishMaster.Application.Features.QuizChoices.Dtos;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizChoices.Commands;

public abstract class QuizChoiceLifecycleCommandHandler<TCommand>
{
    private readonly IQuizChoiceRepository choiceRepository;
    private readonly TimeProvider timeProvider;

    protected QuizChoiceLifecycleCommandHandler(
        IQuizChoiceRepository choiceRepository,
        TimeProvider timeProvider)
    {
        this.choiceRepository = choiceRepository;
        this.timeProvider = timeProvider;
    }

    protected async Task<Result<QuizChoiceDto>> HandleLifecycleAsync(
        Guid id,
        Action<QuizChoice, DateTimeOffset> apply,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return Result<QuizChoiceDto>.Validation(
                new ValidationError(nameof(id), $"{nameof(id)} cannot be empty."));
        }

        var choice = await choiceRepository.GetByIdAsync(id, cancellationToken);
        if (choice is null)
        {
            return Result<QuizChoiceDto>.NotFound(nameof(id), "Quiz choice was not found.");
        }

        apply(choice, timeProvider.GetUtcNow());
        await choiceRepository.SaveChangesAsync(cancellationToken);

        return Result<QuizChoiceDto>.Success(QuizChoiceMapper.ToDto(choice));
    }
}

public sealed class ActivateQuizChoiceCommandHandler : QuizChoiceLifecycleCommandHandler<ActivateQuizChoiceCommand>
{
    public ActivateQuizChoiceCommandHandler(IQuizChoiceRepository choiceRepository, TimeProvider timeProvider)
        : base(choiceRepository, timeProvider)
    {
    }

    public Task<Result<QuizChoiceDto>> HandleAsync(ActivateQuizChoiceCommand command, CancellationToken cancellationToken)
    {
        return HandleLifecycleAsync(command.Id, static (choice, now) => choice.Activate(now), cancellationToken);
    }
}

public sealed class DeactivateQuizChoiceCommandHandler : QuizChoiceLifecycleCommandHandler<DeactivateQuizChoiceCommand>
{
    public DeactivateQuizChoiceCommandHandler(IQuizChoiceRepository choiceRepository, TimeProvider timeProvider)
        : base(choiceRepository, timeProvider)
    {
    }

    public Task<Result<QuizChoiceDto>> HandleAsync(DeactivateQuizChoiceCommand command, CancellationToken cancellationToken)
    {
        return HandleLifecycleAsync(command.Id, static (choice, now) => choice.Deactivate(now), cancellationToken);
    }
}
