using EnglishMaster.Application.Features.LessonSections.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LessonSections.Commands;

public sealed class ReorderLessonSectionsCommandHandler
{
    private readonly ILessonSectionRepository lessonSectionRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public ReorderLessonSectionsCommandHandler(
        ILessonSectionRepository lessonSectionRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.lessonSectionRepository = lessonSectionRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyCollection<LessonSectionDto>>> HandleAsync(
        ReorderLessonSectionsCommand command,
        CancellationToken cancellationToken)
    {
        if (command.LessonId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<LessonSectionDto>>.Validation(
                new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        if (command.OrderedSectionIds is null)
        {
            return Result<IReadOnlyCollection<LessonSectionDto>>.Validation(
                new ValidationError(nameof(command.OrderedSectionIds), $"{nameof(command.OrderedSectionIds)} is required."));
        }

        if (command.OrderedSectionIds.Any(sectionId => sectionId == Guid.Empty))
        {
            return Result<IReadOnlyCollection<LessonSectionDto>>.Validation(
                new ValidationError(nameof(command.OrderedSectionIds), $"{nameof(command.OrderedSectionIds)} cannot contain empty section ids."));
        }

        if (command.OrderedSectionIds.Count != command.OrderedSectionIds.Distinct().Count())
        {
            return Result<IReadOnlyCollection<LessonSectionDto>>.Validation(
                new ValidationError(nameof(command.OrderedSectionIds), $"{nameof(command.OrderedSectionIds)} cannot contain duplicate section ids."));
        }

        var lesson = await lessonRepository.GetByIdAsync(command.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Result<IReadOnlyCollection<LessonSectionDto>>.NotFound(nameof(command.LessonId), "Lesson was not found.");
        }

        var sections = await lessonSectionRepository.GetByLessonIdAsync(command.LessonId, cancellationToken);
        var existingIds = sections.Select(section => section.Id).ToHashSet();
        var providedIds = command.OrderedSectionIds.ToHashSet();

        if (sections.Count != command.OrderedSectionIds.Count || !existingIds.SetEquals(providedIds))
        {
            return Result<IReadOnlyCollection<LessonSectionDto>>.Validation(
                new ValidationError(
                    nameof(command.OrderedSectionIds),
                    "orderedSectionIds must contain exactly the current sections of this lesson."));
        }

        var sectionById = sections.ToDictionary(section => section.Id);
        var now = timeProvider.GetUtcNow();
        for (var index = 0; index < command.OrderedSectionIds.Count; index++)
        {
            sectionById[command.OrderedSectionIds[index]].Reorder(index, now);
        }

        await lessonSectionRepository.SaveChangesAsync(cancellationToken);

        var ordered = command.OrderedSectionIds
            .Select(id => LessonSectionMapper.ToDto(sectionById[id]))
            .ToArray();

        return Result<IReadOnlyCollection<LessonSectionDto>>.Success(ordered);
    }
}
