using FluentAssertions;
using SwiggyClone.Domain.Common;

namespace SwiggyClone.UnitTests.Domain;

public sealed class AuditableEntityTests
{
    private sealed class TestAuditableEntity : AuditableEntity;

    [Fact]
    public void AuditableEntity_HasCreatedByProperty()
    {
        var entity = new TestAuditableEntity();
        var userId = Guid.NewGuid();

        entity.CreatedBy = userId;

        entity.CreatedBy.Should().Be(userId);
    }

    [Fact]
    public void AuditableEntity_HasUpdatedByProperty()
    {
        var entity = new TestAuditableEntity();
        var userId = Guid.NewGuid();

        entity.UpdatedBy = userId;

        entity.UpdatedBy.Should().Be(userId);
    }
}
