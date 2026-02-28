namespace SwiggyClone.Domain.Exceptions;

/// <summary>
/// Thrown when the user is authenticated but not authorized for the operation.
/// Maps to HTTP 403 at the API layer.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base("FORBIDDEN", message)
    {
    }
}
