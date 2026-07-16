using System.Net;

namespace EnglishMaster.Web.Services;

public sealed class ApiRequestException : Exception
{
    public ApiRequestException(
        string message,
        HttpStatusCode statusCode,
        IReadOnlyDictionary<string, string[]> validationErrors)
        : base(message)
    {
        StatusCode = statusCode;
        ValidationErrors = validationErrors;
    }

    public HttpStatusCode StatusCode { get; }

    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }
}
