using FluentAssertions;
using SwiggyClone.Domain.Exceptions;

namespace SwiggyClone.UnitTests.Domain.Exceptions;

public sealed class ForbiddenExceptionTests
{
    [Fact]
    public void Constructor_DefaultMessage()
    {
        var ex = new ForbiddenException();

        ex.Code.Should().Be("FORBIDDEN");
        ex.Message.Should().Be("You do not have permission to perform this action.");
    }

    [Fact]
    public void Constructor_CustomMessage()
    {
        var ex = new ForbiddenException("Admin only");

        ex.Code.Should().Be("FORBIDDEN");
        ex.Message.Should().Be("Admin only");
    }
}
