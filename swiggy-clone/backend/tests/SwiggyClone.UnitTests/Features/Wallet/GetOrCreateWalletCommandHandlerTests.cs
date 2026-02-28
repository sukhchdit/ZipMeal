using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.Commands.GetOrCreateWallet;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Wallet;

public sealed class GetOrCreateWalletCommandHandlerTests
{
    [Fact]
    public async Task Handle_WalletExists_ReturnsExistingWallet()
    {
        var existingWallet = new WalletBuilder().WithBalance(5000).Build();
        var db = MockDbContextFactory.Create(wallets: new List<SwiggyClone.Domain.Entities.Wallet> { existingWallet });
        var handler = new GetOrCreateWalletCommandHandler(db);

        var result = await handler.Handle(
            new GetOrCreateWalletCommand(existingWallet.UserId), CancellationToken.None);

        result.Should().BeSameAs(existingWallet);
        result.BalancePaise.Should().Be(5000);
    }

    [Fact]
    public async Task Handle_WalletNotFound_CreatesNewWallet()
    {
        var wallets = new List<SwiggyClone.Domain.Entities.Wallet>();
        var db = MockDbContextFactory.Create(wallets: wallets);
        var handler = new GetOrCreateWalletCommandHandler(db);
        var userId = Guid.NewGuid();

        var result = await handler.Handle(
            new GetOrCreateWalletCommand(userId), CancellationToken.None);

        result.UserId.Should().Be(userId);
        result.BalancePaise.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WalletNotFound_AddsToDbSet()
    {
        var wallets = new List<SwiggyClone.Domain.Entities.Wallet>();
        var db = MockDbContextFactory.Create(wallets: wallets);
        var handler = new GetOrCreateWalletCommandHandler(db);

        await handler.Handle(
            new GetOrCreateWalletCommand(Guid.NewGuid()), CancellationToken.None);

        wallets.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WalletNotFound_CallsSaveChanges()
    {
        var db = MockDbContextFactory.Create();
        var handler = new GetOrCreateWalletCommandHandler(db);

        await handler.Handle(
            new GetOrCreateWalletCommand(Guid.NewGuid()), CancellationToken.None);

        await db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
