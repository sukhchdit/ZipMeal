using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SwiggyClone.Application.Common.Behaviors;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.UnitTests.Behaviors;

public sealed class LoggingBehaviorTests
{
    public sealed record TestRequest(string Value) : IRequest<string>;

    private readonly ILogger<LoggingBehavior<TestRequest, string>> _logger =
        Substitute.For<ILogger<LoggingBehavior<TestRequest, string>>>();

    private readonly RequestHandlerDelegate<string> _next = Substitute.For<RequestHandlerDelegate<string>>();

    [Fact]
    public async Task Handle_Success_CallsNextAndReturns()
    {
        _next.Invoke().Returns("result");
        var userService = Substitute.For<ICurrentUserService>();
        userService.UserId.Returns(Guid.NewGuid());
        userService.UserRole.Returns("Customer");

        var behavior = new LoggingBehavior<TestRequest, string>(_logger, userService);

        var result = await behavior.Handle(new TestRequest("test"), _next, CancellationToken.None);

        result.Should().Be("result");
        await _next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_Exception_LogsAndRethrows()
    {
        var expected = new InvalidOperationException("boom");
        _next.Invoke().ThrowsAsync(expected);

        var behavior = new LoggingBehavior<TestRequest, string>(_logger);

        var act = () => behavior.Handle(new TestRequest("test"), _next, CancellationToken.None);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task Handle_NullUserService_DoesNotThrow()
    {
        _next.Invoke().Returns("ok");

        var behavior = new LoggingBehavior<TestRequest, string>(_logger, null);

        var result = await behavior.Handle(new TestRequest("test"), _next, CancellationToken.None);

        result.Should().Be("ok");
    }
}
