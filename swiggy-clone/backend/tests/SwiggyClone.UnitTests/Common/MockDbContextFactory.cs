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
        IList<Coupon>? coupons = null,
        IList<Review>? reviews = null,
        IList<ReviewVote>? reviewVotes = null,
        IList<ReviewReport>? reviewReports = null,
        IList<ReviewPhoto>? reviewPhotos = null,
        IList<GroupOrder>? groupOrders = null,
        IList<GroupOrderParticipant>? groupOrderParticipants = null,
        IList<LoyaltyAccount>? loyaltyAccounts = null,
        IList<LoyaltyTransaction>? loyaltyTransactions = null,
        IList<LoyaltyTier>? loyaltyTiers = null,
        IList<LoyaltyReward>? loyaltyRewards = null,
        IList<Dispute>? disputes = null,
        IList<DisputeMessage>? disputeMessages = null,
        IList<Experiment>? experiments = null,
        IList<ExperimentVariant>? experimentVariants = null,
        IList<UserVariantAssignment>? userVariantAssignments = null,
        IList<ExposureEvent>? exposureEvents = null,
        IList<ConversionEvent>? conversionEvents = null,
        IList<UserInteraction>? userInteractions = null,
        IList<MenuItem>? menuItems = null,
        IList<MenuCategory>? menuCategories = null,
        IList<MenuItemVariant>? menuItemVariants = null,
        IList<MenuItemAddon>? menuItemAddons = null,
        IList<OrderItem>? orderItems = null,
        IList<OrderStatusHistory>? orderStatusHistory = null,
        IList<UserAddress>? userAddresses = null,
        IList<CouponUsage>? couponUsages = null,
        IList<DeliveryAssignment>? deliveryAssignments = null,
        IList<UserSubscription>? userSubscriptions = null,
        IList<SubscriptionPlan>? subscriptionPlans = null,
        IList<UserFollow>? userFollows = null,
        IList<ActivityFeedItem>? activityFeedItems = null,
        IList<RestaurantPromotion>? restaurantPromotions = null,
        IList<PromotionMenuItem>? promotionMenuItems = null,
        IList<SupportTicket>? supportTickets = null,
        IList<SupportMessage>? supportMessages = null,
        IList<CannedResponse>? cannedResponses = null,
        IList<Notification>? notifications = null,
        IList<UserDevice>? userDevices = null,
        IList<Banner>? banners = null,
        IList<PlatformConfig>? platformConfigs = null,
        IList<UserFavorite>? userFavorites = null,
        IList<UserFavoriteItem>? userFavoriteItems = null,
        IList<UserDietaryProfile>? userDietaryProfiles = null)
    {
        var db = Substitute.For<IAppDbContext>();

        // ── Create ALL mock DbSets upfront (before any .Returns() calls) ──
        // NSubstitute anti-pattern: calling Substitute.For inside .Returns() corrupts
        // the "last call" tracker. Creating them as variables first avoids this.

        var usersSet = MockDbSetHelper.CreateMockDbSet(users ?? new List<User>());
        var refreshTokensSet = MockDbSetHelper.CreateMockDbSet(refreshTokens ?? new List<RefreshToken>());
        var restaurantsSet = MockDbSetHelper.CreateMockDbSet(restaurants ?? new List<Restaurant>());
        var ordersSet = MockDbSetHelper.CreateMockDbSet(orders ?? new List<Order>());
        var walletsSet = MockDbSetHelper.CreateMockDbSet(wallets ?? new List<Wallet>());
        var walletTransactionsSet = MockDbSetHelper.CreateMockDbSet(walletTransactions ?? new List<WalletTransaction>());
        var couponsSet = MockDbSetHelper.CreateMockDbSet(coupons ?? new List<Coupon>());
        var reviewsSet = MockDbSetHelper.CreateMockDbSet(reviews ?? new List<Review>());
        var reviewVotesSet = MockDbSetHelper.CreateMockDbSet(reviewVotes ?? new List<ReviewVote>());
        var reviewReportsSet = MockDbSetHelper.CreateMockDbSet(reviewReports ?? new List<ReviewReport>());
        var reviewPhotosDbSet = MockDbSetHelper.CreateMockDbSet(reviewPhotos ?? new List<ReviewPhoto>());
        var groupOrdersSet = MockDbSetHelper.CreateMockDbSet(groupOrders ?? new List<GroupOrder>());
        var groupOrderParticipantsSet = MockDbSetHelper.CreateMockDbSet(groupOrderParticipants ?? new List<GroupOrderParticipant>());
        var loyaltyAccountsSet = MockDbSetHelper.CreateMockDbSet(loyaltyAccounts ?? new List<LoyaltyAccount>());
        var loyaltyTransactionsSet = MockDbSetHelper.CreateMockDbSet(loyaltyTransactions ?? new List<LoyaltyTransaction>());
        var loyaltyTiersSet = MockDbSetHelper.CreateMockDbSet(loyaltyTiers ?? new List<LoyaltyTier>());
        var loyaltyRewardsSet = MockDbSetHelper.CreateMockDbSet(loyaltyRewards ?? new List<LoyaltyReward>());
        var disputesSet = MockDbSetHelper.CreateMockDbSet(disputes ?? new List<Dispute>());
        var disputeMessagesSet = MockDbSetHelper.CreateMockDbSet(disputeMessages ?? new List<DisputeMessage>());
        var experimentsSet = MockDbSetHelper.CreateMockDbSet(experiments ?? new List<Experiment>());
        var experimentVariantsSet = MockDbSetHelper.CreateMockDbSet(experimentVariants ?? new List<ExperimentVariant>());
        var userVariantAssignmentsSet = MockDbSetHelper.CreateMockDbSet(userVariantAssignments ?? new List<UserVariantAssignment>());
        var exposureEventsSet = MockDbSetHelper.CreateMockDbSet(exposureEvents ?? new List<ExposureEvent>());
        var conversionEventsSet = MockDbSetHelper.CreateMockDbSet(conversionEvents ?? new List<ConversionEvent>());
        var userInteractionsSet = MockDbSetHelper.CreateMockDbSet(userInteractions ?? new List<UserInteraction>());
        var menuItemsSet = MockDbSetHelper.CreateMockDbSet(menuItems ?? new List<MenuItem>());
        var menuCategoriesSet = MockDbSetHelper.CreateMockDbSet(menuCategories ?? new List<MenuCategory>());
        var menuItemVariantsSet = MockDbSetHelper.CreateMockDbSet(menuItemVariants ?? new List<MenuItemVariant>());
        var menuItemAddonsSet = MockDbSetHelper.CreateMockDbSet(menuItemAddons ?? new List<MenuItemAddon>());
        var orderItemsSet = MockDbSetHelper.CreateMockDbSet(orderItems ?? new List<OrderItem>());
        var orderItemAddonsSet = MockDbSetHelper.CreateMockDbSet(new List<OrderItemAddon>());
        var orderStatusHistorySet = MockDbSetHelper.CreateMockDbSet(orderStatusHistory ?? new List<OrderStatusHistory>());
        var userAddressesSet = MockDbSetHelper.CreateMockDbSet(userAddresses ?? new List<UserAddress>());
        var couponUsagesSet = MockDbSetHelper.CreateMockDbSet(couponUsages ?? new List<CouponUsage>());
        var deliveryAssignmentsSet = MockDbSetHelper.CreateMockDbSet(deliveryAssignments ?? new List<DeliveryAssignment>());
        var deliveryPartnerLocationsSet = MockDbSetHelper.CreateMockDbSet(new List<DeliveryPartnerLocation>());
        var userSubscriptionsSet = MockDbSetHelper.CreateMockDbSet(userSubscriptions ?? new List<UserSubscription>());
        var subscriptionPlansSet = MockDbSetHelper.CreateMockDbSet(subscriptionPlans ?? new List<SubscriptionPlan>());
        var userFollowsSet = MockDbSetHelper.CreateMockDbSet(userFollows ?? new List<UserFollow>());
        var activityFeedItemsSet = MockDbSetHelper.CreateMockDbSet(activityFeedItems ?? new List<ActivityFeedItem>());
        var restaurantPromotionsSet = MockDbSetHelper.CreateMockDbSet(restaurantPromotions ?? new List<RestaurantPromotion>());
        var promotionMenuItemsSet = MockDbSetHelper.CreateMockDbSet(promotionMenuItems ?? new List<PromotionMenuItem>());
        var supportTicketsSet = MockDbSetHelper.CreateMockDbSet(supportTickets ?? new List<SupportTicket>());
        var supportMessagesSet = MockDbSetHelper.CreateMockDbSet(supportMessages ?? new List<SupportMessage>());
        var cannedResponsesSet = MockDbSetHelper.CreateMockDbSet(cannedResponses ?? new List<CannedResponse>());
        var notificationsSet = MockDbSetHelper.CreateMockDbSet(notifications ?? new List<Notification>());
        var userDevicesSet = MockDbSetHelper.CreateMockDbSet(userDevices ?? new List<UserDevice>());
        var bannersSet = MockDbSetHelper.CreateMockDbSet(banners ?? new List<Banner>());
        var platformConfigsSet = MockDbSetHelper.CreateMockDbSet(platformConfigs ?? new List<PlatformConfig>());
        var userFavoritesSet = MockDbSetHelper.CreateMockDbSet(userFavorites ?? new List<UserFavorite>());
        var userFavoriteItemsSet = MockDbSetHelper.CreateMockDbSet(userFavoriteItems ?? new List<UserFavoriteItem>());
        var userDietaryProfilesSet = MockDbSetHelper.CreateMockDbSet(userDietaryProfiles ?? new List<UserDietaryProfile>());
        var restaurantTablesSet = MockDbSetHelper.CreateMockDbSet(new List<RestaurantTable>());
        var dineInSessionsSet = MockDbSetHelper.CreateMockDbSet(new List<DineInSession>());
        var dineInSessionMembersSet = MockDbSetHelper.CreateMockDbSet(new List<DineInSessionMember>());
        var paymentsSet = MockDbSetHelper.CreateMockDbSet(new List<Payment>());
        var cuisineTypesSet = MockDbSetHelper.CreateMockDbSet(new List<CuisineType>());
        var restaurantCuisinesSet = MockDbSetHelper.CreateMockDbSet(new List<RestaurantCuisine>());
        var operatingHoursSet = MockDbSetHelper.CreateMockDbSet(new List<RestaurantOperatingHours>());

        // ── Wire up all .Returns() calls ──

        db.Users.Returns(usersSet);
        db.RefreshTokens.Returns(refreshTokensSet);
        db.Restaurants.Returns(restaurantsSet);
        db.Orders.Returns(ordersSet);
        db.Wallets.Returns(walletsSet);
        db.WalletTransactions.Returns(walletTransactionsSet);
        db.Coupons.Returns(couponsSet);
        db.Reviews.Returns(reviewsSet);
        db.ReviewVotes.Returns(reviewVotesSet);
        db.ReviewReports.Returns(reviewReportsSet);
        db.GroupOrders.Returns(groupOrdersSet);
        db.GroupOrderParticipants.Returns(groupOrderParticipantsSet);
        db.LoyaltyAccounts.Returns(loyaltyAccountsSet);
        db.LoyaltyTransactions.Returns(loyaltyTransactionsSet);
        db.LoyaltyTiers.Returns(loyaltyTiersSet);
        db.LoyaltyRewards.Returns(loyaltyRewardsSet);
        db.Disputes.Returns(disputesSet);
        db.DisputeMessages.Returns(disputeMessagesSet);
        db.Experiments.Returns(experimentsSet);
        db.ExperimentVariants.Returns(experimentVariantsSet);
        db.UserVariantAssignments.Returns(userVariantAssignmentsSet);
        db.ExposureEvents.Returns(exposureEventsSet);
        db.ConversionEvents.Returns(conversionEventsSet);
        db.UserInteractions.Returns(userInteractionsSet);
        db.MenuItems.Returns(menuItemsSet);
        db.MenuCategories.Returns(menuCategoriesSet);
        db.MenuItemVariants.Returns(menuItemVariantsSet);
        db.MenuItemAddons.Returns(menuItemAddonsSet);
        db.OrderItems.Returns(orderItemsSet);
        db.OrderItemAddons.Returns(orderItemAddonsSet);
        db.OrderStatusHistory.Returns(orderStatusHistorySet);
        db.UserAddresses.Returns(userAddressesSet);
        db.CouponUsages.Returns(couponUsagesSet);
        db.DeliveryAssignments.Returns(deliveryAssignmentsSet);
        db.DeliveryPartnerLocations.Returns(deliveryPartnerLocationsSet);
        db.UserSubscriptions.Returns(userSubscriptionsSet);
        db.SubscriptionPlans.Returns(subscriptionPlansSet);
        db.UserFollows.Returns(userFollowsSet);
        db.ActivityFeedItems.Returns(activityFeedItemsSet);
        db.RestaurantPromotions.Returns(restaurantPromotionsSet);
        db.PromotionMenuItems.Returns(promotionMenuItemsSet);
        db.SupportTickets.Returns(supportTicketsSet);
        db.SupportMessages.Returns(supportMessagesSet);
        db.CannedResponses.Returns(cannedResponsesSet);
        db.Notifications.Returns(notificationsSet);
        db.UserDevices.Returns(userDevicesSet);
        db.Banners.Returns(bannersSet);
        db.PlatformConfigs.Returns(platformConfigsSet);
        db.UserFavorites.Returns(userFavoritesSet);
        db.UserFavoriteItems.Returns(userFavoriteItemsSet);
        db.UserDietaryProfiles.Returns(userDietaryProfilesSet);
        db.RestaurantTables.Returns(restaurantTablesSet);
        db.DineInSessions.Returns(dineInSessionsSet);
        db.DineInSessionMembers.Returns(dineInSessionMembersSet);
        db.Payments.Returns(paymentsSet);
        db.CuisineTypes.Returns(cuisineTypesSet);
        db.RestaurantCuisines.Returns(restaurantCuisinesSet);
        db.RestaurantOperatingHours.Returns(operatingHoursSet);

        db.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        return db;
    }
}
