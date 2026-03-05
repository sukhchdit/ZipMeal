/// Centralised API configuration constants.
///
/// All endpoint paths, timeouts, and base-URL definitions live here so that
/// networking code never contains magic strings or numbers.
abstract final class ApiConstants {
  // ─────────────────────── Base URL ─────────────────────────────

  /// API gateway base URL.
  ///
  /// Override via environment variable or flavour-specific config in CI.
  static const String baseUrl = 'http://localhost:5000';

  /// API version prefix prepended to every endpoint path.
  static const String apiPrefix = '/api/v1';

  // ─────────────────────── Timeouts ─────────────────────────────

  /// TCP connect timeout in milliseconds.
  static const int connectTimeoutMs = 30000;

  /// Response receive timeout in milliseconds.
  static const int receiveTimeoutMs = 30000;

  /// Request send timeout in milliseconds.
  static const int sendTimeoutMs = 30000;

  // ─────────────────────── Auth ─────────────────────────────────

  static const String authLogin = '$apiPrefix/auth/login';
  static const String authLoginPhone = '$apiPrefix/auth/login/phone';
  static const String authLoginEmail = '$apiPrefix/auth/login/email';
  static const String authRegister = '$apiPrefix/auth/register';
  static const String authRegisterEmail = '$apiPrefix/auth/register/email';
  static const String authRefreshToken = '$apiPrefix/auth/token/refresh';
  static const String authLogout = '$apiPrefix/auth/logout';
  static const String authLogoutAll = '$apiPrefix/auth/logout/all';
  static const String authSendOtp = '$apiPrefix/auth/otp/send';
  static const String authVerifyOtp = '$apiPrefix/auth/otp/verify';
  static const String authMe = '$apiPrefix/auth/me';
  static const String authSessions = '$apiPrefix/auth/sessions';
  static const String authChangePassword = '$apiPrefix/auth/me/password';

  // ─────────────────────── User ─────────────────────────────────

  static const String userProfile = '$apiPrefix/users/profile';
  static const String userAddresses = '$apiPrefix/users/addresses';
  static String userAddressById(String id) => '$userAddresses/$id';
  static String userAddressSetDefault(String id) => '$userAddresses/$id/default';
  static const String userFavourites = '$apiPrefix/users/favourites';

  // ─────────────────────── Restaurants ──────────────────────────

  static const String restaurants = '$apiPrefix/restaurants';
  static const String restaurantSearch = '$apiPrefix/restaurants/search';
  static const String restaurantMenu = '$apiPrefix/restaurants'; // /{id}/menu

  // ─────────────────────── Restaurant Management ─────────────────
  static const String restaurantsMy = '$apiPrefix/restaurants/my';
  static String restaurantById(String id) => '$apiPrefix/restaurants/$id';
  static String restaurantAcceptingOrders(String id) =>
      '$apiPrefix/restaurants/$id/accepting-orders';
  static String restaurantDineIn(String id) =>
      '$apiPrefix/restaurants/$id/dine-in';
  static String restaurantDashboard(String id) =>
      '$apiPrefix/restaurants/$id/dashboard';
  static String restaurantUpload(String id, String fileType) =>
      '$apiPrefix/restaurants/$id/upload/$fileType';
  static String restaurantMenuCategories(String id) =>
      '$apiPrefix/restaurants/$id/menu-categories';
  static String restaurantMenuCategory(String id, String categoryId) =>
      '$apiPrefix/restaurants/$id/menu-categories/$categoryId';
  static String restaurantMenuCategoryItems(String id, String categoryId) =>
      '$apiPrefix/restaurants/$id/menu-categories/$categoryId/items';
  static String restaurantMenuItems(String id) =>
      '$apiPrefix/restaurants/$id/menu-items';
  static String restaurantMenuItem(String id, String itemId) =>
      '$apiPrefix/restaurants/$id/menu-items/$itemId';
  static String restaurantMenuItemVariants(String id, String itemId) =>
      '$apiPrefix/restaurants/$id/menu-items/$itemId/variants';
  static String restaurantMenuItemVariant(
          String id, String itemId, String variantId) =>
      '$apiPrefix/restaurants/$id/menu-items/$itemId/variants/$variantId';
  static String restaurantMenuItemAddons(String id, String itemId) =>
      '$apiPrefix/restaurants/$id/menu-items/$itemId/addons';
  static String restaurantMenuItemAddon(
          String id, String itemId, String addonId) =>
      '$apiPrefix/restaurants/$id/menu-items/$itemId/addons/$addonId';
  static String restaurantOperatingHours(String id) =>
      '$apiPrefix/restaurants/$id/operating-hours';

