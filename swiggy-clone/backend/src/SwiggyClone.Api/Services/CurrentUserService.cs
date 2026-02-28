using System.Security.Claims;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Api.Services;

/// <summary>
/// Resolves the current authenticated user from <see cref="HttpContext.User"/> claims.
/// Registered as a scoped service so it is tied to the lifetime of a single HTTP request.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public Guid? UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    /// <inheritdoc />
    public string? UserRole =>
        _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.Role);

    /// <inheritdoc />
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
