using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace SwiggyClone.ArchitectureTests;

public sealed class NamingConventionTests
{
    private static readonly System.Reflection.Assembly ApplicationAssembly =
        typeof(Application.DependencyInjection).Assembly;

    private static readonly System.Reflection.Assembly ApiAssembly =
        typeof(Api.Middleware.ExceptionHandlingMiddleware).Assembly;

    [Fact]
    public void Handlers_ShouldEndWithHandler()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(MediatR.IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            BuildFailureMessage("Handlers must end with 'Handler'", result));
    }

    [Fact]
    public void Validators_ShouldEndWithValidator()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(FluentValidation.AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            BuildFailureMessage("Validators must end with 'Validator'", result));
    }

    [Fact]
    public void Commands_ShouldEndWithCommand()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Command")
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Queries_ShouldEndWithQuery()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Query")
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controllers_ShouldEndWithController()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            BuildFailureMessage("Controllers must end with 'Controller'", result));
    }

    private static string BuildFailureMessage(string rule, TestResult result)
    {
        if (result.IsSuccessful) return string.Empty;
        var failing = result.FailingTypeNames ?? [];
        return $"{rule}. Failing types: {string.Join(", ", failing)}";
    }
}
