namespace SwiggyClone.Domain.Common;

/// <summary>
/// Extends BaseEntity with audit trail fields (who created/updated).
/// Use for entities that require user-level change tracking.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public Guid? CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }
}
