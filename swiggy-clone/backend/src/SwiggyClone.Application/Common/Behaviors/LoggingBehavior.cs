using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs request execution details including
/// request name, authenticated user information, and elapsed time.
/// A warning is emitted when a request exceeds the configured performance threshold.
/// </summary>
/// <typeparam name="TRequest">The type of the MediatR request.</typeparam>
/// <typeparam name="TResponse">The type of the MediatR response.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const long PerformanceThresholdMilliseconds = 500;

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService? _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService? currentUserService = null)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService?.UserId;
        var userRole = _currentUserService?.UserRole ?? "Anonymous";

        _logger.LogInformation(
            "Handling {RequestName} | UserId: {UserId} | Role: {UserRole}",
            requestName,
            userId,
            userRole);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > PerformanceThresholdMilliseconds)
            {
                _logger.LogWarning(
                    "Long-running request: {RequestName} ({ElapsedMilliseconds}ms) | UserId: {UserId} | Role: {UserRole}",
                    requestName,
                    elapsedMilliseconds,
                    userId,
                    userRole);
            }
            else
            {
                _logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds}ms",
                    requestName,
                    elapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Unhandled exception for {RequestName} after {ElapsedMilliseconds}ms | UserId: {UserId} | Role: {UserRole}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                userId,
                userRole);

            throw;
        }
    }
}
