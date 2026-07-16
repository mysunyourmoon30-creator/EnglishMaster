using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed class CreateLessonCommandHandler
{
    private readonly ILessonRepository lessonRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly IWordRepository wordRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly TimeProvider timeProvider;

    public CreateLessonCommandHandler(
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
        CreateLessonCommand command,
        CancellationToken cancellationToken)
    {
        var validation = LessonInputValidator.Validate(
            command.Title,
            command.Summary,
            command.Description,
            command.CefrLevel,
            command.CategoryId,
            command.ThumbnailMediaId,
            command.EstimatedMinutes,
            command.SortOrder,
            isPublished: false,
            isActive: true);

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

        if (await lessonRepository.SlugExistsAsync(input.Slug, null, cancellationToken))
        {
            return Result<LessonDto>.Validation(
                new ValidationError(nameof(command.Title), "A lesson with this title already exists."));
        }

        var lesson = Lesson.Create(
            input.Title,
            input.Summary,
            input.Description,
            input.CefrLevel,
            input.CategoryId,
            input.ThumbnailMediaId,
            input.EstimatedMinutes,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await lessonRepository.AddAsync(lesson, cancellationToken);
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
