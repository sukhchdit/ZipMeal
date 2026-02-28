import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/named_value_model.dart';
import '../../data/models/restaurant_analytics_model.dart';
import '../providers/restaurant_analytics_notifier.dart';
import '../widgets/analytics_bar_chart.dart';
import '../widgets/analytics_line_chart.dart';
import '../widgets/analytics_pie_chart.dart';
import '../widgets/period_selector.dart';
import '../widgets/stat_summary_card.dart';

class RestaurantAnalyticsScreen extends ConsumerStatefulWidget {
  const RestaurantAnalyticsScreen({super.key, required this.restaurantId});

  final String restaurantId;

  @override
  ConsumerState<RestaurantAnalyticsScreen> createState() =>
      _RestaurantAnalyticsScreenState();
}

class _RestaurantAnalyticsScreenState
    extends ConsumerState<RestaurantAnalyticsScreen> {
  String _period = 'daily';

  int get _days => switch (_period) {
        'weekly' => 90,
        'monthly' => 365,
        _ => 30,
      };

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(
      restaurantAnalyticsNotifierProvider(widget.restaurantId),
    );

    return Scaffold(
      appBar: AppBar(
        title: const Text('Restaurant Analytics'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(restaurantAnalyticsNotifierProvider(widget.restaurantId)
                    .notifier)
                .loadAnalytics(
                  restaurantId: widget.restaurantId,
                  period: _period,
                  days: _days,
                ),
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
                    .read(restaurantAnalyticsNotifierProvider(
                            widget.restaurantId)
                        .notifier)
                    .loadAnalytics(
                      restaurantId: widget.restaurantId,
                      period: period,
                      days: _days,
                    );
              },
            ),
          ),
          const SizedBox(height: 8),
          Expanded(
            child: switch (state) {
              RestaurantAnalyticsInitial() ||
              RestaurantAnalyticsLoading() =>
                const AppLoadingWidget(message: 'Loading analytics...'),
              RestaurantAnalyticsError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: () => ref
                      .read(restaurantAnalyticsNotifierProvider(
                              widget.restaurantId)
                          .notifier)
                      .loadAnalytics(
                        restaurantId: widget.restaurantId,
                        period: _period,
                        days: _days,
                      ),
                ),
              RestaurantAnalyticsLoaded(:final analytics) =>
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

  final RestaurantAnalyticsModel analytics;

  String _formatPaise(double value) =>
      '\u20B9${(value / 100).toStringAsFixed(0)}';

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // ── Average Order Value ──
          StatSummaryCard(
            icon: Icons.receipt_long_outlined,
            title: 'Average Order Value',
            value: '\u20B9${(analytics.averageOrderValue / 100).toStringAsFixed(0)}',
            iconColor: AppColors.primary,
          ),
          const SizedBox(height: 16),

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

          // ── Rating Trend ──
          AnalyticsLineChart(
            title: 'Average Rating',
            dataPoints: analytics.ratingTrend,
            lineColor: AppColors.rating,
          ),

          // ── Top Menu Items ──
          AnalyticsBarChart(
            title: 'Top Menu Items',
            items: analytics.topMenuItems,
            barColor: AppColors.warning,
          ),

          // ── Distributions ──
          AnalyticsPieChart(
            title: 'Order Type',
            items: analytics.orderTypeDistribution,
          ),
          AnalyticsPieChart(
            title: 'Order Status',
            items: analytics.orderStatusDistribution,
          ),

          // ── Peak Hours ──
          AnalyticsBarChart(
            title: 'Peak Hours',
            items: analytics.peakHoursDistribution
                .map((d) => NamedValueModel(name: d.label, value: d.value))
                .toList(),
            barColor: AppColors.info,
          ),
          const SizedBox(height: 24),
        ],
      ),
    );
  }
}
