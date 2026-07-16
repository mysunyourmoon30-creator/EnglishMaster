using EnglishMaster.Application.Features.GrammarTopics.Dtos;
using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarTopics.Commands;

public sealed class CreateGrammarTopicCommandHandler
{
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly TimeProvider timeProvider;

    public CreateGrammarTopicCommandHandler(
        IGrammarTopicRepository grammarTopicRepository,
        TimeProvider timeProvider)
    {
        this.grammarTopicRepository = grammarTopicRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<GrammarTopicDto>> HandleAsync(
        CreateGrammarTopicCommand command,
        CancellationToken cancellationToken)
    {
        var validation = GrammarTopicInputValidator.Validate(
            command.Title,
            command.Summary,
            command.CefrLevel,
            command.SortOrder,
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<GrammarTopicDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await grammarTopicRepository.SlugExistsAsync(input.Slug, null, cancellationToken))
        {
            return Result<GrammarTopicDto>.Validation(
                new ValidationError(nameof(command.Title), "A grammar topic with this title already exists."));
        }

        var grammarTopic = GrammarTopic.Create(
            input.Title,
            input.Summary,
            input.CefrLevel,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await grammarTopicRepository.AddAsync(grammarTopic, cancellationToken);
        await grammarTopicRepository.SaveChangesAsync(cancellationToken);

        return Result<GrammarTopicDto>.Success(GrammarTopicMapper.ToDto(grammarTopic));
    }
}
