using NetArchTest.Rules;
using Xunit;

namespace SwiggyClone.ArchitectureTests;

public class LayerDependencyTests
{
    private const string DomainNamespace = "SwiggyClone.Domain";
    private const string ApplicationNamespace = "SwiggyClone.Application";
    private const string InfrastructureNamespace = "SwiggyClone.Infrastructure";
    private const string ApiNamespace = "SwiggyClone.Api";

    [Fact]
    public void Domain_ShouldNotDependOnApplication()
    {
        var result = Types.InAssembly(typeof(Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Domain layer must not depend on Application layer.");
    }

    [Fact]
    public void Domain_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Domain layer must not depend on Infrastructure layer.");
    }

    [Fact]
    public void Domain_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(typeof(Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Domain layer must not depend on API layer.");
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Application layer must not depend on Infrastructure layer.");
    }

    [Fact]
    public void Application_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(typeof(Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Application layer must not depend on API layer.");
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(typeof(Infrastructure.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Infrastructure layer must not depend on API layer.");
    }
}
