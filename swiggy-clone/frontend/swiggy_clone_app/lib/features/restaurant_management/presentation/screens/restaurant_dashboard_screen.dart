import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../providers/dashboard_notifier.dart';
import '../providers/dashboard_state.dart';
import '../providers/restaurant_detail_notifier.dart';
import '../providers/restaurant_detail_state.dart';
import '../widgets/dashboard_stat_card.dart';

/// Dashboard screen showing stats and quick-navigation tiles for a restaurant.
///
/// Displays: total orders, pending orders, total menu items, active items,
/// average rating, total ratings, isAcceptingOrders toggle, and status badge.
class RestaurantDashboardScreen extends ConsumerStatefulWidget {
  const RestaurantDashboardScreen({
    required this.restaurantId,
    super.key,
  });

  final String restaurantId;

  @override
  ConsumerState<RestaurantDashboardScreen> createState() =>
      _RestaurantDashboardScreenState();
}

class _RestaurantDashboardScreenState
    extends ConsumerState<RestaurantDashboardScreen> {
  bool _isTogglingOrders = false;

  @override
  Widget build(BuildContext context) {
    final dashboardState =
        ref.watch(dashboardNotifierProvider(widget.restaurantId));
    final detailState =
        ref.watch(restaurantDetailNotifierProvider(widget.restaurantId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: switch (detailState) {
          RestaurantDetailLoaded(:final restaurant) => Text(restaurant.name),
          _ => const Text('Restaurant Dashboard'),
        },
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              ref
                  .read(dashboardNotifierProvider(widget.restaurantId).notifier)
                  .loadDashboard();
              ref
                  .read(restaurantDetailNotifierProvider(widget.restaurantId)
                      .notifier)
                  .loadRestaurant();
            },
          ),
        ],
      ),
      body: switch (dashboardState) {
        DashboardInitial() || DashboardLoading() =>
          const AppLoadingWidget(message: 'Loading dashboard...'),
        DashboardError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(
                    dashboardNotifierProvider(widget.restaurantId).notifier)
                .loadDashboard(),
          ),
        DashboardLoaded(:final dashboard) => RefreshIndicator(
            color: AppColors.primary,
            onRefresh: () async {
              await ref
                  .read(dashboardNotifierProvider(widget.restaurantId).notifier)
                  .loadDashboard();
              ref
                  .read(restaurantDetailNotifierProvider(widget.restaurantId)
                      .notifier)
                  .loadRestaurant();
            },
            child: SingleChildScrollView(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  // ──── Status & Toggle ────
                  Card(
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12)),
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Row(
                        children: [
                          // Status badge
                          _StatusBadge(status: dashboard.status),
                          const Spacer(),

                          // Accepting orders toggle
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.end,
                            children: [
                              Text(
                                'Accepting Orders',
                                style: theme.textTheme.labelMedium,
                              ),
                              Switch(
                                value: dashboard.isAcceptingOrders,
                                activeColor: AppColors.success,
                                onChanged: _isTogglingOrders
                                    ? null
                                    : (value) async {
                                        setState(
                                            () => _isTogglingOrders = true);
                                        await ref
                                            .read(
                                                restaurantDetailNotifierProvider(
                                                        widget.restaurantId)
                                                    .notifier)
                                            .toggleAcceptingOrders(value);
                                        ref.invalidate(
                                            dashboardNotifierProvider(
                                                widget.restaurantId));
                                        if (mounted) {
                                          setState(
                                              () => _isTogglingOrders = false);
                                        }
                                      },
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 16),

                  // ──── Stats Grid ────
                  GridView.count(
                    crossAxisCount: 2,
                    shrinkWrap: true,
                    physics: const NeverScrollableScrollPhysics(),
                    mainAxisSpacing: 8,
                    crossAxisSpacing: 8,
                    childAspectRatio: 1.4,
                    children: [
                      DashboardStatCard(
                        icon: Icons.receipt_long_outlined,
                        title: 'Total Orders',
                        value: dashboard.totalOrders.toString(),
                        iconColor: AppColors.primary,
                      ),
                      DashboardStatCard(
                        icon: Icons.pending_actions_outlined,
                        title: 'Pending Orders',
                        value: dashboard.pendingOrders.toString(),
                        iconColor: AppColors.warning,
                      ),
                      DashboardStatCard(
                        icon: Icons.restaurant_menu_outlined,
                        title: 'Total Menu Items',
                        value: dashboard.totalMenuItems.toString(),
                        iconColor: AppColors.info,
                      ),
                      DashboardStatCard(
                        icon: Icons.check_circle_outline,
                        title: 'Active Items',
                        value: dashboard.activeMenuItems.toString(),
                        iconColor: AppColors.success,
                      ),
                      DashboardStatCard(
                        icon: Icons.star_outline_rounded,
                        title: 'Average Rating',
                        value: dashboard.averageRating.toStringAsFixed(1),
                        iconColor: AppColors.rating,
                      ),
                      DashboardStatCard(
                        icon: Icons.rate_review_outlined,
                        title: 'Total Ratings',
                        value: dashboard.totalRatings.toString(),
                        iconColor: AppColors.secondary,
                      ),
                      DashboardStatCard(
                        icon: Icons.table_restaurant_outlined,
                        title: 'Active Tables',
                        value:
                            '${dashboard.activeTables}/${dashboard.totalTables}',
                        iconColor: Colors.teal,
                      ),
                      DashboardStatCard(
                        icon: Icons.groups_outlined,
                        title: 'Active Sessions',
                        value: dashboard.activeSessions.toString(),
                        iconColor: Colors.purple,
                      ),
                      DashboardStatCard(
                        icon: Icons.dinner_dining_outlined,
                        title: 'Pending Dine-In',
                        value: dashboard.pendingDineInOrders.toString(),
                        iconColor: Colors.deepOrange,
                      ),
                    ],
                  ),
                  const SizedBox(height: 24),

                  // ──── Quick Navigation ────
                  Text(
                    'Manage',
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 12),

                  _NavigationTile(
                    icon: Icons.category_outlined,
                    title: 'Menu Categories',
                    subtitle: 'Manage food categories and items',
                    onTap: () => context.push(
                      RouteNames.menuCategoriesPath(widget.restaurantId),
                    ),
                  ),
                  _NavigationTile(
                    icon: Icons.schedule_outlined,
                    title: 'Operating Hours',
                    subtitle: 'Set daily open and close times',
                    onTap: () => context.push(
                      RouteNames.operatingHoursPath(widget.restaurantId),
                    ),
                  ),
                  _NavigationTile(
                    icon: Icons.upload_file_outlined,
                    title: 'Documents & Images',
                    subtitle: 'Upload logo, banner, FSSAI, GST',
                    onTap: () => context.push(
                      RouteNames.documentUploadPath(widget.restaurantId),
                    ),
                  ),
                  _NavigationTile(
                    icon: Icons.edit_outlined,
                    title: 'Edit Profile',
                    subtitle: 'Update restaurant information',
                    onTap: () => context.push(
                      RouteNames.editRestaurantPath(widget.restaurantId),
                    ),
                  ),
                  _NavigationTile(
                    icon: Icons.analytics_outlined,
                    title: 'Analytics',
                    subtitle: 'Revenue, orders, and trends',
                    onTap: () => context.push(
                      RouteNames.restaurantAnalyticsPath(widget.restaurantId),
                    ),
                  ),

                  const SizedBox(height: 16),
                  Text(
                    'Dine-In',
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 12),

                  _NavigationTile(
                    icon: Icons.table_restaurant_outlined,
                    title: 'Tables',
                    subtitle: 'Manage tables and QR codes',
                    onTap: () => context.push(
                      RouteNames.tableManagementPath(widget.restaurantId),
                    ),
                  ),
                  _NavigationTile(
                    icon: Icons.dinner_dining_outlined,
                    title: 'Dine-In Orders',
                    subtitle: 'View and manage dine-in orders',
                    onTap: () => context.push(
                      RouteNames.dineInOrdersPath(widget.restaurantId),
                    ),
                  ),
                  const SizedBox(height: 24),
                ],
              ),
            ),
          ),
      },
    );
  }
}

