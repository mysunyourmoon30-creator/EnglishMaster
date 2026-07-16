using EnglishMaster.Application.Features.GrammarTopics.Dtos;
using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarTopics.Commands;

public sealed class UpdateGrammarTopicCommandHandler
{
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly TimeProvider timeProvider;

    public UpdateGrammarTopicCommandHandler(
        IGrammarTopicRepository grammarTopicRepository,
        TimeProvider timeProvider)
    {
        this.grammarTopicRepository = grammarTopicRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<GrammarTopicDto>> HandleAsync(
        UpdateGrammarTopicCommand command,
        CancellationToken cancellationToken)
    {
        var grammarTopic = await grammarTopicRepository.GetByIdAsync(command.Id, cancellationToken);
        if (grammarTopic is null)
        {
            return Result<GrammarTopicDto>.NotFound(nameof(command.Id), "Grammar topic was not found.");
        }

        var validation = GrammarTopicInputValidator.Validate(
            command.Title,
            command.Summary,
            command.CefrLevel,
            command.SortOrder,
            command.IsActive);

        if (!validation.IsSuccess)
        {
            return Result<GrammarTopicDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await grammarTopicRepository.SlugExistsAsync(input.Slug, command.Id, cancellationToken))
        {
            return Result<GrammarTopicDto>.Validation(
                new ValidationError(nameof(command.Title), "A grammar topic with this title already exists."));
        }

        grammarTopic.Update(
            input.Title,
            input.Summary,
            input.CefrLevel,
            input.SortOrder,
            input.IsActive,
            timeProvider.GetUtcNow());

        await grammarTopicRepository.SaveChangesAsync(cancellationToken);

        return Result<GrammarTopicDto>.Success(GrammarTopicMapper.ToDto(grammarTopic));
    }
}
