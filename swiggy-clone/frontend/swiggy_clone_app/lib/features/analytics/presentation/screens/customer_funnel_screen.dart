import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/customer_funnel_model.dart';
import '../providers/customer_funnel_notifier.dart';
import '../widgets/analytics_line_chart.dart';
import '../widgets/funnel_chart.dart';
import '../widgets/stat_summary_card.dart';

class CustomerFunnelScreen extends ConsumerStatefulWidget {
  const CustomerFunnelScreen({super.key});

  @override
  ConsumerState<CustomerFunnelScreen> createState() =>
      _CustomerFunnelScreenState();
}

class _CustomerFunnelScreenState extends ConsumerState<CustomerFunnelScreen> {
  int _days = 30;

  static const _dayOptions = [7, 14, 30, 60, 90];

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(customerFunnelNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Customer Funnel'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(customerFunnelNotifierProvider.notifier)
                .loadFunnel(days: _days),
          ),
        ],
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
            child: Row(
              children: [
                Text(
                  'Period: ',
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
                const SizedBox(width: 8),
                DropdownButton<int>(
                  value: _days,
                  items: _dayOptions
                      .map((d) => DropdownMenuItem(
                            value: d,
                            child: Text('$d days'),
                          ))
                      .toList(),
                  onChanged: (value) {
                    if (value == null) return;
                    setState(() => _days = value);
                    ref
                        .read(customerFunnelNotifierProvider.notifier)
                        .loadFunnel(days: value);
                  },
                ),
              ],
            ),
          ),
          const SizedBox(height: 8),
          Expanded(
            child: switch (state) {
              CustomerFunnelInitial() || CustomerFunnelLoading() =>
                const AppLoadingWidget(message: 'Loading funnel data...'),
              CustomerFunnelError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: () => ref
                      .read(customerFunnelNotifierProvider.notifier)
                      .loadFunnel(days: _days),
                ),
              CustomerFunnelLoaded(:final funnel) =>
                _FunnelBody(funnel: funnel),
            },
          ),
        ],
      ),
    );
  }
}

class _FunnelBody extends StatelessWidget {
  const _FunnelBody({required this.funnel});

  final CustomerFunnelModel funnel;

  @override
  Widget build(BuildContext context) {
    final stages = funnel.stages;
    final totalUsers = stages.isNotEmpty ? stages.first.count : 0;
    final activeUsers =
        stages.length > 1 ? stages[1].count : 0;
    final overallConversion = stages.isNotEmpty && stages.length > 1
        ? (totalUsers > 0
            ? (stages.last.count / totalUsers * 100)
            : 0.0)
        : 0.0;

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // ── Summary Row ──
          Row(
            children: [
              Expanded(
                child: StatSummaryCard(
                  icon: Icons.people_outline,
                  title: 'Total Users',
                  value: _formatCount(totalUsers),
                  iconColor: AppColors.primary,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: StatSummaryCard(
                  icon: Icons.trending_up,
                  title: 'Active Users',
                  value: _formatCount(activeUsers),
                  iconColor: AppColors.success,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: StatSummaryCard(
                  icon: Icons.filter_alt_outlined,
                  title: 'Conversion',
                  value: '${overallConversion.toStringAsFixed(1)}%',
                  iconColor: AppColors.info,
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),

          // ── Funnel Chart ──
          FunnelChart(
            title: 'Customer Journey Funnel',
            stages: stages,
          ),

          // ── Active User Trend ──
          AnalyticsLineChart(
            title: 'Active User Trend',
            dataPoints: funnel.activeUserTrend,
            lineColor: AppColors.info,
          ),
          const SizedBox(height: 24),
        ],
      ),
    );
  }

  static String _formatCount(int value) {
    if (value >= 10000000) return '${(value / 10000000).toStringAsFixed(1)}Cr';
    if (value >= 100000) return '${(value / 100000).toStringAsFixed(1)}L';
    if (value >= 1000) return '${(value / 1000).toStringAsFixed(1)}K';
    return value.toString();
  }
}
