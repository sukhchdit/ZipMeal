using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using ValidationException = SwiggyClone.Application.Common.Exceptions.ValidationException;
using SwiggyClone.Application.Common.Behaviors;

namespace SwiggyClone.UnitTests.Behaviors;

public sealed class ValidationBehaviorTests
{
    public sealed record TestRequest(string Name) : IRequest<string>;

    private readonly RequestHandlerDelegate<string> _next = Substitute.For<RequestHandlerDelegate<string>>();

    public ValidationBehaviorTests()
    {
        _next.Invoke().Returns("OK");
    }

    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        var behavior = new ValidationBehavior<TestRequest, string>(
            Enumerable.Empty<IValidator<TestRequest>>());

        var result = await behavior.Handle(new TestRequest("test"), _next, CancellationToken.None);

        result.Should().Be("OK");
        await _next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_AllValidatorsPass_CallsNext()
    {
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });

        var result = await behavior.Handle(new TestRequest("test"), _next, CancellationToken.None);

        result.Should().Be("OK");
    }

    [Fact]
    public async Task Handle_OneValidatorFails_ThrowsValidationException()
    {
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required")
            }));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });

        var act = () => behavior.Handle(new TestRequest(""), _next, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        await _next.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_MultipleFailures_AggregatesErrors()
    {
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Name", "Name too short"),
            }));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });

        var act = () => behavior.Handle(new TestRequest(""), _next, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().ContainKey("Name");
        ex.Which.Errors["Name"].Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_MultipleValidators_AggregatesAllFailures()
    {
        var validator1 = Substitute.For<IValidator<TestRequest>>();
        validator1.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Error from validator 1"),
            }));

        var validator2 = Substitute.For<IValidator<TestRequest>>();
        validator2.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Error from validator 2"),
            }));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator1, validator2 });

        var act = () => behavior.Handle(new TestRequest(""), _next, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors["Name"].Should().HaveCount(2);
    }
}
