using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
// ErrorMessages marker class is in SwiggyClone.Api namespace (root)
using SwiggyClone.Shared;

namespace SwiggyClone.Api.Filters;

/// <summary>
/// Global action filter that intercepts Result-based responses and replaces
/// the <c>ErrorMessage</c> with a localized string from .resx resource files,
/// based on the current request culture (derived from <c>Accept-Language</c>).
/// </summary>
/// <remarks>
/// This is a non-invasive approach: existing handlers continue returning
/// <c>Result.Failure("CODE", "English message")</c> unchanged. The filter
/// operates at the API boundary, swapping the message before serialization.
/// </remarks>
public sealed class ResponseLocalizationFilter : IAsyncResultFilter
{
    private readonly IStringLocalizer<ErrorMessages> _localizer;

    public ResponseLocalizationFilter(IStringLocalizer<ErrorMessages> localizer)
        => _localizer = localizer;

    public async Task OnResultExecutionAsync(
        ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult
            && objectResult.Value is Result result
            && result.IsFailure
            && !string.IsNullOrEmpty(result.ErrorCode))
        {
            var localized = _localizer[result.ErrorCode];

            if (!localized.ResourceNotFound)
            {
                objectResult.Value = new LocalizedErrorResponse
                {
                    IsSuccess = false,
                    IsFailure = true,
                    ErrorCode = result.ErrorCode,
                    ErrorMessage = localized.Value,
                    ValidationErrors = result.ValidationErrors,
                };
            }
        }

        await next();
    }

    /// <summary>
    /// Lightweight DTO that mirrors the <see cref="Result"/> shape for JSON
    /// serialization with the localized error message.
    /// </summary>
    private sealed class LocalizedErrorResponse
    {
        public required bool IsSuccess { get; init; }
        public required bool IsFailure { get; init; }
        public string? ErrorCode { get; init; }
        public string? ErrorMessage { get; init; }
        public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; init; }
    }
}
