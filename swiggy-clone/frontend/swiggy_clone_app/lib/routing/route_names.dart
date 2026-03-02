/// Centralised route path constants used by [GoRouter] and navigation helpers.
///
/// Using constants avoids typos and makes rename-refactors trivial.
abstract final class RouteNames {
  // ─────────────────────── Root ─────────────────────────────────

  /// The initial splash / loading screen.
  static const String splash = '/splash';

  // ─────────────────────── Auth ─────────────────────────────────

  static const String login = '/login';
  static const String signUp = '/sign-up';
  static const String otpVerification = '/otp-verification';
  static const String onboarding = '/onboarding';

  // ─────────────────────── Main Shell ───────────────────────────

  /// Bottom-navigation shell route.
  static const String main = '/';

  /// Home tab.
  static const String home = '/home';

  /// Search tab.
  static const String search = '/search';

  /// Cart tab.
  static const String cart = '/cart';

  /// Dine-In tab.
  static const String dineIn = '/dine-in';

  /// Account / Profile tab.
  static const String account = '/account';

  // ─────────────────────── Restaurants (Customer) ──────────────

  /// Restaurant detail. Expects a path parameter `:restaurantId`.
  static const String restaurantDetail = '/restaurant/:restaurantId';

  /// Menu item detail. Nested under restaurant.
  static const String menuItemDetail =
      '/restaurant/:restaurantId/item/:itemId';

  // ─────────────────────── Restaurant Management (Owner) ──────

  /// My restaurants list.
  static const String myRestaurants = '/my-restaurants';

  /// Register a new restaurant.
  static const String registerRestaurant = '/my-restaurants/register';

  /// Restaurant dashboard. Expects `:restaurantId`.
  static const String restaurantDashboard =
      '/my-restaurants/:restaurantId/dashboard';

  /// Edit restaurant profile.
  static const String editRestaurant =
      '/my-restaurants/:restaurantId/edit';

  /// Menu categories for a restaurant.
  static const String menuCategories =
      '/my-restaurants/:restaurantId/menu-categories';

  /// Menu items for a category.
  static const String menuItems =
      '/my-restaurants/:restaurantId/menu-categories/:categoryId/items';

  /// Menu item create/edit form.
  static const String menuItemForm =
      '/my-restaurants/:restaurantId/menu-item-form';

  /// Operating hours management.
  static const String operatingHours =
      '/my-restaurants/:restaurantId/operating-hours';

  /// Document & image uploads.
  static const String documentUpload =
      '/my-restaurants/:restaurantId/documents';

  /// Table management for a restaurant.
  static const String tableManagement =
      '/my-restaurants/:restaurantId/tables';

  /// Active dine-in sessions for a restaurant.
  static const String activeSessions =
      '/my-restaurants/:restaurantId/active-sessions';

  /// Dine-in orders for a restaurant.
  static const String dineInOrders =
      '/my-restaurants/:restaurantId/dine-in-orders';

  // ─────────────────────── Orders ───────────────────────────────

  static const String orders = '/orders';
  static const String orderDetail = '/orders/:orderId';
  static const String orderTracking = '/orders/:orderId/tracking';

  // ─────────────────────── Notifications ────────────────────────

  static const String notifications = '/notifications';

  // ─────────────────────── Delivery Partner ─────────────────────

  static const String deliveryDashboard = '/delivery-dashboard';
  static const String activeDelivery = '/delivery-dashboard/active';

  // ─────────────────────── Admin ────────────────────────────────

  static const String adminDashboard = '/admin';
  static const String adminUsers = '/admin/users';
  static const String adminUserDetail = '/admin/users/:userId';
  static const String adminRestaurants = '/admin/restaurants';
  static const String adminRestaurantDetail =
      '/admin/restaurants/:restaurantId';
  static const String adminOrders = '/admin/orders';
  static const String adminOrderDetail = '/admin/orders/:orderId';

  // ─────────────────────── Admin Banners & Config ─────────────

  static const String adminBanners = '/admin/banners';
  static const String adminBannerForm = '/admin/banners/form';
  static const String adminConfig = '/admin/config';

  // ─────────────────────── Analytics ───────────────────────────

  static const String adminAnalytics = '/admin/analytics';
  static const String restaurantAnalytics =
      '/my-restaurants/:restaurantId/analytics';
  static const String deliveryAnalytics = '/delivery-dashboard/analytics';

  // ─────────────────────── Advanced Analytics ────────────────────
  static const String restaurantInsights =
      '/my-restaurants/:restaurantId/insights';
  static const String restaurantForecast =
      '/my-restaurants/:restaurantId/forecast';
  static const String adminFunnel = '/admin/analytics/funnel';
  static const String adminForecast = '/admin/analytics/forecast';

  // ─────────────────────── Reviews ──────────────────────────────

