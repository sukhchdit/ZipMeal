using System.Diagnostics;
using MediatR;
using SwiggyClone.Application.Common.Diagnostics;

namespace SwiggyClone.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that records RED (Rate, Errors, Duration) metrics
/// for every command and query passing through the pipeline.
/// </summary>
public sealed class MetricsBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();
        var status = "success";

        try
        {
            var response = await next();
            return response;
        }
        catch
        {
            status = "error";
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var duration = stopwatch.Elapsed.TotalSeconds;

            var tags = new TagList
            {
                { "mediatr.request_name", requestName },
                { "mediatr.status", status },
            };

            ApplicationDiagnostics.MediatRRequests.Add(1, tags);
            ApplicationDiagnostics.MediatRDuration.Record(duration, tags);
        }
    }
}
