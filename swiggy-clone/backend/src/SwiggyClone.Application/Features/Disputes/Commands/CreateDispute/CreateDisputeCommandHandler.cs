using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.CreateDispute;

internal sealed class CreateDisputeCommandHandler(
    IAppDbContext db,
    ISender sender,
    IRealtimeNotifier notifier)
    : IRequestHandler<CreateDisputeCommand, Result<DisputeDto>>
{
    private static readonly DisputeIssueType[] AutoResolvableIssueTypes =
    [
        DisputeIssueType.MissingItems,
        DisputeIssueType.WrongItems,
        DisputeIssueType.DamagedPackaging,
    ];

    private const int AutoResolveMaxAmountPaise = 30000;

    public async Task<Result<DisputeDto>> Handle(
        CreateDisputeCommand request, CancellationToken ct)
    {
        // 1. Validate order exists and belongs to user
        var order = await db.Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, ct);

        if (order is null)
            return Result<DisputeDto>.Failure("DISPUTE_ORDER_NOT_ELIGIBLE", "Order not found or does not belong to you.");

        // 2. Validate order status is Delivered or Cancelled
        if (order.Status != OrderStatus.Delivered && order.Status != OrderStatus.Cancelled)
            return Result<DisputeDto>.Failure("DISPUTE_ORDER_NOT_ELIGIBLE", "Only delivered or cancelled orders are eligible for disputes.");

        // 3. Check no active dispute exists for this order
        var hasActiveDispute = await db.Disputes.AsNoTracking()
            .AnyAsync(d => d.OrderId == request.OrderId
                && d.Status != DisputeStatus.Closed
                && d.Status != DisputeStatus.Rejected, ct);

        if (hasActiveDispute)
            return Result<DisputeDto>.Failure("DISPUTE_ALREADY_EXISTS", "An active dispute already exists for this order.");

        var now = DateTimeOffset.UtcNow;
        var issueType = (DisputeIssueType)request.IssueType;

        // 4. Generate DisputeNumber
        var random = Random.Shared.Next(1000, 10000);
        var disputeNumber = $"DSP-{now:yyyyMMdd}-{random}";

        // 5. Create Dispute entity
        var dispute = new Dispute
        {
            Id = Guid.CreateVersion7(),
            DisputeNumber = disputeNumber,
            OrderId = request.OrderId,
            UserId = request.UserId,
            IssueType = issueType,
            Status = DisputeStatus.Opened,
            Description = request.Description,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.Disputes.Add(dispute);

        // 6. Add user's description as first message
        var userMessage = new DisputeMessage
        {
            Id = Guid.CreateVersion7(),
            DisputeId = dispute.Id,
            SenderId = request.UserId,
            Content = request.Description,
            IsSystemMessage = false,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.DisputeMessages.Add(userMessage);

        // 7. Add system message
        var systemMessage = new DisputeMessage
        {
            Id = Guid.CreateVersion7(),
            DisputeId = dispute.Id,
            SenderId = request.UserId,
            Content = "Dispute created. Our team will review your issue.",
            IsSystemMessage = true,
            IsRead = true,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.DisputeMessages.Add(systemMessage);

        // 8. Auto-resolution check
        var autoResolved = false;
        if (AutoResolvableIssueTypes.Contains(issueType) && order.TotalAmount <= AutoResolveMaxAmountPaise)
        {
            dispute.Status = DisputeStatus.Resolved;
            dispute.ResolutionType = DisputeResolutionType.WalletCredit;
            dispute.ResolutionAmountPaise = order.TotalAmount;
            dispute.ResolvedAt = now;
            dispute.ResolutionNotes = "Auto-resolved: wallet credit issued for the full order amount.";

            // Credit wallet
            await sender.Send(new CreditWalletCommand(
                request.UserId,
                order.TotalAmount,
                (short)WalletTransactionSource.DisputeResolution,
                dispute.Id,
                $"Dispute {disputeNumber} auto-resolution credit"), ct);

            // Add system message for auto-resolution
            var autoResolveMessage = new DisputeMessage
            {
                Id = Guid.CreateVersion7(),
                DisputeId = dispute.Id,
                SenderId = request.UserId,
                Content = "Your issue has been auto-resolved with a wallet credit of \u20b9" + (order.TotalAmount / 100m).ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + ".",
                IsSystemMessage = true,
                IsRead = true,
                CreatedAt = now,
                UpdatedAt = now,
            };
            db.DisputeMessages.Add(autoResolveMessage);
            autoResolved = true;
        }

        await db.SaveChangesAsync(ct);

        // 9. Notify
        await notifier.NotifyDisputeEventAsync(
            request.UserId, dispute.Id,
            autoResolved ? "dispute.auto_resolved" : "dispute.created",
            new { dispute.DisputeNumber, dispute.IssueType, dispute.Status }, ct);

        // 10. Return DTO
        var user = await db.Users.AsNoTracking()
            .FirstAsync(u => u.Id == request.UserId, ct);

        return Result<DisputeDto>.Success(new DisputeDto(
            dispute.Id,
            dispute.DisputeNumber,
            dispute.OrderId,
            null,
            dispute.UserId,
            user.FullName,
            null,
            null,
            (int)dispute.IssueType,
            (int)dispute.Status,
            dispute.Description,
            dispute.ResolutionType is not null ? (int)dispute.ResolutionType : null,
            dispute.ResolutionAmountPaise,
            dispute.ResolutionNotes,
            dispute.ResolvedAt,
            dispute.RejectionReason,
            dispute.EscalatedAt,
            dispute.CreatedAt,
            dispute.UpdatedAt));
    }
}