  static const String submitReview = '/orders/:orderId/review';
  static const String reviewAnalytics =
      '/my-restaurants/:restaurantId/review-analytics';

  // ─────────────────────── Checkout & Payment ───────────────────

  static const String checkout = '/checkout';
  static const String payment = '/payment';
  static const String orderSuccess = '/order-success';

  // ─────────────────────── Dine-In Details ──────────────────────

  static const String qrScanner = '/dine-in/scan';
  static const String dineInSession = '/dine-in/session/:sessionId';
  static const String dineInMenu = '/dine-in/session/:sessionId/menu';
  static const String dineInSessionOrders = '/dine-in/session/:sessionId/orders';
  static const String dineInBill = '/dine-in/session/:sessionId/bill';

  // ─────────────────────── Wallet ───────────────────────────────

  static const String wallet = '/wallet';
  static const String walletAddMoney = '/wallet/add-money';

  // ─────────────────────── Subscriptions ───────────────────────────
  static const String subscriptions = '/subscriptions';

  // ─────────────────────── Referral ──────────────────────────────
  static const String referral = '/account/referral';

  // ─────────────────────── Chat Support ──────────────────────────
  static const String chatTickets = '/account/chat';
  static const String chatConversation = '/account/chat/:ticketId';
  static const String newChatTicket = '/account/chat/new';

  // ─────────────────────── Social ───────────────────────────────
  static const String activityFeed = '/social/feed';
  static const String userProfile = '/social/profile/:userId';
  static const String followers = '/social/:userId/followers';
  static const String following = '/social/:userId/following';

  // ─────────────────────── Promotions (Owner) ──────────────────
  static const String promotionsList = '/promotions';
  static const String createPromotion = '/promotions/create';
  static const String editPromotion = '/promotions/:promotionId/edit';

  // ─────────────────────── Language ─────────────────────────────
  static const String language = '/account/language';

  // ─────────────────────── Loyalty ────────────────────────────
  static const String loyalty = '/account/loyalty';
  static const String loyaltyRewards = '/account/loyalty/rewards';
  static const String loyaltyHistory = '/account/loyalty/history';

  // ─────────────────────── Disputes ──────────────────────────
  static const String disputes = '/account/disputes';
  static const String createDispute = '/account/disputes/create';
  static const String disputeDetail = '/account/disputes/:disputeId';

  // ─────────────────────── Dietary ───────────────────────────
  static const String dietaryProfile = '/account/dietary-profile';

  // ─────────────────────── Group Order ─────────────────────────
  static const String groupOrderCreate = '/group-order/create/:restaurantId';
  static const String groupOrderLobby = '/group-order/:groupOrderId';
  static const String groupOrderMenu = '/group-order/:groupOrderId/menu/:restaurantId';
  static const String groupOrderCheckout = '/group-order/:groupOrderId/checkout';

  // ─────────────────────── A/B Testing (Admin) ─────────────────
  static const String adminExperiments = '/admin/experiments';
  static const String adminCreateExperiment = '/admin/experiments/create';
  static const String adminExperimentDetail =
      '/admin/experiments/:experimentId';
  static const String adminExperimentResults =
      '/admin/experiments/:experimentId/results';

  // ─────────────────────── Profile / Settings ───────────────────

  static const String editProfile = '/account/edit';
  static const String addresses = '/account/addresses';
  static const String addAddress = '/account/addresses/add';
  static const String editAddress = '/account/addresses/:addressId/edit';
  static const String favourites = '/account/favourites';
  static const String settings = '/account/settings';
  static const String helpSupport = '/account/help';
  static const String changePassword = '/account/change-password';
  static const String accountSessions = '/account/sessions';

  // ─────────────────────── Helpers ──────────────────────────────

  /// Builds a restaurant detail path by substituting the [id].
  static String restaurantDetailPath(String id) => '/restaurant/$id';

  /// Builds a menu item detail path.
  static String menuItemDetailPath({
    required String restaurantId,
    required String itemId,
  }) =>
      '/restaurant/$restaurantId/item/$itemId';

  /// Builds an order detail path by substituting the [orderId].
  static String orderDetailPath(String orderId) => '/orders/$orderId';

  /// Builds an order tracking path.
  static String orderTrackingPath(String orderId) =>
      '/orders/$orderId/tracking';

  /// Builds a dine-in session path.
  static String dineInSessionPath(String sessionId) =>
      '/dine-in/session/$sessionId';

  /// Builds a dine-in menu path.
  static String dineInMenuPath(String sessionId) =>
      '/dine-in/session/$sessionId/menu';

  /// Builds a dine-in session orders path.
  static String dineInSessionOrdersPath(String sessionId) =>
      '/dine-in/session/$sessionId/orders';

  /// Builds a dine-in bill path.
  static String dineInBillPath(String sessionId) =>
      '/dine-in/session/$sessionId/bill';

