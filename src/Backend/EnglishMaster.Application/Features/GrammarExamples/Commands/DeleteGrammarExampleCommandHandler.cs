using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarExamples.Commands;

public sealed class DeleteGrammarExampleCommandHandler
{
    private readonly IGrammarExampleRepository grammarExampleRepository;
    private readonly TimeProvider timeProvider;

    public DeleteGrammarExampleCommandHandler(
        IGrammarExampleRepository grammarExampleRepository,
        TimeProvider timeProvider)
    {
        this.grammarExampleRepository = grammarExampleRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteGrammarExampleCommand command,
        CancellationToken cancellationToken)
    {
        var grammarExample = await grammarExampleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (grammarExample is null)
        {
            return Result.NotFound(nameof(command.Id), "Grammar example was not found.");
        }

        grammarExample.Deactivate(timeProvider.GetUtcNow());
        await grammarExampleRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
