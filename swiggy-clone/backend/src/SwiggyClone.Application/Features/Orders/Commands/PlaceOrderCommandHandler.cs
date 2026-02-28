using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Diagnostics;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.Orders.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

internal sealed class PlaceOrderCommandHandler(IAppDbContext db, ICartService cartService, IPublisher publisher)
    : IRequestHandler<PlaceOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        using var activity = ApplicationDiagnostics.ActivitySource.StartActivity("PlaceOrder");
        activity?.SetTag("user.id", request.UserId.ToString());

        // 1. Get cart
        var cartResult = await cartService.GetCartAsync(request.UserId, ct);
        if (cartResult.IsFailure)
            return Result<OrderDto>.Failure(cartResult.ErrorCode!, cartResult.ErrorMessage!);

        var cart = cartResult.Value;
        if (cart.Items.Count == 0)
            return Result<OrderDto>.Failure("CART_EMPTY", "Cannot place an order with an empty cart.");

        // 2. Validate restaurant
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == cart.RestaurantId, ct);

        if (restaurant is null)
            return Result<OrderDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        if (!restaurant.IsAcceptingOrders)
            return Result<OrderDto>.Failure("RESTAURANT_CLOSED", "Restaurant is not currently accepting orders.");

        // 3. Validate delivery address
        var address = await db.UserAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.DeliveryAddressId && a.UserId == request.UserId, ct);

        if (address is null)
            return Result<OrderDto>.Failure("ADDRESS_NOT_FOUND", "Delivery address not found.");

        // 4. Re-validate menu items and recalculate prices
        int subtotal = 0;
        var orderItems = new List<OrderItem>();

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
                Status = OrderStatus.Placed,
                CreatedAt = DateTimeOffset.UtcNow,
                Addons = orderAddons,
            });
        }

        // 5. Calculate totals (read fees from platform config)
        var platformConfig = await db.PlatformConfigs.AsNoTracking().FirstOrDefaultAsync(ct);
        var taxRate = platformConfig?.TaxRatePercent ?? 5.00m;
        var deliveryFee = platformConfig?.DeliveryFeePaise ?? 4900;
        var packagingCharge = platformConfig?.PackagingChargePaise ?? 1500;

        // Waive delivery fee if subtotal exceeds free delivery threshold
        if (platformConfig?.FreeDeliveryThresholdPaise is { } threshold && subtotal >= threshold)
            deliveryFee = 0;

        // 5a. Check subscription benefits
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

        // 5b. Coupon validation & discount calculation
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

            if (appliedCoupon.ApplicableOrderTypes.Length > 0 &&
                !appliedCoupon.ApplicableOrderTypes.Contains((short)OrderType.Delivery))
                return Result<OrderDto>.Failure("INVALID_COUPON", "This coupon is not valid for delivery orders.");

            if (appliedCoupon.RestaurantIds is not null && appliedCoupon.RestaurantIds.Length > 0 &&
                !appliedCoupon.RestaurantIds.Contains(cart.RestaurantId))
                return Result<OrderDto>.Failure("INVALID_COUPON", "This coupon is not valid for this restaurant.");

            var userUsageCount = await db.CouponUsages
                .CountAsync(u => u.CouponId == appliedCoupon.Id && u.UserId == request.UserId, ct);
            if (userUsageCount >= appliedCoupon.MaxUsagesPerUser)
                return Result<OrderDto>.Failure("INVALID_COUPON", "You have already used this coupon the maximum number of times.");

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

        // 6. Generate order number
        var orderNumber = $"SWG-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";

        // 7. Create order
        var isScheduled = request.ScheduledDeliveryTime is not null;
        var orderStatus = isScheduled ? OrderStatus.Scheduled : OrderStatus.Placed;
        var estimatedDelivery = isScheduled
            ? request.ScheduledDeliveryTime!.Value.AddMinutes(45)
            : DateTimeOffset.UtcNow.AddMinutes(45);
        var statusNote = isScheduled
            ? $"Order scheduled for {request.ScheduledDeliveryTime:g}"
            : "Order placed by customer.";

        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            OrderNumber = orderNumber,
            UserId = request.UserId,
            RestaurantId = cart.RestaurantId,
            OrderType = OrderType.Delivery,
            Status = orderStatus,
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            DeliveryFee = deliveryFee,
            PackagingCharge = packagingCharge,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = (PaymentMethod)request.PaymentMethod,
            DeliveryAddressId = request.DeliveryAddressId,
            SpecialInstructions = request.SpecialInstructions,
            EstimatedDeliveryTime = estimatedDelivery,
            ScheduledDeliveryTime = request.ScheduledDeliveryTime,
            Items = orderItems,
            StatusHistory =
            [
                new OrderStatusHistory
                {
                    Id = Guid.CreateVersion7(),
                    Status = orderStatus,
                    Notes = statusNote,
                    ChangedBy = request.UserId,
                    CreatedAt = DateTimeOffset.UtcNow,
                },
            ],
        };

        db.Orders.Add(order);

        // 7b. Record coupon usage
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

        // 8. Create payment record
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

        await db.SaveChangesAsync(ct);

        ApplicationDiagnostics.OrdersPlaced.Add(1);
        activity?.SetTag("order.id", order.Id.ToString());
        activity?.SetTag("order.number", order.OrderNumber);

        if (isScheduled)
        {
            await publisher.Publish(new OrderScheduledNotification(
                order.Id, order.UserId, order.OrderNumber, order.TotalAmount,
                request.ScheduledDeliveryTime!.Value), ct);
        }
        else
        {
            await publisher.Publish(new OrderPlacedNotification(
                order.Id, order.UserId, order.RestaurantId, order.OrderNumber, order.TotalAmount), ct);
        }

        // 9. Clear cart
        await cartService.ClearCartAsync(request.UserId, ct);

        // 10. Map to DTO
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
