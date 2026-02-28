using FluentAssertions;
using SwiggyClone.Domain.Exceptions;

namespace SwiggyClone.UnitTests.Domain.Exceptions;

public sealed class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_FormatsEntityAndGuidKey()
    {
        var key = Guid.NewGuid();

        var ex = new NotFoundException("Order", key);

        ex.Code.Should().Be("NOT_FOUND");
        ex.Message.Should().Contain("Order");
        ex.Message.Should().Contain(key.ToString());
    }

    [Fact]
    public void Constructor_FormatsEntityAndStringKey()
    {
        var ex = new NotFoundException("User", "test@example.com");

        ex.Code.Should().Be("NOT_FOUND");
        ex.Message.Should().Be("User with key 'test@example.com' was not found.");
    }
}
