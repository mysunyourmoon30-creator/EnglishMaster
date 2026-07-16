using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed class RemoveGrammarRuleFromLessonCommandHandler
{
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public RemoveGrammarRuleFromLessonCommandHandler(
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        RemoveGrammarRuleFromLessonCommand command,
        CancellationToken cancellationToken)
    {
        if (command.LessonId == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        if (command.GrammarRuleId == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.GrammarRuleId), $"{nameof(command.GrammarRuleId)} cannot be empty."));
        }

        var lesson = await lessonRepository.GetByIdAsync(command.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Result.NotFound(nameof(command.LessonId), "Lesson was not found.");
        }

        if (!lesson.RemoveGrammarRule(command.GrammarRuleId, timeProvider.GetUtcNow()))
        {
            return Result.NotFound(nameof(command.GrammarRuleId), "Related grammar rule was not found on this lesson.");
        }

        await lessonRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
