import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/named_value_model.dart';
import '../../data/models/platform_analytics_model.dart';
import '../providers/platform_analytics_notifier.dart';
import '../widgets/analytics_bar_chart.dart';
import '../widgets/analytics_line_chart.dart';
import '../widgets/analytics_pie_chart.dart';
import '../widgets/period_selector.dart';
import '../widgets/stat_summary_card.dart';

class PlatformAnalyticsScreen extends ConsumerStatefulWidget {
  const PlatformAnalyticsScreen({super.key});

  @override
  ConsumerState<PlatformAnalyticsScreen> createState() =>
      _PlatformAnalyticsScreenState();
}

class _PlatformAnalyticsScreenState
    extends ConsumerState<PlatformAnalyticsScreen> {
  String _period = 'daily';

  int get _days => switch (_period) {
        'weekly' => 90,
        'monthly' => 365,
        _ => 30,
      };

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(platformAnalyticsNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Platform Analytics'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(platformAnalyticsNotifierProvider.notifier)
                .loadAnalytics(period: _period, days: _days),
          ),
        ],
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
            child: PeriodSelector(
              selected: _period,
              onChanged: (period) {
                setState(() => _period = period);
                ref
                    .read(platformAnalyticsNotifierProvider.notifier)
                    .loadAnalytics(period: period, days: _days);
              },
            ),
          ),
          const SizedBox(height: 8),
          Expanded(
            child: switch (state) {
              PlatformAnalyticsInitial() ||
              PlatformAnalyticsLoading() =>
                const AppLoadingWidget(message: 'Loading analytics...'),
              PlatformAnalyticsError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: () => ref
                      .read(platformAnalyticsNotifierProvider.notifier)
                      .loadAnalytics(period: _period, days: _days),
                ),
              PlatformAnalyticsLoaded(:final analytics) =>
                _AnalyticsBody(analytics: analytics),
            },
          ),
        ],
      ),
    );
  }
}

class _AnalyticsBody extends StatelessWidget {
  const _AnalyticsBody({required this.analytics});

  final PlatformAnalyticsModel analytics;

  String _formatPaise(double value) =>
      '\u20B9${(value / 100).toStringAsFixed(0)}';

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // ── Revenue Trend ──
          AnalyticsLineChart(
            title: 'Revenue Trend',
            dataPoints: analytics.revenueTrend,
            lineColor: AppColors.success,
            formatValue: _formatPaise,
          ),

          // ── Order Trend ──
          AnalyticsLineChart(
            title: 'Order Volume',
            dataPoints: analytics.orderTrend,
            lineColor: AppColors.primary,
          ),

          // ── User Growth ──
          AnalyticsLineChart(
            title: 'New Users',
            dataPoints: analytics.userGrowthTrend,
            lineColor: AppColors.info,
          ),

          // ── Distributions ──
          AnalyticsPieChart(
            title: 'Order Status',
            items: analytics.orderStatusDistribution,
          ),
          AnalyticsPieChart(
            title: 'Order Type',
            items: analytics.orderTypeDistribution,
          ),
          AnalyticsPieChart(
            title: 'Payment Methods',
            items: analytics.paymentMethodDistribution,
          ),

          // ── Top Restaurants ──
          AnalyticsBarChart(
            title: 'Top Restaurants by Revenue',
            items: analytics.topRestaurantsByRevenue,
            barColor: AppColors.success,
            formatValue: _formatPaise,
          ),
          AnalyticsBarChart(
            title: 'Top Restaurants by Orders',
            items: analytics.topRestaurantsByOrders,
            barColor: AppColors.primary,
          ),

          // ── Popular Items ──
          AnalyticsBarChart(
            title: 'Popular Menu Items',
            items: analytics.popularMenuItems,
            barColor: AppColors.warning,
          ),

          // ── Peak Hours ──
          AnalyticsBarChart(
            title: 'Peak Hours',
            items: analytics.peakHoursDistribution
                .map((d) => NamedValueModel(name: d.label, value: d.value))
                .toList(),
            barColor: AppColors.info,
          ),

          // ── Coupon Stats ──
          Text(
            'Coupon Performance',
            style: Theme.of(context).textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
          ),
          const SizedBox(height: 8),
          GridView.count(
            crossAxisCount: 2,
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            mainAxisSpacing: 8,
            crossAxisSpacing: 8,
            childAspectRatio: 1.6,
            children: [
              StatSummaryCard(
                icon: Icons.confirmation_number_outlined,
                title: 'Coupons Used',
                value: analytics.couponStats.totalCouponsUsed.toString(),
                iconColor: AppColors.primary,
              ),
              StatSummaryCard(
                icon: Icons.savings_outlined,
                title: 'Total Discount',
                value:
                    '\u20B9${analytics.couponStats.totalDiscountGiven ~/ 100}',
                iconColor: AppColors.success,
              ),
            ],
          ),
          const SizedBox(height: 16),

          // ── Delivery Performance ──
          Text(
            'Delivery Performance',
            style: Theme.of(context).textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
          ),
          const SizedBox(height: 8),
          GridView.count(
            crossAxisCount: 2,
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            mainAxisSpacing: 8,
            crossAxisSpacing: 8,
            childAspectRatio: 1.6,
            children: [
              StatSummaryCard(
                icon: Icons.timer_outlined,
                title: 'Avg Delivery Time',
                value:
                    '${analytics.deliveryPerformance.avgDeliveryTimeMinutes} min',
                iconColor: AppColors.info,
              ),
              StatSummaryCard(
                icon: Icons.check_circle_outline,
                title: 'Completion Rate',
                value:
                    '${analytics.deliveryPerformance.completionRatePercent}%',
                iconColor: AppColors.success,
              ),
              StatSummaryCard(
                icon: Icons.delivery_dining_outlined,
                title: 'Total Deliveries',
                value: analytics.deliveryPerformance.totalDeliveries.toString(),
                iconColor: AppColors.primary,
              ),
              StatSummaryCard(
                icon: Icons.cancel_outlined,
                title: 'Cancelled',
                value:
                    analytics.deliveryPerformance.cancelledDeliveries.toString(),
                iconColor: AppColors.error,
              ),
            ],
          ),
          const SizedBox(height: 24),
        ],
      ),
    );
  }
}

