import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../core/storage/secure_storage_service.dart';
import '../features/auth/presentation/screens/login_screen.dart';
import '../features/auth/presentation/screens/otp_verification_screen.dart';
import '../features/auth/presentation/screens/register_screen.dart';
import '../features/restaurant_management/presentation/screens/document_upload_screen.dart';
import '../features/restaurant_management/presentation/screens/menu_categories_screen.dart';
import '../features/restaurant_management/presentation/screens/menu_item_form_screen.dart';
import '../features/restaurant_management/presentation/screens/menu_items_screen.dart';
import '../features/restaurant_management/presentation/screens/operating_hours_screen.dart';
import '../features/restaurant_management/presentation/screens/restaurant_dashboard_screen.dart';
import '../features/restaurant_management/presentation/screens/restaurant_edit_screen.dart';
import '../features/restaurant_management/presentation/screens/restaurant_list_screen.dart';
import '../features/restaurant_management/presentation/screens/restaurant_register_screen.dart';
import '../features/restaurant_management/presentation/screens/table_management_screen.dart';
import '../features/restaurant_management/presentation/screens/active_sessions_screen.dart';
import '../features/restaurant_management/presentation/screens/dine_in_orders_screen.dart';
import '../features/payments/presentation/screens/payment_processing_screen.dart';
import '../features/customer_discovery/presentation/screens/home_screen.dart';
import '../features/customer_discovery/presentation/screens/search_screen.dart';
import '../features/customer_discovery/presentation/screens/restaurant_detail_screen.dart';
import '../features/customer_discovery/presentation/screens/favourites_screen.dart';
import '../features/cart/presentation/screens/cart_screen.dart';
import '../features/orders/presentation/screens/checkout_screen.dart';
import '../features/orders/presentation/screens/order_success_screen.dart';
import '../features/orders/presentation/screens/order_history_screen.dart';
import '../features/orders/presentation/screens/order_detail_screen.dart';
import '../features/dine_in/presentation/screens/dine_in_tab_screen.dart';
import '../features/dine_in/presentation/screens/qr_scanner_screen.dart';
import '../features/dine_in/presentation/screens/dine_in_session_screen.dart';
import '../features/dine_in/presentation/screens/dine_in_menu_screen.dart';
import '../features/dine_in/presentation/screens/session_orders_screen.dart';
import '../features/dine_in/presentation/screens/bill_summary_screen.dart';
import '../features/notifications/presentation/screens/notifications_screen.dart';
import '../features/deliveries/presentation/screens/delivery_dashboard_screen.dart';
import '../features/deliveries/presentation/screens/active_delivery_screen.dart';
import '../features/orders/presentation/screens/order_tracking_screen.dart';
import '../features/reviews/presentation/screens/review_analytics_screen.dart';
import '../features/reviews/presentation/screens/submit_review_screen.dart';
import '../features/addresses/presentation/screens/address_list_screen.dart';
import '../features/addresses/presentation/screens/address_form_screen.dart';
import '../features/auth/presentation/screens/account_screen.dart';
import '../features/auth/presentation/screens/edit_profile_screen.dart';
import '../features/auth/presentation/screens/change_password_screen.dart';
import '../features/auth/presentation/screens/account_sessions_screen.dart';
import '../features/auth/presentation/screens/settings_screen.dart';
import '../features/auth/presentation/screens/help_support_screen.dart';
import '../features/onboarding/presentation/screens/splash_screen.dart';
import '../features/onboarding/presentation/screens/onboarding_screen.dart';
import '../features/analytics/presentation/screens/platform_analytics_screen.dart';
import '../features/analytics/presentation/screens/restaurant_analytics_screen.dart';
import '../features/analytics/presentation/screens/customer_funnel_screen.dart';
import '../features/analytics/presentation/screens/partner_analytics_screen.dart';
import '../features/analytics/presentation/screens/restaurant_insights_screen.dart';
import '../features/analytics/presentation/screens/revenue_forecast_screen.dart';
import '../features/admin/presentation/screens/admin_dashboard_screen.dart';
import '../features/admin/presentation/screens/admin_users_screen.dart';
import '../features/admin/presentation/screens/admin_user_detail_screen.dart';
import '../features/admin/presentation/screens/admin_restaurants_screen.dart';
import '../features/admin/presentation/screens/admin_restaurant_detail_screen.dart';
import '../features/admin/presentation/screens/admin_orders_screen.dart';
import '../features/admin/presentation/screens/admin_order_detail_screen.dart';
import '../features/admin/presentation/screens/admin_banners_screen.dart';
import '../features/admin/presentation/screens/admin_banner_form_screen.dart';
import '../features/admin/presentation/screens/admin_config_screen.dart';
import '../features/admin/data/models/admin_banner_model.dart';
import '../features/wallet/presentation/screens/wallet_screen.dart';
import '../features/wallet/presentation/screens/add_money_screen.dart';
import '../features/subscriptions/presentation/screens/subscription_plans_screen.dart';
import '../features/referral/presentation/screens/referral_screen.dart';
import '../features/chat_support/presentation/screens/tickets_list_screen.dart';
import '../features/chat_support/presentation/screens/chat_conversation_screen.dart';
import '../features/chat_support/presentation/screens/new_ticket_screen.dart';
import '../features/auth/presentation/screens/language_screen.dart';
import '../features/dietary/presentation/screens/dietary_profile_screen.dart';
import '../features/social/presentation/screens/activity_feed_screen.dart';
import '../features/social/presentation/screens/user_profile_screen.dart';
import '../features/social/presentation/screens/followers_screen.dart';
import '../features/promotions/presentation/screens/promotions_list_screen.dart';
import '../features/promotions/presentation/screens/create_promotion_screen.dart';
import '../features/promotions/data/models/promotion_model.dart';
import '../features/group_order/presentation/screens/create_group_order_screen.dart';
import '../features/group_order/presentation/screens/group_order_lobby_screen.dart';
import '../features/group_order/presentation/screens/group_order_menu_screen.dart';
import '../features/group_order/presentation/screens/group_order_checkout_screen.dart';
import '../features/loyalty/presentation/screens/loyalty_dashboard_screen.dart';
import '../features/loyalty/presentation/screens/redeem_rewards_screen.dart';
import '../features/loyalty/presentation/screens/points_history_screen.dart';
import '../features/disputes/presentation/screens/dispute_list_screen.dart';
import '../features/disputes/presentation/screens/create_dispute_screen.dart';
import '../features/disputes/presentation/screens/dispute_detail_screen.dart';
import '../features/ab_testing/presentation/screens/experiments_list_screen.dart';
import '../features/ab_testing/presentation/screens/create_experiment_screen.dart';
import '../features/ab_testing/presentation/screens/experiment_detail_screen.dart';
import '../features/ab_testing/presentation/screens/experiment_results_screen.dart';
import 'route_names.dart';

