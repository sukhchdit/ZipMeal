using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class UpdateRestaurantProfileCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<UpdateRestaurantProfileCommand, Result<RestaurantDto>>
{
    public async Task<Result<RestaurantDto>> Handle(
        UpdateRestaurantProfileCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<RestaurantDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var restaurant = ownershipResult.Value;

        if (request.Name is not null)
        {
            restaurant.Name = request.Name;

            var slug = SlugHelper.GenerateSlug(request.Name);
            var baseSlug = slug;

            for (var i = 1; i <= 5; i++)
            {
                var exists = await db.Restaurants
                    .AnyAsync(r => r.Slug == slug && r.Id != restaurant.Id, ct);
                if (!exists) break;

                if (i == 5)
                    return Result<RestaurantDto>.Failure("SLUG_CONFLICT", "Unable to generate a unique slug for this restaurant name.");

                slug = SlugHelper.AppendSuffix(baseSlug, i);
            }

            restaurant.Slug = slug;
        }

        if (request.Description is not null) restaurant.Description = request.Description;
        if (request.PhoneNumber is not null) restaurant.PhoneNumber = request.PhoneNumber;
        if (request.Email is not null) restaurant.Email = request.Email;
        if (request.AddressLine1 is not null) restaurant.AddressLine1 = request.AddressLine1;
        if (request.AddressLine2 is not null) restaurant.AddressLine2 = request.AddressLine2;
        if (request.City is not null) restaurant.City = request.City;
        if (request.State is not null) restaurant.State = request.State;
        if (request.PostalCode is not null) restaurant.PostalCode = request.PostalCode;
        if (request.Latitude.HasValue) restaurant.Latitude = request.Latitude;
        if (request.Longitude.HasValue) restaurant.Longitude = request.Longitude;
        if (request.IsVegOnly.HasValue) restaurant.IsVegOnly = request.IsVegOnly.Value;
        if (request.AvgCostForTwo.HasValue) restaurant.AvgCostForTwo = request.AvgCostForTwo;

        restaurant.UpdatedAt = DateTimeOffset.UtcNow;

        var cuisineDtos = new List<CuisineTypeDto>();

        if (request.CuisineIds is not null)
        {
            var existingCuisines = await db.RestaurantCuisines
                .Where(rc => rc.RestaurantId == restaurant.Id)
                .ToListAsync(ct);

            db.RestaurantCuisines.RemoveRange(existingCuisines);

            if (request.CuisineIds.Count > 0)
            {
                var cuisineTypes = await db.CuisineTypes
                    .Where(c => request.CuisineIds.Contains(c.Id))
                    .ToListAsync(ct);

                foreach (var cuisine in cuisineTypes)
                {
                    db.RestaurantCuisines.Add(new RestaurantCuisine
                    {
                        RestaurantId = restaurant.Id,
                        CuisineId = cuisine.Id
                    });

                    cuisineDtos.Add(new CuisineTypeDto(cuisine.Id, cuisine.Name, cuisine.IconUrl));
                }
            }
        }
        else
        {
            var cuisines = await db.RestaurantCuisines
                .Where(rc => rc.RestaurantId == restaurant.Id)
                .Include(rc => rc.CuisineType)
                .ToListAsync(ct);

            cuisineDtos = cuisines
                .Select(rc => new CuisineTypeDto(rc.CuisineId, rc.CuisineType.Name, rc.CuisineType.IconUrl))
                .ToList();
        }

        await db.SaveChangesAsync(ct);
        await publisher.Publish(new RestaurantIndexRequested(restaurant.Id), ct);

        var dto = new RestaurantDto(
            restaurant.Id, restaurant.Name, restaurant.Slug, restaurant.Description,
            restaurant.PhoneNumber, restaurant.Email, restaurant.LogoUrl, restaurant.BannerUrl,
            restaurant.AddressLine1, restaurant.AddressLine2, restaurant.City, restaurant.State, restaurant.PostalCode,
            restaurant.Latitude, restaurant.Longitude,
            restaurant.AverageRating, restaurant.TotalRatings,
            restaurant.AvgDeliveryTimeMin, restaurant.AvgCostForTwo,
            restaurant.IsVegOnly, restaurant.IsAcceptingOrders, restaurant.IsDineInEnabled,
            restaurant.FssaiLicense, restaurant.GstNumber,
            restaurant.Status.ToString(), restaurant.CreatedAt,
            cuisineDtos);

        return Result<RestaurantDto>.Success(dto);
    }
}
