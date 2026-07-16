namespace EnglishMaster.Contracts.Practice;

public sealed record PracticeItemDto(Guid Id, string ContentType, Guid ContentId, string PracticeType, string Status, DateTimeOffset DueAt, DateTimeOffset? LastPracticedAt, DateTimeOffset NextReviewAt, int ReviewCount, int CorrectCount, int IncorrectCount, int CurrentIntervalDays);
public sealed record PracticeSessionItemDto(Guid Id, Guid PracticeSessionId, Guid PracticeItemId, string ContentType, Guid ContentId, string PracticeType, string PromptText, string AnswerText, string UserAnswer, string? Result, bool? IsCorrect, DateTimeOffset? PracticedAt);
public sealed record PracticeSessionDto(Guid Id, DateTimeOffset StartedAt, DateTimeOffset? CompletedAt, string Status, int TotalItems, int CompletedItems, int CorrectItems, int IncorrectItems, IReadOnlyCollection<PracticeSessionItemDto> Items);
public sealed record PracticeSummaryDto(int DueTodayCount, int NewCount, int ReviewingCount, int MasteredCount);
public sealed record GeneratePracticeItemsResponse(int CreatedCount);
public sealed record SubmitPracticeSessionItemRequest(string? UserAnswer, string Result);

