using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SwiggyClone.Application.Common.Behaviors;

namespace SwiggyClone.UnitTests.Behaviors;

public sealed class MetricsBehaviorTests
{
    public sealed record TestRequest : IRequest<string>;

    private readonly RequestHandlerDelegate<string> _next = Substitute.For<RequestHandlerDelegate<string>>();

    [Fact]
    public async Task Handle_Success_CallsNextAndReturns()
    {
        _next.Invoke().Returns("result");
        var behavior = new MetricsBehavior<TestRequest, string>();

        var result = await behavior.Handle(new TestRequest(), _next, CancellationToken.None);

        result.Should().Be("result");
        await _next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_Exception_Rethrows()
    {
        _next.Invoke().ThrowsAsync(new InvalidOperationException("fail"));
        var behavior = new MetricsBehavior<TestRequest, string>();

        var act = () => behavior.Handle(new TestRequest(), _next, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("fail");
    }
}
