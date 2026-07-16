namespace EnglishMaster.Shared.Results;

public sealed record ValidationError(string Field, string Message);
