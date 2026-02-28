using FluentAssertions;
using SwiggyClone.Domain.Exceptions;

namespace SwiggyClone.UnitTests.Domain.Exceptions;

public sealed class ConflictExceptionTests
{
    [Fact]
    public void Constructor_SetsConflictCode()
    {
        var ex = new ConflictException("Duplicate entry");

        ex.Code.Should().Be("CONFLICT");
        ex.Message.Should().Be("Duplicate entry");
    }

    [Fact]
    public void Constructor_InheritsFromDomainException()
    {
        var ex = new ConflictException("test");

        ex.Should().BeAssignableTo<DomainException>();
    }
}
