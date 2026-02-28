import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/partner_analytics_model.dart';
import '../providers/partner_analytics_notifier.dart';
import '../widgets/analytics_line_chart.dart';
import '../widgets/period_selector.dart';
import '../widgets/stat_summary_card.dart';

class PartnerAnalyticsScreen extends ConsumerStatefulWidget {
  const PartnerAnalyticsScreen({super.key});

  @override
  ConsumerState<PartnerAnalyticsScreen> createState() =>
      _PartnerAnalyticsScreenState();
}

class _PartnerAnalyticsScreenState
    extends ConsumerState<PartnerAnalyticsScreen> {
  String _period = 'daily';

  int get _days => switch (_period) {
        'weekly' => 90,
        'monthly' => 365,
        _ => 30,
      };

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(partnerAnalyticsNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Delivery Analytics'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(partnerAnalyticsNotifierProvider.notifier)
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
                    .read(partnerAnalyticsNotifierProvider.notifier)
                    .loadAnalytics(period: period, days: _days);
              },
            ),
          ),
          const SizedBox(height: 8),
          Expanded(
            child: switch (state) {
              PartnerAnalyticsInitial() ||
              PartnerAnalyticsLoading() =>
                const AppLoadingWidget(message: 'Loading analytics...'),
              PartnerAnalyticsError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: () => ref
                      .read(partnerAnalyticsNotifierProvider.notifier)
                      .loadAnalytics(period: _period, days: _days),
                ),
              PartnerAnalyticsLoaded(:final analytics) =>
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

  final PartnerAnalyticsModel analytics;

  String _formatPaise(double value) =>
      '\u20B9${(value / 100).toStringAsFixed(0)}';

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // ── Summary Cards ──
          GridView.count(
            crossAxisCount: 2,
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            mainAxisSpacing: 8,
            crossAxisSpacing: 8,
            childAspectRatio: 1.6,
            children: [
              StatSummaryCard(
                icon: Icons.delivery_dining_outlined,
                title: 'Total Deliveries',
                value: analytics.totalDeliveries.toString(),
                iconColor: AppColors.primary,
              ),
              StatSummaryCard(
                icon: Icons.currency_rupee,
                title: 'Total Earnings',
                value: '\u20B9${analytics.totalEarnings ~/ 100}',
                iconColor: AppColors.success,
              ),
              StatSummaryCard(
                icon: Icons.timer_outlined,
                title: 'Avg Delivery Time',
                value: '${analytics.avgDeliveryTimeMinutes} min',
                iconColor: AppColors.info,
              ),
              StatSummaryCard(
                icon: Icons.check_circle_outline,
                title: 'Completion Rate',
                value: '${analytics.completionRatePercent}%',
                iconColor: AppColors.success,
              ),
            ],
          ),
          const SizedBox(height: 8),
          StatSummaryCard(
            icon: Icons.star_outline,
            title: 'Average Rating',
            value: analytics.avgRating > 0
                ? analytics.avgRating.toStringAsFixed(1)
                : 'N/A',
            iconColor: AppColors.rating,
          ),
          const SizedBox(height: 16),

          // ── Earnings Trend ──
          AnalyticsLineChart(
            title: 'Earnings Trend',
            dataPoints: analytics.earningsTrend,
            lineColor: AppColors.success,
            formatValue: _formatPaise,
          ),

          // ── Delivery Count Trend ──
          AnalyticsLineChart(
            title: 'Delivery Count',
            dataPoints: analytics.deliveryCountTrend,
            lineColor: AppColors.primary,
          ),
          const SizedBox(height: 24),
        ],
      ),
    );
  }
}
