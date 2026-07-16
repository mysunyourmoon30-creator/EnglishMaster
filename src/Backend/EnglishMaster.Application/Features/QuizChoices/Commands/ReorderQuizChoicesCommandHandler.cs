using EnglishMaster.Application.Features.QuizChoices.Dtos;
using EnglishMaster.Application.Features.QuizQuestions;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizChoices.Commands;

public sealed class ReorderQuizChoicesCommandHandler
{
    private readonly IQuizChoiceRepository choiceRepository;
    private readonly IQuizQuestionRepository questionRepository;
    private readonly TimeProvider timeProvider;

    public ReorderQuizChoicesCommandHandler(
        IQuizChoiceRepository choiceRepository,
        IQuizQuestionRepository questionRepository,
        TimeProvider timeProvider)
    {
        this.choiceRepository = choiceRepository;
        this.questionRepository = questionRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyCollection<QuizChoiceDto>>> HandleAsync(
        ReorderQuizChoicesCommand command,
        CancellationToken cancellationToken)
    {
        if (command.QuizQuestionId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<QuizChoiceDto>>.Validation(
                new ValidationError(nameof(command.QuizQuestionId), $"{nameof(command.QuizQuestionId)} cannot be empty."));
        }

        if (command.OrderedChoiceIds is null)
        {
            return Result<IReadOnlyCollection<QuizChoiceDto>>.Validation(
                new ValidationError(nameof(command.OrderedChoiceIds), $"{nameof(command.OrderedChoiceIds)} is required."));
        }

        if (command.OrderedChoiceIds.Any(id => id == Guid.Empty))
        {
            return Result<IReadOnlyCollection<QuizChoiceDto>>.Validation(
                new ValidationError(nameof(command.OrderedChoiceIds), $"{nameof(command.OrderedChoiceIds)} cannot contain empty choice ids."));
        }

        if (command.OrderedChoiceIds.Count != command.OrderedChoiceIds.Distinct().Count())
        {
            return Result<IReadOnlyCollection<QuizChoiceDto>>.Validation(
                new ValidationError(nameof(command.OrderedChoiceIds), $"{nameof(command.OrderedChoiceIds)} cannot contain duplicate choice ids."));
        }

        var question = await questionRepository.GetByIdAsync(command.QuizQuestionId, cancellationToken);
        if (question is null)
        {
            return Result<IReadOnlyCollection<QuizChoiceDto>>.NotFound(nameof(command.QuizQuestionId), "Quiz question was not found.");
        }

        var choices = await choiceRepository.GetByQuestionIdAsync(command.QuizQuestionId, cancellationToken);
        var existingIds = choices.Select(choice => choice.Id).ToHashSet();
        var providedIds = command.OrderedChoiceIds.ToHashSet();

        if (choices.Count != command.OrderedChoiceIds.Count || !existingIds.SetEquals(providedIds))
        {
            return Result<IReadOnlyCollection<QuizChoiceDto>>.Validation(
                new ValidationError(
                    nameof(command.OrderedChoiceIds),
                    "orderedChoiceIds must contain exactly the current choices of this quiz question."));
        }

        var choiceById = choices.ToDictionary(choice => choice.Id);
        var now = timeProvider.GetUtcNow();
        for (var index = 0; index < command.OrderedChoiceIds.Count; index++)
        {
            choiceById[command.OrderedChoiceIds[index]].Reorder(index, now);
        }

        await choiceRepository.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyCollection<QuizChoiceDto>>.Success(
            QuizChoiceMapper.ToDtos(command.OrderedChoiceIds.Select(id => choiceById[id]).ToArray()));
    }
}
