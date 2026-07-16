using EnglishMaster.Application.Features.QuizQuestions.Dtos;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Commands;

public abstract class QuizQuestionLifecycleCommandHandler<TCommand>
{
    private readonly IQuizQuestionRepository questionRepository;
    private readonly TimeProvider timeProvider;

    protected QuizQuestionLifecycleCommandHandler(
        IQuizQuestionRepository questionRepository,
        TimeProvider timeProvider)
    {
        this.questionRepository = questionRepository;
        this.timeProvider = timeProvider;
    }

    protected async Task<Result<QuizQuestionDto>> HandleLifecycleAsync(
        Guid id,
        Action<QuizQuestion, DateTimeOffset> apply,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return Result<QuizQuestionDto>.Validation(
                new ValidationError(nameof(id), $"{nameof(id)} cannot be empty."));
        }

        var question = await questionRepository.GetByIdAsync(id, cancellationToken);
        if (question is null)
        {
            return Result<QuizQuestionDto>.NotFound(nameof(id), "Quiz question was not found.");
        }

        apply(question, timeProvider.GetUtcNow());
        await questionRepository.SaveChangesAsync(cancellationToken);

        return Result<QuizQuestionDto>.Success(QuizQuestionReadModelBuilder.Map(question));
    }
}

public sealed class ActivateQuizQuestionCommandHandler : QuizQuestionLifecycleCommandHandler<ActivateQuizQuestionCommand>
{
    public ActivateQuizQuestionCommandHandler(IQuizQuestionRepository questionRepository, TimeProvider timeProvider)
        : base(questionRepository, timeProvider)
    {
    }

    public Task<Result<QuizQuestionDto>> HandleAsync(ActivateQuizQuestionCommand command, CancellationToken cancellationToken)
    {
        return HandleLifecycleAsync(command.Id, static (question, now) => question.Activate(now), cancellationToken);
    }
}

public sealed class DeactivateQuizQuestionCommandHandler : QuizQuestionLifecycleCommandHandler<DeactivateQuizQuestionCommand>
{
    public DeactivateQuizQuestionCommandHandler(IQuizQuestionRepository questionRepository, TimeProvider timeProvider)
        : base(questionRepository, timeProvider)
    {
    }

    public Task<Result<QuizQuestionDto>> HandleAsync(DeactivateQuizQuestionCommand command, CancellationToken cancellationToken)
    {
        return HandleLifecycleAsync(command.Id, static (question, now) => question.Deactivate(now), cancellationToken);
    }
}