  // ─────────────────────── Restaurant Dine-In Management ──────────

  static String restaurantTables(String id) =>
      '$apiPrefix/restaurants/$id/tables';
  static String restaurantTable(String id, String tableId) =>
      '$apiPrefix/restaurants/$id/tables/$tableId';
  static String restaurantDineInSessions(String id) =>
      '$apiPrefix/restaurants/$id/dine-in-sessions';
  static String restaurantDineInOrders(String id) =>
      '$apiPrefix/restaurants/$id/dine-in-orders';
  static String restaurantDineInOrderStatus(String id, String orderId) =>
      '$apiPrefix/restaurants/$id/dine-in-orders/$orderId/status';

  // ─────────────────────── Favourite Items ──────────────────────

  static const String favouriteItems = '$apiPrefix/favourites/items';
  static String favouriteItemById(String id) => '$favouriteItems/$id';

  // ─────────────────────── Orders ───────────────────────────────

  static const String orders = '$apiPrefix/orders';
  static const String orderTracking = '$apiPrefix/orders'; // /{id}/track
  static String orderReorder(String orderId) => '$orders/$orderId/reorder';

  // ─────────────────────── Cart ─────────────────────────────────

  static const String cart = '$apiPrefix/cart';

  // ─────────────────────── Payments ─────────────────────────────

  static const String payments = '$apiPrefix/payments';
  static const String paymentVerify = '$apiPrefix/payments/verify';
  static const String paymentDineInSession = '$apiPrefix/payments/dine-in-session';
  static String paymentByOrder(String orderId) =>
      '$apiPrefix/payments/$orderId';
  static String paymentRefund(String orderId) =>
      '$apiPrefix/payments/$orderId/refund';

  // ─────────────────────── Wallet ───────────────────────────────

  static const String wallet = '$apiPrefix/wallet';
  static const String walletAddMoney = '$apiPrefix/wallet/add-money';
  static const String walletTransactions = '$apiPrefix/wallet/transactions';

  // ─────────────────────── Subscriptions ──────────────────────────
  static const String subscriptionPlans = '$apiPrefix/subscriptions/plans';
  static const String subscriptionMy = '$apiPrefix/subscriptions/my';
  static const String subscriptionSubscribe = '$apiPrefix/subscriptions/subscribe';
  static const String subscriptionCancel = '$apiPrefix/subscriptions/cancel';
  static const String adminSubscriptionPlans =
      '$apiPrefix/admin/subscription-plans';
  static String adminSubscriptionPlanById(String id) =>
      '$apiPrefix/admin/subscription-plans/$id';
  static String adminSubscriptionPlanToggle(String id) =>
      '$apiPrefix/admin/subscription-plans/$id/toggle';

  // ─────────────────────── Referrals ──────────────────────────────

  static const String referralStats = '$apiPrefix/referrals/stats';

  // ─────────────────────── Dine-In ──────────────────────────────

  static const String dineInSessions = '$apiPrefix/dine-in/sessions';
  static const String dineInSessionsJoin = '$apiPrefix/dine-in/sessions/join';
  static const String dineInSessionsActive =
      '$apiPrefix/dine-in/sessions/active';
  static const String dineInTables = '$apiPrefix/dine-in/tables';

