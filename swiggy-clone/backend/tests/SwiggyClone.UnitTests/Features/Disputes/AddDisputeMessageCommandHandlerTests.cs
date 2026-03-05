using FluentAssertions;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.Commands.AddDisputeMessage;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Disputes;

public sealed class AddDisputeMessageCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly IRealtimeNotifier _notifier;
    private readonly AddDisputeMessageCommandHandler _handler;
    private readonly List<Dispute> _disputes;
    private readonly List<User> _users;

    public AddDisputeMessageCommandHandlerTests()
    {
        var user = new UserBuilder().Build();
        _users = [user];
        _disputes = [];

        _db = MockDbContextFactory.Create(
            users: _users,
            disputes: _disputes);

        _notifier = Substitute.For<IRealtimeNotifier>();
        _handler = new AddDisputeMessageCommandHandler(_db, _notifier);
    }

    [Fact]
    public async Task Handle_ValidMessage_AddsMessage()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Opened)
            .Build();
        _disputes.Add(dispute);

        var command = new AddDisputeMessageCommand(
            TestConstants.UserId,
            TestConstants.DisputeId,
            "I have additional photos to share.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Content.Should().Be("I have additional photos to share.");
        result.Value.DisputeId.Should().Be(TestConstants.DisputeId);
        result.Value.SenderId.Should().Be(TestConstants.UserId);
        result.Value.IsSystemMessage.Should().BeFalse();
        result.Value.SenderName.Should().Be(TestConstants.ValidFullName);

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DisputeNotFound_ReturnsFailure()
    {
        var command = new AddDisputeMessageCommand(
            TestConstants.UserId,
            Guid.NewGuid(),
            "Message to non-existent dispute.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_UserNotParticipant_ReturnsFailure()
    {
        var otherUserId = Guid.NewGuid();
        var dispute = new DisputeBuilder()
            .WithUserId(otherUserId)
            .WithStatus(DisputeStatus.Opened)
            .Build();
        _disputes.Add(dispute);

        var command = new AddDisputeMessageCommand(
            TestConstants.UserId, // Not the dispute owner or assigned agent
            dispute.Id,
            "I should not be able to send this.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_ResolvedDispute_ReturnsFailure()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Resolved)
            .Build();
        _disputes.Add(dispute);

        var command = new AddDisputeMessageCommand(
            TestConstants.UserId,
            TestConstants.DisputeId,
            "Trying to message a resolved dispute.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_CANNOT_ADD_MESSAGE");
    }

    [Fact]
    public async Task Handle_RejectedDispute_ReturnsFailure()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Rejected)
            .Build();
        _disputes.Add(dispute);

        var command = new AddDisputeMessageCommand(
            TestConstants.UserId,
            TestConstants.DisputeId,
            "Trying to message a rejected dispute.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_CANNOT_ADD_MESSAGE");
    }

    [Fact]
    public async Task Handle_ClosedDispute_ReturnsFailure()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Closed)
            .Build();
        _disputes.Add(dispute);

        var command = new AddDisputeMessageCommand(
            TestConstants.UserId,
            TestConstants.DisputeId,
            "Trying to message a closed dispute.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_CANNOT_ADD_MESSAGE");
    }

    [Fact]
    public async Task Handle_AssignedAgent_CanSendMessage()
    {
        var agentUser = new UserBuilder()
            .WithId(TestConstants.AdminId)
            .WithFullName("Agent Smith")
            .WithPhone("+919999999999")
            .WithEmail("agent@example.com")
            .Build();
        _users.Add(agentUser);

        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.UnderReview)
            .Build();
        dispute.AssignedAgentId = TestConstants.AdminId;
        _disputes.Add(dispute);

        var command = new AddDisputeMessageCommand(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            "We are looking into your issue.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.SenderId.Should().Be(TestConstants.AdminId);
        result.Value.SenderName.Should().Be("Agent Smith");
    }

    [Fact]
    public async Task Handle_EscalatedDispute_AllowsMessage()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Escalated)
            .Build();
        _disputes.Add(dispute);

        var command = new AddDisputeMessageCommand(
            TestConstants.UserId,
            TestConstants.DisputeId,
            "Please prioritize this issue.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidMessage_NotifiesRecipient()
    {
        var dispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Opened)
            .Build();
        dispute.AssignedAgentId = TestConstants.AdminId;
        _disputes.Add(dispute);

        var command = new AddDisputeMessageCommand(
            TestConstants.UserId,
            TestConstants.DisputeId,
            "Any update on my dispute?");

        await _handler.Handle(command, CancellationToken.None);

        await _notifier.Received(1).NotifyDisputeEventAsync(
            TestConstants.AdminId,
            TestConstants.DisputeId,
            "dispute.new_message",
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }
}
