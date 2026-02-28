import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/admin_dashboard_model.dart';
import '../providers/admin_dashboard_notifier.dart';

/// Admin dashboard showing platform-wide statistics and quick-navigation tiles.
class AdminDashboardScreen extends ConsumerWidget {
  const AdminDashboardScreen({super.key});

  static const _orderStatusLabels = {
    0: 'Placed',
    1: 'Confirmed',
    2: 'Preparing',
    3: 'Ready',
    4: 'Out for Delivery',
    5: 'Delivered',
    6: 'Cancelled',
  };

  static const _orderStatusColors = {
    0: AppColors.info,
    1: AppColors.info,
    2: AppColors.primary,
    3: AppColors.primary,
    4: AppColors.primary,
    5: AppColors.success,
    6: AppColors.error,
  };

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(adminDashboardNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Admin Dashboard'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(adminDashboardNotifierProvider.notifier)
                .loadDashboard(),
          ),
        ],
      ),
      body: switch (state) {
        AdminDashboardInitial() || AdminDashboardLoading() =>
          const AppLoadingWidget(message: 'Loading dashboard...'),
        AdminDashboardError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(adminDashboardNotifierProvider.notifier)
                .loadDashboard(),
          ),
        AdminDashboardLoaded(:final dashboard) => RefreshIndicator(
            color: AppColors.primary,
            onRefresh: () => ref
                .read(adminDashboardNotifierProvider.notifier)
                .loadDashboard(),
            child: SingleChildScrollView(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  // ──── User Stats ────
                  _SectionTitle(title: 'Users'),
                  const SizedBox(height: 8),
                  GridView.count(
                    crossAxisCount: 2,
                    shrinkWrap: true,
                    physics: const NeverScrollableScrollPhysics(),
                    mainAxisSpacing: 8,
                    crossAxisSpacing: 8,
                    childAspectRatio: 1.6,
                    children: [
                      _StatCard(
                        icon: Icons.people_outline,
                        title: 'Customers',
                        value: dashboard.userCounts.customers.toString(),
                        iconColor: AppColors.info,
                      ),
                      _StatCard(
                        icon: Icons.store_outlined,
                        title: 'Restaurant Owners',
                        value:
                            dashboard.userCounts.restaurantOwners.toString(),
                        iconColor: AppColors.primary,
                      ),
                      _StatCard(
                        icon: Icons.delivery_dining_outlined,
                        title: 'Delivery Partners',
                        value:
                            dashboard.userCounts.deliveryPartners.toString(),
                        iconColor: Colors.deepPurple,
                      ),
                      _StatCard(
                        icon: Icons.admin_panel_settings_outlined,
                        title: 'Admins',
                        value: dashboard.userCounts.admins.toString(),
                        iconColor: AppColors.error,
                      ),
                    ],
                  ),

                  const SizedBox(height: 24),

                  // ──── Restaurant Stats ────
                  _SectionTitle(title: 'Restaurants'),
                  const SizedBox(height: 8),
                  SingleChildScrollView(
                    scrollDirection: Axis.horizontal,
                    child: Row(
                      children: [
                        _CountChip(
                          label: 'Pending',
                          count: dashboard.restaurantCounts.pending,
                          color: Colors.orange,
                          highlighted: true,
                        ),
                        const SizedBox(width: 8),
                        _CountChip(
                          label: 'Approved',
                          count: dashboard.restaurantCounts.approved,
                          color: AppColors.success,
                        ),
                        const SizedBox(width: 8),
                        _CountChip(
                          label: 'Suspended',
                          count: dashboard.restaurantCounts.suspended,
                          color: AppColors.error,
                        ),
                        const SizedBox(width: 8),
                        _CountChip(
                          label: 'Rejected',
                          count: dashboard.restaurantCounts.rejected,
                          color: AppColors.textTertiaryLight,
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(height: 24),

                  // ──── Order Stats ────
                  _SectionTitle(title: 'Orders'),
                  const SizedBox(height: 8),
                  SingleChildScrollView(
                    scrollDirection: Axis.horizontal,
                    child: Row(
                      children: [
                        _MiniStatCard(
                          title: 'Today',
                          value: dashboard.orderCounts.today.toString(),
                        ),
                        _MiniStatCard(
                          title: 'This Week',
                          value: dashboard.orderCounts.thisWeek.toString(),
                        ),
                        _MiniStatCard(
                          title: 'This Month',
                          value: dashboard.orderCounts.thisMonth.toString(),
                        ),
                        _MiniStatCard(
                          title: 'All Time',
                          value: dashboard.orderCounts.allTime.toString(),
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(height: 24),

                  // ──── Revenue Stats ────
                  _SectionTitle(title: 'Revenue'),
                  const SizedBox(height: 8),
                  SingleChildScrollView(
                    scrollDirection: Axis.horizontal,
                    child: Row(
                      children: [
                        _MiniStatCard(
                          title: 'Today',
                          value:
                              '\u20B9${dashboard.revenue.today ~/ 100}',
                        ),
                        _MiniStatCard(
                          title: 'This Week',
                          value:
                              '\u20B9${dashboard.revenue.thisWeek ~/ 100}',
                        ),
                        _MiniStatCard(
                          title: 'This Month',
                          value:
                              '\u20B9${dashboard.revenue.thisMonth ~/ 100}',
                        ),
                        _MiniStatCard(
                          title: 'All Time',
                          value:
                              '\u20B9${dashboard.revenue.allTime ~/ 100}',
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(height: 24),

                  // ──── Recent Orders ────
                  _SectionTitle(title: 'Recent Orders'),
                  const SizedBox(height: 8),
                  if (dashboard.recentOrders.isEmpty)
                    Padding(
                      padding: const EdgeInsets.symmetric(vertical: 24),
                      child: Center(
                        child: Text(
                          'No recent orders',
                          style:
                              Theme.of(context).textTheme.bodyMedium?.copyWith(
                                    color: AppColors.textTertiaryLight,
                                  ),
                        ),
                      ),
                    )
                  else
                    ...dashboard.recentOrders.map(
                      (order) => _RecentOrderTile(
                        order: order,
                        statusLabel:
                            _orderStatusLabels[order.status] ?? 'Unknown',
                        statusColor: _orderStatusColors[order.status] ??
                            AppColors.textSecondaryLight,
                        onTap: () => context
                            .push(RouteNames.adminOrderDetailPath(order.id)),
                      ),
                    ),

                  const SizedBox(height: 24),

                  // ──── Navigation Tiles ────
                  _SectionTitle(title: 'Manage'),
                  const SizedBox(height: 12),
                  _NavigationTile(
                    icon: Icons.people_outline,
                    title: 'Users',
                    subtitle: 'Manage all platform users',
                    onTap: () => context.push(RouteNames.adminUsers),
                  ),
                  _NavigationTile(
                    icon: Icons.store_outlined,
                    title: 'Restaurants',
                    subtitle: 'Review and manage restaurants',
                    onTap: () => context.push(RouteNames.adminRestaurants),
                  ),
                  _NavigationTile(
                    icon: Icons.receipt_long_outlined,
                    title: 'Orders',
                    subtitle: 'View all platform orders',
                    onTap: () => context.push(RouteNames.adminOrders),
                  ),
                  _NavigationTile(
                    icon: Icons.analytics_outlined,
                    title: 'Analytics',
                    subtitle: 'Revenue, trends, and insights',
                    onTap: () => context.push(RouteNames.adminAnalytics),
                  ),
                  _NavigationTile(
                    icon: Icons.image_outlined,
                    title: 'Banners',
                    subtitle: 'Manage promotional banners',
                    onTap: () => context.push(RouteNames.adminBanners),
                  ),
                  _NavigationTile(
                    icon: Icons.settings_outlined,
                    title: 'Platform Config',
                    subtitle: 'Delivery fees, tax rate, and thresholds',
                    onTap: () => context.push(RouteNames.adminConfig),
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

// ──────────────────── Private Widgets ────────────────────

class _SectionTitle extends StatelessWidget {
  const _SectionTitle({required this.title});

  final String title;

  @override
  Widget build(BuildContext context) => Text(
        title,
        style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
      );
}

class _StatCard extends StatelessWidget {
  const _StatCard({
    required this.icon,
    required this.title,
    required this.value,
    required this.iconColor,
  });

  final IconData icon;
  final String title;
  final String value;
  final Color iconColor;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(icon, color: iconColor, size: 28),
            const Spacer(),
            Text(
              value,
              style: theme.textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 2),
            Text(
              title,
              style: theme.textTheme.bodySmall?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ),
      ),
    );
  }
}

class _CountChip extends StatelessWidget {
  const _CountChip({
    required this.label,
    required this.count,
    required this.color,
    this.highlighted = false,
  });

  final String label;
  final int count;
  final Color color;
  final bool highlighted;

  @override
  Widget build(BuildContext context) => Chip(
        label: Text(
          '$label ($count)',
          style: Theme.of(context).textTheme.labelMedium?.copyWith(
                color: highlighted ? Colors.white : color,
                fontWeight: FontWeight.w600,
              ),
        ),
        backgroundColor: highlighted ? color : color.withValues(alpha: 0.12),
        side: BorderSide.none,
        padding: const EdgeInsets.symmetric(horizontal: 4),
      );
}

class _MiniStatCard extends StatelessWidget {
  const _MiniStatCard({required this.title, required this.value});

  final String title;
  final String value;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.only(right: 8),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        child: Column(
          children: [
            Text(
              value,
              style: theme.textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              title,
              style: theme.textTheme.bodySmall?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _RecentOrderTile extends StatelessWidget {
  const _RecentOrderTile({
    required this.order,
    required this.statusLabel,
    required this.statusColor,
    required this.onTap,
  });

  final AdminOrderSummaryModel order;
  final String statusLabel;
  final Color statusColor;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.only(bottom: 6),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(10),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      '#${order.orderNumber}',
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      '${order.customerName} - ${order.restaurantName}',
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),
              Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: statusColor.withValues(alpha: 0.12),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      statusLabel,
                      style: theme.textTheme.labelSmall?.copyWith(
                        color: statusColor,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    '\u20B9${order.totalAmount ~/ 100}',
                    style: theme.textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
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
