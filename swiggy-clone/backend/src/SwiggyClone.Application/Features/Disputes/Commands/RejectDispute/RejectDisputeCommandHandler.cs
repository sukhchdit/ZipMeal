using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.RejectDispute;

internal sealed class RejectDisputeCommandHandler(
    IAppDbContext db,
    IRealtimeNotifier notifier)
    : IRequestHandler<RejectDisputeCommand, Result>
{
    public async Task<Result> Handle(RejectDisputeCommand request, CancellationToken ct)
    {
        var dispute = await db.Disputes
            .FirstOrDefaultAsync(d => d.Id == request.DisputeId, ct);

        if (dispute is null)
            return Result.Failure("DISPUTE_NOT_FOUND", "Dispute not found.");

        if (dispute.Status is DisputeStatus.Resolved or DisputeStatus.Closed or DisputeStatus.Rejected)
            return Result.Failure("DISPUTE_ALREADY_RESOLVED", "This dispute has already been resolved or closed.");

        var now = DateTimeOffset.UtcNow;

        dispute.Status = DisputeStatus.Rejected;
        dispute.RejectionReason = request.Reason;
        dispute.ResolvedByAgentId = request.AgentId;
        dispute.ResolvedAt = now;
        dispute.UpdatedAt = now;

        var systemMessage = new DisputeMessage
        {
            Id = Guid.CreateVersion7(),
            DisputeId = dispute.Id,
            SenderId = request.AgentId,
            Content = $"Dispute rejected. Reason: {request.Reason}",
            IsSystemMessage = true,
            IsRead = true,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.DisputeMessages.Add(systemMessage);

        await db.SaveChangesAsync(ct);

        await notifier.NotifyDisputeEventAsync(
            dispute.UserId, dispute.Id, "dispute.rejected",
            new { dispute.DisputeNumber, request.Reason }, ct);

        return Result.Success();
    }
}