  static String dineInSessionById(String id) =>
      '$apiPrefix/dine-in/sessions/$id';
  static String dineInSessionMenu(String id) =>
      '$apiPrefix/dine-in/sessions/$id/menu';
  static String dineInSessionOrders(String id) =>
      '$apiPrefix/dine-in/sessions/$id/orders';
  static String dineInSessionRequestBill(String id) =>
      '$apiPrefix/dine-in/sessions/$id/request-bill';
  static String dineInSessionEnd(String id) =>
      '$apiPrefix/dine-in/sessions/$id/end';
  static String dineInSessionLeave(String id) =>
      '$apiPrefix/dine-in/sessions/$id/leave';

  // ─────────────────────── Reviews ──────────────────────────────

  static const String reviews = '$apiPrefix/reviews';
  static const String myReviews = '$apiPrefix/reviews/my';
  static String restaurantReviews(String restaurantId) =>
      '$apiPrefix/reviews/restaurant/$restaurantId';

  // Reviews v2
  static const String reviewUploadPhoto = '$apiPrefix/reviews/upload-photo';
  static String reviewVote(String reviewId) => '$reviews/$reviewId/vote';
  static String reviewReport(String reviewId) => '$reviews/$reviewId/report';
  static String reviewReplyDelete(String reviewId) =>
      '$reviews/$reviewId/reply';
  static String reviewAnalytics(String restaurantId) =>
      '$apiPrefix/reviews/restaurant/$restaurantId/analytics';
  static const String adminReviewReports = '$apiPrefix/reviews/admin/reports';
  static String adminReviewReportResolve(String reportId) =>
      '$adminReviewReports/$reportId/resolve';

  // ─────────────────────── Coupons ──────────────────────────────

  static const String couponValidate = '$apiPrefix/coupons/validate';
  static const String adminCoupons = '$apiPrefix/admin/coupons';
  static String adminCouponById(String id) => '$apiPrefix/admin/coupons/$id';
  static String adminCouponToggle(String id) =>
      '$apiPrefix/admin/coupons/$id/toggle';

  // ─────────────────────── Analytics ──────────────────────────────

  static const String adminAnalytics = '$apiPrefix/admin/analytics';
  static String restaurantAnalytics(String id) =>
      '$apiPrefix/restaurants/$id/analytics';
  static const String deliveryAnalytics = '$apiPrefix/deliveries/analytics';

  // Advanced Analytics (Module 47)
  static String restaurantInsights(String id) =>
      '$apiPrefix/restaurants/$id/insights';
  static String restaurantForecast(String id) =>
      '$apiPrefix/restaurants/$id/forecast';
  static const String adminAnalyticsFunnel = '$apiPrefix/admin/analytics/funnel';
  static const String adminAnalyticsForecast =
      '$apiPrefix/admin/analytics/forecast';

  // ─────────────────────── Config ───────────────────────────────

  static const String configFees = '$apiPrefix/config/fees';

  // ─────────────────────── Misc ─────────────────────────────────

  static const String banners = '$apiPrefix/banners';
  static const String cuisines = '$apiPrefix/cuisines';
  static const String notifications = '$apiPrefix/notifications';
  static const String notificationUnreadCount =
      '$apiPrefix/notifications/unread-count';
  static String notificationMarkRead(String id) =>
      '$apiPrefix/notifications/$id/read';
  static const String notificationReadAll =
      '$apiPrefix/notifications/read-all';
  static const String notificationDevices =
      '$apiPrefix/notifications/devices';

  // ─────────────────────── Deliveries ────────────────────────────

  static const String deliveries = '$apiPrefix/deliveries';
  static const String deliveryOnlineStatus =
      '$apiPrefix/deliveries/online-status';
  static const String deliveryActive = '$apiPrefix/deliveries/active';
  static const String deliveryDashboard = '$apiPrefix/deliveries/dashboard';
  static const String deliveryLocation = '$apiPrefix/deliveries/location';
  static String deliveryAccept(String id) =>
      '$apiPrefix/deliveries/$id/accept';
  static String deliveryStatus(String id) =>
      '$apiPrefix/deliveries/$id/status';
  static String deliveryTracking(String orderId) =>
      '$apiPrefix/deliveries/tracking/$orderId';

