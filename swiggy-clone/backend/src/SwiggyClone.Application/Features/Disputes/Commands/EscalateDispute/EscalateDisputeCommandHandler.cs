using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.EscalateDispute;

internal sealed class EscalateDisputeCommandHandler(
    IAppDbContext db,
    IRealtimeNotifier notifier)
    : IRequestHandler<EscalateDisputeCommand, Result>
{
    public async Task<Result> Handle(EscalateDisputeCommand request, CancellationToken ct)
    {
        var dispute = await db.Disputes
            .FirstOrDefaultAsync(d => d.Id == request.DisputeId, ct);

        if (dispute is null)
            return Result.Failure("DISPUTE_NOT_FOUND", "Dispute not found.");

        var now = DateTimeOffset.UtcNow;

        dispute.Status = DisputeStatus.Escalated;
        dispute.EscalatedAt = now;
        dispute.UpdatedAt = now;

        var systemMessage = new DisputeMessage
        {
            Id = Guid.CreateVersion7(),
            DisputeId = dispute.Id,
            SenderId = dispute.UserId,
            Content = "This dispute has been escalated for priority review.",
            IsSystemMessage = true,
            IsRead = true,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.DisputeMessages.Add(systemMessage);

        await db.SaveChangesAsync(ct);

        await notifier.NotifyDisputeEventAsync(
            dispute.UserId, dispute.Id, "dispute.escalated",
            new { dispute.DisputeNumber }, ct);

        return Result.Success();
    }
}
