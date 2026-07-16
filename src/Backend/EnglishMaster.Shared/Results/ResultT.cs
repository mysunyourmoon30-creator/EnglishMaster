namespace EnglishMaster.Shared.Results;

public sealed class Result<T> : Result
{
    private Result(T value)
        : base(ResultStatus.Success)
    {
        Value = value;
    }

    private Result(ResultStatus status, IReadOnlyCollection<ValidationError> errors)
        : base(status, errors)
    {
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> NotFound(string field, string message) =>
        new(ResultStatus.NotFound, [new ValidationError(field, message)]);

    public static new Result<T> Validation(params ValidationError[] errors) =>
        new(ResultStatus.ValidationError, errors);
}