  /// Builds an edit-address path.
  static String editAddressPath(String addressId) =>
      '/account/addresses/$addressId/edit';

  // ─────────────────── Restaurant Management Helpers ──────────

  /// Builds a restaurant dashboard path.
  static String restaurantDashboardPath(String id) =>
      '/my-restaurants/$id/dashboard';

  /// Builds an edit restaurant path.
  static String editRestaurantPath(String id) =>
      '/my-restaurants/$id/edit';

  /// Builds a menu categories path.
  static String menuCategoriesPath(String id) =>
      '/my-restaurants/$id/menu-categories';

  /// Builds a menu items path for a category.
  static String menuItemsPath(String restaurantId, String categoryId) =>
      '/my-restaurants/$restaurantId/menu-categories/$categoryId/items';

  /// Builds a menu item form path (create/edit).
  static String menuItemFormPath(String restaurantId) =>
      '/my-restaurants/$restaurantId/menu-item-form';

  /// Builds an operating hours path.
  static String operatingHoursPath(String id) =>
      '/my-restaurants/$id/operating-hours';

  /// Builds a document upload path.
  static String documentUploadPath(String id) =>
      '/my-restaurants/$id/documents';

  /// Builds a table management path.
  static String tableManagementPath(String id) =>
      '/my-restaurants/$id/tables';

  /// Builds an active sessions path.
  static String activeSessionsPath(String id) =>
      '/my-restaurants/$id/active-sessions';

  /// Builds a dine-in orders path.
  static String dineInOrdersPath(String id) =>
      '/my-restaurants/$id/dine-in-orders';

  /// Builds a restaurant analytics path.
  static String restaurantAnalyticsPath(String id) =>
      '/my-restaurants/$id/analytics';

  /// Builds a submit review path.
  static String submitReviewPath(String orderId) =>
      '/orders/$orderId/review';

  /// Builds a review analytics path.
  static String reviewAnalyticsPath(String restaurantId) =>
      '/my-restaurants/$restaurantId/review-analytics';

  /// Builds a restaurant insights path.
  static String restaurantInsightsPath(String id) =>
      '/my-restaurants/$id/insights';

  /// Builds a restaurant forecast path.
  static String restaurantForecastPath(String id) =>
      '/my-restaurants/$id/forecast';

  // ─────────────────── Social Helpers ───────────────────────────

  /// Builds a user profile path.
  static String userProfilePath(String userId) => '/social/profile/$userId';

  /// Builds a followers path.
  static String followersPath(String userId) => '/social/$userId/followers';

  /// Builds a following path.
  static String followingPath(String userId) => '/social/$userId/following';

  // ─────────────────── Promotion Helpers ────────────────────────

  /// Builds an edit promotion path by substituting the [id].
  static String editPromotionPath(String id) => '/promotions/$id/edit';

  // ─────────────────── Group Order Helpers ──────────────────────

  /// Builds a group order create path by substituting the [restaurantId].
  static String groupOrderCreatePath(String restaurantId) =>
      '/group-order/create/$restaurantId';

  /// Builds a group order lobby path by substituting the [groupOrderId].
  static String groupOrderLobbyPath(String groupOrderId) =>
      '/group-order/$groupOrderId';

  /// Builds a group order menu path.
  static String groupOrderMenuPath(String groupOrderId, String restaurantId) =>
      '/group-order/$groupOrderId/menu/$restaurantId';

  /// Builds a group order checkout path by substituting the [groupOrderId].
  static String groupOrderCheckoutPath(String groupOrderId) =>
      '/group-order/$groupOrderId/checkout';

  // ─────────────────── Dispute Helpers ────────────────────────

  /// Builds a dispute detail path by substituting the [disputeId].
  static String disputeDetailPath(String disputeId) =>
      '/account/disputes/$disputeId';

  // ─────────────────── A/B Testing Helpers ──────────────────────

  /// Builds an admin experiment detail path.
  static String adminExperimentDetailPath(String experimentId) =>
      '/admin/experiments/$experimentId';

  /// Builds an admin experiment results path.
  static String adminExperimentResultsPath(String experimentId) =>
      '/admin/experiments/$experimentId/results';

  // ─────────────────── Chat Support Helpers ──────────────────────

  /// Builds a chat conversation path by substituting the [ticketId].
  static String chatConversationPath(String ticketId) =>
      '/account/chat/$ticketId';

  // ─────────────────── Admin Helpers ────────────────────────────

  /// Builds an admin user detail path.
  static String adminUserDetailPath(String userId) =>
      '/admin/users/$userId';

  /// Builds an admin restaurant detail path.
  static String adminRestaurantDetailPath(String restaurantId) =>
      '/admin/restaurants/$restaurantId';

  /// Builds an admin order detail path.
  static String adminOrderDetailPath(String orderId) =>
      '/admin/orders/$orderId';
}
