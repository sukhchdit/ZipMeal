using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.AssignDispute;

internal sealed class AssignDisputeCommandHandler(IAppDbContext db)
    : IRequestHandler<AssignDisputeCommand, Result>
{
    public async Task<Result> Handle(AssignDisputeCommand request, CancellationToken ct)
    {
        var dispute = await db.Disputes
            .FirstOrDefaultAsync(d => d.Id == request.DisputeId, ct);

        if (dispute is null)
            return Result.Failure("DISPUTE_NOT_FOUND", "Dispute not found.");

        var agentExists = await db.Users.AsNoTracking()
            .AnyAsync(u => u.Id == request.AgentId, ct);

        if (!agentExists)
            return Result.Failure("USER_NOT_FOUND", "Agent not found.");

        var now = DateTimeOffset.UtcNow;

        dispute.AssignedAgentId = request.AgentId;
        if (dispute.Status == DisputeStatus.Opened)
        {
            dispute.Status = DisputeStatus.UnderReview;
        }
        dispute.UpdatedAt = now;

        var systemMessage = new DisputeMessage
        {
            Id = Guid.CreateVersion7(),
            DisputeId = dispute.Id,
            SenderId = request.AgentId,
            Content = "An agent has been assigned to your dispute.",
            IsSystemMessage = true,
            IsRead = true,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.DisputeMessages.Add(systemMessage);

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
