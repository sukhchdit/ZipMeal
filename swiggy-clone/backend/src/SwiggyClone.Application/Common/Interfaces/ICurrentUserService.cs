namespace SwiggyClone.Application.Common.Interfaces;

/// <summary>
/// Provides access to information about the currently authenticated user.
/// Implementations are typically scoped to the current HTTP request.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// The unique identifier of the authenticated user, or <c>null</c> if the request is anonymous.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// The role assigned to the authenticated user, or <c>null</c> if unavailable.
    /// </summary>
    string? UserRole { get; }

    /// <summary>
    /// Indicates whether the current request is made by an authenticated user.
    /// </summary>
    bool IsAuthenticated { get; }
}
