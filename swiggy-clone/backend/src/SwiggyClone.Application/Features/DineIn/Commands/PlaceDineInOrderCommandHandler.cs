using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.DineIn.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class PlaceDineInOrderCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<PlaceDineInOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(PlaceDineInOrderCommand request, CancellationToken ct)
    {
        // 1. Validate session
        var session = await db.DineInSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SessionId
                && s.Status == DineInSessionStatus.Active, ct);

        if (session is null)
            return Result<OrderDto>.Failure("SESSION_NOT_FOUND",
                "No active dine-in session found.");

        // 2. Validate user is a member
        var isMember = await db.DineInSessionMembers
            .AnyAsync(m => m.SessionId == request.SessionId
                && m.UserId == request.UserId, ct);

        if (!isMember)
            return Result<OrderDto>.Failure("NOT_SESSION_MEMBER",
                "You are not a member of this dine-in session.");

        // 3. Validate restaurant
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == session.RestaurantId, ct);

        if (restaurant is null || !restaurant.IsAcceptingOrders)
            return Result<OrderDto>.Failure("RESTAURANT_CLOSED",
                "Restaurant is not currently accepting orders.");

        // 4. Build order items with price validation
        int subtotal = 0;
        var orderItems = new List<OrderItem>();

        foreach (var input in request.Items)
        {
            var menuItem = await db.MenuItems
                .AsNoTracking()
                .Include(m => m.Variants)
                .Include(m => m.Addons)
                .FirstOrDefaultAsync(m => m.Id == input.MenuItemId
                    && m.Category.RestaurantId == session.RestaurantId, ct);

            if (menuItem is null || !menuItem.IsAvailable)
                return Result<OrderDto>.Failure("ITEM_UNAVAILABLE",
                    $"Menu item is no longer available.");

            var basePrice = menuItem.DiscountedPrice ?? menuItem.Price;
            int variantAdjustment = 0;
            if (input.VariantId is not null)
            {
                var variant = menuItem.Variants.FirstOrDefault(v => v.Id == input.VariantId);
                if (variant is not null)
                    variantAdjustment = variant.PriceAdjustment;
            }

            int addonsTotal = 0;
            var orderAddons = new List<OrderItemAddon>();
            foreach (var addonInput in input.Addons)
            {
                var addon = menuItem.Addons.FirstOrDefault(a => a.Id == addonInput.AddonId);
                if (addon is not null)
                {
                    addonsTotal += addon.Price * addonInput.Quantity;
                    orderAddons.Add(new OrderItemAddon
                    {
                        Id = Guid.CreateVersion7(),
                        AddonId = addon.Id,
                        AddonName = addon.Name,
                        Quantity = addonInput.Quantity,
                        Price = addon.Price,
                        CreatedAt = DateTimeOffset.UtcNow,
                    });
                }
            }

            var unitPrice = basePrice + variantAdjustment + addonsTotal;
            var itemTotal = unitPrice * input.Quantity;
            subtotal += itemTotal;

            orderItems.Add(new OrderItem
            {
                Id = Guid.CreateVersion7(),
                MenuItemId = menuItem.Id,
                VariantId = input.VariantId,
                ItemName = menuItem.Name,
                Quantity = input.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = itemTotal,
                SpecialInstructions = input.SpecialInstructions,
                Status = OrderStatus.Placed,
                CreatedAt = DateTimeOffset.UtcNow,
                Addons = orderAddons,
            });
        }

        // 5. Calculate totals (no delivery fee, no packaging for dine-in)
        var taxAmount = (int)(subtotal * 0.05m); // 5% GST
        var totalAmount = subtotal + taxAmount;

        // 6. Generate order number
        var orderNumber = $"SWG-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";

        // 7. Create order
        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            OrderNumber = orderNumber,
            UserId = request.UserId,
            RestaurantId = session.RestaurantId,
            OrderType = OrderType.DineIn,
            Status = OrderStatus.Placed,
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            DeliveryFee = 0,
            PackagingCharge = 0,
            DiscountAmount = 0,
            TotalAmount = totalAmount,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = Domain.Enums.PaymentMethod.PayAtRestaurant,
            DineInTableId = session.TableId,
            DineInSessionId = session.Id,
            SpecialInstructions = request.SpecialInstructions,
            Items = orderItems,
            StatusHistory =
            [
                new OrderStatusHistory
                {
                    Id = Guid.CreateVersion7(),
                    Status = OrderStatus.Placed,
                    Notes = "Dine-in order placed.",
                    ChangedBy = request.UserId,
                    CreatedAt = DateTimeOffset.UtcNow,
                },
            ],
        };

        db.Orders.Add(order);

        // 8. Create payment record
        var payment = new Payment
        {
            Id = Guid.CreateVersion7(),
            OrderId = order.Id,
            PaymentGateway = "PayAtRestaurant",
            Amount = totalAmount,
            Currency = "INR",
            Status = PaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        db.Payments.Add(payment);

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new DineInOrderPlacedNotification(
            session.Id, order.Id, order.UserId, order.TotalAmount), ct);

        // 9. Map to DTO
        var dto = new OrderDto(
            order.Id,
            order.OrderNumber,
            order.RestaurantId,
            restaurant.Name,
            order.OrderType,
            order.Status,
            order.Subtotal,
            order.TaxAmount,
            order.DeliveryFee,
            order.PackagingCharge,
            order.DiscountAmount,
            order.TotalAmount,
            order.PaymentStatus,
            order.PaymentMethod,
            order.SpecialInstructions,
            null, // No estimated delivery time for dine-in
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.MenuItemId,
                i.ItemName,
                i.VariantId,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice,
                i.SpecialInstructions,
                i.Addons.Select(a => new OrderItemAddonDto(
                    a.AddonId, a.AddonName, a.Quantity, a.Price)).ToList()
            )).ToList(),
            false,
            0,
            false);

        return Result<OrderDto>.Success(dto);
    }
}
