using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Commands;

public sealed class DeleteQuizQuestionCommandHandler
{
    private readonly IQuizQuestionRepository questionRepository;
    private readonly TimeProvider timeProvider;

    public DeleteQuizQuestionCommandHandler(
        IQuizQuestionRepository questionRepository,
        TimeProvider timeProvider)
    {
        this.questionRepository = questionRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(DeleteQuizQuestionCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.Id), $"{nameof(command.Id)} cannot be empty."));
        }

        var question = await questionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (question is null)
        {
            return Result.NotFound(nameof(command.Id), "Quiz question was not found.");
        }

        question.Deactivate(timeProvider.GetUtcNow());
        await questionRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
