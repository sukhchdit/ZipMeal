using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Wallet;

public sealed class CreditWalletCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly ISender _sender;
    private readonly CreditWalletCommandHandler _handler;
    private readonly List<WalletTransaction> _transactions = [];

    public CreditWalletCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(walletTransactions: _transactions);
        _sender = Substitute.For<ISender>();
        _handler = new CreditWalletCommandHandler(_db, _sender);
    }

    [Fact]
    public async Task Handle_Success_CreditsWallet()
    {
        var wallet = new WalletBuilder().WithBalance(0).Build();
        _sender.Send(Arg.Any<IRequest<SwiggyClone.Domain.Entities.Wallet>>(), Arg.Any<CancellationToken>())
            .Returns(wallet);

        var command = new CreditWalletCommand(wallet.UserId, 5000, 0, null, "Add money");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        wallet.BalancePaise.Should().Be(5000);
    }

    [Fact]
    public async Task Handle_Success_ReturnsTransactionDto()
    {
        var wallet = new WalletBuilder().WithBalance(1000).Build();
        _sender.Send(Arg.Any<IRequest<SwiggyClone.Domain.Entities.Wallet>>(), Arg.Any<CancellationToken>())
            .Returns(wallet);

        var command = new CreditWalletCommand(wallet.UserId, 3000, 0, null, "Cashback");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Value.AmountPaise.Should().Be(3000);
        result.Value.Description.Should().Be("Cashback");
        result.Value.BalanceAfterPaise.Should().Be(4000);
    }

    [Fact]
    public async Task Handle_Success_SavesChanges()
    {
        var wallet = new WalletBuilder().Build();
        _sender.Send(Arg.Any<IRequest<SwiggyClone.Domain.Entities.Wallet>>(), Arg.Any<CancellationToken>())
            .Returns(wallet);

        var command = new CreditWalletCommand(wallet.UserId, 1000, 0, null, "Test");

        await _handler.Handle(command, CancellationToken.None);

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _transactions.Should().HaveCount(1);
    }
}
