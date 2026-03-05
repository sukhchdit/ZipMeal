using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Application.Features.Reviews.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands;

internal sealed class SubmitReviewCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<SubmitReviewCommand, Result<ReviewDto>>
{
    public async Task<Result<ReviewDto>> Handle(SubmitReviewCommand request, CancellationToken ct)
    {
        // 1. Verify order belongs to user and is delivered
        var order = await db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, ct);

        if (order is null)
            return Result<ReviewDto>.Failure("ORDER_NOT_FOUND", "Order not found.");

        if (order.Status != OrderStatus.Delivered)
            return Result<ReviewDto>.Failure("ORDER_NOT_DELIVERED", "You can only review delivered orders.");

        // 2. Check no existing review
        var existingReview = await db.Reviews.AnyAsync(r => r.OrderId == request.OrderId, ct);
        if (existingReview)
            return Result<ReviewDto>.Failure("REVIEW_EXISTS", "A review already exists for this order.");

        // 3. Create review
        var review = new Review
        {
            Id = Guid.CreateVersion7(),
            OrderId = request.OrderId,
            UserId = request.UserId,
            RestaurantId = order.RestaurantId,
            Rating = request.Rating,
            ReviewText = request.ReviewText,
            DeliveryRating = request.DeliveryRating,
            IsAnonymous = request.IsAnonymous,
            IsVisible = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        // Add photos
        for (var i = 0; i < request.PhotoUrls.Count; i++)
        {
            review.Photos.Add(new ReviewPhoto
            {
                Id = Guid.CreateVersion7(),
                PhotoUrl = request.PhotoUrls[i],
                SortOrder = i,
                CreatedAt = DateTimeOffset.UtcNow,
            });
        }

        db.Reviews.Add(review);

        // 4. Update restaurant rating atomically
        var restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.Id == order.RestaurantId, ct);
        if (restaurant is not null)
        {
            var newTotal = restaurant.TotalRatings + 1;
            restaurant.AverageRating = ((restaurant.AverageRating * restaurant.TotalRatings) + request.Rating) / newTotal;
            restaurant.TotalRatings = newTotal;
        }

        // 5. Save
        await db.SaveChangesAsync(ct);

        // 6. Publish events
        await publisher.Publish(new RestaurantIndexRequested(order.RestaurantId), ct);
        await publisher.Publish(new ReviewSubmittedNotification(review.Id, request.UserId, order.RestaurantId), ct);

        // 7. Get reviewer info for DTO
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        var dto = new ReviewDto(
            review.Id,
            review.OrderId,
            review.UserId,
            request.IsAnonymous ? "Anonymous" : user?.FullName,
            request.IsAnonymous ? null : user?.AvatarUrl,
            review.RestaurantId,
            review.Rating,
            review.ReviewText,
            review.DeliveryRating,
            review.IsAnonymous,
            review.IsVisible,
            null,
            null,
            review.CreatedAt,
            review.Photos.OrderBy(p => p.SortOrder)
                .Select(p => new ReviewPhotoDto(p.Id, p.PhotoUrl, p.SortOrder))
                .ToList(),
            0,
            null,
            null);

        return Result<ReviewDto>.Success(dto);
    }
}
