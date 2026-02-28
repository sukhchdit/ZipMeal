using FluentAssertions;
using SwiggyClone.Domain.Exceptions;

namespace SwiggyClone.UnitTests.Domain.Exceptions;

public sealed class DomainExceptionTests
{
    [Fact]
    public void Constructor_SetsCodeAndMessage()
    {
        var ex = new DomainException("TEST_CODE", "Something went wrong");

        ex.Code.Should().Be("TEST_CODE");
        ex.Message.Should().Be("Something went wrong");
    }

    [Fact]
    public void Constructor_WithInnerException_SetsAll()
    {
        var inner = new InvalidOperationException("inner");

        var ex = new DomainException("INNER_CODE", "Outer message", inner);

        ex.Code.Should().Be("INNER_CODE");
        ex.Message.Should().Be("Outer message");
        ex.InnerException.Should().BeSameAs(inner);
    }
}
