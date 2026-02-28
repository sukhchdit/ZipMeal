import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../providers/active_delivery_notifier.dart';
import '../providers/active_delivery_state.dart';
import '../providers/partner_dashboard_notifier.dart';
import '../providers/partner_online_notifier.dart';
import '../providers/partner_online_state.dart';

class DeliveryDashboardScreen extends ConsumerWidget {
  const DeliveryDashboardScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final dashboardState = ref.watch(partnerDashboardNotifierProvider);
    final onlineState = ref.watch(partnerOnlineNotifierProvider);
    final activeState = ref.watch(activeDeliveryNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Delivery Dashboard'),
        actions: [
          _OnlineToggle(onlineState: onlineState, ref: ref),
          const SizedBox(width: 8),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: () async {
          ref.invalidate(partnerDashboardNotifierProvider);
          ref.invalidate(activeDeliveryNotifierProvider);
        },
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // Active delivery card
            _ActiveDeliverySection(activeState: activeState),
            const SizedBox(height: 16),

            // Dashboard stats
            switch (dashboardState) {
              PartnerDashboardInitial() || PartnerDashboardLoading() =>
                const AppLoadingWidget(message: 'Loading stats...'),
              PartnerDashboardError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: () => ref
                      .read(partnerDashboardNotifierProvider.notifier)
                      .loadDashboard(),
                ),
              PartnerDashboardLoaded(:final dashboard) => Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      "Today's Performance",
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: _StatCard(
                            label: 'Deliveries',
                            value: dashboard.todayDeliveries.toString(),
                            icon: Icons.delivery_dining,
                            color: AppColors.primary,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: _StatCard(
                            label: 'Earnings',
                            value:
                                '\u20B9${dashboard.todayEarnings ~/ 100}',
                            icon: Icons.currency_rupee,
                            color: AppColors.success,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: _StatCard(
                            label: 'Tips',
                            value:
                                '\u20B9${dashboard.todayTips ~/ 100}',
                            icon: Icons.volunteer_activism,
                            color: Colors.amber,
                          ),
                        ),
                        const SizedBox(width: 12),
                        const Expanded(child: SizedBox.shrink()),
                      ],
                    ),
                    const SizedBox(height: 24),
                    Text(
                      'Overall Stats',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: _StatCard(
                            label: 'Total Deliveries',
                            value: dashboard.totalDeliveries.toString(),
                            icon: Icons.check_circle_outline,
                            color: AppColors.info,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: _StatCard(
                            label: 'Total Earnings',
                            value:
                                '\u20B9${dashboard.totalEarnings ~/ 100}',
                            icon: Icons.account_balance_wallet,
                            color: Colors.deepPurple,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: _StatCard(
                            label: 'Total Tips',
                            value:
                                '\u20B9${dashboard.totalTips ~/ 100}',
                            icon: Icons.volunteer_activism,
                            color: Colors.amber,
                          ),
                        ),
                        const SizedBox(width: 12),
                        const Expanded(child: SizedBox.shrink()),
                      ],
                    ),
                    const SizedBox(height: 24),
                    Card(
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: ListTile(
                        leading: Container(
                          padding: const EdgeInsets.all(8),
                          decoration: BoxDecoration(
                            color: AppColors.primaryLight,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: const Icon(
                            Icons.analytics_outlined,
                            color: AppColors.primary,
                          ),
                        ),
                        title: Text(
                          'Analytics',
                          style:
                              Theme.of(context).textTheme.titleSmall?.copyWith(
                                    fontWeight: FontWeight.w600,
                                  ),
                        ),
                        subtitle: Text(
                          'Earnings trends and performance',
                          style:
                              Theme.of(context).textTheme.bodySmall?.copyWith(
                                    color: AppColors.textSecondaryLight,
                                  ),
                        ),
                        trailing: const Icon(
                          Icons.chevron_right,
                          color: AppColors.textTertiaryLight,
                        ),
                        onTap: () =>
                            context.push(RouteNames.deliveryAnalytics),
                      ),
                    ),
                  ],
                ),
            },
          ],
        ),
      ),
    );
  }
}

class _OnlineToggle extends StatelessWidget {
  const _OnlineToggle({required this.onlineState, required this.ref});

  final PartnerOnlineState onlineState;
  final WidgetRef ref;

  @override
  Widget build(BuildContext context) {
    final isOnline = switch (onlineState) {
      PartnerOnlineLoaded(:final isOnline) => isOnline,
      _ => false,
    };
    final isLoading = onlineState is PartnerOnlineLoading;

    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          isOnline ? 'Online' : 'Offline',
          style: Theme.of(context).textTheme.labelMedium?.copyWith(
                color: isOnline ? AppColors.success : AppColors.textTertiaryLight,
                fontWeight: FontWeight.bold,
              ),
        ),
        if (isLoading)
          const Padding(
            padding: EdgeInsets.symmetric(horizontal: 8),
            child: SizedBox(
              width: 20,
              height: 20,
              child: CircularProgressIndicator(strokeWidth: 2),
            ),
          )
        else
          Switch.adaptive(
            value: isOnline,
            onChanged: (value) {
              ref
                  .read(partnerOnlineNotifierProvider.notifier)
                  .toggleOnline(isOnline: value);
            },
            activeColor: AppColors.success,
          ),
      ],
    );
  }
}

class _ActiveDeliverySection extends StatelessWidget {
  const _ActiveDeliverySection({required this.activeState});

  final ActiveDeliveryState activeState;

  @override
  Widget build(BuildContext context) {
    return switch (activeState) {
      ActiveDeliveryInitial() || ActiveDeliveryLoading() => const SizedBox.shrink(),
      ActiveDeliveryError() => const SizedBox.shrink(),
      ActiveDeliveryLoaded(:final assignment) => assignment == null
          ? const SizedBox.shrink()
          : Card(
              color: AppColors.primary.withValues(alpha: 0.06),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
                side: BorderSide(
                  color: AppColors.primary.withValues(alpha: 0.3),
                ),
              ),
              child: InkWell(
                onTap: () => context.push(RouteNames.activeDelivery),
                borderRadius: BorderRadius.circular(12),
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Row(
                    children: [
                      const Icon(
                        Icons.delivery_dining,
                        color: AppColors.primary,
                        size: 36,
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'Active Delivery',
                              style: Theme.of(context)
                                  .textTheme
                                  .titleSmall
                                  ?.copyWith(
                                    fontWeight: FontWeight.bold,
                                    color: AppColors.primary,
                                  ),
                            ),
                            const SizedBox(height: 2),
                            Text(
                              '${assignment.restaurantName} - #${assignment.orderNumber}',
                              style: Theme.of(context).textTheme.bodySmall,
                            ),
                          ],
                        ),
                      ),
                      const Icon(
                        Icons.chevron_right,
                        color: AppColors.primary,
                      ),
                    ],
                  ),
                ),
              ),
            ),
    };
  }
}

class _StatCard extends StatelessWidget {
  const _StatCard({
    required this.label,
    required this.value,
    required this.icon,
    required this.color,
  });

  final String label;
  final String value;
  final IconData icon;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, color: color, size: 28),
            const SizedBox(height: 8),
            Text(
              value,
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
            ),
            const SizedBox(height: 2),
            Text(
              label,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: AppColors.textSecondaryLight,
                  ),
            ),
          ],
        ),
      ),
    );
  }
}
