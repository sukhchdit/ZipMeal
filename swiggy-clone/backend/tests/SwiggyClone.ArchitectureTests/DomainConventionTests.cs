using FluentAssertions;
using NetArchTest.Rules;
using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Exceptions;
using Xunit;

namespace SwiggyClone.ArchitectureTests;

public sealed class DomainConventionTests
{
    private static readonly System.Reflection.Assembly DomainAssembly =
        typeof(BaseEntity).Assembly;

    [Fact]
    public void Entities_ShouldResideInEntitiesNamespace()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(BaseEntity))
            .And()
            .AreNotAbstract()
            .Should()
            .ResideInNamespace("SwiggyClone.Domain.Entities")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("Entities must reside in Domain.Entities namespace", result));
    }

    [Fact]
    public void DomainExceptions_ShouldInheritDomainException()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("SwiggyClone.Domain.Exceptions")
            .And()
            .AreNotAbstract()
            .And()
            .DoNotHaveName(nameof(DomainException))
            .Should()
            .Inherit(typeof(DomainException))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            FailMessage("Domain exceptions must inherit DomainException", result));
    }

    [Fact]
    public void Enums_ShouldResideInEnumsNamespace()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ArePublic()
            .And()
            .ResideInNamespace("SwiggyClone.Domain.Enums")
            .Should()
            .ResideInNamespace("SwiggyClone.Domain.Enums")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    private static string FailMessage(string rule, TestResult result)
    {
        if (result.IsSuccessful) return string.Empty;
        var failing = result.FailingTypeNames ?? [];
        return $"{rule}. Failing: {string.Join(", ", failing)}";
    }
}
