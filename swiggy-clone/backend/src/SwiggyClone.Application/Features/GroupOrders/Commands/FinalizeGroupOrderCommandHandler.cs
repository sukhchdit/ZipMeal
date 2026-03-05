using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Diagnostics;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Application.Features.Orders.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

internal sealed class FinalizeGroupOrderCommandHandler(
    IAppDbContext db,
    IGroupCartService groupCartService,
    IPublisher publisher)
    : IRequestHandler<FinalizeGroupOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(FinalizeGroupOrderCommand request, CancellationToken ct)
    {
        using var activity = ApplicationDiagnostics.ActivitySource.StartActivity("FinalizeGroupOrder");

        // 1. Validate group order
        var groupOrder = await db.GroupOrders
            .Include(g => g.Participants)
            .FirstOrDefaultAsync(g => g.Id == request.GroupOrderId, ct);

        if (groupOrder is null)
            return Result<OrderDto>.Failure("GROUP_ORDER_NOT_FOUND", "Group order not found.");

        if (groupOrder.Status != GroupOrderStatus.Active)
            return Result<OrderDto>.Failure("GROUP_ORDER_NOT_ACTIVE", "Group order is not active.");

        if (groupOrder.InitiatorUserId != request.UserId)
            return Result<OrderDto>.Failure("NOT_INITIATOR", "Only the initiator can finalize the group order.");

        if (groupOrder.ExpiresAt <= DateTimeOffset.UtcNow)
            return Result<OrderDto>.Failure("GROUP_ORDER_EXPIRED", "This group order has expired.");

        // 2. Verify all active participants are ready
        var activeParticipants = groupOrder.Participants
            .Where(p => p.Status != GroupOrderParticipantStatus.Left)
            .ToList();

        var notReady = activeParticipants
            .Where(p => p.Status == GroupOrderParticipantStatus.Joined && !p.IsInitiator)
            .ToList();

        if (notReady.Count > 0)
            return Result<OrderDto>.Failure("NOT_ALL_READY",
                "Not all participants are ready. Please wait for everyone to mark as ready.");

        // 3. Load combined cart from Redis
        var participantCarts = await groupCartService.GetAllParticipantCartsAsync(request.GroupOrderId, ct);

        if (participantCarts.Count == 0)
            return Result<OrderDto>.Failure("GROUP_CART_EMPTY", "No items in the group cart.");

        // 4. Validate restaurant
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == groupOrder.RestaurantId, ct);

        if (restaurant is null)
            return Result<OrderDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        if (!restaurant.IsAcceptingOrders)
            return Result<OrderDto>.Failure("RESTAURANT_CLOSED", "Restaurant is not currently accepting orders.");

        // 5. Validate delivery address
        var address = await db.UserAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.DeliveryAddressId && a.UserId == request.UserId, ct);

        if (address is null)
            return Result<OrderDto>.Failure("ADDRESS_NOT_FOUND", "Delivery address not found.");

        // 6. Build participant lookup
        var participantLookup = activeParticipants.ToDictionary(p => p.UserId, p => p.Id);

        // 7. Re-validate menu items and recalculate prices
        int subtotal = 0;
        var orderItems = new List<OrderItem>();

        foreach (var (userId, cart) in participantCarts)
        {
            participantLookup.TryGetValue(userId, out var participantId);

            foreach (var cartItem in cart.Items)
            {
                var menuItem = await db.MenuItems
                    .AsNoTracking()
                    .Include(m => m.Variants)
                    .Include(m => m.Addons)
                    .FirstOrDefaultAsync(m => m.Id == cartItem.MenuItemId, ct);

                if (menuItem is null || !menuItem.IsAvailable)
                    return Result<OrderDto>.Failure("ITEM_UNAVAILABLE",
                        $"'{cartItem.ItemName}' is no longer available.");

                var basePrice = menuItem.DiscountedPrice ?? menuItem.Price;
                int variantAdjustment = 0;
                if (cartItem.VariantId is not null)
                {
                    var variant = menuItem.Variants.FirstOrDefault(v => v.Id == cartItem.VariantId);
                    if (variant is not null)
                        variantAdjustment = variant.PriceAdjustment;
                }

                int addonsTotal = 0;
                var orderAddons = new List<OrderItemAddon>();
                foreach (var cartAddon in cartItem.Addons)
                {
                    var addon = menuItem.Addons.FirstOrDefault(a => a.Id == cartAddon.AddonId);
                    if (addon is not null)
                    {
                        addonsTotal += addon.Price * cartAddon.Quantity;
                        orderAddons.Add(new OrderItemAddon
                        {
                            Id = Guid.CreateVersion7(),
                            AddonId = addon.Id,
                            AddonName = addon.Name,
                            Quantity = cartAddon.Quantity,
                            Price = addon.Price,
                            CreatedAt = DateTimeOffset.UtcNow,
                        });
                    }
                }

                var unitPrice = basePrice + variantAdjustment + addonsTotal;
                var itemTotal = unitPrice * cartItem.Quantity;
                subtotal += itemTotal;

                orderItems.Add(new OrderItem
                {
                    Id = Guid.CreateVersion7(),
                    MenuItemId = menuItem.Id,
                    VariantId = cartItem.VariantId,
                    ItemName = menuItem.Name,
                    Quantity = cartItem.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = itemTotal,
                    SpecialInstructions = cartItem.SpecialInstructions,
                    GroupOrderParticipantId = participantId == Guid.Empty ? null : participantId,
                    Status = OrderStatus.Placed,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Addons = orderAddons,
                });
            }
        }

        // 8. Calculate totals
        var platformConfig = await db.PlatformConfigs.AsNoTracking().FirstOrDefaultAsync(ct);
        var taxRate = platformConfig?.TaxRatePercent ?? 5.00m;
        var deliveryFee = platformConfig?.DeliveryFeePaise ?? 4900;
        var packagingCharge = platformConfig?.PackagingChargePaise ?? 1500;

        if (platformConfig?.FreeDeliveryThresholdPaise is { } threshold && subtotal >= threshold)
            deliveryFee = 0;

        var activeSub = await db.UserSubscriptions
            .AsNoTracking()
            .Include(s => s.Plan)
            .Where(s => s.UserId == request.UserId
                && s.Status == SubscriptionStatus.Active
                && s.EndDate > DateTimeOffset.UtcNow)
            .FirstOrDefaultAsync(ct);

        if (activeSub?.Plan.FreeDelivery == true)
            deliveryFee = 0;

        var taxAmount = (int)(subtotal * (taxRate / 100m));

        // Coupon validation
        int discountAmount = 0;
        Coupon? appliedCoupon = null;

        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var couponCode = request.CouponCode.Trim().ToUpperInvariant();
            appliedCoupon = await db.Coupons
                .FirstOrDefaultAsync(c => c.Code == couponCode, ct);

            if (appliedCoupon is null)
                return Result<OrderDto>.Failure("INVALID_COUPON", "Coupon code not found.");

            if (!appliedCoupon.IsActive)
                return Result<OrderDto>.Failure("INVALID_COUPON", "This coupon is no longer active.");

            var now = DateTimeOffset.UtcNow;
            if (now < appliedCoupon.ValidFrom || now > appliedCoupon.ValidUntil)
                return Result<OrderDto>.Failure("INVALID_COUPON", "This coupon has expired.");

            if (appliedCoupon.MaxUsages.HasValue && appliedCoupon.CurrentUsages >= appliedCoupon.MaxUsages.Value)
                return Result<OrderDto>.Failure("INVALID_COUPON", "This coupon has reached its usage limit.");

            if (subtotal < appliedCoupon.MinOrderAmount)
                return Result<OrderDto>.Failure("INVALID_COUPON",
                    $"Minimum order amount is \u20B9{appliedCoupon.MinOrderAmount / 100}.");

            if (appliedCoupon.DiscountType == DiscountType.Percentage)
            {
                discountAmount = (int)(subtotal * (appliedCoupon.DiscountValue / 100.0));
                if (appliedCoupon.MaxDiscount.HasValue && discountAmount > appliedCoupon.MaxDiscount.Value)
                    discountAmount = appliedCoupon.MaxDiscount.Value;
            }
            else
            {
                discountAmount = Math.Min(appliedCoupon.DiscountValue, subtotal);
            }

            appliedCoupon.CurrentUsages++;
        }

        var totalAmount = subtotal + taxAmount + deliveryFee + packagingCharge - discountAmount;

        // 9. Create order
        var orderNumber = $"SWG-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";

        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            OrderNumber = orderNumber,
            UserId = request.UserId,
            RestaurantId = groupOrder.RestaurantId,
            OrderType = OrderType.GroupDelivery,
            Status = OrderStatus.Placed,
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            DeliveryFee = deliveryFee,
            PackagingCharge = packagingCharge,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = (PaymentMethod)request.PaymentMethod,
            CouponId = appliedCoupon?.Id,
            DeliveryAddressId = request.DeliveryAddressId,
            GroupOrderId = groupOrder.Id,
            SpecialInstructions = request.SpecialInstructions,
            EstimatedDeliveryTime = DateTimeOffset.UtcNow.AddMinutes(45),
            Items = orderItems,
            StatusHistory =
            [
                new OrderStatusHistory
                {
                    Id = Guid.CreateVersion7(),
                    Status = OrderStatus.Placed,
                    Notes = "Group order placed by initiator.",
                    ChangedBy = request.UserId,
                    CreatedAt = DateTimeOffset.UtcNow,
                },
            ],
        };

        db.Orders.Add(order);

        // Record coupon usage
        if (appliedCoupon is not null)
        {
            db.CouponUsages.Add(new CouponUsage
            {
                Id = Guid.CreateVersion7(),
                CouponId = appliedCoupon.Id,
                UserId = request.UserId,
                OrderId = order.Id,
                DiscountAmount = discountAmount,
                UsedAt = DateTimeOffset.UtcNow,
            });
        }

        // 10. Create payment record
        var payment = new Payment
        {
            Id = Guid.CreateVersion7(),
            OrderId = order.Id,
            PaymentGateway = request.PaymentMethod == 5 ? "CashOnDelivery" : "Pending",
            Amount = totalAmount,
            Currency = "INR",
            Status = PaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        db.Payments.Add(payment);

        // 11. Update group order status
        groupOrder.Status = GroupOrderStatus.OrderPlaced;
        groupOrder.FinalizedAt = DateTimeOffset.UtcNow;
        groupOrder.OrderId = order.Id;
        groupOrder.DeliveryAddressId = request.DeliveryAddressId;
        groupOrder.SpecialInstructions = request.SpecialInstructions;
        groupOrder.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        // 12. Clear all Redis carts
        await groupCartService.ClearAllCartsAsync(request.GroupOrderId, ct);

        ApplicationDiagnostics.OrdersPlaced.Add(1);
        activity?.SetTag("order.id", order.Id.ToString());
        activity?.SetTag("order.number", order.OrderNumber);

        await publisher.Publish(new GroupOrderFinalizedNotification(
            groupOrder.Id, order.Id, totalAmount), ct);

        await publisher.Publish(new OrderPlacedNotification(
            order.Id, order.UserId, order.RestaurantId, order.OrderNumber, order.TotalAmount), ct);

        // 13. Map to DTO
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
            order.EstimatedDeliveryTime,
            order.ScheduledDeliveryTime,
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
