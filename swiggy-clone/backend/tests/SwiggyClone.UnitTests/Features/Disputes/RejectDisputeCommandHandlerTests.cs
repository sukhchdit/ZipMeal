using FluentAssertions;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.Commands.RejectDispute;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Disputes;

public sealed class RejectDisputeCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly IRealtimeNotifier _notifier;
    private readonly RejectDisputeCommandHandler _handler;
    private readonly List<Dispute> _disputes;

    public RejectDisputeCommandHandlerTests()
    {
        _disputes = [];

        _db = MockDbContextFactory.Create(disputes: _disputes);

        _notifier = Substitute.For<IRealtimeNotifier>();
        _handler = new RejectDisputeCommandHandler(_db, _notifier);
    }

    [Fact]
    public async Task Handle_ValidRequest_RejectsDispute()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Opened)
            .Build();
        _disputes.Add(dispute);

        var command = new RejectDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            "Insufficient evidence provided.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dispute.Status.Should().Be(DisputeStatus.Rejected);
        dispute.RejectionReason.Should().Be("Insufficient evidence provided.");
        dispute.ResolvedByAgentId.Should().Be(TestConstants.AdminId);
        dispute.ResolvedAt.Should().NotBeNull();

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        await _notifier.Received(1).NotifyDisputeEventAsync(
            TestConstants.UserId,
            TestConstants.DisputeId,
            "dispute.rejected",
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DisputeNotFound_ReturnsFailure()
    {
        var command = new RejectDisputeCommand(
            TestConstants.AdminId,
            Guid.NewGuid(),
            "Not found.");

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

        var command = new RejectDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            "Cannot reject resolved dispute.");

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

        var command = new RejectDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            "Double rejection attempt.");

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

        var command = new RejectDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            "Cannot reject closed dispute.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_ALREADY_RESOLVED");
    }

    [Fact]
    public async Task Handle_EscalatedDispute_CanBeRejected()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Escalated)
            .Build();
        _disputes.Add(dispute);

        var command = new RejectDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            "Escalated dispute reviewed and rejected.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dispute.Status.Should().Be(DisputeStatus.Rejected);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsSystemMessage()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.UnderReview)
            .Build();
        _disputes.Add(dispute);

        var command = new RejectDisputeCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            "No valid claim found.");

        await _handler.Handle(command, CancellationToken.None);

        _db.DisputeMessages.Received(1).Add(Arg.Is<DisputeMessage>(m =>
            m.IsSystemMessage &&
            m.Content.Contains("No valid claim found.") &&
            m.SenderId == TestConstants.AdminId));
    }
}
