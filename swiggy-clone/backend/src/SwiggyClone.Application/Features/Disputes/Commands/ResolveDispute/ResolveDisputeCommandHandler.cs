using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.ResolveDispute;

internal sealed class ResolveDisputeCommandHandler(
    IAppDbContext db,
    ISender mediator,
    IRealtimeNotifier notifier)
    : IRequestHandler<ResolveDisputeCommand, Result>
{
    public async Task<Result> Handle(ResolveDisputeCommand request, CancellationToken ct)
    {
        var dispute = await db.Disputes
            .FirstOrDefaultAsync(d => d.Id == request.DisputeId, ct);

        if (dispute is null)
            return Result.Failure("DISPUTE_NOT_FOUND", "Dispute not found.");

        if (dispute.Status is DisputeStatus.Resolved or DisputeStatus.Closed or DisputeStatus.Rejected)
            return Result.Failure("DISPUTE_ALREADY_RESOLVED", "This dispute has already been resolved or closed.");

        var now = DateTimeOffset.UtcNow;
        var resolutionType = (DisputeResolutionType)request.ResolutionType;

        dispute.Status = DisputeStatus.Resolved;
        dispute.ResolutionType = resolutionType;
        dispute.ResolutionAmountPaise = request.ResolutionAmountPaise;
        dispute.ResolutionNotes = request.ResolutionNotes;
        dispute.ResolvedAt = now;
        dispute.ResolvedByAgentId = request.AgentId;
        dispute.UpdatedAt = now;

        // Execute resolution action
        switch (resolutionType)
        {
            case DisputeResolutionType.FullRefund:
            case DisputeResolutionType.PartialRefund:
            case DisputeResolutionType.WalletCredit:
                if (request.ResolutionAmountPaise is > 0)
                {
                    await mediator.Send(new CreditWalletCommand(
                        dispute.UserId,
                        request.ResolutionAmountPaise.Value,
                        (short)WalletTransactionSource.DisputeResolution,
                        dispute.Id,
                        $"Dispute {dispute.DisputeNumber} resolution credit"), ct);
                }
                break;
        }

        // Add system message
        var resolutionLabel = resolutionType switch
        {
            DisputeResolutionType.FullRefund => "full refund",
            DisputeResolutionType.PartialRefund => "partial refund",
            DisputeResolutionType.WalletCredit => "wallet credit",
            DisputeResolutionType.Replacement => "replacement",
            DisputeResolutionType.Coupon => "coupon",
            _ => "no further action",
        };

        var amountText = request.ResolutionAmountPaise is > 0
            ? $" of \u20b9{request.ResolutionAmountPaise.Value / 100m:F2}"
            : "";

        var systemMessage = new DisputeMessage
        {
            Id = Guid.CreateVersion7(),
            DisputeId = dispute.Id,
            SenderId = request.AgentId,
            Content = $"Dispute resolved with {resolutionLabel}{amountText}.{(request.ResolutionNotes is not null ? $" Notes: {request.ResolutionNotes}" : "")}",
            IsSystemMessage = true,
            IsRead = true,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.DisputeMessages.Add(systemMessage);

        await db.SaveChangesAsync(ct);

        await notifier.NotifyDisputeEventAsync(
            dispute.UserId, dispute.Id, "dispute.resolved",
            new { dispute.DisputeNumber, ResolutionType = (int)resolutionType, request.ResolutionAmountPaise }, ct);

        return Result.Success();
    }
}
