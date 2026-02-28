using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.UnitTests.Common;

public static class MockDbContextFactory
{
    public static IAppDbContext Create(
        IList<User>? users = null,
        IList<RefreshToken>? refreshTokens = null,
        IList<Restaurant>? restaurants = null,
        IList<Order>? orders = null,
        IList<Wallet>? wallets = null,
        IList<WalletTransaction>? walletTransactions = null,
        IList<Coupon>? coupons = null)
    {
        var db = Substitute.For<IAppDbContext>();

        db.Users.Returns(MockDbSetHelper.CreateMockDbSet(users ?? new List<User>()));
        db.RefreshTokens.Returns(MockDbSetHelper.CreateMockDbSet(refreshTokens ?? new List<RefreshToken>()));
        db.Restaurants.Returns(MockDbSetHelper.CreateMockDbSet(restaurants ?? new List<Restaurant>()));
        db.Orders.Returns(MockDbSetHelper.CreateMockDbSet(orders ?? new List<Order>()));
        db.Wallets.Returns(MockDbSetHelper.CreateMockDbSet(wallets ?? new List<Wallet>()));
        db.WalletTransactions.Returns(MockDbSetHelper.CreateMockDbSet(walletTransactions ?? new List<WalletTransaction>()));
        db.Coupons.Returns(MockDbSetHelper.CreateMockDbSet(coupons ?? new List<Coupon>()));

        // Wire up remaining DbSets with empty collections
        db.RestaurantOperatingHours.Returns(MockDbSetHelper.CreateMockDbSet(new List<RestaurantOperatingHours>()));
        db.CuisineTypes.Returns(MockDbSetHelper.CreateMockDbSet(new List<CuisineType>()));
        db.RestaurantCuisines.Returns(MockDbSetHelper.CreateMockDbSet(new List<RestaurantCuisine>()));
        db.MenuCategories.Returns(MockDbSetHelper.CreateMockDbSet(new List<MenuCategory>()));
        db.MenuItems.Returns(MockDbSetHelper.CreateMockDbSet(new List<MenuItem>()));
        db.MenuItemVariants.Returns(MockDbSetHelper.CreateMockDbSet(new List<MenuItemVariant>()));
        db.MenuItemAddons.Returns(MockDbSetHelper.CreateMockDbSet(new List<MenuItemAddon>()));
        db.RestaurantTables.Returns(MockDbSetHelper.CreateMockDbSet(new List<RestaurantTable>()));
        db.DineInSessions.Returns(MockDbSetHelper.CreateMockDbSet(new List<DineInSession>()));
        db.DineInSessionMembers.Returns(MockDbSetHelper.CreateMockDbSet(new List<DineInSessionMember>()));
        db.OrderItems.Returns(MockDbSetHelper.CreateMockDbSet(new List<OrderItem>()));
        db.OrderItemAddons.Returns(MockDbSetHelper.CreateMockDbSet(new List<OrderItemAddon>()));
        db.OrderStatusHistory.Returns(MockDbSetHelper.CreateMockDbSet(new List<OrderStatusHistory>()));
        db.Payments.Returns(MockDbSetHelper.CreateMockDbSet(new List<Payment>()));
        db.UserAddresses.Returns(MockDbSetHelper.CreateMockDbSet(new List<UserAddress>()));
        db.CouponUsages.Returns(MockDbSetHelper.CreateMockDbSet(new List<CouponUsage>()));
        db.UserFavorites.Returns(MockDbSetHelper.CreateMockDbSet(new List<UserFavorite>()));
        db.Reviews.Returns(MockDbSetHelper.CreateMockDbSet(new List<Review>()));
        db.Notifications.Returns(MockDbSetHelper.CreateMockDbSet(new List<Notification>()));
        db.UserDevices.Returns(MockDbSetHelper.CreateMockDbSet(new List<UserDevice>()));
        db.DeliveryAssignments.Returns(MockDbSetHelper.CreateMockDbSet(new List<DeliveryAssignment>()));
        db.DeliveryPartnerLocations.Returns(MockDbSetHelper.CreateMockDbSet(new List<DeliveryPartnerLocation>()));
        db.Banners.Returns(MockDbSetHelper.CreateMockDbSet(new List<Banner>()));
        db.PlatformConfigs.Returns(MockDbSetHelper.CreateMockDbSet(new List<PlatformConfig>()));
        db.SubscriptionPlans.Returns(MockDbSetHelper.CreateMockDbSet(new List<SubscriptionPlan>()));
        db.UserSubscriptions.Returns(MockDbSetHelper.CreateMockDbSet(new List<UserSubscription>()));

        db.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        return db;
    }
}
