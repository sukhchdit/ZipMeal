using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Notifications.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Queries;

internal sealed class GetUnreadCountQueryHandler(IAppDbContext db)
    : IRequestHandler<GetUnreadCountQuery, Result<UnreadCountDto>>
{
    public async Task<Result<UnreadCountDto>> Handle(
        GetUnreadCountQuery request,
        CancellationToken ct)
    {
        var count = await db.Notifications
            .CountAsync(n => n.UserId == request.UserId && !n.IsRead, ct);

        return Result<UnreadCountDto>.Success(new UnreadCountDto(count));
    }
}
