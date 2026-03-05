using FluentAssertions;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Domain;

public sealed class DisputeTests
{
    [Fact]
    public void NewDispute_DisputeNumber_DefaultsToEmpty()
    {
        var dispute = new Dispute();

        dispute.DisputeNumber.Should().BeEmpty();
    }

    [Fact]
    public void NewDispute_Description_DefaultsToEmpty()
    {
        var dispute = new Dispute();

        dispute.Description.Should().BeEmpty();
    }

    [Fact]
    public void NewDispute_Status_DefaultsToOpened()
    {
        var dispute = new Dispute();

        dispute.Status.Should().Be(DisputeStatus.Opened);
    }

    [Fact]
    public void NewDispute_MessagesCollection_IsInitializedEmpty()
    {
        var dispute = new Dispute();

        dispute.Messages.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void NewDispute_NullableProperties_AreNull()
    {
        var dispute = new Dispute();

        dispute.AssignedAgentId.Should().BeNull();
        dispute.ResolutionType.Should().BeNull();
        dispute.ResolutionAmountPaise.Should().BeNull();
        dispute.ResolutionNotes.Should().BeNull();
        dispute.ResolvedAt.Should().BeNull();
        dispute.ResolvedByAgentId.Should().BeNull();
        dispute.RejectionReason.Should().BeNull();
        dispute.EscalatedAt.Should().BeNull();
        dispute.AssignedAgent.Should().BeNull();
        dispute.ResolvedByAgent.Should().BeNull();
    }

    [Fact]
    public void Dispute_SetProperties_RetainsValues()
    {
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var resolvedByAgentId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var dispute = new Dispute
        {
            DisputeNumber = "DSP-20260303-0001",
            OrderId = orderId,
            UserId = userId,
            AssignedAgentId = agentId,
            IssueType = DisputeIssueType.WrongItems,
            Status = DisputeStatus.Resolved,
            Description = "Received wrong items in order",
            ResolutionType = DisputeResolutionType.FullRefund,
            ResolutionAmountPaise = 50000,
            ResolutionNotes = "Full refund issued",
            ResolvedAt = now,
            ResolvedByAgentId = resolvedByAgentId,
            EscalatedAt = now.AddHours(-1)
        };

        dispute.DisputeNumber.Should().Be("DSP-20260303-0001");
        dispute.OrderId.Should().Be(orderId);
        dispute.UserId.Should().Be(userId);
        dispute.AssignedAgentId.Should().Be(agentId);
        dispute.IssueType.Should().Be(DisputeIssueType.WrongItems);
        dispute.Status.Should().Be(DisputeStatus.Resolved);
        dispute.Description.Should().Be("Received wrong items in order");
        dispute.ResolutionType.Should().Be(DisputeResolutionType.FullRefund);
        dispute.ResolutionAmountPaise.Should().Be(50000);
        dispute.ResolutionNotes.Should().Be("Full refund issued");
        dispute.ResolvedAt.Should().Be(now);
        dispute.ResolvedByAgentId.Should().Be(resolvedByAgentId);
        dispute.EscalatedAt.Should().Be(now.AddHours(-1));
    }

    [Fact]
    public void Dispute_MessagesCollection_CanAddMessages()
    {
        var dispute = new Dispute();
        var message = new DisputeMessage
        {
            DisputeId = dispute.Id,
            SenderId = Guid.NewGuid(),
            Content = "I received the wrong items",
            IsSystemMessage = false,
            IsRead = false
        };

        dispute.Messages.Add(message);

        dispute.Messages.Should().HaveCount(1);
        dispute.Messages.Should().Contain(message);
    }

    [Fact]
    public void Dispute_InheritsBaseEntity_HasSoftDeleteSupport()
    {
        var dispute = new Dispute();

        dispute.IsDeleted.Should().BeFalse();
        dispute.DeletedAt.Should().BeNull();
        dispute.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Dispute_SoftDelete_SetsIsDeletedAndDeletedAt()
    {
        var dispute = new Dispute();

        dispute.SoftDelete();

        dispute.IsDeleted.Should().BeTrue();
        dispute.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Dispute_AllStatusValues_CanBeSet()
    {
        var dispute = new Dispute();

        dispute.Status = DisputeStatus.Opened;
        dispute.Status.Should().Be(DisputeStatus.Opened);

        dispute.Status = DisputeStatus.UnderReview;
        dispute.Status.Should().Be(DisputeStatus.UnderReview);

        dispute.Status = DisputeStatus.AwaitingCustomerResponse;
        dispute.Status.Should().Be(DisputeStatus.AwaitingCustomerResponse);

        dispute.Status = DisputeStatus.Resolved;
        dispute.Status.Should().Be(DisputeStatus.Resolved);

        dispute.Status = DisputeStatus.Closed;
        dispute.Status.Should().Be(DisputeStatus.Closed);

        dispute.Status = DisputeStatus.Escalated;
        dispute.Status.Should().Be(DisputeStatus.Escalated);

        dispute.Status = DisputeStatus.Rejected;
        dispute.Status.Should().Be(DisputeStatus.Rejected);
    }

    [Fact]
    public void Dispute_RejectionReason_CanBeSet()
    {
        var dispute = new Dispute
        {
            Status = DisputeStatus.Rejected,
            RejectionReason = "Issue not covered by policy"
        };

        dispute.RejectionReason.Should().Be("Issue not covered by policy");
    }

    [Fact]
    public void Dispute_EscalatedAt_CanBeSet()
    {
        var now = DateTimeOffset.UtcNow;

        var dispute = new Dispute
        {
            Status = DisputeStatus.Escalated,
            EscalatedAt = now
        };

        dispute.EscalatedAt.Should().Be(now);
    }
}
