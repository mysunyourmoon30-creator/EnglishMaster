using EnglishMaster.Application.Features.QuizQuestions.Dtos;
using EnglishMaster.Application.Features.Quizzes;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Commands;

public sealed class ReorderQuizQuestionsCommandHandler
{
    private readonly IQuizQuestionRepository questionRepository;
    private readonly IQuizRepository quizRepository;
    private readonly TimeProvider timeProvider;

    public ReorderQuizQuestionsCommandHandler(
        IQuizQuestionRepository questionRepository,
        IQuizRepository quizRepository,
        TimeProvider timeProvider)
    {
        this.questionRepository = questionRepository;
        this.quizRepository = quizRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyCollection<QuizQuestionDto>>> HandleAsync(
        ReorderQuizQuestionsCommand command,
        CancellationToken cancellationToken)
    {
        if (command.QuizId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<QuizQuestionDto>>.Validation(
                new ValidationError(nameof(command.QuizId), $"{nameof(command.QuizId)} cannot be empty."));
        }

        if (command.OrderedQuestionIds is null)
        {
            return Result<IReadOnlyCollection<QuizQuestionDto>>.Validation(
                new ValidationError(nameof(command.OrderedQuestionIds), $"{nameof(command.OrderedQuestionIds)} is required."));
        }

        if (command.OrderedQuestionIds.Any(id => id == Guid.Empty))
        {
            return Result<IReadOnlyCollection<QuizQuestionDto>>.Validation(
                new ValidationError(nameof(command.OrderedQuestionIds), $"{nameof(command.OrderedQuestionIds)} cannot contain empty question ids."));
        }

        if (command.OrderedQuestionIds.Count != command.OrderedQuestionIds.Distinct().Count())
        {
            return Result<IReadOnlyCollection<QuizQuestionDto>>.Validation(
                new ValidationError(nameof(command.OrderedQuestionIds), $"{nameof(command.OrderedQuestionIds)} cannot contain duplicate question ids."));
        }

        var quiz = await quizRepository.GetByIdAsync(command.QuizId, cancellationToken);
        if (quiz is null)
        {
            return Result<IReadOnlyCollection<QuizQuestionDto>>.NotFound(nameof(command.QuizId), "Quiz was not found.");
        }

        var questions = await questionRepository.GetByQuizIdAsync(command.QuizId, cancellationToken);
        var existingIds = questions.Select(question => question.Id).ToHashSet();
        var providedIds = command.OrderedQuestionIds.ToHashSet();

        if (questions.Count != command.OrderedQuestionIds.Count || !existingIds.SetEquals(providedIds))
        {
            return Result<IReadOnlyCollection<QuizQuestionDto>>.Validation(
                new ValidationError(
                    nameof(command.OrderedQuestionIds),
                    "orderedQuestionIds must contain exactly the current questions of this quiz."));
        }

        var questionById = questions.ToDictionary(question => question.Id);
        var now = timeProvider.GetUtcNow();
        for (var index = 0; index < command.OrderedQuestionIds.Count; index++)
        {
            questionById[command.OrderedQuestionIds[index]].Reorder(index, now);
        }

        await questionRepository.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyCollection<QuizQuestionDto>>.Success(
            QuizQuestionReadModelBuilder.Map(command.OrderedQuestionIds.Select(id => questionById[id]).ToArray()));
    }
}
