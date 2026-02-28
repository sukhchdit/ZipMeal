using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class RegisterRestaurantCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<RegisterRestaurantCommand, Result<RestaurantDto>>
{
    public async Task<Result<RestaurantDto>> Handle(
        RegisterRestaurantCommand request, CancellationToken ct)
    {
        var slug = SlugHelper.GenerateSlug(request.Name);
        var baseSlug = slug;

        for (var i = 1; i <= 5; i++)
        {
            var exists = await db.Restaurants.AnyAsync(r => r.Slug == slug, ct);
            if (!exists) break;

            if (i == 5)
                return Result<RestaurantDto>.Failure("SLUG_CONFLICT", "Unable to generate a unique slug for this restaurant name.");

            slug = SlugHelper.AppendSuffix(baseSlug, i);
        }

        var restaurant = new Restaurant
        {
            Id = Guid.CreateVersion7(),
            OwnerId = request.OwnerId,
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsVegOnly = request.IsVegOnly,
            AvgCostForTwo = request.AvgCostForTwo,
            Status = RestaurantStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        db.Restaurants.Add(restaurant);

        var cuisineDtos = new List<CuisineTypeDto>();

        if (request.CuisineIds is { Count: > 0 })
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
