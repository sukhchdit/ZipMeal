using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SwiggyClone.Domain.Common;

namespace SwiggyClone.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that converts physical deletes into soft deletes.
/// When an entity derived from <see cref="BaseEntity"/> is marked as <see cref="EntityState.Deleted"/>,
/// this interceptor sets <c>IsDeleted = true</c> and <c>DeletedAt = UtcNow</c>, then changes
/// the entry state to <see cref="EntityState.Modified"/> so the row is updated rather than removed.
/// </summary>
public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplySoftDelete(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            // Prevent physical deletion — mark as soft-deleted instead.
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}
