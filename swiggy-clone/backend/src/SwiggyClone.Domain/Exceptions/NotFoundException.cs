namespace SwiggyClone.Domain.Exceptions;

/// <summary>
/// Thrown when a requested entity does not exist.
/// Maps to HTTP 404 at the API layer.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base("NOT_FOUND", $"{entityName} with key '{key}' was not found.")
    {
    }
}