part 'app_router.g.dart';

// ────────────────────────────────────────────────────────────────────────────
// Router Provider
// ────────────────────────────────────────────────────────────────────────────

/// Provides the application-wide [GoRouter] instance.
///
/// The router is configured with:
/// - A redirect guard ([_authGuard]) that redirects unauthenticated users to
///   the login screen.
/// - A shell route placeholder for the bottom-navigation scaffold.
/// - Sub-routes for each feature area.
@riverpod
GoRouter appRouter(Ref ref) {
  final secureStorage = ref.watch(secureStorageServiceProvider);

  return GoRouter(
    initialLocation: RouteNames.splash,
    debugLogDiagnostics: true,
    navigatorKey: _rootNavigatorKey,
    redirect: (BuildContext context, GoRouterState state) =>
        _authGuard(state, secureStorage),
    routes: [
      // Splash / loading
      GoRoute(
        path: RouteNames.splash,
        name: 'splash',
        builder: (context, state) => const SplashScreen(),
      ),

      // ────────────── Auth Routes ──────────────────────────────
      GoRoute(
        path: RouteNames.login,
        name: 'login',
        builder: (context, state) => const LoginScreen(),
      ),
      GoRoute(
        path: RouteNames.signUp,
        name: 'signUp',
        builder: (context, state) => const RegisterScreen(),
      ),
      GoRoute(
        path: RouteNames.otpVerification,
        name: 'otpVerification',
        builder: (context, state) {
          final extra = state.extra as Map<String, dynamic>?;
          return OtpVerificationScreen(
            phoneNumber: extra?['phoneNumber'] as String? ?? '',
            isLogin: extra?['isLogin'] as bool? ?? true,
            fullName: extra?['fullName'] as String?,
            referralCode: extra?['referralCode'] as String?,
          );
        },
      ),
      GoRoute(
        path: RouteNames.onboarding,
        name: 'onboarding',
        builder: (context, state) => const OnboardingScreen(),
      ),

      // ────────────── Main Shell (Bottom Nav) ──────────────────
      ShellRoute(
        navigatorKey: _shellNavigatorKey,
        builder: (context, state, child) =>
            _MainShellScaffold(child: child),
        routes: [
          GoRoute(
            path: RouteNames.home,
            name: 'home',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: HomeScreen(),
            ),
          ),
          GoRoute(
            path: RouteNames.search,
            name: 'search',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: SearchScreen(),
            ),
          ),
          GoRoute(
            path: RouteNames.cart,
            name: 'cart',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: CartScreen(),
            ),
          ),
          GoRoute(
            path: RouteNames.dineIn,
            name: 'dineIn',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: DineInTabScreen(),
            ),
          ),
          GoRoute(
            path: RouteNames.account,
            name: 'account',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: AccountScreen(),
            ),
          ),
        ],
      ),

      // ────────────── Restaurant Routes ────────────────────────
      GoRoute(
        path: RouteNames.restaurantDetail,
        name: 'restaurantDetail',
        builder: (context, state) {
          final restaurantId = state.pathParameters['restaurantId']!;
          return RestaurantDetailScreen(
            restaurantId: restaurantId,
          );
        },
        routes: [
          GoRoute(
            path: 'item/:itemId',
            name: 'menuItemDetail',
            builder: (context, state) {
              final itemId = state.pathParameters['itemId']!;
              return _PlaceholderPage(title: 'Menu Item $itemId');
            },
          ),
        ],
      ),

      // ────────────── Restaurant Management (Owner) ─────────────
      GoRoute(
        path: RouteNames.myRestaurants,
        name: 'myRestaurants',
        builder: (context, state) => const RestaurantListScreen(),
        routes: [
          GoRoute(
            path: 'register',
            name: 'registerRestaurant',
            builder: (context, state) => const RestaurantRegisterScreen(),
          ),
          GoRoute(
            path: ':restaurantId/dashboard',
            name: 'restaurantDashboard',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return RestaurantDashboardScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/edit',
            name: 'editRestaurant',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return RestaurantEditScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/menu-categories',
            name: 'menuCategories',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return MenuCategoriesScreen(restaurantId: restaurantId);
            },
            routes: [
              GoRoute(
                path: ':categoryId/items',
                name: 'menuItems',
                builder: (context, state) {
                  final restaurantId =
                      state.pathParameters['restaurantId']!;
                  final categoryId =
                      state.pathParameters['categoryId']!;
                  return MenuItemsScreen(
                    restaurantId: restaurantId,
                    categoryId: categoryId,
                  );
                },
              ),
            ],
          ),
          GoRoute(
            path: ':restaurantId/menu-item-form',
            name: 'menuItemForm',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              final extra = state.extra as Map<String, dynamic>?;
              return MenuItemFormScreen(
                restaurantId: restaurantId,
                categoryId: extra?['categoryId'] as String?,
                itemId: extra?['itemId'] as String?,
              );
            },
          ),
          GoRoute(
            path: ':restaurantId/operating-hours',
            name: 'operatingHours',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return OperatingHoursScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/documents',
            name: 'documentUpload',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return DocumentUploadScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/tables',
            name: 'tableManagement',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return TableManagementScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/active-sessions',
            name: 'activeSessions',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return ActiveSessionsScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/dine-in-orders',
            name: 'dineInOrders',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return DineInOrdersScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/analytics',
            name: 'restaurantAnalytics',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return RestaurantAnalyticsScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/review-analytics',
            name: 'reviewAnalytics',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return ReviewAnalyticsScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/insights',
            name: 'restaurantInsights',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return RestaurantInsightsScreen(restaurantId: restaurantId);
            },
          ),
          GoRoute(
            path: ':restaurantId/forecast',
            name: 'restaurantForecast',
            builder: (context, state) {
              final restaurantId = state.pathParameters['restaurantId']!;
              return RevenueForecastScreen(restaurantId: restaurantId);
            },
          ),
        ],
      ),

      // ────────────── Order Routes ─────────────────────────────
      GoRoute(
        path: RouteNames.orders,
        name: 'orders',
        builder: (context, state) => const OrderHistoryScreen(),
        routes: [
          GoRoute(
            path: ':orderId',
            name: 'orderDetail',
            builder: (context, state) {
              final orderId = state.pathParameters['orderId']!;
              return OrderDetailScreen(orderId: orderId);
            },
            routes: [
              GoRoute(
                path: 'tracking',
                name: 'orderTracking',
                builder: (context, state) {
                  final orderId = state.pathParameters['orderId']!;
                  return OrderTrackingScreen(orderId: orderId);
                },
              ),
              GoRoute(
                path: 'review',
                name: 'submitReview',
                builder: (context, state) {
                  final orderId = state.pathParameters['orderId']!;
                  final extra = state.extra as Map<String, dynamic>?;
                  return SubmitReviewScreen(
                    orderId: orderId,
                    restaurantName:
                        extra?['restaurantName'] as String? ?? '',
                  );
                },
              ),
            ],
          ),
        ],
      ),

      // ────────────── Notifications ──────────────────────────
      GoRoute(
        path: RouteNames.notifications,
        name: 'notifications',
        builder: (context, state) => const NotificationsScreen(),
      ),

      // ────────────── Delivery Partner ───────────────────────
      GoRoute(
        path: RouteNames.deliveryDashboard,
        name: 'deliveryDashboard',
        builder: (context, state) => const DeliveryDashboardScreen(),
        routes: [
          GoRoute(
            path: 'active',
            name: 'activeDelivery',
            builder: (context, state) => const ActiveDeliveryScreen(),
          ),
          GoRoute(
            path: 'analytics',
            name: 'deliveryAnalytics',
            builder: (context, state) => const PartnerAnalyticsScreen(),
          ),
        ],
      ),

      // ────────────── Admin Panel ─────────────────────────────
      GoRoute(
        path: RouteNames.adminDashboard,
        name: 'adminDashboard',
        builder: (context, state) => const AdminDashboardScreen(),
        routes: [
          GoRoute(
            path: 'users',
            name: 'adminUsers',
            builder: (context, state) => const AdminUsersScreen(),
            routes: [
              GoRoute(
                path: ':userId',
                name: 'adminUserDetail',
                builder: (context, state) {
                  final userId = state.pathParameters['userId']!;
                  return AdminUserDetailScreen(userId: userId);
                },
              ),
            ],
          ),
          GoRoute(
            path: 'restaurants',
            name: 'adminRestaurants',
            builder: (context, state) => const AdminRestaurantsScreen(),
            routes: [
              GoRoute(
                path: ':restaurantId',
                name: 'adminRestaurantDetail',
                builder: (context, state) {
                  final restaurantId =
                      state.pathParameters['restaurantId']!;
                  return AdminRestaurantDetailScreen(
                    restaurantId: restaurantId,
                  );
                },
              ),
            ],
          ),
          GoRoute(
            path: 'orders',
            name: 'adminOrders',
            builder: (context, state) => const AdminOrdersScreen(),
            routes: [
              GoRoute(
                path: ':orderId',
                name: 'adminOrderDetail',
                builder: (context, state) {
                  final orderId = state.pathParameters['orderId']!;
                  return AdminOrderDetailScreen(orderId: orderId);
                },
              ),
            ],
          ),
          GoRoute(
            path: 'analytics/funnel',
            name: 'adminFunnel',
            builder: (context, state) => const CustomerFunnelScreen(),
          ),
          GoRoute(
            path: 'analytics/forecast',
            name: 'adminForecast',
            builder: (context, state) =>
                const RevenueForecastScreen(),
          ),
          GoRoute(
            path: 'analytics',
            name: 'adminAnalytics',
            builder: (context, state) => const PlatformAnalyticsScreen(),
          ),
          GoRoute(
            path: 'banners',
            name: 'adminBanners',
            builder: (context, state) => const AdminBannersScreen(),
            routes: [
              GoRoute(
                path: 'form',
                name: 'adminBannerForm',
                builder: (context, state) {
                  final banner = state.extra as AdminBannerModel?;
                  return AdminBannerFormScreen(banner: banner);
                },
              ),
            ],
          ),
          GoRoute(
            path: 'config',
            name: 'adminConfig',
            builder: (context, state) => const AdminConfigScreen(),
          ),
          GoRoute(
            path: 'experiments',
            name: 'adminExperiments',
            builder: (context, state) => const ExperimentsListScreen(),
            routes: [
              GoRoute(
                path: 'create',
                name: 'adminCreateExperiment',
                builder: (context, state) => const CreateExperimentScreen(),
              ),
              GoRoute(
                path: ':experimentId',
                name: 'adminExperimentDetail',
                builder: (context, state) {
                  final experimentId =
                      state.pathParameters['experimentId']!;
                  return ExperimentDetailScreen(
                      experimentId: experimentId);
                },
                routes: [
                  GoRoute(
                    path: 'results',
                    name: 'adminExperimentResults',
                    builder: (context, state) {
                      final experimentId =
                          state.pathParameters['experimentId']!;
                      return ExperimentResultsScreen(
                          experimentId: experimentId);
                    },
                  ),
                ],
              ),
            ],
          ),
        ],
      ),

      // ────────────── Checkout & Payment ───────────────────────
      GoRoute(
        path: RouteNames.checkout,
        name: 'checkout',
        builder: (context, state) => const CheckoutScreen(),
      ),
      GoRoute(
        path: RouteNames.payment,
        name: 'payment',
        builder: (context, state) {
          final extra = state.extra as Map<String, dynamic>?;
          return PaymentProcessingScreen(
            orderId: extra?['orderId'] as String?,
            orderNumber: extra?['orderNumber'] as String?,
            sessionId: extra?['sessionId'] as String?,
            paymentMethod: extra?['paymentMethod'] as int? ?? 1,
          );
        },
      ),
      GoRoute(
        path: RouteNames.orderSuccess,
        name: 'orderSuccess',
        builder: (context, state) {
          final extra = state.extra as Map<String, dynamic>?;
          return OrderSuccessScreen(
            orderId: extra?['orderId'] as String? ?? '',
            orderNumber: extra?['orderNumber'] as String? ?? '',
          );
        },
      ),

      // ────────────── Dine-In Detail Routes ────────────────────
      GoRoute(
        path: RouteNames.qrScanner,
        name: 'qrScanner',
        builder: (context, state) => const QrScannerScreen(),
      ),
      GoRoute(
        path: RouteNames.dineInSession,
        name: 'dineInSession',
        builder: (context, state) {
          final sessionId = state.pathParameters['sessionId']!;
          return DineInSessionScreen(sessionId: sessionId);
        },
        routes: [
          GoRoute(
            path: 'menu',
            name: 'dineInMenu',
            builder: (context, state) {
              final sessionId = state.pathParameters['sessionId']!;
              final extra = state.extra as Map<String, dynamic>?;
              final restaurantId = extra?['restaurantId'] as String? ?? '';
              return DineInMenuScreen(
                sessionId: sessionId,
                restaurantId: restaurantId,
              );
            },
          ),
          GoRoute(
            path: 'orders',
            name: 'dineInSessionOrders',
            builder: (context, state) {
              final sessionId = state.pathParameters['sessionId']!;
              return SessionOrdersScreen(sessionId: sessionId);
            },
          ),
          GoRoute(
            path: 'bill',
            name: 'dineInBill',
            builder: (context, state) {
              final sessionId = state.pathParameters['sessionId']!;
              return BillSummaryScreen(sessionId: sessionId);
            },
          ),
        ],
      ),

      // ────────────── Profile / Settings ───────────────────────
      GoRoute(
        path: RouteNames.editProfile,
        name: 'editProfile',
        builder: (context, state) => const EditProfileScreen(),
      ),
      GoRoute(
        path: RouteNames.addresses,
        name: 'addresses',
        builder: (context, state) => const AddressListScreen(),
        routes: [
          GoRoute(
            path: 'add',
            name: 'addAddress',
            builder: (context, state) => const AddressFormScreen(),
          ),
          GoRoute(
            path: ':addressId/edit',
            name: 'editAddress',
            builder: (context, state) {
              final addressId = state.pathParameters['addressId']!;
              return AddressFormScreen(addressId: addressId);
            },
          ),
        ],
      ),
      GoRoute(
        path: RouteNames.favourites,
        name: 'favourites',
        builder: (context, state) => const FavouritesScreen(),
      ),
      GoRoute(
        path: RouteNames.settings,
        name: 'settings',
        builder: (context, state) => const SettingsScreen(),
      ),
      GoRoute(
        path: RouteNames.helpSupport,
        name: 'helpSupport',
        builder: (context, state) => const HelpSupportScreen(),
      ),
      GoRoute(
        path: RouteNames.changePassword,
        name: 'changePassword',
        builder: (context, state) => const ChangePasswordScreen(),
      ),
      GoRoute(
        path: RouteNames.accountSessions,
        name: 'accountSessions',
        builder: (context, state) => const AccountSessionsScreen(),
      ),

      // Wallet
      GoRoute(
        path: RouteNames.wallet,
        name: 'wallet',
        builder: (context, state) => const WalletScreen(),
        routes: [
          GoRoute(
            path: 'add-money',
            name: 'walletAddMoney',
            builder: (context, state) => const AddMoneyScreen(),
          ),
        ],
      ),

      // Subscriptions
      GoRoute(
        path: RouteNames.subscriptions,
        name: 'subscriptions',
        builder: (context, state) => const SubscriptionPlansScreen(),
      ),

      // Referral
      GoRoute(
        path: RouteNames.referral,
        name: 'referral',
        builder: (context, state) => const ReferralScreen(),
      ),

      // Language
      GoRoute(
        path: RouteNames.language,
        name: 'language',
        builder: (context, state) => const LanguageScreen(),
      ),

      // Loyalty
      GoRoute(
        path: RouteNames.loyalty,
        name: 'loyalty',
        builder: (context, state) => const LoyaltyDashboardScreen(),
        routes: [
          GoRoute(
            path: 'rewards',
            name: 'loyaltyRewards',
            builder: (context, state) => const RedeemRewardsScreen(),
          ),
          GoRoute(
            path: 'history',
            name: 'loyaltyHistory',
            builder: (context, state) => const PointsHistoryScreen(),
          ),
        ],
      ),

      // Dietary Profile
      GoRoute(
        path: RouteNames.dietaryProfile,
        name: 'dietaryProfile',
        builder: (context, state) => const DietaryProfileScreen(),
      ),

      // Social
      GoRoute(
        path: RouteNames.activityFeed,
        name: 'activityFeed',
        builder: (context, state) => const ActivityFeedScreen(),
      ),
      GoRoute(
        path: RouteNames.userProfile,
        name: 'userProfile',
        builder: (context, state) {
          final userId = state.pathParameters['userId']!;
          return UserProfileScreen(userId: userId);
        },
      ),
      GoRoute(
        path: RouteNames.followers,
        name: 'followers',
        builder: (context, state) {
          final userId = state.pathParameters['userId']!;
          return FollowersScreen(userId: userId, isFollowers: true);
        },
      ),
      GoRoute(
        path: RouteNames.following,
        name: 'following',
        builder: (context, state) {
          final userId = state.pathParameters['userId']!;
          return FollowersScreen(userId: userId, isFollowers: false);
        },
      ),

      // Promotions (Owner)
      GoRoute(
        path: RouteNames.promotionsList,
        name: 'promotionsList',
        builder: (context, state) => const PromotionsListScreen(),
        routes: [
          GoRoute(
            path: 'create',
            name: 'createPromotion',
            builder: (context, state) => const CreatePromotionScreen(),
          ),
          GoRoute(
            path: ':promotionId/edit',
            name: 'editPromotion',
            builder: (context, state) {
              final promotion = state.extra as PromotionModel?;
              return CreatePromotionScreen(promotion: promotion);
            },
          ),
        ],
      ),

      // Group Order
      GoRoute(
        path: RouteNames.groupOrderCreate,
        name: 'groupOrderCreate',
        builder: (context, state) {
          final restaurantId = state.pathParameters['restaurantId']!;
          return CreateGroupOrderScreen(restaurantId: restaurantId);
        },
      ),
      GoRoute(
        path: RouteNames.groupOrderLobby,
        name: 'groupOrderLobby',
        builder: (context, state) {
          final groupOrderId = state.pathParameters['groupOrderId']!;
          return GroupOrderLobbyScreen(groupOrderId: groupOrderId);
        },
        routes: [
          GoRoute(
            path: 'menu/:restaurantId',
            name: 'groupOrderMenu',
            builder: (context, state) {
              final groupOrderId = state.pathParameters['groupOrderId']!;
              final restaurantId = state.pathParameters['restaurantId']!;
              return GroupOrderMenuScreen(
                groupOrderId: groupOrderId,
                restaurantId: restaurantId,
              );
            },
          ),
          GoRoute(
            path: 'checkout',
            name: 'groupOrderCheckout',
            builder: (context, state) {
              final groupOrderId = state.pathParameters['groupOrderId']!;
              return GroupOrderCheckoutScreen(groupOrderId: groupOrderId);
            },
          ),
        ],
      ),

      // Disputes
      GoRoute(
        path: RouteNames.disputes,
        name: 'disputes',
        builder: (context, state) => const DisputeListScreen(),
        routes: [
          GoRoute(
            path: 'create',
            name: 'createDispute',
            builder: (context, state) => const CreateDisputeScreen(),
          ),
          GoRoute(
            path: ':disputeId',
            name: 'disputeDetail',
            builder: (context, state) {
              final disputeId = state.pathParameters['disputeId']!;
              return DisputeDetailScreen(disputeId: disputeId);
            },
          ),
        ],
      ),

      // Chat Support
      GoRoute(
        path: RouteNames.chatTickets,
        name: 'chatTickets',
        builder: (context, state) => const TicketsListScreen(),
        routes: [
          GoRoute(
            path: 'new',
            name: 'newChatTicket',
            builder: (context, state) => const NewTicketScreen(),
          ),
          GoRoute(
            path: ':ticketId',
            name: 'chatConversation',
            builder: (context, state) {
              final ticketId = state.pathParameters['ticketId']!;
              return ChatConversationScreen(ticketId: ticketId);
            },
          ),
        ],
      ),
    ],
    errorBuilder: (context, state) => _ErrorPage(error: state.error),
  );
}

