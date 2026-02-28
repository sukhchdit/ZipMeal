using FluentAssertions;
using NSubstitute;
using SwiggyClone.Application.Features.Wallet.Commands.DebitWallet;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Wallet;

public sealed class DebitWalletCommandHandlerTests
{
    [Fact]
    public async Task Handle_WalletNotFound_ReturnsFailure()
    {
        var db = MockDbContextFactory.Create();
        var handler = new DebitWalletCommandHandler(db);
        var command = new DebitWalletCommand(Guid.NewGuid(), 1000, null, "Payment");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("WALLET_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_InsufficientBalance_ReturnsFailure()
    {
        var wallet = new WalletBuilder().WithBalance(500).Build();
        var db = MockDbContextFactory.Create(wallets: new List<SwiggyClone.Domain.Entities.Wallet> { wallet });
        var handler = new DebitWalletCommandHandler(db);
        var command = new DebitWalletCommand(wallet.UserId, 1000, null, "Payment");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("INSUFFICIENT_BALANCE");
    }

    [Fact]
    public async Task Handle_Success_DebitsWallet()
    {
        var wallet = new WalletBuilder().WithBalance(5000).Build();
        var db = MockDbContextFactory.Create(wallets: new List<SwiggyClone.Domain.Entities.Wallet> { wallet });
        var handler = new DebitWalletCommandHandler(db);
        var command = new DebitWalletCommand(wallet.UserId, 2000, null, "Order payment");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        wallet.BalancePaise.Should().Be(3000);
    }

    [Fact]
    public async Task Handle_Success_CreatesTransaction()
    {
        var transactions = new List<WalletTransaction>();
        var wallet = new WalletBuilder().WithBalance(5000).Build();
        var db = MockDbContextFactory.Create(
            wallets: new List<SwiggyClone.Domain.Entities.Wallet> { wallet },
            walletTransactions: transactions);
        var handler = new DebitWalletCommandHandler(db);
        var command = new DebitWalletCommand(wallet.UserId, 1000, null, "Order payment");

        await handler.Handle(command, CancellationToken.None);

        transactions.Should().HaveCount(1);
        transactions[0].AmountPaise.Should().Be(1000);
    }

    [Fact]
    public async Task Handle_Success_CallsSaveChanges()
    {
        var wallet = new WalletBuilder().WithBalance(5000).Build();
        var db = MockDbContextFactory.Create(wallets: new List<SwiggyClone.Domain.Entities.Wallet> { wallet });
        var handler = new DebitWalletCommandHandler(db);
        var command = new DebitWalletCommand(wallet.UserId, 1000, null, "Payment");

        await handler.Handle(command, CancellationToken.None);

        await db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
