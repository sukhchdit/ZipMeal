using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Commands;

internal sealed class ApproveRestaurantCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<ApproveRestaurantCommand, Result<AdminRestaurantDto>>
{
    public async Task<Result<AdminRestaurantDto>> Handle(ApproveRestaurantCommand request, CancellationToken ct)
    {
        var restaurant = await db.Restaurants
            .Include(r => r.Owner)
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, ct);

        if (restaurant is null)
            return Result<AdminRestaurantDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        if (restaurant.Status != RestaurantStatus.Pending)
            return Result<AdminRestaurantDto>.Failure("INVALID_STATUS", "Only pending restaurants can be approved.");

        restaurant.Status = RestaurantStatus.Approved;
        restaurant.StatusReason = null;
        await db.SaveChangesAsync(ct);

        await publisher.Publish(new RestaurantIndexRequested(restaurant.Id), ct);

        return Result<AdminRestaurantDto>.Success(new AdminRestaurantDto(
            restaurant.Id,
            restaurant.Name,
            restaurant.Slug,
            restaurant.Description,
            restaurant.City,
            restaurant.State,
            restaurant.Owner.FullName,
            restaurant.Owner.PhoneNumber,
            restaurant.LogoUrl,
            restaurant.Status,
            restaurant.StatusReason,
            restaurant.AverageRating,
            restaurant.TotalRatings,
            restaurant.IsAcceptingOrders,
            restaurant.FssaiLicense,
            restaurant.GstNumber,
            restaurant.CreatedAt));
    }
}