// ────────────────────────────────────────────────────────────────────────────
// Navigator Keys
// ────────────────────────────────────────────────────────────────────────────

final GlobalKey<NavigatorState> _rootNavigatorKey =
    GlobalKey<NavigatorState>(debugLabel: 'root');
final GlobalKey<NavigatorState> _shellNavigatorKey =
    GlobalKey<NavigatorState>(debugLabel: 'shell');

// ────────────────────────────────────────────────────────────────────────────
// Auth Guard
// ────────────────────────────────────────────────────────────────────────────

/// Route-level redirect that enforces authentication.
///
/// - Unauthenticated users accessing protected routes are sent to [login].
/// - Authenticated users accessing auth routes are sent to [home].
/// - The splash screen is always accessible.
Future<String?> _authGuard(
  GoRouterState state,
  SecureStorageService secureStorage,
) async {
  final currentPath = state.matchedLocation;

  // Allow splash to load without checks.
  if (currentPath == RouteNames.splash) return null;

  final token = await secureStorage.getAccessToken();
  final isAuthenticated = token != null && token.isNotEmpty;

  const authPaths = <String>[
    RouteNames.login,
    RouteNames.signUp,
    RouteNames.otpVerification,
    RouteNames.onboarding,
  ];
  final isAuthRoute = authPaths.contains(currentPath);

  // Unauthenticated user trying to access a protected page.
  if (!isAuthenticated && !isAuthRoute) {
    return RouteNames.login;
  }

  // Authenticated user trying to access an auth page (login, signup, etc.).
  if (isAuthenticated && isAuthRoute) {
    return RouteNames.home;
  }

  // No redirect needed.
  return null;
}

