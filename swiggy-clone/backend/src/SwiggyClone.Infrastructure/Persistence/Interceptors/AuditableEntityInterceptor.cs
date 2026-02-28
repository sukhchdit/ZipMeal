using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Common;

namespace SwiggyClone.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that automatically stamps audit fields on entities during save.
/// <list type="bullet">
///   <item><description>Added entities: sets <c>CreatedAt</c>, <c>UpdatedAt</c>, and optionally <c>CreatedBy</c> / <c>UpdatedBy</c>.</description></item>
///   <item><description>Modified entities: sets <c>UpdatedAt</c> and optionally <c>UpdatedBy</c>.</description></item>
/// </list>
/// </summary>
public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntityInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditFields(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTimeOffset.UtcNow;
        var currentUserId = _currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;

                    if (entry.Entity is AuditableEntity addedAuditable)
                    {
                        addedAuditable.CreatedBy = currentUserId;
                        addedAuditable.UpdatedBy = currentUserId;
                    }

                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;

                    if (entry.Entity is AuditableEntity modifiedAuditable)
                    {
                        modifiedAuditable.UpdatedBy = currentUserId;
                    }

                    break;
            }
        }
    }
}
