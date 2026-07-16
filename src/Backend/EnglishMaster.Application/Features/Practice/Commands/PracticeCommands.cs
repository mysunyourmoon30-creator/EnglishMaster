using EnglishMaster.Application.Features.Practice.Dtos;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Practice.Commands;

public sealed record CreatePracticeItemCommand(Guid UserId, string ContentType, Guid ContentId, string PracticeType);
public sealed record GeneratePracticeItemsCommand(Guid UserId);
public sealed record StartPracticeSessionCommand(Guid UserId, int? Limit);
public sealed record SubmitPracticeSessionItemCommand(Guid UserId, Guid SessionItemId, string? UserAnswer, string Result);
public sealed record CompletePracticeSessionCommand(Guid UserId, Guid SessionId);
public sealed record PracticeItemLifecycleCommand(Guid UserId, Guid PracticeItemId);

public sealed class PracticeCommandHandler
{
    private readonly IPracticeRepository repository;

    public PracticeCommandHandler(IPracticeRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<PracticeItemDto>> CreateAsync(CreatePracticeItemCommand command, CancellationToken cancellationToken) =>
        Result<PracticeItemDto>.Success(await repository.CreatePracticeItemAsync(command.UserId, command.ContentType, command.ContentId, command.PracticeType, cancellationToken));

    public async Task<Result<int>> GenerateAsync(GeneratePracticeItemsCommand command, CancellationToken cancellationToken) =>
        Result<int>.Success(await repository.GeneratePracticeItemsAsync(command.UserId, cancellationToken));

    public async Task<Result<PracticeSessionDto>> StartSessionAsync(StartPracticeSessionCommand command, CancellationToken cancellationToken) =>
        Result<PracticeSessionDto>.Success(await repository.StartPracticeSessionAsync(command.UserId, Math.Clamp(command.Limit ?? 10, 1, 20), cancellationToken));

    public async Task<Result<PracticeSessionItemDto>> SubmitAsync(SubmitPracticeSessionItemCommand command, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PracticeResult>(command.Result, ignoreCase: true, out _))
        {
            return Result<PracticeSessionItemDto>.Validation(new ValidationError(nameof(command.Result), "Practice result must be Again, Hard, Good, or Easy."));
        }

        var item = await repository.SubmitPracticeSessionItemAsync(command.UserId, command.SessionItemId, command.UserAnswer, command.Result, cancellationToken);
        return item is null ? Result<PracticeSessionItemDto>.NotFound(nameof(command.SessionItemId), "Practice session item was not found.") : Result<PracticeSessionItemDto>.Success(item);
    }

    public async Task<Result<PracticeSessionDto>> CompleteAsync(CompletePracticeSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await repository.CompletePracticeSessionAsync(command.UserId, command.SessionId, cancellationToken);
        return session is null ? Result<PracticeSessionDto>.NotFound(nameof(command.SessionId), "Practice session was not found.") : Result<PracticeSessionDto>.Success(session);
    }

    public async Task<Result<PracticeItemDto>> SuspendAsync(PracticeItemLifecycleCommand command, CancellationToken cancellationToken)
    {
        var item = await repository.SuspendPracticeItemAsync(command.UserId, command.PracticeItemId, cancellationToken);
        return item is null ? Result<PracticeItemDto>.NotFound(nameof(command.PracticeItemId), "Practice item was not found.") : Result<PracticeItemDto>.Success(item);
    }

    public async Task<Result<PracticeItemDto>> ResumeAsync(PracticeItemLifecycleCommand command, CancellationToken cancellationToken)
    {
        var item = await repository.ResumePracticeItemAsync(command.UserId, command.PracticeItemId, cancellationToken);
        return item is null ? Result<PracticeItemDto>.NotFound(nameof(command.PracticeItemId), "Practice item was not found.") : Result<PracticeItemDto>.Success(item);
    }
}