// ────────────────────────────────────────────────────────────────────────────
// Shell Scaffold (Bottom Navigation Placeholder)
// ────────────────────────────────────────────────────────────────────────────

/// Placeholder bottom-navigation scaffold.
///
/// Replace with the real implementation once feature screens are built.
class _MainShellScaffold extends StatelessWidget {
  const _MainShellScaffold({required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context) => Scaffold(
        body: child,
        bottomNavigationBar: NavigationBar(
          selectedIndex: _selectedIndex(GoRouterState.of(context)),
          onDestinationSelected: (index) =>
              _onTabSelected(context, index),
          destinations: const [
            NavigationDestination(
              icon: Icon(Icons.home_outlined),
              selectedIcon: Icon(Icons.home_rounded),
              label: 'Home',
            ),
            NavigationDestination(
              icon: Icon(Icons.search_outlined),
              selectedIcon: Icon(Icons.search_rounded),
              label: 'Search',
            ),
            NavigationDestination(
              icon: Icon(Icons.shopping_cart_outlined),
              selectedIcon: Icon(Icons.shopping_cart_rounded),
              label: 'Cart',
            ),
            NavigationDestination(
              icon: Icon(Icons.restaurant_outlined),
              selectedIcon: Icon(Icons.restaurant_rounded),
              label: 'Dine-In',
            ),
            NavigationDestination(
              icon: Icon(Icons.person_outline_rounded),
              selectedIcon: Icon(Icons.person_rounded),
              label: 'Account',
            ),
          ],
        ),
      );

  int _selectedIndex(GoRouterState state) {
    final location = state.matchedLocation;
    if (location.startsWith(RouteNames.search)) return 1;
    if (location.startsWith(RouteNames.cart)) return 2;
    if (location.startsWith(RouteNames.dineIn)) return 3;
    if (location.startsWith(RouteNames.account)) return 4;
    return 0;
  }

  void _onTabSelected(BuildContext context, int index) {
    final paths = [
      RouteNames.home,
      RouteNames.search,
      RouteNames.cart,
      RouteNames.dineIn,
      RouteNames.account,
    ];
    context.go(paths[index]);
  }
}

// ────────────────────────────────────────────────────────────────────────────
// Placeholder / Error Pages
// ────────────────────────────────────────────────────────────────────────────

/// Temporary placeholder used until real feature screens are wired in.
class _PlaceholderPage extends StatelessWidget {
  const _PlaceholderPage({required this.title});

  final String title;

  @override
  Widget build(BuildContext context) => Scaffold(
        appBar: AppBar(title: Text(title)),
        body: Center(
          child: Text(
            title,
            style: Theme.of(context).textTheme.headlineMedium,
          ),
        ),
      );
}

/// Fallback page shown when the router encounters an unknown path.
class _ErrorPage extends StatelessWidget {
  const _ErrorPage({this.error});

  final Exception? error;

  @override
  Widget build(BuildContext context) => Scaffold(
        appBar: AppBar(title: const Text('Page Not Found')),
        body: Center(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.error_outline, size: 64, color: Colors.red),
              const SizedBox(height: 16),
              Text(
                'Oops! The page you are looking for does not exist.',
                style: Theme.of(context).textTheme.bodyLarge,
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 24),
              ElevatedButton(
                onPressed: () => context.go(RouteNames.home),
                child: const Text('Go Home'),
              ),
            ],
          ),
        ),
      );
}
