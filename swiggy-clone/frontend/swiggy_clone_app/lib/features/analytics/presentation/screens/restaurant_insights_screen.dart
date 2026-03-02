import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/named_value_model.dart';
import '../../data/models/restaurant_insights_model.dart';
import '../providers/restaurant_insights_notifier.dart';
import '../widgets/analytics_bar_chart.dart';
import '../widgets/analytics_line_chart.dart';
import '../widgets/analytics_pie_chart.dart';
import '../widgets/period_selector.dart';
import '../widgets/stat_summary_card.dart';

class RestaurantInsightsScreen extends ConsumerStatefulWidget {
  const RestaurantInsightsScreen({super.key, required this.restaurantId});

  final String restaurantId;

  @override
  ConsumerState<RestaurantInsightsScreen> createState() =>
      _RestaurantInsightsScreenState();
}

class _RestaurantInsightsScreenState
    extends ConsumerState<RestaurantInsightsScreen> {
  String _period = 'daily';

  int get _days => switch (_period) {
        'weekly' => 90,
        'monthly' => 365,
        _ => 30,
      };

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(
      restaurantInsightsNotifierProvider(widget.restaurantId),
    );

    return Scaffold(
      appBar: AppBar(
        title: const Text('Restaurant Insights'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(restaurantInsightsNotifierProvider(widget.restaurantId)
                    .notifier)
                .loadInsights(
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
                    .read(restaurantInsightsNotifierProvider(
                            widget.restaurantId)
                        .notifier)
                    .loadInsights(
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
              RestaurantInsightsInitial() ||
              RestaurantInsightsLoading() =>
                const AppLoadingWidget(message: 'Loading insights...'),
              RestaurantInsightsError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: () => ref
                      .read(restaurantInsightsNotifierProvider(
                              widget.restaurantId)
                          .notifier)
                      .loadInsights(
                        restaurantId: widget.restaurantId,
                        period: _period,
                        days: _days,
                      ),
                ),
              RestaurantInsightsLoaded(:final insights) =>
                _InsightsBody(insights: insights),
            },
          ),
        ],
      ),
    );
  }
}

class _InsightsBody extends StatelessWidget {
  const _InsightsBody({required this.insights});

  final RestaurantInsightsModel insights;

  String _formatPaise(double value) =>
      '\u20B9${(value / 100).toStringAsFixed(0)}';

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // ── Summary Grid (2x3) ──
          GridView.count(
            crossAxisCount: 2,
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            mainAxisSpacing: 8,
            crossAxisSpacing: 8,
            childAspectRatio: 2.2,
            children: [
              StatSummaryCard(
                icon: Icons.person_add_outlined,
                title: 'New Customers',
                value: insights.newCustomers.toString(),
                iconColor: AppColors.info,
              ),
              StatSummaryCard(
                icon: Icons.people_outline,
                title: 'Repeat Customers',
                value: insights.repeatCustomers.toString(),
                iconColor: AppColors.success,
              ),
              StatSummaryCard(
                icon: Icons.repeat,
                title: 'Repeat Rate',
                value: '${insights.repeatRate.toStringAsFixed(1)}%',
                iconColor: AppColors.primary,
              ),
              StatSummaryCard(
                icon: Icons.check_circle_outline,
                title: 'Completion Rate',
                value: '${insights.completionRate.toStringAsFixed(1)}%',
                iconColor: AppColors.success,
              ),
              StatSummaryCard(
                icon: Icons.cancel_outlined,
                title: 'Cancellation Rate',
                value: '${insights.cancellationRate.toStringAsFixed(1)}%',
                iconColor: AppColors.error,
              ),
              StatSummaryCard(
                icon: Icons.currency_rupee,
                title: 'Avg Rev/Customer',
                value: _formatPaise(insights.avgRevenuePerCustomer),
                iconColor: AppColors.warning,
              ),
            ],
          ),
          const SizedBox(height: 16),

          // ── Customer Retention Trend ──
          AnalyticsLineChart(
            title: 'Customer Retention Trend',
            dataPoints: insights.customerRetentionTrend,
            lineColor: AppColors.info,
          ),

          // ── Order Completion Trend ──
          AnalyticsLineChart(
            title: 'Order Completion Trend',
            dataPoints: insights.orderCompletionTrend,
            lineColor: AppColors.success,
            formatValue: (v) => '${v.toStringAsFixed(1)}%',
          ),

          // ── Menu Performance Table ──
          if (insights.menuPerformance.isNotEmpty) ...[
            Text(
              'Menu Item Performance',
              style: theme.textTheme.titleSmall
                  ?.copyWith(fontWeight: FontWeight.w600),
            ),
            const SizedBox(height: 8),
            SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: DataTable(
                columnSpacing: 16,
                columns: const [
                  DataColumn(label: Text('Item')),
                  DataColumn(label: Text('Qty'), numeric: true),
                  DataColumn(label: Text('Revenue'), numeric: true),
                  DataColumn(label: Text('Orders'), numeric: true),
                  DataColumn(label: Text('Rating'), numeric: true),
                ],
                rows: insights.menuPerformance.map((item) {
                  return DataRow(cells: [
                    DataCell(SizedBox(
                      width: 120,
                      child: Text(
                        item.itemName,
                        overflow: TextOverflow.ellipsis,
                      ),
                    )),
                    DataCell(Text(item.totalQuantitySold.toString())),
                    DataCell(Text(_formatPaise(item.totalRevenue))),
                    DataCell(Text(item.orderCount.toString())),
                    DataCell(Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Icon(Icons.star, size: 14, color: AppColors.rating),
                        const SizedBox(width: 2),
                        Text(item.avgRating.toStringAsFixed(1)),
                      ],
                    )),
                  ]);
                }).toList(),
              ),
            ),
            const SizedBox(height: 16),
          ],

          // ── Revenue by Order Type ──
          AnalyticsPieChart(
            title: 'Revenue by Order Type',
            items: insights.revenueByOrderType,
          ),

          // ── Revenue by Day of Week ──
          AnalyticsBarChart(
            title: 'Revenue by Day of Week',
            items: insights.revenueByDayOfWeek,
            barColor: AppColors.info,
          ),
          const SizedBox(height: 24),
        ],
      ),
    );
  }
}
