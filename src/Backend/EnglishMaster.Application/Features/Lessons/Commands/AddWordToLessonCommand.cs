namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed record AddWordToLessonCommand(Guid LessonId, Guid WordId, int SortOrder);
