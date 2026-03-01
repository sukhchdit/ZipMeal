using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using SwiggyClone.Application.Common.Exceptions;
using SwiggyClone.Domain.Exceptions;
using SwiggyClone.Shared;

namespace SwiggyClone.Api.Middleware;

/// <summary>
/// Catches unhandled exceptions thrown during request processing and transforms
/// them into a consistent RFC 7807-style JSON error response.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    private readonly IStringLocalizer<ErrorMessages> _localizer;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment,
        IStringLocalizer<ErrorMessages> localizer)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, title, detail, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                _localizer[LocalizedErrorCodes.TitleValidationError].Value,
                validationEx.Message,
                (IReadOnlyDictionary<string, string[]>?)validationEx.Errors),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                _localizer[LocalizedErrorCodes.TitleNotFound].Value,
                notFoundEx.Message,
                (IReadOnlyDictionary<string, string[]>?)null),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                _localizer[LocalizedErrorCodes.TitleConflict].Value,
                conflictEx.Message,
                (IReadOnlyDictionary<string, string[]>?)null),

            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                _localizer[LocalizedErrorCodes.TitleForbidden].Value,
                forbiddenEx.Message,
                (IReadOnlyDictionary<string, string[]>?)null),

            DomainException domainEx => (
                HttpStatusCode.UnprocessableEntity,
                _localizer[LocalizedErrorCodes.TitleDomainError].Value,
                domainEx.Message,
                (IReadOnlyDictionary<string, string[]>?)null),

            _ => (
                HttpStatusCode.InternalServerError,
                _localizer[LocalizedErrorCodes.TitleInternalError].Value,
                _environment.IsDevelopment()
                    ? exception.ToString()
                    : _localizer[LocalizedErrorCodes.InternalError].Value,
                (IReadOnlyDictionary<string, string[]>?)null),
        };

        LogException(exception, statusCode, traceId);

        var response = new ErrorResponse
        {
            Type = GetRfcType(statusCode),
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            TraceId = traceId,
            Errors = errors,
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private void LogException(Exception exception, HttpStatusCode statusCode, string traceId)
    {
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception | TraceId: {TraceId} | Message: {Message}",
                traceId,
                exception.Message);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Handled domain/application exception ({StatusCode}) | TraceId: {TraceId} | Message: {Message}",
                (int)statusCode,
                traceId,
                exception.Message);
        }
    }

    private static string GetRfcType(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        HttpStatusCode.Forbidden => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        HttpStatusCode.Conflict => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        HttpStatusCode.UnprocessableEntity => "https://tools.ietf.org/html/rfc9110#section-15.5.21",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1",
    };

    /// <summary>
    /// Standard error response envelope matching the RFC 7807 Problem Details shape.
    /// </summary>
    private sealed class ErrorResponse
    {
        public required string Type { get; init; }
        public required string Title { get; init; }
        public required int Status { get; init; }
        public required string Detail { get; init; }
        public required string TraceId { get; init; }
        public IReadOnlyDictionary<string, string[]>? Errors { get; init; }
    }
}
