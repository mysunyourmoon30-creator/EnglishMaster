using EnglishMaster.Application.Features.QuizChoices.Dtos;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizChoices.Queries;

public sealed class GetQuizChoiceByIdQueryHandler
{
    private readonly IQuizChoiceRepository choiceRepository;

    public GetQuizChoiceByIdQueryHandler(IQuizChoiceRepository choiceRepository)
    {
        this.choiceRepository = choiceRepository;
    }

    public async Task<Result<QuizChoiceDto>> HandleAsync(
        GetQuizChoiceByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
        {
            return Result<QuizChoiceDto>.Validation(
                new ValidationError(nameof(query.Id), $"{nameof(query.Id)} cannot be empty."));
        }

        var choice = await choiceRepository.GetByIdAsync(query.Id, cancellationToken);
        return choice is null
            ? Result<QuizChoiceDto>.NotFound(nameof(query.Id), "Quiz choice was not found.")
            : Result<QuizChoiceDto>.Success(QuizChoiceMapper.ToDto(choice));
    }
}
