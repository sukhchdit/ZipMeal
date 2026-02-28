using FluentAssertions;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Domain;

public sealed class WalletTransactionTests
{
    [Fact]
    public void Create_ValidParams_SetsAllProperties()
    {
        var walletId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();

        var txn = WalletTransaction.Create(
            walletId, 5000, WalletTransactionType.Credit,
            WalletTransactionSource.AddMoney, referenceId,
            "Add money", 5000);

        txn.Id.Should().NotBeEmpty();
        txn.WalletId.Should().Be(walletId);
        txn.AmountPaise.Should().Be(5000);
        txn.Type.Should().Be(WalletTransactionType.Credit);
        txn.Source.Should().Be(WalletTransactionSource.AddMoney);
        txn.ReferenceId.Should().Be(referenceId);
        txn.Description.Should().Be("Add money");
        txn.BalanceAfterPaise.Should().Be(5000);
    }

    [Fact]
    public void Create_NullReferenceId_SetsToNull()
    {
        var txn = WalletTransaction.Create(
            Guid.NewGuid(), 1000, WalletTransactionType.Debit,
            WalletTransactionSource.OrderPayment, null,
            "Order payment", 4000);

        txn.ReferenceId.Should().BeNull();
    }

    [Fact]
    public void Create_SetsCreatedAtTimestamp()
    {
        var before = DateTimeOffset.UtcNow;

        var txn = WalletTransaction.Create(
            Guid.NewGuid(), 1000, WalletTransactionType.Credit,
            WalletTransactionSource.Refund, null,
            "Refund", 6000);

        txn.CreatedAt.Should().BeOnOrAfter(before);
    }
}
