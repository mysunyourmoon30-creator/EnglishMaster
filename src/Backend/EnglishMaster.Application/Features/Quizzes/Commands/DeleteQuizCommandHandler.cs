using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Quizzes.Commands;

public sealed class DeleteQuizCommandHandler
{
    private readonly IQuizRepository quizRepository;
    private readonly TimeProvider timeProvider;

    public DeleteQuizCommandHandler(IQuizRepository quizRepository, TimeProvider timeProvider)
    {
        this.quizRepository = quizRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(DeleteQuizCommand command, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(command.Id, cancellationToken);
        if (quiz is null)
        {
            return Result.NotFound(nameof(command.Id), "Quiz was not found.");
        }

        quiz.Deactivate(timeProvider.GetUtcNow());
        await quizRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