  // ─────────────────────── Admin ──────────────────────────────

  static const String adminDashboard = '$apiPrefix/admin/dashboard';
  static const String adminUsers = '$apiPrefix/admin/users';
  static String adminUserById(String id) => '$apiPrefix/admin/users/$id';
  static String adminUserActive(String id) =>
      '$apiPrefix/admin/users/$id/active';
  static String adminUserRole(String id) =>
      '$apiPrefix/admin/users/$id/role';
  static const String adminRestaurants = '$apiPrefix/admin/restaurants';
  static String adminRestaurantById(String id) =>
      '$apiPrefix/admin/restaurants/$id';
  static String adminRestaurantApprove(String id) =>
      '$apiPrefix/admin/restaurants/$id/approve';
  static String adminRestaurantReject(String id) =>
      '$apiPrefix/admin/restaurants/$id/reject';
  static String adminRestaurantSuspend(String id) =>
      '$apiPrefix/admin/restaurants/$id/suspend';
  static String adminRestaurantReactivate(String id) =>
      '$apiPrefix/admin/restaurants/$id/reactivate';
  static const String adminOrders = '$apiPrefix/admin/orders';
  static String adminOrderById(String id) => '$apiPrefix/admin/orders/$id';
  static const String adminReindex = '$apiPrefix/admin/reindex';
  static const String adminBanners = '$apiPrefix/admin/banners';
  static String adminBannerById(String id) => '$apiPrefix/admin/banners/$id';
  static String adminBannerToggle(String id) =>
      '$apiPrefix/admin/banners/$id/toggle';
  static const String adminConfig = '$apiPrefix/admin/config';

  // ─────────────────────── Discovery Search ────────────────────

  static const String discoveryBase = '$apiPrefix/discovery';
  static const String discoveryMenuItemSearch =
      '$discoveryBase/menu-items/search';
  static const String discoverySuggestions = '$discoveryBase/suggestions';

  // ─────────────────────── WebSocket (legacy) ──────────────────
  static const String wsOrderTracking = 'ws://localhost:5000/ws/orders';
  static const String wsDineIn = 'ws://localhost:5000/ws/dine-in';

  // ─────────────────────── Chat Support ───────────────────────

  static const String supportTickets = '$apiPrefix/support/tickets';
  static String supportTicketMessages(String ticketId) =>
      '$supportTickets/$ticketId/messages';
  static String supportTicketClose(String ticketId) =>
      '$supportTickets/$ticketId/close';
  static String supportTicketMessagesRead(String ticketId) =>
      '$supportTickets/$ticketId/messages/read';
  static const String supportUnreadCount =
      '$apiPrefix/support/tickets/unread-count';
  static const String cannedResponses = '$apiPrefix/support/canned-responses';

  // ─────────────────────── Social ─────────────────────────────

  static const String socialFeed = '$apiPrefix/social/feed';
  static const String socialProfile = '$apiPrefix/social/profile';
  static String socialProfileById(String userId) => '$socialProfile/$userId';
  static String socialFollow(String userId) =>
      '$apiPrefix/social/follow/$userId';
  static String socialFollowStatus(String userId) =>
      '$apiPrefix/social/follow/$userId/status';
  static String socialFollowers(String userId) =>
      '$apiPrefix/social/$userId/followers';
  static String socialFollowing(String userId) =>
      '$apiPrefix/social/$userId/following';
  static String socialShare(String type, String entityId) =>
      '$apiPrefix/social/share/$type/$entityId';

  // ─────────────────────── Promotions ────────────────────────

  static const String promotions = '$apiPrefix/promotions';
  static String promotionById(String id) => '$promotions/$id';
  static String promotionToggle(String id) => '$promotions/$id/toggle';
  static String restaurantPromotions(String restaurantId) =>
      '$apiPrefix/restaurants/$restaurantId/promotions';
  static const String adminPromotions = '$apiPrefix/admin/promotions';
  static String adminPromotionToggle(String id) =>
      '$adminPromotions/$id/toggle';

