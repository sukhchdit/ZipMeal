using FluentAssertions;
using MediatR;
using NetArchTest.Rules;
using Xunit;

namespace SwiggyClone.ArchitectureTests;

public sealed class CqrsConventionTests
{
    private static readonly System.Reflection.Assembly ApplicationAssembly =
        typeof(Application.DependencyInjection).Assembly;

    [Fact]
    public void Commands_ShouldImplementIRequest()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Command")
            .And()
            .AreNotAbstract()
            .Should()
            .ImplementInterface(typeof(IRequest<>))
            .Or()
            .ImplementInterface(typeof(IRequest))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("Commands must implement IRequest", result));
    }

    [Fact]
    public void Queries_ShouldImplementIRequest()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Query")
            .And()
            .AreNotAbstract()
            .Should()
            .ImplementInterface(typeof(IRequest<>))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("Queries must implement IRequest<>", result));
    }

    [Fact]
    public void CommandHandlers_ShouldImplementIRequestHandler()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<>))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("CommandHandlers must implement IRequestHandler", result));
    }

    [Fact]
    public void QueryHandlers_ShouldImplementIRequestHandler()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("QueryHandlers must implement IRequestHandler<,>", result));
    }

    private static string FailMessage(string rule, TestResult result)
    {
        if (result.IsSuccessful) return string.Empty;
        var failing = result.FailingTypeNames ?? [];
        return $"{rule}. Failing: {string.Join(", ", failing)}";
    }
}
