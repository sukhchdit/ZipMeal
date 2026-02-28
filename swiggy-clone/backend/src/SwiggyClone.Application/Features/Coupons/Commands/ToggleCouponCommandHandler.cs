using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Commands;

internal sealed class ToggleCouponCommandHandler(IAppDbContext db)
    : IRequestHandler<ToggleCouponCommand, Result>
{
    public async Task<Result> Handle(ToggleCouponCommand request, CancellationToken ct)
    {
        var coupon = await db.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (coupon is null)
            return Result.Failure("COUPON_NOT_FOUND", "Coupon not found.");

        coupon.IsActive = request.IsActive;
        coupon.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