class _StatusBadge extends StatelessWidget {
  const _StatusBadge({required this.status});

  final String status;

  @override
  Widget build(BuildContext context) {
    final Color color;
    final IconData icon;
    switch (status.toLowerCase()) {
      case 'active':
        color = AppColors.success;
        icon = Icons.check_circle;
      case 'pending':
        color = AppColors.warning;
        icon = Icons.hourglass_empty;
      case 'suspended':
        color = AppColors.error;
        icon = Icons.block;
      default:
        color = AppColors.textTertiaryLight;
        icon = Icons.info_outline;
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, color: color, size: 18),
          const SizedBox(width: 6),
          Text(
            status,
            style: Theme.of(context).textTheme.labelLarge?.copyWith(
                  color: color,
                  fontWeight: FontWeight.w600,
                ),
          ),
        ],
      ),
    );
  }
}

class _NavigationTile extends StatelessWidget {
  const _NavigationTile({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.onTap,
  });

  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      child: ListTile(
        leading: Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: AppColors.primaryLight,
            borderRadius: BorderRadius.circular(8),
          ),
          child: Icon(icon, color: AppColors.primary),
        ),
        title: Text(
          title,
          style: theme.textTheme.titleSmall?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        subtitle: Text(
          subtitle,
          style: theme.textTheme.bodySmall?.copyWith(
            color: AppColors.textSecondaryLight,
          ),
        ),
        trailing: const Icon(
          Icons.chevron_right,
          color: AppColors.textTertiaryLight,
        ),
        onTap: onTap,
      ),
    );
  }
}
