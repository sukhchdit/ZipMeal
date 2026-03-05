using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Queries.GetDisputeDetail;

internal sealed class GetDisputeDetailQueryHandler(IAppDbContext db)
    : IRequestHandler<GetDisputeDetailQuery, Result<DisputeDto>>
{
    public async Task<Result<DisputeDto>> Handle(
        GetDisputeDetailQuery request, CancellationToken ct)
    {
        var dto = await db.Disputes.AsNoTracking()
            .Where(d => d.Id == request.DisputeId && d.UserId == request.UserId)
            .Select(d => new DisputeDto(
                d.Id,
                d.DisputeNumber,
                d.OrderId,
                d.Order.OrderNumber,
                d.UserId,
                d.User.FullName,
                d.AssignedAgentId,
                d.AssignedAgent != null ? d.AssignedAgent.FullName : null,
                (int)d.IssueType,
                (int)d.Status,
                d.Description,
                d.ResolutionType != null ? (int)d.ResolutionType : null,
                d.ResolutionAmountPaise,
                d.ResolutionNotes,
                d.ResolvedAt,
                d.RejectionReason,
                d.EscalatedAt,
                d.CreatedAt,
                d.UpdatedAt))
            .FirstOrDefaultAsync(ct);

        return dto is not null
            ? Result<DisputeDto>.Success(dto)
            : Result<DisputeDto>.Failure("DISPUTE_NOT_FOUND", "Dispute not found.");
    }
}
