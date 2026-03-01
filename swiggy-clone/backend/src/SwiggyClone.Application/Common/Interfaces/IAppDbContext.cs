using Microsoft.EntityFrameworkCore;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Restaurant> Restaurants { get; }
    DbSet<RestaurantOperatingHours> RestaurantOperatingHours { get; }
    DbSet<CuisineType> CuisineTypes { get; }
    DbSet<RestaurantCuisine> RestaurantCuisines { get; }
    DbSet<MenuCategory> MenuCategories { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MenuItemVariant> MenuItemVariants { get; }
    DbSet<MenuItemAddon> MenuItemAddons { get; }
    DbSet<RestaurantTable> RestaurantTables { get; }
    DbSet<DineInSession> DineInSessions { get; }
    DbSet<DineInSessionMember> DineInSessionMembers { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderItemAddon> OrderItemAddons { get; }
    DbSet<OrderStatusHistory> OrderStatusHistory { get; }
    DbSet<Payment> Payments { get; }
    DbSet<UserAddress> UserAddresses { get; }
    DbSet<Coupon> Coupons { get; }
    DbSet<CouponUsage> CouponUsages { get; }
    DbSet<UserFavorite> UserFavorites { get; }
    DbSet<UserFavoriteItem> UserFavoriteItems { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<UserDevice> UserDevices { get; }
    DbSet<DeliveryAssignment> DeliveryAssignments { get; }
    DbSet<DeliveryPartnerLocation> DeliveryPartnerLocations { get; }
    DbSet<Banner> Banners { get; }
    DbSet<PlatformConfig> PlatformConfigs { get; }
    DbSet<Wallet> Wallets { get; }
    DbSet<WalletTransaction> WalletTransactions { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<UserSubscription> UserSubscriptions { get; }
    DbSet<SupportTicket> SupportTickets { get; }
    DbSet<SupportMessage> SupportMessages { get; }
    DbSet<CannedResponse> CannedResponses { get; }
    DbSet<UserFollow> UserFollows { get; }
    DbSet<ActivityFeedItem> ActivityFeedItems { get; }
    DbSet<RestaurantPromotion> RestaurantPromotions { get; }
    DbSet<PromotionMenuItem> PromotionMenuItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
