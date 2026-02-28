using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace SwiggyClone.ArchitectureTests;

public sealed class InterfaceConventionTests
{
    private static readonly System.Reflection.Assembly ApplicationAssembly =
        typeof(Application.DependencyInjection).Assembly;

    private static readonly System.Reflection.Assembly ApiAssembly =
        typeof(Api.Middleware.ExceptionHandlingMiddleware).Assembly;

    [Fact]
    public void Interfaces_ShouldStartWithI()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("Interfaces must start with 'I'", result));
    }

    [Fact]
    public void Interfaces_ShouldResideInInterfacesNamespace()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .AreInterfaces()
            .And()
            .DoNotHaveNameStartingWith("IPipelineBehavior")
            .Should()
            .ResideInNamespaceContaining("Interfaces")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("Application interfaces should reside in an Interfaces namespace", result));
    }

    [Fact]
    public void Controllers_ShouldNotDirectlyDependOnInfrastructure()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
            .ShouldNot()
            .HaveDependencyOn("SwiggyClone.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("Controllers must not depend on Infrastructure", result));
    }

    private static string FailMessage(string rule, TestResult result)
    {
        if (result.IsSuccessful) return string.Empty;
        var failing = result.FailingTypeNames ?? [];
        return $"{rule}. Failing: {string.Join(", ", failing)}";
    }
}
