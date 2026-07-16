using EnglishMaster.Application.Features.ImportJobs.Dtos;
using EnglishMaster.Domain.ImportJobs;
using EnglishMaster.Shared.Results;
using System.Text.Json;

namespace EnglishMaster.Application.Features.ImportJobs.Commands;

public sealed record UploadImportFileCommand(string ImportType, string Format, string OriginalFileName, string Content, string? RequestedBy);
public sealed record ValidateImportJobCommand(Guid Id);
public sealed record ConfirmImportJobCommand(Guid Id);
public sealed record RunImportJobCommand(Guid Id);
public sealed record CancelImportJobCommand(Guid Id);
public sealed record RollbackImportJobCommand(Guid Id);

public sealed class ImportJobCommandHandler
{
    private const int MaxImportContentBytes = 1_048_576;
    private static readonly HashSet<string> SupportedImportTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "words",
        "categories",
        "tags",
        "grammartopics",
        "grammarrules",
        "lessons",
        "courses",
        "books",
        "quizzes"
    };

    private readonly IImportJobRepository repository;
    private readonly IImportParser parser;
    private readonly IImportValidationService validationService;
    private readonly IImportPreviewService previewService;
    private readonly IImportRunService runService;
    private readonly IImportRollbackService rollbackService;
    private readonly TimeProvider timeProvider;

    public ImportJobCommandHandler(IImportJobRepository repository, IImportParser parser, IImportValidationService validationService, IImportPreviewService previewService, IImportRunService runService, IImportRollbackService rollbackService, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.parser = parser;
        this.validationService = validationService;
        this.previewService = previewService;
        this.runService = runService;
        this.rollbackService = rollbackService;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<ImportJobDto>> UploadAsync(UploadImportFileCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Content))
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(command.Content), "Import content is required."));
        }

        if (!IsSupportedFormat(command.Format))
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(command.Format), "Format must be CSV or JSON."));
        }

        var importType = Normalize(command.ImportType);
        if (!SupportedImportTypes.Contains(importType))
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(command.ImportType), "Import type is not supported."));
        }

        if (string.IsNullOrWhiteSpace(command.OriginalFileName))
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(command.OriginalFileName), "Original file name is required."));
        }

        var trimmedOriginalFileName = command.OriginalFileName.Trim();
        var normalizedOriginalFileName = Path.GetFileName(trimmedOriginalFileName);
        if (trimmedOriginalFileName.Contains('/') ||
            trimmedOriginalFileName.Contains('\\') ||
            string.IsNullOrWhiteSpace(normalizedOriginalFileName) ||
            normalizedOriginalFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(command.OriginalFileName), "Original file name must be a file name, not a path."));
        }

        if (System.Text.Encoding.UTF8.GetByteCount(command.Content) > MaxImportContentBytes)
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(command.Content), "Import content must be 1 MB or smaller."));
        }

        if (!HasExpectedExtension(normalizedOriginalFileName, command.Format))
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(command.OriginalFileName), "Original file extension must match the selected format."));
        }

        try
        {
            var now = timeProvider.GetUtcNow();
            var safeFileName = $"{Guid.NewGuid():N}-{normalizedOriginalFileName}";
            var job = ImportJob.Create(importType, command.Format.Trim().ToUpperInvariant(), safeFileName, normalizedOriginalFileName, System.Text.Encoding.UTF8.GetByteCount(command.Content), command.RequestedBy, now);
            var rows = parser.ParseRows(command.Format, command.Content);
            var rowNumber = 1;
            foreach (var row in rows)
            {
                job.AddRow(rowNumber++, row, now);
            }

            return Result<ImportJobDto>.Success(await repository.AddAsync(job, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<ImportJobDto>.Validation(new ValidationError(exception.ParamName ?? "import", exception.Message));
        }
        catch (JsonException)
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(command.Content), "Import content is not valid JSON."));
        }
    }

    public async Task<Result<ImportJobDto>> ValidateAsync(ValidateImportJobCommand command, CancellationToken cancellationToken) =>
        await MutateAsync(command.Id, async job => await validationService.ValidateAsync(job, cancellationToken), cancellationToken);

    public async Task<Result<ImportJobDto>> ConfirmAsync(ConfirmImportJobCommand command, CancellationToken cancellationToken) =>
        await MutateAsync(command.Id, async job => await previewService.ConfirmAsync(job, cancellationToken), cancellationToken);

    public async Task<Result<ImportJobDto>> RunAsync(RunImportJobCommand command, CancellationToken cancellationToken) =>
        await MutateAsync(command.Id, async job => await runService.RunAsync(job, cancellationToken), cancellationToken);

    public async Task<Result<ImportJobDto>> CancelAsync(CancelImportJobCommand command, CancellationToken cancellationToken) =>
        await MutateAsync(command.Id, job =>
        {
            job.Cancel(timeProvider.GetUtcNow());
            return Task.CompletedTask;
        }, cancellationToken);

    public async Task<Result<ImportJobDto>> RollbackAsync(RollbackImportJobCommand command, CancellationToken cancellationToken) =>
        await MutateAsync(command.Id, async job => await rollbackService.RollbackAsync(job, cancellationToken), cancellationToken);

    private async Task<Result<ImportJobDto>> MutateAsync(Guid id, Func<ImportJob, Task> action, CancellationToken cancellationToken)
    {
        var job = await repository.GetEntityAsync(id, cancellationToken);
        if (job is null)
        {
            return Result<ImportJobDto>.NotFound(nameof(id), "Import job was not found.");
        }

        try
        {
            await action(job);
            return Result<ImportJobDto>.Success(await repository.SaveAsync(job, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Result<ImportJobDto>.Validation(new ValidationError(nameof(id), exception.Message));
        }
    }

    private static bool IsSupportedFormat(string value) =>
        value.Equals("CSV", StringComparison.OrdinalIgnoreCase) || value.Equals("JSON", StringComparison.OrdinalIgnoreCase);

    private static bool HasExpectedExtension(string fileName, string format)
    {
        var extension = Path.GetExtension(Path.GetFileName(fileName));
        return format.Equals("CSV", StringComparison.OrdinalIgnoreCase)
            ? extension.Equals(".csv", StringComparison.OrdinalIgnoreCase)
            : extension.Equals(".json", StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string value) => value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();
}