  // ─────────────────────── Dietary ───────────────────────────

  static const String dietaryProfile = '$apiPrefix/account/dietary-profile';

  // ─────────────────────── Group Orders ──────────────────────

  static const String groupOrders = '$apiPrefix/group-orders';
  static const String groupOrderJoin = '$groupOrders/join';
  static const String groupOrderActive = '$groupOrders/active';
  static String groupOrderById(String id) => '$groupOrders/$id';
  static String groupOrderReady(String id) => '$groupOrders/$id/ready';
  static String groupOrderFinalize(String id) => '$groupOrders/$id/finalize';
  static String groupOrderCancel(String id) => '$groupOrders/$id/cancel';
  static String groupOrderLeave(String id) => '$groupOrders/$id/leave';
  static String groupOrderCartItems(String id) => '$groupOrders/$id/cart/items';
  static String groupOrderCartItem(String id, String itemId) =>
      '$groupOrders/$id/cart/items/$itemId';
  static String groupOrderCart(String id) => '$groupOrders/$id/cart';

  // ─────────────────────── Loyalty ───────────────────────────

  static const String loyaltyDashboard = '$apiPrefix/loyalty/dashboard';
  static const String loyaltyTransactions = '$apiPrefix/loyalty/transactions';
  static const String loyaltyRewards = '$apiPrefix/loyalty/rewards';
  static String loyaltyRedeem(String rewardId) =>
      '$apiPrefix/loyalty/redeem/$rewardId';
  static const String loyaltyAdminAdjust = '$apiPrefix/loyalty/admin/adjust';

  // ─────────────────────── Disputes ────────────────────────────

  static const String disputes = '$apiPrefix/disputes';
  static String disputeById(String id) => '$disputes/$id';
  static String disputeMessages(String id) => '$disputes/$id/messages';
  static const String adminDisputes = '$apiPrefix/admin/disputes';
  static String adminDisputeAssign(String id) =>
      '$adminDisputes/$id/assign';
  static String adminDisputeResolve(String id) =>
      '$adminDisputes/$id/resolve';
  static String adminDisputeReject(String id) =>
      '$adminDisputes/$id/reject';

  // ─────────────────────── Recommendations ────────────────────
  static const String recommendationsPersonalized =
      '$apiPrefix/recommendations/personalized';
  static const String recommendationsTrending =
      '$apiPrefix/recommendations/trending';
  static String recommendationsSimilarRestaurants(String restaurantId) =>
      '$apiPrefix/recommendations/similar-restaurants/$restaurantId';
  static String recommendationsSimilarItems(String menuItemId) =>
      '$apiPrefix/recommendations/similar-items/$menuItemId';
  static const String recommendationsInteractions =
      '$apiPrefix/recommendations/interactions';

  // ─────────────────────── A/B Testing ───────────────────────

  static const String abTestingExperiments =
      '$apiPrefix/ab-testing/experiments';
  static String abTestingExperimentById(String id) =>
      '$abTestingExperiments/$id';
  static String abTestingActivate(String id) =>
      '$abTestingExperiments/$id/activate';
  static String abTestingPause(String id) =>
      '$abTestingExperiments/$id/pause';
  static String abTestingComplete(String id) =>
      '$abTestingExperiments/$id/complete';
  static String abTestingResults(String id) =>
      '$abTestingExperiments/$id/results';
  static const String abTestingAssignments =
      '$apiPrefix/ab-testing/assignments';
  static const String abTestingExposure = '$apiPrefix/ab-testing/exposure';
  static const String abTestingConversion = '$apiPrefix/ab-testing/conversion';

  // ─────────────────────── SignalR Hubs ───────────────────────
  static const String hubOrderTracking = '$baseUrl/hubs/order-tracking';
  static const String hubDineIn = '$baseUrl/hubs/dine-in';
  static const String hubChatSupport = '$baseUrl/hubs/chat-support';
  static const String hubGroupOrder = '$baseUrl/hubs/group-order';
}
