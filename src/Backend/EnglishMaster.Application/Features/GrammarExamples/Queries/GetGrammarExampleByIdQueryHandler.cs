using EnglishMaster.Application.Features.GrammarExamples.Dtos;
using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarExamples.Queries;

public sealed class GetGrammarExampleByIdQueryHandler
{
    private readonly IGrammarExampleRepository grammarExampleRepository;

    public GetGrammarExampleByIdQueryHandler(IGrammarExampleRepository grammarExampleRepository)
    {
        this.grammarExampleRepository = grammarExampleRepository;
    }

    public async Task<Result<GrammarExampleDto>> HandleAsync(
        GetGrammarExampleByIdQuery query,
        CancellationToken cancellationToken)
    {
        var grammarExample = await grammarExampleRepository.GetByIdAsync(query.Id, cancellationToken);
        return grammarExample is null
            ? Result<GrammarExampleDto>.NotFound(nameof(query.Id), "Grammar example was not found.")
            : Result<GrammarExampleDto>.Success(GrammarExampleMapper.ToDto(grammarExample));
    }
}
