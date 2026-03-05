using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.Commands.ResolveDispute;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Disputes;

public sealed class ResolveDisputeCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly ISender _sender;
    private readonly IRealtimeNotifier _notifier;
    private readonly ResolveDisputeCommandHandler _handler;
    private readonly List<Dispute> _disputes;

    public ResolveDisputeCommandHandlerTests()
    {
        _disputes = [];

        _db = MockDbContextFactory.Create(disputes: _disputes);

        _sender = Substitute.For<ISender>();
        _sender.Send(Arg.Any<CreditWalletCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<WalletTransactionDto>.Success(
                new WalletTransactionDto(Guid.NewGuid(), 0, 0, 0, null, "credit", 0, DateTimeOffset.UtcNow)));

        _notifier = Substitute.For<IRealtimeNotifier>();
        _handler = new ResolveDisputeCommandHandler(_db, _sender, _notifier);
    }

    [Fact]
    public async Task Handle_FullRefund_ResolvesAndCreditsWallet()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Opened)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.FullRefund,
            50000,
            "Full refund approved.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dispute.Status.Should().Be(DisputeStatus.Resolved);
        dispute.ResolutionType.Should().Be(DisputeResolutionType.FullRefund);
        dispute.ResolutionAmountPaise.Should().Be(50000);
        dispute.ResolvedByAgentId.Should().Be(TestConstants.AdminId);
        dispute.ResolvedAt.Should().NotBeNull();

        await _sender.Received(1).Send(
            Arg.Is<CreditWalletCommand>(c =>
                c.UserId == TestConstants.UserId &&
                c.AmountPaise == 50000),
            Arg.Any<CancellationToken>());

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        await _notifier.Received(1).NotifyDisputeEventAsync(
            TestConstants.UserId, TestConstants.DisputeId, "dispute.resolved", Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PartialRefund_ResolvesAndCreditsWallet()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.UnderReview)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.PartialRefund,
            15000,
            "Partial refund for missing item.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dispute.Status.Should().Be(DisputeStatus.Resolved);
        dispute.ResolutionType.Should().Be(DisputeResolutionType.PartialRefund);
        dispute.ResolutionAmountPaise.Should().Be(15000);

        await _sender.Received(1).Send(
            Arg.Is<CreditWalletCommand>(c => c.AmountPaise == 15000),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WalletCredit_ResolvesAndCreditsWallet()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Escalated)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.WalletCredit,
            20000,
            "Wallet credit issued.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await _sender.Received(1).Send(
            Arg.Is<CreditWalletCommand>(c => c.AmountPaise == 20000),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoAction_ResolvesWithoutCredit()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Opened)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.NoAction,
            null,
            "No action needed after investigation.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dispute.Status.Should().Be(DisputeStatus.Resolved);
        dispute.ResolutionType.Should().Be(DisputeResolutionType.NoAction);
        dispute.ResolutionNotes.Should().Be("No action needed after investigation.");

        await _sender.DidNotReceive().Send(
            Arg.Any<CreditWalletCommand>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Replacement_ResolvesWithoutCredit()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Opened)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.Replacement,
            null,
            "Replacement order dispatched.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dispute.Status.Should().Be(DisputeStatus.Resolved);
        dispute.ResolutionType.Should().Be(DisputeResolutionType.Replacement);

        await _sender.DidNotReceive().Send(
            Arg.Any<CreditWalletCommand>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DisputeNotFound_ReturnsFailure()
    {
        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            Guid.NewGuid(),
            (int)DisputeResolutionType.FullRefund,
            50000,
            "Refund issued.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_AlreadyResolved_ReturnsFailure()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Resolved)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.FullRefund,
            50000,
            "Duplicate resolve attempt.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_ALREADY_RESOLVED");
    }

    [Fact]
    public async Task Handle_AlreadyRejected_ReturnsFailure()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Rejected)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.FullRefund,
            30000,
            "Trying to resolve rejected dispute.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_ALREADY_RESOLVED");
    }

    [Fact]
    public async Task Handle_AlreadyClosed_ReturnsFailure()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Closed)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.WalletCredit,
            10000,
            "Closed dispute.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_ALREADY_RESOLVED");
    }

    [Fact]
    public async Task Handle_ResolvesEscalatedDispute_Succeeds()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Escalated)
            .Build();
        _disputes.Add(dispute);

        var command = new ResolveDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            (int)DisputeResolutionType.FullRefund,
            50000,
            "Escalated dispute resolved.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dispute.Status.Should().Be(DisputeStatus.Resolved);
    }
}
