using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.AddDisputeMessage;

internal sealed class AddDisputeMessageCommandHandler(
    IAppDbContext db,
    IRealtimeNotifier notifier)
    : IRequestHandler<AddDisputeMessageCommand, Result<DisputeMessageDto>>
{
    public async Task<Result<DisputeMessageDto>> Handle(
        AddDisputeMessageCommand request, CancellationToken ct)
    {
        var dispute = await db.Disputes.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.DisputeId, ct);

        if (dispute is null)
            return Result<DisputeMessageDto>.Failure("DISPUTE_NOT_FOUND", "Dispute not found.");

        // Validate user is participant (customer or assigned agent)
        if (dispute.UserId != request.UserId && dispute.AssignedAgentId != request.UserId)
            return Result<DisputeMessageDto>.Failure("DISPUTE_NOT_FOUND", "Dispute not found.");

        // Cannot add message to resolved/closed/rejected disputes
        if (dispute.Status is DisputeStatus.Resolved or DisputeStatus.Closed or DisputeStatus.Rejected)
            return Result<DisputeMessageDto>.Failure("DISPUTE_CANNOT_ADD_MESSAGE", "Cannot add messages to a resolved, closed, or rejected dispute.");

        var now = DateTimeOffset.UtcNow;
        var message = new DisputeMessage
        {
            Id = Guid.CreateVersion7(),
            DisputeId = request.DisputeId,
            SenderId = request.UserId,
            Content = request.Content,
            IsSystemMessage = false,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.DisputeMessages.Add(message);
        await db.SaveChangesAsync(ct);

        var sender = await db.Users.AsNoTracking()
            .FirstAsync(u => u.Id == request.UserId, ct);

        // Notify the other party
        var recipientId = dispute.UserId == request.UserId
            ? dispute.AssignedAgentId
            : dispute.UserId;

        if (recipientId.HasValue)
        {
            await notifier.NotifyDisputeEventAsync(
                recipientId.Value, dispute.Id, "dispute.new_message",
                new { message.Id, sender.FullName, message.Content }, ct);
        }

        return Result<DisputeMessageDto>.Success(new DisputeMessageDto(
            message.Id,
            message.DisputeId,
            message.SenderId,
            sender.FullName,
            message.Content,
            message.IsSystemMessage,
            message.IsRead,
            message.ReadAt,
            message.CreatedAt));
    }
}
