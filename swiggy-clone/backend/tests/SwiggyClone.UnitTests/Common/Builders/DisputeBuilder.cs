using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class DisputeBuilder
{
    private Guid _id = TestConstants.DisputeId;
    private Guid _orderId = TestConstants.OrderId;
    private Guid _userId = TestConstants.UserId;
    private DisputeIssueType _issueType = DisputeIssueType.MissingItems;
    private DisputeStatus _status = DisputeStatus.Opened;
    private string _description = "Items were missing from my order.";

    public DisputeBuilder WithId(Guid id) { _id = id; return this; }
    public DisputeBuilder WithOrderId(Guid orderId) { _orderId = orderId; return this; }
    public DisputeBuilder WithUserId(Guid userId) { _userId = userId; return this; }
    public DisputeBuilder WithIssueType(DisputeIssueType type) { _issueType = type; return this; }
    public DisputeBuilder WithStatus(DisputeStatus status) { _status = status; return this; }
    public DisputeBuilder WithDescription(string desc) { _description = desc; return this; }

    public Dispute Build() => new()
    {
        Id = _id,
        DisputeNumber = $"DSP-{DateTime.UtcNow:yyyyMMdd}-ABC123",
        OrderId = _orderId,
        UserId = _userId,
        IssueType = _issueType,
        Status = _status,
        Description = _description,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
