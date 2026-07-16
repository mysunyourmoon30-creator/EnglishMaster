using EnglishMaster.Application.Features.GrammarTopics.Dtos;
using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarTopics.Queries;

public sealed class GetGrammarTopicByIdQueryHandler
{
    private readonly IGrammarTopicRepository grammarTopicRepository;

    public GetGrammarTopicByIdQueryHandler(IGrammarTopicRepository grammarTopicRepository)
    {
        this.grammarTopicRepository = grammarTopicRepository;
    }

    public async Task<Result<GrammarTopicDto>> HandleAsync(
        GetGrammarTopicByIdQuery query,
        CancellationToken cancellationToken)
    {
        var grammarTopic = await grammarTopicRepository.GetByIdAsync(query.Id, cancellationToken);
        return grammarTopic is null
            ? Result<GrammarTopicDto>.NotFound(nameof(query.Id), "Grammar topic was not found.")
            : Result<GrammarTopicDto>.Success(GrammarTopicMapper.ToDto(grammarTopic));
    }
}
