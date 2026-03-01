using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

internal sealed class GetDineInMenuQueryHandler(IAppDbContext db)
    : IRequestHandler<GetDineInMenuQuery, Result<PublicRestaurantDetailDto>>
{
    public async Task<Result<PublicRestaurantDetailDto>> Handle(
        GetDineInMenuQuery request, CancellationToken ct)
    {
        // 1. Validate user is a member of an active session
        var session = await db.DineInSessionMembers
            .AsNoTracking()
            .Where(m => m.SessionId == request.SessionId
                && m.UserId == request.UserId
                && m.Session.Status == DineInSessionStatus.Active)
            .Select(m => new { m.Session.RestaurantId })
            .FirstOrDefaultAsync(ct);

        if (session is null)
            return Result<PublicRestaurantDetailDto>.Failure("NOT_SESSION_MEMBER",
                "You are not a member of this active dine-in session.");

        // 2. Reuse the same projection as GetPublicRestaurantDetailQuery
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .Where(r => r.Id == session.RestaurantId && r.Status == RestaurantStatus.Approved)
            .Select(r => new PublicRestaurantDetailDto(
                r.Id,
                r.Name,
                r.Slug,
                r.Description,
                r.LogoUrl,
                r.BannerUrl,
                r.AddressLine1,
                r.City,
                r.State,
                r.PostalCode,
                r.Latitude,
                r.Longitude,
                r.AverageRating,
                r.TotalRatings,
                r.AvgDeliveryTimeMin,
                r.AvgCostForTwo,
                r.IsVegOnly,
                r.IsAcceptingOrders,
                r.IsDineInEnabled,
                r.RestaurantCuisines
                    .Select(rc => rc.CuisineType.Name)
                    .ToList(),
                r.OperatingHours
                    .OrderBy(oh => oh.DayOfWeek)
                    .Select(oh => new OperatingHoursDto(
                        oh.Id,
                        oh.DayOfWeek,
                        oh.OpenTime,
                        oh.CloseTime,
                        oh.IsClosed))
                    .ToList(),
                r.MenuCategories
                    .Where(mc => mc.IsActive)
                    .OrderBy(mc => mc.SortOrder)
                    .Select(mc => new MenuSectionDto(
                        mc.Id,
                        mc.Name,
                        mc.SortOrder,
                        mc.MenuItems
                            .Where(mi => mi.IsAvailable)
                            .OrderBy(mi => mi.SortOrder)
                            .Select(mi => new MenuItemDto(
                                mi.Id,
                                mi.CategoryId,
                                mi.Name,
                                mi.Description,
                                mi.Price,
                                mi.DiscountedPrice,
                                mi.ImageUrl,
                                mi.IsVeg,
                                mi.IsAvailable,
                                mi.IsBestseller,
                                mi.PreparationTimeMin,
                                mi.SortOrder,
                                mi.SpiceLevel,
                                mi.Allergens,
                                mi.DietaryTags,
                                mi.CalorieCount,
                                mi.Variants
                                    .Where(v => v.IsAvailable)
                                    .OrderBy(v => v.SortOrder)
                                    .Select(v => new MenuItemVariantDto(
                                        v.Id, v.Name, v.PriceAdjustment, v.IsDefault, v.IsAvailable, v.SortOrder))
                                    .ToList(),
                                mi.Addons
                                    .Where(a => a.IsAvailable)
                                    .OrderBy(a => a.SortOrder)
                                    .Select(a => new MenuItemAddonDto(
                                        a.Id, a.Name, a.Price, a.IsVeg, a.IsAvailable, a.MaxQuantity, a.SortOrder))
                                    .ToList()))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(ct);

        if (restaurant is null)
            return Result<PublicRestaurantDetailDto>.Failure("RESTAURANT_NOT_FOUND",
                "Restaurant not found or not approved.");

        return Result<PublicRestaurantDetailDto>.Success(restaurant);
    }
}
