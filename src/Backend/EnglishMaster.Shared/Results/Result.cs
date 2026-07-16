namespace EnglishMaster.Shared.Results;

public class Result
{
    private static readonly ValidationError[] NoErrors = [];

    protected Result(ResultStatus status, IReadOnlyCollection<ValidationError>? errors = null)
    {
        Status = status;
        Errors = errors ?? NoErrors;
    }

    public ResultStatus Status { get; }

    public IReadOnlyCollection<ValidationError> Errors { get; }

    public bool IsSuccess => Status == ResultStatus.Success;

    public static Result Success() => new(ResultStatus.Success);

    public static Result NotFound(string field, string message) =>
        new(ResultStatus.NotFound, [new ValidationError(field, message)]);

    public static Result Validation(params ValidationError[] errors) =>
        new(ResultStatus.ValidationError, errors);
}
