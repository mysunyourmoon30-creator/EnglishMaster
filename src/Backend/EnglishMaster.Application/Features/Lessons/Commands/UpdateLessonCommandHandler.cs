using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed class UpdateLessonCommandHandler
{
    private readonly ILessonRepository lessonRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly IWordRepository wordRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly TimeProvider timeProvider;

    public UpdateLessonCommandHandler(
        ILessonRepository lessonRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository,
        TimeProvider timeProvider)
    {
        this.lessonRepository = lessonRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.wordRepository = wordRepository;
        this.grammarRuleRepository = grammarRuleRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<LessonDto>> HandleAsync(
        UpdateLessonCommand command,
        CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(command.Id, cancellationToken);
        if (lesson is null)
        {
            return Result<LessonDto>.NotFound(nameof(command.Id), "Lesson was not found.");
        }

        var validation = LessonInputValidator.Validate(
            command.Title,
            command.Summary,
            command.Description,
            command.CefrLevel,
            command.CategoryId,
            command.ThumbnailMediaId,
            command.EstimatedMinutes,
            command.SortOrder,
            command.IsPublished,
            command.IsActive);

        if (!validation.IsSuccess)
        {
            return Result<LessonDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var referenceValidation = await LessonReferenceValidator.ValidateReferencesAsync(
            categoryRepository,
            mediaRepository,
            input,
            cancellationToken);
        if (referenceValidation.Errors.Count > 0)
        {
            return Result<LessonDto>.Validation([.. referenceValidation.Errors]);
        }

        if (await lessonRepository.SlugExistsAsync(input.Slug, command.Id, cancellationToken))
        {
            return Result<LessonDto>.Validation(
                new ValidationError(nameof(command.Title), "A lesson with this title already exists."));
        }

        lesson.Update(
            input.Title,
            input.Summary,
            input.Description,
            input.CefrLevel,
            input.CategoryId,
            input.ThumbnailMediaId,
            input.EstimatedMinutes,
            input.SortOrder,
            input.IsPublished,
            input.IsActive,
            timeProvider.GetUtcNow());

        await lessonRepository.SaveChangesAsync(cancellationToken);

        return Result<LessonDto>.Success(await LessonReadModelBuilder.MapAsync(
            lesson,
            categoryRepository,
            mediaRepository,
            wordRepository,
            grammarRuleRepository,
            cancellationToken));
    }
}
