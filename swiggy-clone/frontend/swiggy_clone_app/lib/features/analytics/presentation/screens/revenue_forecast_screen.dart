import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/revenue_forecast_model.dart';
import '../providers/revenue_forecast_notifier.dart';
import '../widgets/forecast_line_chart.dart';
import '../widgets/stat_summary_card.dart';

class RevenueForecastScreen extends ConsumerStatefulWidget {
  const RevenueForecastScreen({super.key, this.restaurantId});

  final String? restaurantId;

  @override
  ConsumerState<RevenueForecastScreen> createState() =>
      _RevenueForecastScreenState();
}

class _RevenueForecastScreenState
    extends ConsumerState<RevenueForecastScreen> {
  double _days = 30;
  double _forecastDays = 14;

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(
      revenueForecastNotifierProvider(widget.restaurantId),
    );

    return Scaffold(
      appBar: AppBar(
        title: Text(
          widget.restaurantId != null
              ? 'Revenue Forecast'
              : 'Platform Revenue Forecast',
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _reload,
          ),
        ],
      ),
      body: Column(
        children: [
          // ── Controls ──
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
            child: Column(
              children: [
                _SliderRow(
                  label: 'Historical',
                  value: _days,
                  min: 14,
                  max: 90,
                  suffix: 'd',
                  onChanged: (v) => setState(() => _days = v),
                  onChangeEnd: (_) => _reload(),
                ),
                const SizedBox(height: 4),
                _SliderRow(
                  label: 'Forecast',
                  value: _forecastDays,
                  min: 7,
                  max: 30,
                  suffix: 'd',
                  onChanged: (v) => setState(() => _forecastDays = v),
                  onChangeEnd: (_) => _reload(),
                ),
              ],
            ),
          ),
          const SizedBox(height: 8),
          Expanded(
            child: switch (state) {
              RevenueForecastInitial() || RevenueForecastLoading() =>
                const AppLoadingWidget(message: 'Loading forecast...'),
              RevenueForecastError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: _reload,
                ),
              RevenueForecastLoaded(:final forecast) =>
                _ForecastBody(forecast: forecast),
            },
          ),
        ],
      ),
    );
  }

  void _reload() {
    ref
        .read(
            revenueForecastNotifierProvider(widget.restaurantId).notifier)
        .loadForecast(
          restaurantId: widget.restaurantId,
          days: _days.round(),
          forecastDays: _forecastDays.round(),
        );
  }
}

class _ForecastBody extends StatelessWidget {
  const _ForecastBody({required this.forecast});

  final RevenueForecastModel forecast;

  String _formatPaise(double value) =>
      '\u20B9${(value / 100).toStringAsFixed(0)}';

  @override
  Widget build(BuildContext context) {
    final growthColor =
        forecast.growthRate >= 0 ? AppColors.success : AppColors.error;

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // ── Summary Grid ──
          Row(
            children: [
              Expanded(
                child: StatSummaryCard(
                  icon: Icons.trending_up,
                  title: 'Projected Total',
                  value: _formatPaise(forecast.projectedTotalRevenue),
                  iconColor: AppColors.primary,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: StatSummaryCard(
                  icon: Icons.calendar_today,
                  title: 'Avg Daily',
                  value: _formatPaise(forecast.avgDailyProjected),
                  iconColor: AppColors.info,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: StatSummaryCard(
                  icon: forecast.growthRate >= 0
                      ? Icons.arrow_upward
                      : Icons.arrow_downward,
                  title: 'Growth Rate',
                  value: '${forecast.growthRate.toStringAsFixed(1)}%',
                  iconColor: growthColor,
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),

          // ── Forecast Chart ──
          ForecastLineChart(
            title: 'Revenue Forecast',
            historicalData: forecast.historicalData,
            forecastData: forecast.forecastData,
          ),
          const SizedBox(height: 24),
        ],
      ),
    );
  }
}

class _SliderRow extends StatelessWidget {
  const _SliderRow({
    required this.label,
    required this.value,
    required this.min,
    required this.max,
    required this.suffix,
    required this.onChanged,
    required this.onChangeEnd,
  });

  final String label;
  final double value;
  final double min;
  final double max;
  final String suffix;
  final ValueChanged<double> onChanged;
  final ValueChanged<double> onChangeEnd;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        SizedBox(
          width: 72,
          child: Text(
            '$label:',
            style: Theme.of(context).textTheme.bodySmall,
          ),
        ),
        Expanded(
          child: Slider(
            value: value,
            min: min,
            max: max,
            divisions: (max - min).round(),
            label: '${value.round()}$suffix',
            onChanged: onChanged,
            onChangeEnd: onChangeEnd,
          ),
        ),
        SizedBox(
          width: 40,
          child: Text(
            '${value.round()}$suffix',
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
          ),
        ),
      ],
    );
  }
}
