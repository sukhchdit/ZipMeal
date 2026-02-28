namespace SwiggyClone.Domain.Exceptions;

/// <summary>
/// Thrown when an operation conflicts with existing state (duplicate, stale data, etc.).
/// Maps to HTTP 409 at the API layer.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message)
        : base("CONFLICT", message)
    {
    }
}
