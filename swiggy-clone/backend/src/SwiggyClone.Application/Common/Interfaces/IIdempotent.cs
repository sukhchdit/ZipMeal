namespace SwiggyClone.Application.Common.Interfaces;

/// <summary>
/// Marker interface for MediatR requests that support idempotency.
/// Implement this on mutation commands to prevent duplicate processing
/// through <see cref="Behaviors.IdempotencyBehavior{TRequest, TResponse}"/>.
/// Requests that do not implement this interface pass through unaffected.
/// </summary>
public interface IIdempotent
{
    /// <summary>
    /// A caller-provided idempotency key. When null or empty, the request
    /// is processed normally without idempotency checks.
    /// </summary>
    string? IdempotencyKey { get; }
}
