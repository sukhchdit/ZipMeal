using FluentAssertions;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.UnitTests.Domain;

public sealed class WalletTests
{
    [Fact]
    public void Create_ValidUserId_ReturnsWalletWithZeroBalance()
    {
        var userId = Guid.NewGuid();

        var wallet = Wallet.Create(userId);

        wallet.UserId.Should().Be(userId);
        wallet.BalancePaise.Should().Be(0);
        wallet.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ValidUserId_SetsCreatedAndUpdatedTimestamps()
    {
        var before = DateTimeOffset.UtcNow;

        var wallet = Wallet.Create(Guid.NewGuid());

        wallet.CreatedAt.Should().BeOnOrAfter(before);
        wallet.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void Credit_PositiveAmount_IncreasesBalance()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        wallet.Credit(1000);

        wallet.BalancePaise.Should().Be(1000);
    }

    [Fact]
    public void Credit_MultipleTimes_AccumulatesBalance()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        wallet.Credit(500);
        wallet.Credit(300);

        wallet.BalancePaise.Should().Be(800);
    }

    [Fact]
    public void Credit_ZeroAmount_ThrowsArgumentOutOfRangeException()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        var act = () => wallet.Credit(0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Credit_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        var act = () => wallet.Credit(-100);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Debit_ValidAmount_DecreasesBalance()
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Credit(1000);

        wallet.Debit(400);

        wallet.BalancePaise.Should().Be(600);
    }

    [Fact]
    public void Debit_ExactBalance_SetsToZero()
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Credit(500);

        wallet.Debit(500);

        wallet.BalancePaise.Should().Be(0);
    }

    [Fact]
    public void Debit_InsufficientBalance_ThrowsInvalidOperationException()
    {
        var wallet = Wallet.Create(Guid.NewGuid());
        wallet.Credit(100);

        var act = () => wallet.Debit(200);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Debit_ZeroAmount_ThrowsArgumentOutOfRangeException()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        var act = () => wallet.Debit(0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Debit_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        var act = () => wallet.Debit(-50);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Debit_ZeroBalance_ThrowsInvalidOperationException()
    {
        var wallet = Wallet.Create(Guid.NewGuid());

        var act = () => wallet.Debit(1);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var wallet1 = Wallet.Create(Guid.NewGuid());
        var wallet2 = Wallet.Create(Guid.NewGuid());

        wallet1.Id.Should().NotBe(wallet2.Id);
    }
}
