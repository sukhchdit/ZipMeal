namespace SwiggyClone.Domain.Common;

/// <summary>
/// Unit of Work interface for coordinating transactional persistence.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
