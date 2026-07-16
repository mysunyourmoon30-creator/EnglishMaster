namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed record RemoveWordFromLessonCommand(Guid LessonId, Guid WordId);
