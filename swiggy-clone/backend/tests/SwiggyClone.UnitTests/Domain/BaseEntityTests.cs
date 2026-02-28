using SwiggyClone.Domain.Common;
using Xunit;

namespace SwiggyClone.UnitTests.Domain;

public class BaseEntityTests
{
    private sealed class TestEntity : BaseEntity;

    private sealed class TestDomainEvent : DomainEvent;

    [Fact]
    public void SoftDelete_SetsIsDeletedAndDeletedAt()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        entity.SoftDelete();

        // Assert
        Assert.True(entity.IsDeleted);
        Assert.NotNull(entity.DeletedAt);
    }

    [Fact]
    public void RaiseDomainEvent_AddsEventToCollection()
    {
        // Arrange
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();

        // Act
        entity.RaiseDomainEvent(domainEvent);

        // Assert
        Assert.Single(entity.DomainEvents);
        Assert.Contains(domainEvent, entity.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        // Arrange
        var entity = new TestEntity();
        entity.RaiseDomainEvent(new TestDomainEvent());
        entity.RaiseDomainEvent(new TestDomainEvent());

        // Act
        entity.ClearDomainEvents();

        // Assert
        Assert.Empty(entity.DomainEvents);
    }
}
